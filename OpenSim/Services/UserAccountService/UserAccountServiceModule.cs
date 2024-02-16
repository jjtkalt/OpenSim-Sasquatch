using Autofac;

using Microsoft.Extensions.Configuration;
using OpenSim.Services.Interfaces;

namespace OpenSim.Services.UserAccountService;

public class UserAccountServiceModule : Module
{
    protected override void Load(ContainerBuilder builder)
    {
        builder.RegisterType<AgentPreferencesService>()
            .Named<IAgentPreferencesService>("AgentPreferencesService")
            .AsImplementedInterfaces().SingleInstance();

        builder.RegisterType<GridUserService>()
            .Named<IGridUserService>("GridUserService")
            .AsImplementedInterfaces().SingleInstance();

        builder.RegisterType<UserAccountService>()
            .Named<IUserAccountService>("UserAccountService")
            .AsImplementedInterfaces().SingleInstance();

        builder.RegisterType<UserAliasService>()
            .Named<IUserAliasService>("UserAliasService")
            .AsImplementedInterfaces().SingleInstance();            
    }
}
