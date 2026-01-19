/*
using MetaHammer.Interpreter.Interfaces;
using MetaHammer.Interpreter.Methods;
using Microsoft.Extensions.DependencyInjection;

namespace MetaHammer.Interpreter.Configuration;

public static class Configuration
{
    public static IServiceCollection AddMetaHammerInterpreter(this IServiceCollection services)
    {
        // 1. Registro del motor principal
        services.AddSingleton<IInterpreter, Interpreter>();
        services.AddSingleton<INativeMethodProvider, NativeMethodProvider>();

        // 2. Método "Boot" de registro de funciones nativas
        RegisterNativeFunctions(services);

        return services;
    }

    private static void RegisterNativeFunctions(IServiceCollection services)
    {
        // Aquí centralizas todas tus funciones nativas
        services.AddSingleton<INativeMethod, CreateInstance>();

        // Si mañana creas una nueva, solo la agregas aquí y
        // el sistema la reconocerá automáticamente.
    }
}
*/
