using OpenMetaverse;

namespace OpenSim.Server.HyperGridServer.Dto;

public class LinkRegionResponse
{
    UUID regionID;
    ulong regionHandle;
    string externalName;
    string imageURL;
    string reason;
    int sizeX;
    int sizeY;
}