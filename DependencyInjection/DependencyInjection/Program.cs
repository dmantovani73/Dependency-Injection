using Microsoft.Extensions.DependencyInjection;
using System;
using System.Diagnostics;

class Program
{
    static void Main(string[] args)
    {
        var serviceCollection = new ServiceCollection();
        //serviceCollection.AddSingleton<ILog, ConsoleLog>();
        serviceCollection.AddScoped<ILog, ConsoleLog>();
        //serviceCollection.AddTransient<ILog, ConsoleLog>();

        var container = serviceCollection.BuildServiceProvider();
        DumpServiceScope(container);
        DumpServiceScope(container);
    }

    static void DumpServiceScope(ServiceProvider container)
    {
        var serviceScopeFactory = container.GetRequiredService<IServiceScopeFactory>();
        using (var scope = serviceScopeFactory.CreateScope())
        {
            var a = scope.ServiceProvider.GetServices<INone>();
            Console.WriteLine($"a: {a?.GetHashCode()}");

            Console.WriteLine($"Scope: {scope.GetHashCode()}");

            Dump<ILog>(scope);
            Dump<ILog>(scope);
        }

        void Dump<T>(IServiceScope scope)
        {
            var watch = Stopwatch.StartNew();
            var obj = scope.ServiceProvider.GetService<T>();
            watch.Stop();

            Console.WriteLine($"\tHashCode: {obj?.GetHashCode()} -> {watch.Elapsed}");
        }
    }
}

interface INone
{ }

interface ILog
{
    void Log(string message);
}

class ConsoleLog : ILog
{
    public void Log(string message) => Console.WriteLine(message);
}
