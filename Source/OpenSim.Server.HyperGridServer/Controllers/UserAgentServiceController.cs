using Microsoft.AspNetCore.Mvc;
using OpenSim.Framework;
using OpenMetaverse;
using OpenSim.Services.Interfaces;

using GridRegion = OpenSim.Services.Interfaces.GridRegion;

namespace OpenSim.Server.HyperGrid.Controllers;

[ApiController]
[Route("[controller]")]
public class UserAgentServiceController : ControllerBase, IUserAgentService
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<UserAgentServiceController> _logger;

    public UserAgentServiceController(
        IConfiguration configuration,
        ILogger<UserAgentServiceController> logger)
    {
        _configuration = configuration;
        _logger = logger;
    }

    [HttpGet(Name = "LoginAgentToGrid")]
    [Produces("application/xml")]
    public bool LoginAgentToGrid(GridRegion source, AgentCircuitData agent, GridRegion gatekeeper, GridRegion finalDestination, bool fromLogin, out string reason)
    {
        throw new NotImplementedException();
    }

    [HttpGet(Name = "LogoutAgent")]
    [Produces("application/xml")]
    public void LogoutAgent(UUID userID, UUID sessionID)
    {
        throw new NotImplementedException();
    }

    [HttpGet(Name = "GetHomeRegion")]
    [Produces("application/xml")]
    public GridRegion GetHomeRegion(UUID userID, out Vector3 position, out Vector3 lookAt)
    {
        throw new NotImplementedException();
    }

    [HttpGet(Name = "GetServerURLs")]
    [Produces("application/xml")]
    public Dictionary<string, object> GetServerURLs(UUID userID)
    {
        throw new NotImplementedException();
    }

    [HttpGet(Name = "GetUserInfo")]
    [Produces("application/xml")]
    public Dictionary<string, object> GetUserInfo(UUID userID)
    {
        throw new NotImplementedException();
    }

    [HttpGet(Name = "LocateUser")]
    [Produces("application/xml")]
    public string LocateUser(UUID userID)
    {
        throw new NotImplementedException();
    }

    [HttpGet(Name = "GetUUI")]
    [Produces("application/xml")]
    public string GetUUI(UUID userID, UUID targetUserID)
    {
        throw new NotImplementedException();
    }

    [HttpGet(Name = "GetUUID")]
    [Produces("application/xml")]
    public UUID GetUUID(string first, string last)
    {
        throw new NotImplementedException();
    }

    [HttpGet(Name = "StatusNotification")]
    [Produces("application/xml")]
    public List<UUID> StatusNotification(List<string> friends, UUID userID, bool online)
    {
        throw new NotImplementedException();
    }

    [HttpGet(Name = "IsAgentComingHome")]
    [Produces("application/xml")]
    public bool IsAgentComingHome(UUID sessionID, string thisGridExternalName)
    {
        throw new NotImplementedException();
    }

    [HttpGet(Name = "VerifyAgent")]
    [Produces("application/xml")]
    public bool VerifyAgent(UUID sessionID, string token)
    {
        throw new NotImplementedException();
    }

    [HttpGet(Name = "VerifyClient")]
    [Produces("application/xml")]
    public bool VerifyClient(UUID sessionID, string reportedIP)
    {
        throw new NotImplementedException();
    }
}
