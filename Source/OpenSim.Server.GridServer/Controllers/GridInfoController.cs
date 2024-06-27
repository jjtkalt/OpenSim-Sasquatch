/*
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/.
 *
 * (c) 2024 Utopia Skye LLC
 */

using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

using OpenMetaverse;
using OpenMetaverse.ImportExport.Collada14;
using OpenSim.Framework;
using OpenSim.Services.Interfaces;

namespace OpenSim.Server.GridServer.Controllers;

[ApiController]
public class GridInfoController: ControllerBase //, IGridService
{
    private readonly string LogHeader = "[GRID HANDLER]";
    private readonly ILogger m_logger;
    private readonly IConfiguration m_config;

    //private readonly IGridService m_GridService;

    public GridInfoController(IConfiguration configuration, ILogger<GridInfoController> logger)
    {
        m_config = configuration;
        m_logger = logger;
    }

    [Route("gridinfo/register")]
    [HttpPost]
    public string RegisterRegion([FromBody]Services.Interfaces.GridRegion regionInfos)
    {
        throw new NotImplementedException();
    }

    // public bool DeregisterRegion(UUID regionID)
    // {
    //     throw new NotImplementedException();
    // }

    // public List<Services.Interfaces.GridRegion> GetNeighbours(UUID scopeID, UUID regionID)
    // {
    //     throw new NotImplementedException();
    // }

    // public Services.Interfaces.GridRegion GetRegionByUUID(UUID scopeID, UUID regionID)
    // {
    //     throw new NotImplementedException();
    // }

    // public Services.Interfaces.GridRegion GetRegionByHandle(UUID scopeID, ulong regionhandle)
    // {
    //     throw new NotImplementedException();
    // }

    // public Services.Interfaces.GridRegion GetRegionByPosition(UUID scopeID, int x, int y)
    // {
    //     throw new NotImplementedException();
    // }

    // public Services.Interfaces.GridRegion GetRegionByName(UUID scopeID, string regionName)
    // {
    //     throw new NotImplementedException();
    // }

    // public Services.Interfaces.GridRegion GetRegionByURI(UUID scopeID, RegionURI uri)
    // {
    //     throw new NotImplementedException();
    // }

    // public List<Services.Interfaces.GridRegion> GetRegionsByName(UUID scopeID, string name, int maxNumber)
    // {
    //     throw new NotImplementedException();
    // }

    // public List<Services.Interfaces.GridRegion> GetRegionsByURI(UUID scopeID, RegionURI uri, int maxNumber)
    // {
    //     throw new NotImplementedException();
    // }

    // public List<Services.Interfaces.GridRegion> GetRegionRange(UUID scopeID, int xmin, int xmax, int ymin, int ymax)
    // {
    //     throw new NotImplementedException();
    // }

    // public List<Services.Interfaces.GridRegion> GetDefaultRegions(UUID scopeID)
    // {
    //     throw new NotImplementedException();
    // }

    // public List<Services.Interfaces.GridRegion> GetDefaultHypergridRegions(UUID scopeID)
    // {
    //     throw new NotImplementedException();
    // }

    // public List<Services.Interfaces.GridRegion> GetFallbackRegions(UUID scopeID, int x, int y)
    // {
    //     throw new NotImplementedException();
    // }

    // public List<Services.Interfaces.GridRegion> GetHyperlinks(UUID scopeID)
    // {
    //     throw new NotImplementedException();
    // }

    // public List<Services.Interfaces.GridRegion> GetOnlineRegions(UUID scopeID, int x, int y, int maxCount)
    // {
    //     throw new NotImplementedException();
    // }

    [Route("gridinfo/get_region_flags/{scopeId}/{regionId}")]
    [HttpPost]
    public int GetRegionFlags([FromQuery]UUID scopeID, [FromQuery, BindRequired]UUID regionID)
    {
        return 0;
    }

    // public Dictionary<string, object> GetExtraFeatures()
    // {
    //     throw new NotImplementedException();
    // }
}
