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