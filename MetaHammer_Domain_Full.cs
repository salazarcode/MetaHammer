// MetaHammer.Domain - Full Project Classes
// Generated on: 2026-02-01

// --------------------------------------------------------------------------------
// File: MetaHammer.Domain/Features/Objects/MetaObject.cs
// --------------------------------------------------------------------------------
using MetaHammer.Domain.Exceptions;
using MetaHammer.Domain.Features.Classes;
using MetaHammer.Domain.Features.Organizations;
using MetaHammer.Domain.Features.Organizations.Identity;

namespace MetaHammer.Domain.Features.Objects;

public class MetaObject
{
    public MetaObject(MetaClass metaClass, Guid guid, Guid tenantGuid, Guid createdByGuid)
    {
        Guid = guid;
        MetaClass = metaClass;
        TenantGuid = tenantGuid;
        CreatedByGuid = createdByGuid;
    }
    
    public Guid? Guid { get; init; } = null;
    public bool IsLoaded { get; set; } = true;
    public Guid CreatedByGuid { get; set; }
    public User? CreatedBy { get; set; }
    public Guid TenantGuid { get; set; }
    public Organization? Tenant { get; set; }
    public MetaClass MetaClass { get; private set; }
    private object? Value { get; set; } = null;

    private Dictionary<string, MetaObject> _properties { get; set; } = new();
    
    public MetaObject? Property(string propertyName)
    {
        var property = MetaClass.Properties.FirstOrDefault(x => x.Name ==  propertyName);
        
        if(property is null)
            throw new DomainException($"Property '{propertyName}' does not exist in MetaClass '{MetaClass.Name}'");
        
        _properties.TryGetValue(propertyName, out var metaObject);
        
        return metaObject;
    }
    
    public void SetProperty(string propertyName, MetaObject metaObject)
    {
        var property = MetaClass.Properties.FirstOrDefault(x => x.Name == propertyName);
        
        if(property is null)
            throw new DomainException($"Property '{propertyName}' does not exist in MetaClass '{MetaClass.Name}'");
        
        if(property.MetaClass.Guid != metaObject.MetaClass.Guid)
            throw new DomainException($"Property '{propertyName}' is of type '{property.MetaClass.Name}' but got '{metaObject.MetaClass.Name}'");
        
        _properties[propertyName] = metaObject;
    }
    
}

// --------------------------------------------------------------------------------
// File: MetaHammer.Domain/Features/Classes/MetaClass.cs
// --------------------------------------------------------------------------------
using MetaHammer.Domain.Common;
using MetaHammer.Domain.Exceptions;
using MetaHammer.Domain.Features.Classes.Enums;
using MetaHammer.Domain.Features.Classes.Methods;
using MetaHammer.Domain.Features.Organizations.Identity;
using MetaHammer.Domain.Features.Organizations;
using MetaHammer.Domain.Validation;

namespace MetaHammer.Domain.Features.Classes;

/// <summary>
/// Tipo base que contiene cualquier clase.
/// </summary>
public class MetaClass : AggregateRoot
{
    public MetaClass(Guid guid, string name, MetaNature metaNature, Organization organization, User user, MetaClass? parentClass = null, List<MetaClass>? interfaces = null) : base(guid)
    {
        NameFormatValidator.ValidatePascalCase(name, "Type");
        Name = name;
        Version = 1;
        MetaNature = metaNature;
        
        Organization = organization;
        OrganizationGuid = organization.Guid;
        CreatedBy = user; 
        CreatedByGuid = user.Guid;
        
        
        var allowedClassNaturesForParentClass = new List<MetaNature>()
        {
            MetaNature.Primitive,
            MetaNature.Entity,
            MetaNature.ValueObject
        };
        
        if (parentClass is not null)
            if (allowedClassNaturesForParentClass.Contains(parentClass.MetaNature))
            {
                ParentClass = parentClass;
                ParentClassGuid = parentClass.Guid;
            }
        
        if(interfaces is not null)
        {
            if (interfaces.Any(x => x.MetaNature != MetaNature.Interface))
                throw new DomainException("Interfaces collection must only contain MetaClasses with MetaNature = MetaNature.INTERFACE");
            
            foreach (var interfaceMetaClass in interfaces)
                Interfaces.Add(interfaceMetaClass);
        }
    }
    
    #region Identity
    
    public string Name { get; init; }

    public MetaNature MetaNature { get; set; }

    public MetaClass? ParentClass { get; set; }

    public Guid ParentClassGuid { get; set; }
    
    public List<MetaClass> Interfaces { get; set; } = new();

    //Un tipo le pertenece a un organization
    public Guid OrganizationGuid { get; private set; }
    
    //Un tipo tiene una propiedad lazyloading para el Organization
    private Organization Organization { get; init; }
    
    //Un tipo es creado por un usuario
    public Guid CreatedByGuid { get; private set; }
    
    //Un tipo tiene una propiedad lazyloading para el usuario creador
    private User CreatedBy { get; init; }

    #endregion

    #region Versioning
    public int Version { get; private set; }
    
    public void SetVersion(int version) => Version = version;
    
    protected void IncrementVersion() => Version++;
    
    #endregion

    #region Methods
    
    private List<MetaMethod> Methods { get; set; } = new();
    public IReadOnlyCollection<MetaMethod> GetMethods => Methods.AsReadOnly();

    public IReadOnlyCollection<MetaMethod> Constructors() => Methods.Where(m => m.IsConstructor).ToList().AsReadOnly();

    public MetaMethod AddMethod(string name, MetaClass? returnClass, bool returnsArray = false, bool isStatic = false)
    {
        var method = new MetaMethod(name, returnClass, returnsArray, isStatic);
        Methods.Add(method);
        return method;
    }
    public MetaMethod AddConstructor()
    {
        var method = new MetaMethod("_constructor", null, false, false, true);
        Methods.Add(method);
        return method;
    }
    public MetaMethod? GetConstructorBySignature(params MetaClass[] parameterTypes)
    {
        var signature = string.Join("_constructor(", parameterTypes.Select(t => t.Name), ")");
        var constructor = GetMethods.FirstOrDefault(m => m.GetSignature() == signature);
        return constructor;
    }

    public MetaMethod? Method(string methodName)
    {
        var method = Methods.FirstOrDefault(m => m.Name == methodName);
        if(method is null)
            throw new Exception($"El método '{methodName}' no existe en el tipo '{this.Name}'.");
        return method;
    }
    #endregion
    
    #region Properties
    private List<MetaProperty> _properties { get; set; } = new();
    
    public IReadOnlyCollection<MetaProperty> Properties => _properties.AsReadOnly();

    public void AddProperty(string name, MetaClass propertyClass, bool isCollection = false)
    {

        var property = Properties.FirstOrDefault(p => p.Name == name && p.MetaClass.Name == propertyClass.Name);
        
        if(property == null)
        {
            _properties.Add(new MetaProperty(Guid.NewGuid(), name, propertyClass, isCollection));
        }
        else
        {
            throw new DomainException($"La propiedad con nombre '{name}' y tipo {property.MetaClass.Name} ya existe en el tipo '{this.Name}'.");
        }
    }
    
    public MetaProperty? Property(string propertyName) =>Properties.FirstOrDefault(p => p.Name == propertyName);
    #endregion
}

// --------------------------------------------------------------------------------
// File: MetaHammer.Domain/Services/Interpreter/ExpressionEvaluator/MetaObjectAccessor.cs
// --------------------------------------------------------------------------------
using System.Dynamic;
using MetaHammer.Domain.Features.Objects;

namespace MetaHammer.Domain.Services.Interpreter.ExpressionEvaluator;

public class MetaObjectAccessor : DynamicObject
{
    private readonly MetaObject _target;

    public MetaObjectAccessor(MetaObject target)
    {
        _target = target;
    }

    // Esta es la magia: Intercepta "objeto.Propiedad"
    public override bool TryGetMember(GetMemberBinder binder, out object? result)
    {
        if (_target.Properties.TryGetValue(binder.Name, out var value))
        {
            result = WrapValue(value);
            return true;
        }

        // Si la propiedad no existe, devolvemos null (o puedes lanzar excepción si prefieres strict mode)
        result = null;
        return true; 
    }

    // Helper recursivo vital para navegación profunda (ej: user.Direccion.Ciudad)
    // y para que LINQ funcione sobre listas (ej: user.Pedidos.Sum(...))
    private object? WrapValue(object? value)
    {
        if (value == null) return null;

        // 1. Si es un hijo MetaObject, lo envolvemos también
        if (value is MetaObject metaObj)
            return new MetaObjectAccessor(metaObj);

        // 2. Si es una lista de MetaObjects, proyectamos cada uno a un Accessor
        // Esto permite hacer .Where(), .Sum(), .First() dentro de la expresión string
        if (value is IEnumerable<MetaObject> list)
            return list.Select(x => new MetaObjectAccessor(x)).ToList();

        // 3. Si es primitivo (int, string, bool), se devuelve crudo
        return value;
    }
}

// --------------------------------------------------------------------------------
// File: MetaHammer.Domain/Services/Interpreter/ExpressionEvaluator/ExpressionEvaluator.cs
// --------------------------------------------------------------------------------
namespace MetaHammer.Domain.Services.Interpreter.ExpressionEvaluator;

public class ExpressionEvaluator
{
    
}

// --------------------------------------------------------------------------------
// File: MetaHammer.Domain/Services/Interpreter/Interpreter.cs
// --------------------------------------------------------------------------------
namespace MetaHammer.Domain.Services.Interpreter;

public class Interpreter
{
    
}

// --------------------------------------------------------------------------------
// File: MetaHammer.Domain/Features/Classes/Methods/Statements/ReturnStatement.cs
// --------------------------------------------------------------------------------
namespace MetaHammer.Domain.Features.Classes.Methods;

public class ReturnStatement
{
    
}

// --------------------------------------------------------------------------------
// File: MetaHammer.Domain/Features/Classes/Methods/Statements/NewStatement.cs
// --------------------------------------------------------------------------------
namespace MetaHammer.Domain.Features.Classes.Methods;

public class NewStatement
{
    
}

// --------------------------------------------------------------------------------
// File: MetaHammer.Domain/Features/Classes/Methods/Statements/AssignStatement.cs
// --------------------------------------------------------------------------------
namespace MetaHammer.Domain.Features.Classes.Methods;

public class AssignStatement
{
    
}

// --------------------------------------------------------------------------------
// File: MetaHammer.Domain/Features/Classes/Methods/Statements/ForStatement.cs
// --------------------------------------------------------------------------------
namespace MetaHammer.Domain.Features.Classes.Methods;

public class ForStatement
{
    
}

// --------------------------------------------------------------------------------
// File: MetaHammer.Domain/Features/Classes/Methods/Statements/ForEachStatement.cs
// --------------------------------------------------------------------------------
namespace MetaHammer.Domain.Features.Classes.Methods;

public class ForEachStatement
{
    
}

// --------------------------------------------------------------------------------
// File: MetaHammer.Domain/Features/Classes/Methods/Statements/WhileStatement.cs
// --------------------------------------------------------------------------------
namespace MetaHammer.Domain.Features.Classes.Methods;

public class WhileStatement
{
    
}

// --------------------------------------------------------------------------------
// File: MetaHammer.Domain/Features/Classes/Methods/Statements/SwitchStatement.cs
// --------------------------------------------------------------------------------
namespace MetaHammer.Domain.Features.Classes.Methods;

public class SwitchStatement
{
    
}

// --------------------------------------------------------------------------------
// File: MetaHammer.Domain/Features/Classes/Methods/Statements/IfStatement.cs
// --------------------------------------------------------------------------------
namespace MetaHammer.Domain.Features.Classes.Methods;

public class IfStatement
{
    
}

// --------------------------------------------------------------------------------
// File: MetaHammer.Domain/Features/Classes/Methods/Block.cs
// --------------------------------------------------------------------------------
using MetaHammer.Domain.Common;

namespace MetaHammer.Domain.Features.Classes.Methods;

public class Block : Entity
{
    public List<BaseStatement> Statements { get; set; }
    public Scope Scope { get; set; } 
    public Block(Guid guid) : base(guid)
    {
        Scope = new Scope(Guid.NewGuid());
    }
}

// --------------------------------------------------------------------------------
// File: MetaHammer.Domain/Features/Classes/Methods/Statements/Base/BaseStatement.cs
// --------------------------------------------------------------------------------
using MetaHammer.Domain.Common;
using MetaHammer.Domain.Exceptions;

namespace MetaHammer.Domain.Features.Classes.Methods;

public abstract class BaseStatement : Entity
{
    public BaseStatement(Guid guid) : base(guid)
    {
    }
}

// --------------------------------------------------------------------------------
// File: MetaHammer.Domain/Features/Classes/MetaMethod.cs
// --------------------------------------------------------------------------------
using MetaHammer.Domain.Common;
using MetaHammer.Domain.Features.Classes.Enums;
using MetaHammer.Domain.Features.Classes.Methods;

namespace MetaHammer.Domain.Features.Classes;

public class MetaMethod : Entity
{
    public MetaMethod(string name, MetaClass? returnClass, bool returnsCollection = false, bool isStatic = false, bool isConstructor = false, bool isNative = false, bool isAbstract = false) : base(System.Guid.NewGuid())
    {
        Name = name;
        IsStatic = isStatic;
        IsConstructor = isConstructor;
        ReturnClass = returnClass;
        ReturnsCollection = returnsCollection;
        IsNative = isNative;
        IsAbstract = isAbstract;
        
        if(returnClass != null)
            if(returnClass.MetaNature is not MetaNature.Interface or MetaNature.Abstract)
            {
                Block = new Block(Guid.NewGuid());
            }
    }
    public string Name { get; private set; }
    public bool IsStatic { get; private set; }
    public bool IsConstructor { get; private set; }
    public bool IsAbstract { get; set; }
    //Bloque de instrucciones que es nulo para metodos de interfaces y/o metodos abstractos
    //Contiene las instrucciones del metodo
    public Block? Block { get; set; }
    public MetaClass? ReturnClass { get; set; }
    public bool ReturnsCollection { get; private set; }
    public bool IsNative { get; private set; }
    private List<MetaParameter> _parameters { get; set; } = new();
    private List<BaseStatement> _instructions { get; set; } = new();
    
    [System.Text.Json.Serialization.JsonIgnore]
    public MetaClass ParentClass { get; private set; }

    public IReadOnlyCollection<MetaParameter> Parameters() => _parameters.AsReadOnly();
    public IReadOnlyCollection<BaseStatement> Instructions() => _instructions.AsReadOnly();
    
    /// <summary>
    /// Añade un parámetro al método actual por nombre, tipo y si es array o no.
    /// </summary>
    /// <param name="name"></param>
    /// <param name="type"></param>
    /// <param name="isArray"></param>
    public void AddParameter(string name, MetaClass type, bool isArray = false)
    {
        var parameter = new MetaParameter(name, type, _parameters.Count + 1, isArray);
        _parameters.Add(parameter);
    }

    /// <summary>
    /// Una instrucción es siempre una llamada a un método con sus argumentos.
    /// </summary>
    /// <param name="method"></param>
    /// <param name="arguments"></param>
    public void AddInstruction(MetaMethod invokeMetaMethod, List<Argument> arguments)
    {
        //Por implementar basandonos en la logica nueva de statements
    }

    public string GetSignature()
    {
        var returnClassName = ReturnClass?.Name ?? "void";
        var staticPrefix = IsStatic ? "static" : "";
        var collectionPostfix = ReturnsCollection ? "[]" : "";

        var parameters = string.Join(", ", _parameters
            .OrderBy(p => p.Order)
            .Select(p => $"{p.Type.Name}{(p.IsCollection ? "[]" : "")} {p.Name}"));

        return $"{staticPrefix}{returnClassName}{collectionPostfix} {Name}({parameters})";
    }
}

// --------------------------------------------------------------------------------
// File: MetaHammer.Domain/Features/Organizations/Organization.cs
// --------------------------------------------------------------------------------
using MetaHammer.Domain.Common;
using MetaHammer.Domain.Exceptions;
using MetaHammer.Domain.Features.Classes;
using MetaHammer.Domain.Features.Classes.Enums;
using MetaHammer.Domain.Features.Organizations.Identity;

namespace MetaHammer.Domain.Features.Organizations;

public class Organization : Entity
{
    public Organization(Guid guid, string name) : base(guid)
    {
        Name = name;
    }
    
    //Name
    public string Name { get; set; }
    
    //Tiene muchos tipos
    public List<MetaClass> Classes { get; set; } = new();
    
    //Tiene muchos usuarios
    public List<User> Users { get; set; } = new();
    
    //Tiene muchos grupos
    public List<Group> Groups { get; set; } = new();
    
    //Tiene muchos permisos
    public List<Permission> Permissions { get; set; } = new();
    
    //El organization tiene un metodo para agregar grupos
    public Group AddGroup(string name)
    {
        if (Groups.FirstOrDefault(x => x.Name == name) != null)
            throw new DomainException($"El organization ya tiene un permiso de nombre '{name}'");
        
        var group = new Group(Guid.NewGuid(), name);
        Groups.Add(group);
        return group;

    }
    
    //Metodo para agregar permisos al organization
    public Permission AddPermission(string name)
    {
        if (Permissions.FirstOrDefault(x => x.Name == name) != null)
            throw new DomainException($"El organization ya tiene un permiso de nombre '{name}'");

        var permission = new Permission(Guid.NewGuid(), name);
        Permissions.Add(permission);
        return permission;
    }

    public User AddUser(Guid guid, string userName, Guid? creatorGuid = null)
    {
        if (Users.FirstOrDefault(x => x.Credential.Username == userName) != null)
            throw new DomainException($"El organization ya tiene un permiso de userName '{userName}'");
        
        var user = new User(this, guid, userName);
        return user;
    }

    public MetaClass AddMetaClass(string name, MetaNature metaNature, User user)
    {
        if (Classes.Any(x => x.Name == name))
            throw new DomainException($"El organization ya tiene una clase de nombre '{name}'");
        
        var metaClass = new MetaClass(Guid.NewGuid(), name, metaNature, this, user);
        return metaClass;
    }
    
}

// --------------------------------------------------------------------------------
// File: MetaHammer.Domain/Features/Classes/Methods/Expression.cs
// --------------------------------------------------------------------------------
namespace MetaHammer.Domain.Features.Classes.Methods;

public class Expression
{
    
}

// --------------------------------------------------------------------------------
// File: MetaHammer.Domain/Features/Classes/Methods/Scope.cs
// --------------------------------------------------------------------------------
using MetaHammer.Domain.Common;
using MetaHammer.Domain.Exceptions;

namespace MetaHammer.Domain.Features.Classes.Methods;

public class Scope(Guid guid) : Entity(guid)
{
    // Diccionario de: Nombre de Variable -> Definición del Tipo
    private readonly Dictionary<string, MetaClass> _symbols = new();

    // Registra una variable (Parámetros, This, o Variables Locales creadas por instrucciones)
    public void Define(string name, MetaClass type)
    {
        if (_symbols.ContainsKey(name))
            throw new DomainException($"La variable '{name}' ya está definida en este scope.");
        
        _symbols[name] = type;
    }

    // Busca una variable para validar su uso y si no la tiene la busca en su parent
    public MetaClass Resolve(string name)
    {
        if (!_symbols.TryGetValue(name, out var type))
            throw new DomainException($"Error de Diseño: La variable '{name}' no existe en el contexto actual.");
        
        return type;
    }

    public bool IsDefined(string name) => _symbols.ContainsKey(name);
}

// --------------------------------------------------------------------------------
// File: MetaHammer.Domain/Features/Classes/MetaProperty.cs
// --------------------------------------------------------------------------------
using MetaHammer.Domain.Common;
using MetaHammer.Domain.Exceptions;
using MetaHammer.Domain.Features.Classes.Enums;
using MetaHammer.Domain.Validation;

namespace MetaHammer.Domain.Features.Classes;

public class MetaProperty(Guid guid, string name, MetaClass metaClass, bool isCollection = false, bool isComposition = true) : Entity(guid)
{
    public string Name { get; init; } = name;
    public MetaClass MetaClass { get; init; } = metaClass;
    public bool IsCollection { get; init; } = isCollection;
    public bool IsComposition { get; init; } = isComposition;
}

// --------------------------------------------------------------------------------
// File: MetaHammer.Domain/Features/Classes/MetaParameter.cs
// --------------------------------------------------------------------------------
using MetaHammer.Domain.Common;

namespace MetaHammer.Domain.Features.Classes;

public class MetaParameter(string name, MetaClass metaClass, int order, bool IsCollection = false) : Entity(System.Guid.NewGuid())
{
    public string Name { get; private set; } = name;

    public MetaClass Type { get; private set; } = metaClass;

    public bool IsCollection { get; private set; } = IsCollection;

    public int Order { get; private set; } = order;
}

// --------------------------------------------------------------------------------
// File: MetaHammer.Domain/Features/Classes/Methods/Argument.cs
// --------------------------------------------------------------------------------
using MetaHammer.Domain.Common;

namespace MetaHammer.Domain.Features.Classes.Methods;

public class Argument(MetaParameter metaParameter, string variableNameFromContext) : Entity(Guid.NewGuid())
{
    public MetaParameter MetaParameter { get; private set; } = metaParameter;
    public string VariableNameFromContext { get; private set; } = variableNameFromContext;
}

// --------------------------------------------------------------------------------
// File: MetaHammer.Domain/Features/Classes/Enums/MetaNature.cs
// --------------------------------------------------------------------------------
namespace MetaHammer.Domain.Features.Classes.Enums;

public enum MetaNature
{
    Primitive,
    ValueObject,
    Entity,
    Interface,
    Abstract
}

// --------------------------------------------------------------------------------
// File: MetaHammer.Domain/Features/Organizations/Identity/Permission.cs
// --------------------------------------------------------------------------------
using MetaHammer.Domain.Common;

namespace MetaHammer.Domain.Features.Organizations.Identity;

public class Permission(Guid guid, string name) : Entity(guid)
{
    //Un permiso tiene un nombre
    public string Name { get; private set; } = name;
}

// --------------------------------------------------------------------------------
// File: MetaHammer.Domain/Features/Organizations/Identity/User.cs
// --------------------------------------------------------------------------------
using MetaHammer.Domain.Common;

namespace MetaHammer.Domain.Features.Organizations.Identity;

public class User : Entity
{
    public Guid TenantGuid { get; private set;  }
    public Organization Organization { get; private init; }
    
    public Guid? CreatedByGuid { get; private set;  }
    public User? CreatedBy { get; init; }
    
    
    public Guid CredentialGuid { get; private set; }
    public Credential Credential { get; set; }
    
    
    public Guid PersonGuid { get; private set;  }
    public Person Person { get; private set; }
    
    
    public Guid PrimaryGroupGuid { get; init; }
    public Group PrimaryGroup { get; private set; }
    
    
    //Un user tiene credenciales
    
    //Estado del usuario
    public bool IsActive { get; private set; }
    
    //Fecha de creacion del usuario
    public DateTime CreatedAt { get; private set; }
    
    //Un usuario puede pertenecer a muchos grupos
    public List<Group> Groups { get; private set; } = new();
    
    //Un usuario tiene credenciales

    public User(Organization organization, Guid guid, string userName, User? creator = null, bool isActive = true) : base(guid)
    {
        Person = new Person(Guid.NewGuid());
        PersonGuid = Person.Guid;
        
        Credential = new Credential(Guid.NewGuid(), userName);
        CredentialGuid = Credential.Guid;
        
        PrimaryGroup = new Group(Guid.NewGuid(), userName);
        PrimaryGroupGuid = PrimaryGroup.Guid;
        Groups.Add(PrimaryGroup);
        
        TenantGuid = organization.Guid;
        Organization = organization;
        
        CreatedBy = creator;
        CreatedByGuid = CreatedBy?.Guid;
        
        IsActive = isActive;
        CreatedAt = DateTime.UtcNow;
    }
    
    //Metodo que devuelve los permisos a los que tiene acceso el usuario a traves de sus grupos
    public IReadOnlyList<Permission> GetPermissions()
    {
        var permissions = new List<Permission>();
        
        foreach (var group in Groups)
        {
            permissions.AddRange(group.GetPermissions());
        }
        
        return permissions.Distinct().ToList().AsReadOnly();
    }
        
}

// --------------------------------------------------------------------------------
// File: MetaHammer.Domain/Features/Organizations/Identity/Credential.cs
// --------------------------------------------------------------------------------
using MetaHammer.Domain.Common;

namespace MetaHammer.Domain.Features.Organizations.Identity;

public class Credential: Entity
{
    //Una credencial tiene username
    public string Username { get; private set; }
    
    //Una credencial tiene passwordHash
    public string PasswordHash { get; private set; }
    
    //Una credencial tiene salt
    public string Salt { get; private set; }
    
    //Una credencial tiene un constructor
    public Credential(Guid guid, string username) : base(guid)
    {
        Username = username;
    }
}

// --------------------------------------------------------------------------------
// File: MetaHammer.Domain/Features/Organizations/Identity/Person.cs
// --------------------------------------------------------------------------------
using MetaHammer.Domain.Common;

namespace MetaHammer.Domain.Features.Organizations.Identity;

public class Person : Entity
{
    //Primer nombre
    public string FirstName { get; set; }
    
    //Apellido
    public string LastName { get; set; }
    
    //Constructor de persona con Guid
    public Person(Guid guid) : base(guid)
    {
    }
}

// --------------------------------------------------------------------------------
// File: MetaHammer.Domain/Features/Organizations/Identity/Group.cs
// --------------------------------------------------------------------------------
using MetaHammer.Domain.Common;
using MetaHammer.Domain.Exceptions;

namespace MetaHammer.Domain.Features.Organizations.Identity;

public class Group(Guid guid, string name) : Entity(guid)
{
    //Un grupo tiene Name
    public string Name { get; set; } = name;
    //Un grupo tiene muchos permisos
    private List<Permission> Permissions { get; set; } = new();
    
    //Metodo que devuelve permisos de un grupo como readonly list
    public IReadOnlyList<Permission> GetPermissions() => Permissions.AsReadOnly();
    
    //Agregar un permiso al grupo
    public void AddPermission(Permission permission)
    {
        if (Permissions.Any(p => p.Guid == permission.Guid))
        {
            throw new DomainException($"El permiso con GUID '{permission.Guid}' ya existe en el grupo.");
        }
        
        Permissions.Add(permission);
    }
}

// --------------------------------------------------------------------------------
// File: MetaHammer.Domain/Common/Entity.cs
// --------------------------------------------------------------------------------
namespace MetaHammer.Domain.Common;

public abstract class Entity(Guid guid)
{
    public Guid Guid { get; protected set; } = guid;
}

// --------------------------------------------------------------------------------
// File: MetaHammer.Domain/Features/Classes/Events/ComplexTypeCreated.cs
// --------------------------------------------------------------------------------
using MetaHammer.Domain.Interfaces;

namespace MetaHammer.Domain.Features.Classes.Events;

public class ComplexTypeCreated : IDomainEvent
{
    
}

// --------------------------------------------------------------------------------
// File: MetaHammer.Domain/Interfaces/Repositories/IMetaObjectRepository.cs
// --------------------------------------------------------------------------------
namespace MetaHammer.Domain.Interfaces.Repositories;

public interface IMetaObjectRepository
{
    
}

// --------------------------------------------------------------------------------
// File: MetaHammer.Domain/Interfaces/Repositories/IMetaTypeRepository.cs
// --------------------------------------------------------------------------------
namespace MetaHammer.Domain.Interfaces.Repositories;

public interface IMetaTypeRepository
{
    
}

// --------------------------------------------------------------------------------
// File: MetaHammer.Domain/Interfaces/IDomainEvent.cs
// --------------------------------------------------------------------------------
namespace MetaHammer.Domain.Interfaces;

public interface IDomainEvent
{
    
}

// --------------------------------------------------------------------------------
// File: MetaHammer.Domain/Common/AggregateRoot.cs
// --------------------------------------------------------------------------------
using MetaHammer.Domain.Interfaces;

namespace MetaHammer.Domain.Common;

public abstract class AggregateRoot : Entity
{
    public AggregateRoot(Guid guid) : base(guid)
    {
    }
    
    private readonly List<IDomainEvent> _domainEvents = new();
    public IReadOnlyCollection<IDomainEvent> DomainEvents => _domainEvents.AsReadOnly();

    protected void AddDomainEvent(IDomainEvent domainEvent) => _domainEvents.Add(domainEvent);
    public void ClearDomainEvents() => _domainEvents.Clear();
}

// --------------------------------------------------------------------------------
// File: MetaHammer.Domain/Exceptions/DomainException.cs
// --------------------------------------------------------------------------------
namespace MetaHammer.Domain.Exceptions;

public class DomainException : Exception
{
    public DomainException(string errorMessage) : base(errorMessage)
    {
        
    }
}

// --------------------------------------------------------------------------------
// File: MetaHammer.Domain/Validation/NameFormatValidator.cs
// --------------------------------------------------------------------------------
using System.Text.RegularExpressions;
using MetaHammer.Domain.Exceptions;

namespace MetaHammer.Domain.Validation;

public static partial class NameFormatValidator
{
    private static readonly Regex PascalCaseRegex = GeneratePascalCaseRegex();
    private static readonly Regex SnakeCaseRegex = GenerateSnakeCaseRegex();

    public static void ValidatePascalCase(string name, string context)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new DomainException($"{context} name cannot be empty");

        if (!PascalCaseRegex.IsMatch(name))
            throw new DomainException($"{context} name '{name}' must be in PascalCase format (e.g., 'Person', 'OrderLine')");
    }

    public static void ValidateSnakeCase(string name, string context)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new DomainException($"{context} name cannot be empty");

        if (!SnakeCaseRegex.IsMatch(name))
            throw new DomainException($"{context} name '{name}' must be in snake_case format (e.g., 'first_name', 'order_date')");
    }

    [GeneratedRegex(@"^[A-Z][a-zA-Z0-9]*$", RegexOptions.Compiled)]
    private static partial Regex GeneratePascalCaseRegex();

    [GeneratedRegex(@"^[a-z][a-z0-9]*(_[a-z0-9]+)*$", RegexOptions.Compiled)]
    private static partial Regex GenerateSnakeCaseRegex();
}
