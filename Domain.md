# MetaHammer Domain - Sistema de Metamodelado

MetaHammer es un sistema de **metamodelado** que permite definir tipos personalizados en tiempo de ejecución y crear instancias de esos tipos. Es un "sistema de tipos sobre tipos" para modelar dominios de negocio de forma dinámica.

## Arquitectura General

```
┌─────────────────────────────────────────────────────────────────┐
│                      NIVEL META (Definición de Tipos)           │
├─────────────────────────────────────────────────────────────────┤
│                                                                 │
│  PrimitiveMetaType     ValueObjectMetaType      ComplexMetaType │
│  ┌─────────────┐       ┌─────────────────┐     ┌─────────────┐  │
│  │ IPropertyType│       │ IPropertyType   │     │ IMetaType   │  │
│  │             │       │ + Properties    │     │ + Properties│  │
│  │ String, Int │       │   - street      │     │ + Relations │  │
│  │ Bool, etc.  │       │   - city        │     │ + Methods   │  │
│  └─────────────┘       └─────────────────┘     └─────────────┘  │
│                                                                 │
├─────────────────────────────────────────────────────────────────┤
│                   NIVEL INSTANCIA (Objetos)                     │
├─────────────────────────────────────────────────────────────────┤
│                                                                 │
│                          MetaObject                             │
│  ┌──────────────┐  ┌────────────────┐  ┌────────────────────┐   │
│  │ type: String │  │ type: Address  │  │ type: Person       │   │
│  │ value: "Hola"│  │ street: "..."  │  │ name: "Juan"       │   │
│  └──────────────┘  │ city: "..."    │  │ addresses: [...]   │   │
│                    └────────────────┘  └────────────────────┘   │
│                                                                 │
└─────────────────────────────────────────────────────────────────┘
```

## Jerarquía de Clases

### Clases Base (Common)

```
Entity                    → Identidad con Guid
  └── AggregateRoot       → + Eventos de dominio
```

| Clase | Responsabilidad |
|-------|-----------------|
| `Entity` | Identidad única (`Guid`) |
| `AggregateRoot` | Gestión de eventos de dominio (`DomainEvents`) |

### Jerarquía de MetaTypes

```
MetaType (AggregateRoot)
├── PrimitiveMetaType ─────────────────► IPropertyType
└── MetaTypeWithProperties (abstract)
    ├── ValueObjectMetaType ───────────► IPropertyType
    └── ComplexMetaType ───────────────► IMetaType
```

### Interfaces

| Interface | Hereda de | Implementado por | Propósito |
|-----------|-----------|------------------|-----------|
| `IMetaType` | - | ComplexMetaType | Tipos usables en MetaObject |
| `IPropertyType` | IMetaType | PrimitiveMetaType, ValueObjectMetaType | Tipos usables como valor de propiedades |

## Tipos de MetaType

### 1. PrimitiveMetaType

Tipos primitivos nativos del sistema.

```csharp
var stringType = new PrimitiveMetaType("String");
var intType = new PrimitiveMetaType(Guid.NewGuid(), "Int");
```

**Características:**
- Implementa `IPropertyType` (puede usarse como propiedad)
- Sin propiedades ni relaciones
- Nombres en PascalCase

### 2. ValueObjectMetaType

Objetos de valor inmutables compuestos.

```csharp
var addressType = new ValueObjectMetaType("Address");
addressType.AddProperty("street", stringType);
addressType.AddProperty("city", stringType);
addressType.AddProperty("zip_code", intType);
```

**Características:**
- Implementa `IPropertyType` (puede usarse como propiedad de otros tipos)
- Hereda de `MetaTypeWithProperties` → tiene propiedades
- Las propiedades solo aceptan `IPropertyType` (Primitivo o ValueObject)
- Sin relaciones

### 3. ComplexMetaType

Entidades de dominio con identidad, propiedades, relaciones y comportamiento.

```csharp
var personType = new ComplexMetaType(Guid.NewGuid(), "Person");
personType.AddProperty("name", stringType);
personType.AddProperty("age", intType);
personType.AddRelation("home_address", addressType, isArray: false, isComposition: true);
personType.AddMethod("GetFullName", stringType);
```

**Características:**
- Implementa `IMetaType`
- Hereda de `MetaTypeWithProperties` → tiene propiedades
- Puede tener relaciones (`MetaRelation`) con otros `ComplexMetaType`
- Puede definir métodos y constructores
- Parámetro opcional `isNative` en constructor

## Componentes de un Tipo

### MetaProperty

Propiedad de valor en tipos con propiedades.

```csharp
public void AddProperty(string name, IPropertyType propertyType, bool isArray = false)
```

| Campo | Tipo | Descripción |
|-------|------|-------------|
| `Name` | string | Nombre en snake_case |
| `MetaType` | IPropertyType | Tipo de la propiedad |
| `IsArray` | bool | Si es colección |

### MetaRelation

Relación entre tipos complejos (solo en `ComplexMetaType`).

```csharp
public void AddRelation(string name, ComplexMetaType relationType, bool isArray = false, bool isComposition = true)
```

| Campo | Tipo | Descripción |
|-------|------|-------------|
| `Name` | string | Nombre en snake_case |
| `MetaType` | ComplexMetaType | Tipo relacionado |
| `IsArray` | bool | Si es colección |
| `IsComposition` | bool | `true` = composición, `false` = agregación |

## Sistema de Métodos

Todos los `MetaType` pueden definir métodos.

### Method

```csharp
var method = personType.AddMethod("Calculate", intType, returnsArray: false, isStatic: false);
method.AddParameter("factor", intType);
method.AddInstruction(otherMethod, arguments);
```

| Campo | Tipo | Descripción |
|-------|------|-------------|
| `Name` | string | Nombre del método |
| `ReturnType` | IMetaType? | Tipo de retorno (null = void) |
| `IsArray` | bool | Si retorna colección |
| `IsStatic` | bool | Método de clase vs instancia |
| `IsConstructor` | bool | Si es constructor (`_constructor`) |
| `IsNative` | bool | Implementado nativamente |
| `ParentTypeId` | Guid | ID del tipo padre |

**Métodos:**
- `Parameters()` → Parámetros ordenados
- `Instructions()` → Instrucciones ordenadas
- `GetSignature()` → Firma completa del método

### Parameter

```csharp
method.AddParameter("name", stringType, isArray: false);
```

| Campo | Tipo | Descripción |
|-------|------|-------------|
| `Name` | string | Nombre del parámetro |
| `Type` | IMetaType | Tipo del parámetro |
| `IsArray` | bool | Si es colección |
| `Order` | int | Posición (1-based) |

### Instruction

Una instrucción es una **llamada a un método** con argumentos.

```csharp
method.AddInstruction(targetMethod, new List<Argument> { arg1, arg2 });
```

| Campo | Descripción |
|-------|-------------|
| `ParentMethod` | Método que contiene esta instrucción |
| `InvokedMethod` | Método que se invoca |
| `Order` | Orden de ejecución |
| `Arguments` | Lista de argumentos |

**Validación:** Al crear una instrucción, se verifica que exista un argumento para cada parámetro del método invocado.

### Argument

Liga un parámetro con una variable del contexto.

```csharp
var arg = new Argument(parameter, "variableName");
```

| Campo | Descripción |
|-------|-------------|
| `Parameter` | Parámetro al que corresponde |
| `VariableNameFromContext` | Nombre de variable en el contexto |

### DesignContext

Tabla de símbolos para validar variables durante el diseño de métodos.

```csharp
var context = new DesignContext(parentScope);
context.Define("person", personType);      // Registra variable
var type = context.Resolve("person");      // Obtiene tipo
bool exists = context.IsDefined("person"); // Verifica existencia
```

## Instancias (MetaObject)

`MetaObject` representa una instancia concreta de cualquier `MetaType`.

### Constructores

```csharp
// Primitivo con valor
new MetaObject(stringType, "Hola")

// ValueObject (sin valor primitivo)
new MetaObject(addressType)

// ComplexType (con identidad)
new MetaObject(personType)

// Referencia lazy (solo Guid, sin cargar datos)
new MetaObject(personType, existingGuid)  // IsLoaded = false
```

### Estructura

| Campo | Tipo | Descripción |
|-------|------|-------------|
| `Guid` | Guid? | Identidad (solo ComplexMetaType) |
| `MetaType` | IMetaType | Tipo del objeto |
| `IsLoaded` | bool | Si está completamente cargado |
| `Value` | object? | Valor escalar (solo primitivos, null para otros) |
| `Properties` | Dictionary | Valores de propiedades |
| `Relations` | Dictionary | Objetos relacionados |

### Operaciones con Propiedades

```csharp
// Establecer propiedad simple
person.SetPropertyValue("name", nameObj);

// Agregar a propiedad array
person.AddItemToProperty("tags", tagObj);

// Leer propiedad simple
var name = person.Property("name");

// Leer propiedad lista
var tags = person.GetPropertyList("tags");
```

### Operaciones con Relaciones

```csharp
// Establecer relación simple
person.SetRelationValue("manager", managerObj);

// Agregar a relación array
person.AddItemToRelation("addresses", addressObj);
```

### Validaciones en MetaObject

| Operación | Validación |
|-----------|------------|
| `SetPropertyValue` | Tipo debe tener propiedades, propiedad debe existir, no debe ser array |
| `AddItemToProperty` | Tipo debe tener propiedades, propiedad debe existir, debe ser array |
| `SetRelationValue` | Tipo debe ser ComplexMetaType, relación debe existir, no debe ser array |
| `AddItemToRelation` | Tipo debe ser ComplexMetaType, relación debe existir, debe ser array |
| `Value` (set) | Solo para PrimitiveMetaType |

## Validación de Nombres

```csharp
NameFormatValidator.ValidatePascalCase(name, "Type");     // ^[A-Z][a-zA-Z0-9]*$
NameFormatValidator.ValidateSnakeCase(name, "Property");  // ^[a-z][a-z0-9]*(_[a-z0-9]+)*$
```

| Elemento | Formato | Ejemplos |
|----------|---------|----------|
| Tipos | PascalCase | `Person`, `OrderLine`, `String` |
| Propiedades | snake_case | `first_name`, `order_date` |
| Relaciones | snake_case | `home_address`, `line_items` |

## Versionado

Todos los `MetaType` tienen control de versión:

```csharp
type.Version              // Versión actual (inicia en 1)
type.SetVersion(n)        // Establecer versión explícita
type.IncrementVersion()   // Incrementar (protected)
```

## Ejemplo Completo

```csharp
// 1. Tipos primitivos
var stringType = new PrimitiveMetaType("String");
var intType = new PrimitiveMetaType("Int");

// 2. ValueObject
var addressType = new ValueObjectMetaType("Address");
addressType.AddProperty("street", stringType);
addressType.AddProperty("city", stringType);
addressType.AddProperty("zip_code", intType);

// 3. ComplexType con propiedades, relaciones y métodos
var personType = new ComplexMetaType(Guid.NewGuid(), "Person");
personType.AddProperty("name", stringType);
personType.AddProperty("age", intType);
personType.AddRelation("addresses", addressType, isArray: true, isComposition: true);

var greetMethod = personType.AddMethod("Greet", stringType);
greetMethod.AddParameter("greeting", stringType);

// 4. Crear instancias
var address = new MetaObject(addressType);
address.SetPropertyValue("street", new MetaObject(stringType, "Calle 123"));
address.SetPropertyValue("city", new MetaObject(stringType, "Buenos Aires"));
address.SetPropertyValue("zip_code", new MetaObject(intType, 1234));

var person = new MetaObject(personType);
person.SetPropertyValue("name", new MetaObject(stringType, "Juan"));
person.SetPropertyValue("age", new MetaObject(intType, 30));
person.AddItemToRelation("addresses", address);

// 5. Leer valores
var name = person.Property("name");           // MetaObject
var allAddresses = person.Relations;          // Dictionary
```

## Resumen de Restricciones

| Regla | Descripción |
|-------|-------------|
| Propiedades (ValueObject/Complex) | Solo tipos `IPropertyType` (Primitivo o ValueObject) |
| Relaciones | Solo entre `ComplexMetaType` |
| Nombres de tipos | PascalCase obligatorio |
| Nombres de propiedades/relaciones | snake_case obligatorio |
| `Value` en MetaObject | Solo disponible para `PrimitiveMetaType` |
| Argumentos en Instruction | Debe existir uno por cada parámetro del método |
