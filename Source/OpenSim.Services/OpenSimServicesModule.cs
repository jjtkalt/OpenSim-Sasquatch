using Autofac;
using OpenSim.Services.Interfaces;
using OpenSim.Services.FSAssetService;
using OpenSim.Services.UserAccountService;
using OpenSim.Services.EstateService;
using OpenSim.Framework;
using OpenSim.Framework.AssetLoader.Filesystem;

namespace OpenSim.Services;

public class OpenSimServicesModule : Module
{
    protected override void Load(ContainerBuilder builder)
    {
        builder.RegisterType<FSAssetConnector>()
            .Named<IAssetService>("FSAssetConnector")
            .AsImplementedInterfaces().SingleInstance();

        builder.RegisterType<AssetService.AssetService>()
            .Named<IAssetService>("AssetService")
            .AsImplementedInterfaces().SingleInstance();  

        builder.RegisterType<AssetLoaderFileSystem>()
            .Named<IAssetLoader>("AssetLoaderFileSystem")
            .AsImplementedInterfaces().SingleInstance();

        builder.RegisterType<AgentPreferencesService>()
            .Named<IAgentPreferencesService>("AgentPreferencesService")
            .AsImplementedInterfaces().SingleInstance();

        builder.RegisterType<EstateDataService>()
            .Named<IEstateDataService>("EstateDataService")
            .AsImplementedInterfaces()
            .SingleInstance();

        builder.RegisterType<GridUserService>()
            .Named<IGridUserService>("GridUserService")
            .AsImplementedInterfaces().SingleInstance();

        builder.RegisterType<UserAccountService.UserAccountService>()
            .Named<IUserAccountService>("UserAccountService")
            .AsImplementedInterfaces().SingleInstance();

        builder.RegisterType<UserAliasService>()
            .Named<IUserAliasService>("UserAliasService")
            .AsImplementedInterfaces().SingleInstance();            
    }
}
