using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Oakton.Environment;

namespace JasperFx.CodeGeneration.Commands;

public class AllPreGeneratedTypesExist : IEnvironmentCheck
{
    public async Task Assert(IServiceProvider services, CancellationToken cancellation)
    {
        var collections = services.GetServices<ICodeFileCollection>().ToArray();
        foreach (var collection in collections)
        {
            collection.AssertPreBuildTypesExist(services);
        }
    }

    public string Description { get; } =
        "Asserting that all expected pre-built generated types exist in the configured assembly";
}