using Microsoft.AspNetCore.Mvc;
using OpenSim.Framework;
using OpenMetaverse;
using OpenSim.Services.Interfaces;

using GridRegion = OpenSim.Services.Interfaces.GridRegion;

namespace OpenSim.Server.HyperGrid.Controllers;

[ApiController]
[Route("[controller]")]
public class GatekeeperServiceController : ControllerBase, IGatekeeperService
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<GatekeeperServiceController> _logger;

    public GatekeeperServiceController(
        IConfiguration configuration,
        ILogger<GatekeeperServiceController> logger)
    {
        _configuration = configuration;
        _logger = logger;
    }

    [HttpGet(Name = "GetHyperLinkRegion")]
    [Produces("application/xml")]
    public GridRegion GetHyperlinkRegion(global::OpenMetaverse.UUID regionID, global::OpenMetaverse.UUID agentID, string agentHomeURI, out string message)
    {
        throw new NotImplementedException();
    }


    [HttpGet(Name = "LinkRegion")]
    [Produces("application/xml")]
    public bool LinkRegion(string regionDescriptor, out global::OpenMetaverse.UUID regionID, out ulong regionHandle, out string externalName, out string imageURL, out string reason, out int sizeX, out int sizeY)
    {
        throw new NotImplementedException();
    }

    [HttpGet(Name = "LoginAgent")]
    [Produces("application/xml")]
    public bool LoginAgent(GridRegion source, AgentCircuitData aCircuit, GridRegion destination, out string reason)
    {
        throw new NotImplementedException();
    }
}
