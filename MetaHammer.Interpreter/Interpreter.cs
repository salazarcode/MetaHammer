/*
using MetaHammer.Domain.Features.Objects;
using MetaHammer.Interpreter.Interfaces;

namespace MetaHammer.Interpreter;

public class Interpreter : IInterpreter
{
    public Interpreter()
    {
    }

    public MetaObject? Execute(Context context, Guid contextVariableGuid, string methodName, params MetaObject[] arguments)
    {
        // MetaObject? instance = null;//context.GetVariable(guid);
        //
        // var method = instance.Methods.FirstOrDefault(m => m.Name == methodName);
        //
        // if (method == null)
        //     throw new ApplicationException($"MetaMethod '{methodName}' not found in instance of type '{instance.Type.Name}'.");
        //
        // // 1. Validar tipos de argumentos contra la firma del método
        // method.ValidateArguments(arguments);
        //
        // // 2. Crear el scope local
        // var localContext = new Context(parentContext);
        //
        // // 3. Mapear argumentos a variables locales
        // for (int i = 0; i < method.Parameters.Count; i++)
        // {
        //     localContext.SetVariable(method.Parameters[i].Name, args[i]);
        // }
        //
        // // 4. Ciclo de ejecución de instrucciones
        // foreach (var instruction in method.Instructions)
        // {
        //     ExecuteInstruction(instruction, localContext);
        // }

        return null;
    }
}
*/
