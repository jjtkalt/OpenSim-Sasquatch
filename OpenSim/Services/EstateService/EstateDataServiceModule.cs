using Autofac;
using OpenSim.Region.Framework;
using OpenSim.Region.Framework.Interfaces;
using OpenSim.Services.Interfaces;

namespace OpenSim.Services.EstateService
{
    public class EstateDataServiceModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<EstateDataService>()
                .Named<IEstateDataService>("EstateDataService")
                .AsImplementedInterfaces()
                .SingleInstance();
        }
    }
}
