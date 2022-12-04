using System.Threading.Tasks;
using JasperFx.CodeGeneration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Oakton;

[assembly:OaktonCommandAssembly]

namespace GeneratorTarget
{
    static class Program
    {
        static Task<int> Main(string[] args)
        {
            return Host.CreateDefaultBuilder()
                .ConfigureServices(services =>
                {
                    services.AddSingleton<ICodeFileCollection>(new GreeterGenerator());
                    services.AddSingleton<ICodeFileCollection>(new GreeterGenerator2());
                })
                .RunOaktonCommands(args);

        }
    }
}