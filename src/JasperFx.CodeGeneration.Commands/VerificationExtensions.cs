using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using JasperFx.CodeGeneration.Model;
using JasperFx.Core;
using JasperFx.RuntimeCompiler;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Oakton.Environment;

namespace JasperFx.CodeGeneration.Commands;

public static class VerificationExtensions
{
    /// <summary>
    /// Verify that every single, configured generated code file
    /// in this .NET system can generate code that will compile.
    /// NOT A FAST OPERATION, MOSTLY USEFUL FOR TROUBLESHOOTING OR
    /// TEST PROJECTS
    /// </summary>
    /// <param name="host"></param>
    /// <exception cref="AggregateException"></exception>
    public static void AssertAllGeneratedCodeCanCompile(this IHost host)
    {
        var exceptions = new List<Exception>();
        var failures = new List<string>();
        
        var collections = host.Services.GetServices<ICodeFileCollection>().ToArray();

        var services = host.Services.GetService<IServiceVariableSource>();

        foreach (var collection in collections)
        {
            foreach (var file in collection.BuildFiles())
            {
                var fileName = collection.ChildNamespace.Replace(".", "/").AppendPath(file.FileName);
                
                try
                {
                    var assembly = new GeneratedAssembly(collection.Rules);
                    file.AssembleTypes(assembly);
                    new AssemblyGenerator().Compile(assembly, services);
                    
                    Debug.WriteLine($"U+2713 {fileName} ");
                }
                catch (Exception e)
                {
                    Debug.WriteLine($"Failed: {fileName}");
                    Debug.WriteLine(e);
                    
                    failures.Add(fileName);
                    exceptions.Add(e);
                }
            }
        }

        if (failures.Any())
        {
            throw new AggregateException($"Compilation failures for:\n{failures.Join("\n")}", exceptions);
        }
    }

    /// <summary>
    /// Add an environment check that all expected pre-built generated
    /// types exist in the configured assembly
    /// </summary>
    /// <param name="services"></param>
    public static void AssertAllExpectedPreBuiltTypesExistOnStartUp(this IServiceCollection services)
    {
        services.AddSingleton<IEnvironmentCheck, AllPreGeneratedTypesExist>();
    }
}