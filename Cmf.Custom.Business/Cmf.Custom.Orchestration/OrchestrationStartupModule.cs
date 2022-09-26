using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Builder;
using Cmf.Foundation.Services.HostStartup;
using Cmf.Custom.amsOSRAM.Orchestration.Abstractions;

namespace Cmf.Custom.amsOSRAM.Orchestration;

public class OrchestrationStartupModule : IStartupModule
{
    public MiddlewarePositioning MiddlewarePositioning => MiddlewarePositioning.None;

    public int ServiceRegistrationOrder => 0;

    public void Configure(IApplicationBuilder app, ConfigureMiddlewareContext context)
    {
    }

    public void ConfigureRootServices(IServiceCollection services)
    {
    }

    public void ConfigureServices(IServiceCollection services, ConfigureServicesContext context)
    {
        services.AddTransient<IamsOSRAMManagementOrchestration, amsOSRAMManagementOrchestration>();
    }
}
