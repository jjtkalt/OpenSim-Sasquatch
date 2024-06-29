using System.CommandLine;
using Humanizer;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using MySqlConnector;
using OpenSim.Data.Model.Core;
using OpenSim.Data.Model.Economy;
using OpenSim.Data.Model.Identity;
using OpenSim.Data.Model.Region;
using OpenSim.Data.Model.Search;

namespace OpenSim.Server.GridServer;

public class Program
{
    static string _console = "local";
    static string _prompt = "Grid$ ";

    static List<string>? _inifiles = null;

    public static async Task Main(string[] args)
    {
        var rootCommand = new RootCommand("Grid Server");

        var consoleOption = new Option<string>
            (name: "--console", description: "console type, one of basic, local or rest.", 
            getDefaultValue: () => "local")
            .FromAmong("basic", "local", "rest");
        var promptOption = new Option<string>
            (name: "--prompt", description: "Overide the server prompt",
            getDefaultValue: () => "Grid$ ");
        var inifileOption = new Option<List<string>>
            (name: "--inifile", description: "Specify the location of zero or more .ini file(s) to read.");

        rootCommand.Add(consoleOption);
        rootCommand.Add(inifileOption);
        rootCommand.Add(promptOption);
        
        rootCommand.SetHandler(
            (consoleOptionValue, promptOptionValue, inifileOptionValue) =>
            {
                _console = consoleOptionValue;
                _prompt = promptOptionValue;
                _inifiles = inifileOptionValue;
            },
            consoleOption, promptOption, inifileOption);

        await rootCommand.InvokeAsync(args);

        // Create Builder and run program
        var builder = WebApplication.CreateBuilder(args);
        //builder.Configuration.EnableSubstitutions("$(", ")");

        builder.Configuration.AddIniFile("GridServer.ini", optional: true, reloadOnChange: false);
        builder.Configuration.AddEnvironmentVariables();

        if (_inifiles is not null)
        {
            foreach (var item in _inifiles)
            {
                builder.Configuration.AddIniFile(item, optional: true, reloadOnChange: false);
            }
        }
        
        // Initialize Database
        var connectionString = builder.Configuration.GetConnectionString("IdentityConnection");
        builder.Services.AddDbContext<IdentityContext>(
            options => options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));

        var coreConnectionString = builder.Configuration.GetConnectionString("OpenSimCoreConnection");
        builder.Services.AddDbContext<OpenSimCoreContext>(
            options => options.UseMySql(coreConnectionString, ServerVersion.AutoDetect(coreConnectionString)));

        var regionConnectionString = builder.Configuration.GetConnectionString("OpenSimRegionConnection");
        builder.Services.AddDbContext<OpenSimRegionContext>(
            options => options.UseMySql(regionConnectionString, ServerVersion.AutoDetect(regionConnectionString)));

        var economyConnectionString = builder.Configuration.GetConnectionString("OpenSimEconomyConnection");
        builder.Services.AddDbContext<OpenSimEconomyContext>(
            options => options.UseMySql(economyConnectionString, ServerVersion.AutoDetect(economyConnectionString)));

        var searchConnectionString = builder.Configuration.GetConnectionString("OpenSimSearchConnection");
        builder.Services.AddDbContext<OpenSimSearchContext>(
            options => options.UseMySql(searchConnectionString, ServerVersion.AutoDetect(searchConnectionString)));

        builder.Services.AddDatabaseDeveloperPageExceptionFilter();

        builder.Services.AddControllers()
            .AddXmlDataContractSerializerFormatters();
        
        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new OpenApiInfo { Title = "OpenSimulator Grid Services v0.8", Version = "v1" });
        });

        var app = builder.Build();

        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseHttpsRedirection();
        app.UseAuthorization();

        app.MapControllers();

        app.Run();
    }
}
