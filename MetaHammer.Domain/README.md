# MetaHammer.Domain

Sistema de meta-tipos que permite definir esquemas (tipos) en tiempo de ejecución y crear instancias a partir de ellos. Inspirado en conceptos de DDD (Domain-Driven Design) y el patrón Meta-Object Protocol.

## Conceptos Fundamentales

### Meta-Tipos vs Instancias

MetaHammer separa claramente dos niveles:

| Nivel | Descripción | Ejemplo |
|-------|-------------|---------|
| **Tipos (Esquemas)** | Definen la estructura y comportamiento | `PersonType` con propiedades `name`, `age` |
| **Instancias (Datos)** | Objetos concretos creados a partir de un tipo | Una persona específica: "Juan", 30 años |

```
┌─────────────────────────────────────────────────────────────┐
│                     TIPOS (Esquemas)                        │
│  Definen: propiedades, relaciones, métodos, constructores   │
└─────────────────────────────────────────────────────────────┘
                            │
                            │ CreateInstance()
                            ▼
┌─────────────────────────────────────────────────────────────┐
│                   INSTANCIAS (Datos)                        │
│  Contienen: valores de propiedades, referencias a otras     │
│  instancias (relaciones)                                    │
└─────────────────────────────────────────────────────────────┘
```

---

## Jerarquía de Tipos

```
MetaType (abstract)
├── Guid, Name, Version
│
├── PrimitiveType : IPropertyType
│   └── Singleton: Int, String, Bool, Decimal, Double, DateTime, Guid
│
└── StructuralType (abstract) : IStructuralType
    ├── Properties: List<PropertyDefinition>
    ├── Methods: List<Method>
    │
    ├── ValueObjectType : IPropertyType
    │   └── CreateInstance() → ValueObjectInstance
    │
    └── ComplexType
        ├── Relations: List<RelationDefinition>
        └── CreateInstance() → ComplexInstance
```

### PrimitiveType

Tipos primitivos del sistema. Usa el patrón **Singleton** para garantizar una única instancia por tipo.

```csharp
PrimitiveType.Int      // System.Int32
PrimitiveType.String   // System.String
PrimitiveType.Bool     // System.Boolean
PrimitiveType.Decimal  // System.Decimal
PrimitiveType.Double   // System.Double
PrimitiveType.DateTime // System.DateTime
PrimitiveType.Guid     // System.Guid
```

### ValueObjectType

Objetos de valor inmutables que se comparan por valor, no por identidad. Solo pueden existir como propiedades de otros tipos.

```csharp
// Definir el tipo
var addressType = new ValueObjectType("Address");
addressType.AddProperty("street", PrimitiveType.String);
addressType.AddProperty("city", PrimitiveType.String);
addressType.AddProperty("zip_code", PrimitiveType.String);

// Crear instancia
var address = addressType.CreateInstance(new {
    street = "Calle Principal 123",
    city = "Madrid",
    zip_code = "28001"
});
```

### ComplexType

Entidades o agregados con identidad propia. Pueden tener propiedades, relaciones con otros tipos complejos y métodos.

```csharp
// Definir el tipo
var personType = new ComplexType("Person");
personType.AddProperty("name", PrimitiveType.String);
personType.AddProperty("age", PrimitiveType.Int);
personType.AddProperty("email", PrimitiveType.String);
personType.AddProperty("address", addressType);  // ValueObject anidado
personType.AddRelation("friends", isArray: true, personType);  // Auto-relación

// Crear instancia
var person = personType.CreateInstance(new {
    name = "Juan García",
    age = 30,
    email = "juan@example.com"
});
```

---

## Jerarquía de Instancias

```
MetaInstance (abstract)
├── Guid
├── Type: MetaType
│
└── MetaInstanceWithProperties (abstract)
    ├── Properties: List<IProperty>
    ├── SetPropertyValue(name, value) × 8 overloads
    │
    ├── ValueObjectInstance : IInstanceProperty
    │   └── Puede usarse como valor de propiedad
    │
    └── ComplexInstance
        ├── Relations: List<Relation>
        └── SetRelation(name, targetGuid)
```

### Propiedades Tipadas

Las propiedades usan polimorfismo para almacenar valores de forma tipada:

```
IProperty (interface)
└── PropertyBase (abstract)
    ├── IntProperty      → Value: int
    ├── StringProperty   → Value: string
    ├── BoolProperty     → Value: bool
    ├── DecimalProperty  → Value: decimal
    ├── DoubleProperty   → Value: double
    ├── DateTimeProperty → Value: DateTime
    ├── GuidProperty     → Value: Guid
    └── ValueObjectProperty → Value: ValueObjectInstance
```

### Relaciones

Las relaciones conectan instancias complejas entre sí mediante GUIDs:

```csharp
var order = orderType.CreateInstance(new { total = 150.00m });
var customer = customerType.CreateInstance(new { name = "María" });

// Establecer relación
order.SetRelation("customer", customer.Guid);
```

---

## Definiciones de Propiedades y Relaciones

### PropertyDefinition

Define la estructura de una propiedad en un tipo:

| Propiedad | Tipo | Descripción |
|-----------|------|-------------|
| `Guid` | Guid | Identificador único |
| `Name` | string | Nombre en snake_case |
| `Type` | IPropertyType | PrimitiveType o ValueObjectType |
| `IsArray` | bool | Permite múltiples valores |

### RelationDefinition

Define una relación entre tipos complejos:

| Propiedad | Tipo | Descripción |
|-----------|------|-------------|
| `Guid` | Guid | Identificador único |
| `Name` | string | Nombre en snake_case |
| `Type` | ComplexType | Tipo destino de la relación |
| `IsArray` | bool | Permite múltiples referencias |
| `IsComposition` | bool | Si es true, los hijos se eliminan con el padre |

---

## Sistema de Métodos

Los tipos estructurales pueden tener métodos, incluyendo constructores:

```csharp
// Crear un constructor
var constructor = new Method();  // IsConstructor = true
constructor.AddParameter("name", PrimitiveType.String);
constructor.AddParameter("age", PrimitiveType.Int);

// Método regular
var method = new Method("CalculateAge", PrimitiveType.Int);
method.AddParameter("birth_date", PrimitiveType.DateTime);

// Obtener firma
constructor.GetSignature();  // "void _constructor(String name, Int age)"
method.GetSignature();       // "Int CalculateAge(DateTime birth_date)"
```

### MethodContext

Contexto de ejecución que almacena instancias durante la ejecución de un método:

```csharp
var context = new MethodContext();
context.AddInstance("customer", customerInstance);
var customer = context.GetInstance("customer");
```

---

## Validaciones

### Nombres

- **Tipos**: PascalCase (`Person`, `OrderLine`)
- **Propiedades y Relaciones**: snake_case (`first_name`, `order_date`)

### Propiedades

- El tipo del valor debe coincidir con la definición
- Si `IsArray = false`, solo se permite un valor por propiedad
- Los ValueObjects anidados deben ser del tipo correcto

### Relaciones

- Solo disponibles en `ComplexType`
- Si `IsArray = false`, solo se permite una relación por nombre

---

## Ejemplo Completo

```csharp
// 1. Definir tipos
var addressType = new ValueObjectType("Address");
addressType.AddProperty("street", PrimitiveType.String);
addressType.AddProperty("city", PrimitiveType.String);

var personType = new ComplexType("Person");
personType.AddProperty("name", PrimitiveType.String);
personType.AddProperty("age", PrimitiveType.Int);
personType.AddProperty("address", addressType);
personType.AddRelation("friends", isArray: true, personType);

// 2. Crear instancias
var address = addressType.CreateInstance(new {
    street = "Calle Mayor 1",
    city = "Barcelona"
});

var juan = personType.CreateInstance(new {
    name = "Juan",
    age = 30,
    address = address
});

var maria = personType.CreateInstance(new {
    name = "María",
    age = 28
});

// 3. Establecer relaciones
juan.SetRelation("friends", maria.Guid);

// 4. Acceder a datos
foreach (var prop in juan.PropertiesAsReadOnly)
{
    if (prop is StringProperty sp)
        Console.WriteLine($"{sp.Name}: {sp.Value}");
    else if (prop is IntProperty ip)
        Console.WriteLine($"{ip.Name}: {ip.Value}");
}
```

---

## Estructura de Carpetas

```
MetaHammer.Domain/
├── Types/
│   ├── Abstract/
│   │   ├── MetaType.cs              # Base de todos los tipos
│   │   ├── StructuralType.cs        # Base para tipos con estructura
│   │   ├── PropertyDefinition.cs    # Definición de propiedad
│   │   └── RelationDefinition.cs    # Definición de relación
│   ├── Interfaces/
│   │   ├── IPropertyType.cs         # Marca tipos usables como propiedad
│   │   └── IStructuralType.cs       # Contrato para tipos estructurales
│   ├── Methods/
│   │   ├── Method.cs                # Método o constructor
│   │   ├── MethodParameter.cs       # Parámetro de método
│   │   ├── MethodInstruction.cs     # Instrucción del cuerpo
│   │   └── MethodContext.cs         # Contexto de ejecución
│   ├── PrimitiveType.cs             # Tipos primitivos singleton
│   ├── ValueObjectType.cs           # Tipo de objeto de valor
│   └── ComplexType.cs               # Tipo complejo (entidad)
│
├── Instances/
│   ├── Abstract/
│   │   ├── MetaInstance.cs          # Base de todas las instancias
│   │   └── MetaInstanceWithProperties.cs  # Base con propiedades
│   ├── Interfaces/
│   │   ├── IProperty.cs             # Contrato de propiedad
│   │   └── IInstanceProperty.cs     # Marca instancias como valor
│   ├── Properties/
│   │   ├── PropertyBase.cs          # Base de propiedades tipadas
│   │   ├── IntProperty.cs
│   │   ├── StringProperty.cs
│   │   ├── BoolProperty.cs
│   │   ├── DecimalProperty.cs
│   │   ├── DoubleProperty.cs
│   │   ├── DateTimeProperty.cs
│   │   ├── GuidProperty.cs
│   │   └── ValueObjectProperty.cs
│   ├── ComplexInstance.cs           # Instancia de tipo complejo
│   ├── ValueObjectInstance.cs       # Instancia de valor
│   └── Relation.cs                  # Relación entre instancias
│
├── Exceptions/
│   └── DomainException.cs           # Excepción de dominio
│
└── Validation/
    └── NameFormatValidator.cs       # Validación de nombres
```

---

## Patrones de Diseño Utilizados

| Patrón | Uso |
|--------|-----|
| **Singleton** | `PrimitiveType` garantiza una instancia por tipo |
| **Factory Method** | `CreateInstance()` en tipos estructurales |
| **Template Method** | `MetaInstanceWithProperties` define flujo de validación |
| **Strategy** | Propiedades polimórficas (`IProperty`) |
| **Meta-Object Protocol** | Separación tipos/instancias |

---

## Consideraciones de Diseño

1. **Tipos definen estructura, Instancias contienen datos**: Los tipos no almacenan valores, solo definen qué propiedades y relaciones son válidas.

2. **Propiedades tipadas**: Usar clases específicas (`IntProperty`, `StringProperty`) en lugar de `object` proporciona type-safety y evita boxing.

3. **Relaciones por GUID**: Las relaciones almacenan solo el GUID de la instancia destino, no una referencia directa. Esto facilita la persistencia y evita referencias circulares.

4. **Validación en asignación**: Las validaciones ocurren al asignar valores (`SetPropertyValue`, `SetRelation`), garantizando consistencia.

5. **Inmutabilidad de ValueObjects**: Los `ValueObjectInstance` están diseñados para ser inmutables conceptualmente, comparándose por valor.
