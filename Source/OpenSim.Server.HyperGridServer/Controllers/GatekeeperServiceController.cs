using Microsoft.AspNetCore.Mvc;
using OpenSim.Framework;
using OpenMetaverse;
using OpenSim.Services.Interfaces;

using GridRegion = OpenSim.Services.Interfaces.GridRegion;
using OpenSim.Server.HyperGridServer.Dto;

namespace OpenSim.Server.HyperGrid.Controllers;

[ApiController]
[Route("[controller]")]
public class GatekeeperServiceController : ControllerBase // , IGatekeeperService
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

    [HttpGet(Name = "get_region")]
    [Produces("application/xml")]
    public GetRegionResponse GetHyperlinkRegion(UUID regionID)
    {
        throw new NotImplementedException();
    }



    // [HttpGet(Name = "link_region")]
    // [Produces("application/xml")]
    // public LinkRegionResponse LinkRegion(string regionDescriptor)
    // {
    //     throw new NotImplementedException();
    // }

    // [HttpPost(Name = "LoginAgent")]
    // [Produces("application/xml")]
    // public LoginAgentResponse LoginAgent([FromBody]GridRegion source, [FromBody]AgentCircuitData aCircuit, [FromBody]GridRegion destination)
    // {
    //     throw new NotImplementedException();
    // }
}
