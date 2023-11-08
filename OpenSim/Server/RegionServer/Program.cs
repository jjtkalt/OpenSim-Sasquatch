/*
 * Copyright (c) Contributors, http://opensimulator.org/
 * See CONTRIBUTORS.TXT for a full list of copyright holders.
 *
 * Redistribution and use in source and binary forms, with or without
 * modification, are permitted provided that the following conditions are met:
 *     * Redistributions of source code must retain the above copyright
 *       notice, this list of conditions and the following disclaimer.
 *     * Redistributions in binary form must reproduce the above copyright
 *       notice, this list of conditions and the following disclaimer in the
 *       documentation and/or other materials provided with the distribution.
 *     * Neither the name of the OpenSim Project nor the
 *       names of its contributors may be used to endorse or promote products
 *       derived from this software without specific prior written permission.
 *
 * THIS SOFTWARE IS PROVIDED BY THE DEVELOPERS ``AS IS'' AND ANY
 * EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED
 * WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
 * DISCLAIMED. IN NO EVENT SHALL THE CONTRIBUTORS BE LIABLE FOR ANY
 * DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES
 * (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES;
 * LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND
 * ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT
 * (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS
 * SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
 */


using log4net.Config;
using Nini.Config;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;
using Autofac.Extensions.DependencyInjection;
using Autofac;

using OpenSim.ApplicationPlugins.LoadRegions;
using OpenSim.ApplicationPlugins.RegionModulesController;
using OpenSim.ApplicationPlugins.RemoteController;

using OpenSim.Region.OptionalModules;
using OpenSim.Region.CoreModules;
using OpenSim.Region.Framework;

using OpenSim.Services.SimulationService;
using OpenSim.Services.EstateService;
using OpenSim.Region.PhysicsModule.BasicPhysics;
using OpenSim.Region.ClientStack.LindenUDP;
using OpenSim.Region.ClientStack.Linden;
using OpenSim.Region.PhysicsModule.Meshing;
using OpenSim.Region.PhysicsModule.POS;
using OpenSim.Region.PhysicsModule.BulletS;
using OpenSim.Region.PhysicsModule.ubOde;
using OpenSim.Region.PhysicsModule.ubOdeMeshing;
using OpenSim.Region.CoreModules.Framework.Search;
using OpenSimSearch.Modules.OpenSearch;
using OpenSim.Groups;
using Gloebit.GloebitMoneyModule;
using OpenSim.Region.ScriptEngine.Yengine;

namespace OpenSim.Server.RegionServer
{
    class Program
    {
        public static void Main(string[] args)
        {
            var switchMappings = new Dictionary<string, string>()
            {
                { "-console", "console" },
                { "-logfile", "logfile" },
                { "-inifile", "inifile" },
                { "-inimaster", "inimaster" },
                { "-prompt", "prompt" },
                { "-logconfig", "logconfig" }
            };

            XmlConfigurator.Configure();
            Application.Configure(args); 

            IHostBuilder builder = Host.CreateDefaultBuilder()
                .UseServiceProviderFactory(new AutofacServiceProviderFactory())
                .ConfigureAppConfiguration(configuration =>
                {
                    configuration.AddIniFile("OpenSimDefaults.ini", optional: true, reloadOnChange: true);
                })
                .ConfigureServices(services =>
                {
                    services.AddHostedService<RegionService>();
                })
                .ConfigureContainer<ContainerBuilder>(builder =>
                {
                    // Register the main configuration
                    builder.Register(x => Application.Configuration)
                        .As<IConfigSource>()
                        .SingleInstance();

                    // Startup Application Plugins
                    builder.RegisterType<RegionModulesControllerPlugin>()
                        .As<IApplicationPlugin>()
                        .SingleInstance();

                    builder.RegisterType<LoadRegionsPlugin>().
                        As<IApplicationPlugin>()
                        .SingleInstance();

                    builder.RegisterType<RemoteAdminPlugin>()
                        .As<IApplicationPlugin>()
                        .SingleInstance();

                    // Data Services
                    builder.RegisterModule(new SimulationDataServiceModule());
                    builder.RegisterModule(new EstateDataServiceModule());

                    // Register Region Modules
                    builder.RegisterModule(new CoreModulesModule());
                    builder.RegisterModule(new OptionalModulesModule());
                    builder.RegisterModule(new LindenUDPModule());
                    builder.RegisterModule(new LindenCapsModule());
                    builder.RegisterModule(new BasicPhysicsModule());
                    builder.RegisterModule(new POSModule());
                    builder.RegisterModule(new BulletSModule());
                    builder.RegisterModule(new ubOdePhysicsModule());
                    builder.RegisterModule(new MeshingModule());
                    builder.RegisterModule(new ubOdePhysicsMeshingModule());
                    builder.RegisterModule(new YEngineModule());
                    builder.RegisterModule(new OpenSimSearchModule());
                    builder.RegisterModule(new GroupsAddonModule());
                    builder.RegisterModule(new GloebitModule());
                });

            IHost host = builder.Build();
            
            host.Run();
        }
    }
}
