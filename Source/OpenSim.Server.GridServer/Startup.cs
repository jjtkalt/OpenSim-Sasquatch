namespace OpenSim.Server.GridServer;

public class Startup
{
    public IConfiguration Configuration { get; }

    public Startup(IConfiguration configuration)
    {
        Configuration = configuration;
    }

    public void ConfigureServices(IServiceCollection services)
    {
        // add your services here
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        //add configuration/middleware here

    }
}
