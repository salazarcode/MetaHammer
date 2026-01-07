# MetaHammer Domain - Sistema de Metamodelado

MetaHammer es un sistema de **metamodelado** que permite definir tipos personalizados en tiempo de ejecución y crear instancias de esos tipos. Es esencialmente un "sistema de tipos sobre tipos" que permite modelar dominios de negocio de forma dinámica.

## Arquitectura General

```
┌─────────────────────────────────────────────────────────────┐
│                      NIVEL META (Tipos)                     │
├─────────────────────────────────────────────────────────────┤
│  PrimitiveMetaType    ValueObjectMetaType    ComplexMetaType│
│       (String)           (Address)              (Person)    │
│                      ┌─────────────┐       ┌─────────────┐  │
│                      │ Properties  │       │ Properties  │  │
│                      │ - street    │       │ - name      │  │
│                      │ - city      │       │ Relations   │  │
│                      └─────────────┘       │ - addresses │  │
│                                            │ Methods     │  │
│                                            └─────────────┘  │
├─────────────────────────────────────────────────────────────┤
│                   NIVEL INSTANCIA (Objetos)                 │
├─────────────────────────────────────────────────────────────┤
│                         MetaObject                          │
│  ┌──────────────┐  ┌──────────────┐  ┌──────────────────┐   │
│  │ type: String │  │type: Address │  │  type: Person    │   │
│  │ value: "Hola"│  │ street: "..."│  │  name: "Juan"    │   │
│  └──────────────┘  │ city: "..."  │  │  addresses: [...]│   │
│                    └──────────────┘  └──────────────────┘   │
└─────────────────────────────────────────────────────────────┘
```

## Jerarquía de Tipos

### Clases Base (Common)

| Clase | Descripción |
|-------|-------------|
| `Entity` | Clase base con identidad (`Guid`) |
| `AggregateRoot` | Entidad raíz con soporte para eventos de dominio |
| `IDomainEvent` | Contrato para eventos de dominio |

### Jerarquía de MetaTypes

```
MetaType (Base)
├── PrimitiveMetaType      → IMetaType
└── MetaTypeWithProperties
    ├── ValueObjectMetaType → IPropertyType
    └── ComplexMetaType     → IMetaType
```

### Interfaces

| Interface | Propósito |
|-----------|-----------|
| `IMetaType` | Contrato común: `Guid`, `Name`, `Methods` |
| `IPropertyType` | Marca tipos usables como valor de propiedades (Primitivo y ValueObject) |

## Tipos de MetaType

### 1. PrimitiveMetaType

Representa tipos primitivos nativos del sistema (String, Int, Bool, etc.).

```csharp
var stringType = new PrimitiveMetaType("String");
var intType = new PrimitiveMetaType("Int");
```

**Características:**
- No tiene propiedades ni relaciones
- Usado para valores escalares
- Los nombres deben ser PascalCase

### 2. ValueObjectMetaType

Representa objetos de valor inmutables compuestos por propiedades primitivas u otros ValueObjects.

```csharp
var addressType = new ValueObjectMetaType("Address");
addressType.AddProperty("street", stringType);
addressType.AddProperty("city", stringType);
addressType.AddProperty("zip_code", intType);
```

**Características:**
- Puede tener propiedades (`MetaProperty`)
- Las propiedades solo pueden ser de tipo `IPropertyType` (Primitivo o ValueObject)
- No tiene relaciones con otros objetos complejos
- Implementa `IPropertyType` (puede usarse como propiedad de otros tipos)

### 3. ComplexMetaType

Representa entidades de dominio completas con identidad, propiedades, relaciones y métodos.

```csharp
var personType = new ComplexMetaType(Guid.NewGuid(), "Person");
personType.AddProperty("name", stringType);
personType.AddProperty("age", intType);
personType.AddRelation("addresses", addressType, isArray: true, isComposition: true);
```

**Características:**
- Puede tener propiedades (`MetaProperty`)
- Puede tener relaciones (`MetaRelation`) con otros `ComplexMetaType`
- Puede definir métodos y constructores
- Soporta composición y agregación

## Componentes de un Tipo

### MetaProperty

Define una propiedad de valor en un tipo.

| Campo | Tipo | Descripción |
|-------|------|-------------|
| `Name` | string | Nombre en snake_case |
| `MetaType` | IPropertyType | Tipo de la propiedad |
| `IsArray` | bool | Si es una colección |

### MetaRelation

Define una relación entre tipos complejos.

| Campo | Tipo | Descripción |
|-------|------|-------------|
| `Name` | string | Nombre en snake_case |
| `MetaType` | ComplexMetaType | Tipo relacionado |
| `IsArray` | bool | Si es una colección |
| `IsComposition` | bool | true = composición, false = agregación |

## Sistema de Métodos

Los tipos pueden definir métodos que representan comportamiento.

### Method

```csharp
var method = personType.AddMethod("GetFullName", stringType);
method.AddParameter("separator", stringType);
method.AddInstruction(concatMethod, arguments);
```

| Campo | Descripción |
|-------|-------------|
| `Name` | Nombre del método |
| `ReturnType` | Tipo de retorno (null para void) |
| `IsArray` | Si retorna colección |
| `IsStatic` | Método de clase vs instancia |
| `IsConstructor` | Si es constructor |
| `IsNative` | Si es implementado nativamente |
| `Parameters()` | Lista de parámetros |
| `Instructions()` | Lista de instrucciones |

### Parameter

Define un parámetro de un método.

```csharp
method.AddParameter("name", stringType, isArray: false);
```

### Instruction

Una instrucción es siempre una **llamada a otro método** con argumentos.

```csharp
method.AddInstruction(targetMethod, arguments);
```

### Argument

Liga un parámetro con una variable del contexto de diseño.

```csharp
var arg = new Argument(parameter, "variableName");
```

### DesignContext

Tabla de símbolos para validar variables durante el diseño de métodos.

```csharp
var context = new DesignContext(parentScope);
context.Define("name", stringType);
var type = context.Resolve("name"); // Obtiene el tipo de la variable
```

## Instancias (MetaObject)

`MetaObject` es la instancia concreta de cualquier `MetaType`.

### Creación de Instancias

```csharp
// Primitivo con valor
var strObj = new MetaObject(stringType, "Hola") { MetaType = stringType };

// ValueObject
var addressObj = new MetaObject(addressType) { MetaType = addressType };

// ComplexType
var personObj = new MetaObject(personType) { MetaType = personType };

// Referencia lazy (solo Guid, sin cargar)
var lazyRef = new MetaObject(personType, existingGuid) { MetaType = personType };
```

### Estructura de MetaObject

| Campo | Tipo | Descripción |
|-------|------|-------------|
| `Guid` | Guid? | Identidad (solo ComplexMetaType) |
| `MetaType` | IMetaType | Tipo del objeto |
| `IsLoaded` | bool | Si está completamente cargado |
| `Value` | object? | Valor (solo primitivos) |
| `Properties` | Dictionary | Valores de propiedades |
| `Relations` | Dictionary | Objetos relacionados |

### Operaciones

```csharp
// Establecer propiedad simple
personObj.SetPropertyValue("name", nameObj);

// Agregar a propiedad array
personObj.AddItemToProperty("tags", tagObj);

// Establecer relación simple
personObj.SetRelationValue("manager", managerObj);

// Agregar a relación array
personObj.AddItemToRelation("addresses", addressObj);
```

## Validaciones

### NameFormatValidator

| Método | Formato | Ejemplo |
|--------|---------|---------|
| `ValidatePascalCase` | `^[A-Z][a-zA-Z0-9]*$` | `Person`, `OrderLine` |
| `ValidateSnakeCase` | `^[a-z][a-z0-9]*(_[a-z0-9]+)*$` | `first_name`, `order_date` |

**Aplicación:**
- Tipos → PascalCase
- Propiedades y Relaciones → snake_case

### DomainException

Excepción personalizada para errores de dominio (validaciones, reglas de negocio).

## Versionado

Todos los `MetaType` tienen control de versión:

```csharp
type.Version        // Versión actual (inicia en 1)
type.SetVersion(n)  // Establecer versión
type.IncrementVersion() // Incrementar (protected)
```

## Ejemplo Completo

```csharp
// 1. Definir tipos primitivos
var stringType = new PrimitiveMetaType("String");
var intType = new PrimitiveMetaType("Int");

// 2. Definir ValueObject
var addressType = new ValueObjectMetaType("Address");
addressType.AddProperty("street", stringType);
addressType.AddProperty("city", stringType);

// 3. Definir ComplexType
var personType = new ComplexMetaType(Guid.NewGuid(), "Person");
personType.AddProperty("name", stringType);
personType.AddProperty("age", intType);
personType.AddRelation("home_address", addressType, isArray: false, isComposition: true);

// 4. Crear instancias
var nameValue = new MetaObject(stringType, "Juan") { MetaType = stringType };
var ageValue = new MetaObject(intType, 30) { MetaType = intType };

var address = new MetaObject(addressType) { MetaType = addressType };
address.SetPropertyValue("street", new MetaObject(stringType, "Calle 123") { MetaType = stringType });
address.SetPropertyValue("city", new MetaObject(stringType, "Buenos Aires") { MetaType = stringType });

var person = new MetaObject(personType) { MetaType = personType };
person.SetPropertyValue("name", nameValue);
person.SetPropertyValue("age", ageValue);
person.SetRelationValue("home_address", address);
```

## Resumen de Restricciones

| Regla | Descripción |
|-------|-------------|
| Propiedades de ValueObject | Solo pueden ser Primitivo o ValueObject |
| Propiedades de ComplexType | Solo pueden ser Primitivo o ValueObject |
| Relaciones | Solo entre ComplexMetaType |
| Nombres de tipos | PascalCase obligatorio |
| Nombres de propiedades/relaciones | snake_case obligatorio |
| Valor en MetaObject | Solo disponible para PrimitiveMetaType |
