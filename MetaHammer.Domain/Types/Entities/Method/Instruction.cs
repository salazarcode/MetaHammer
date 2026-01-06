using MetaHammer.Domain.Common;
using MetaHammer.Domain.Exceptions;

namespace MetaHammer.Domain.Types.Entities.Method;

public class Instruction : Entity
{
    public Method ParentMethod { get; init; }
    public Method InvokedMethod { get; init; }
    public int Order { get; private set; }
    
    private List<Argument> _arguments { get; set; } = new();
    public IReadOnlyCollection<Argument> Arguments => _arguments.AsReadOnly();
    
    public Instruction(Method parentMethod, Method invokedMethod, int order, List<Argument> arguments) : base(System.Guid.NewGuid())
    {
        ParentMethod = parentMethod;
        InvokedMethod = invokedMethod;
        Order = order;

        //Verifico que todos los parametros del metodo existan en el diccionario de argumentos
        foreach (var parameter in InvokedMethod.Parameters())
        {
            var argument = arguments.FirstOrDefault(x => x.Parameter.Name == parameter.Name);
                
            if(argument == null)
                throw new DomainException($"There's no argument present for parameter '{parameter.Name}' in method '{InvokedMethod.Name}'.");
                
            _arguments.Add(argument);
        }
    }
}