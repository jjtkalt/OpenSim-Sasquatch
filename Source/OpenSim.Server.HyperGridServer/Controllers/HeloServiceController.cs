/*
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/.
 *
 * (c) 2024 Utopia Skye LLC
 */

using Microsoft.AspNetCore.Mvc;
using OpenSim.Framework;
using OpenMetaverse;
using OpenSim.Services.Interfaces;

using GridRegion = OpenSim.Services.Interfaces.GridRegion;
using OpenSim.Server.HyperGridServer.Dto;

namespace OpenSim.Server.HyperGrid.Controllers;

[ApiController]
[Route("[controller]")]
public class HeloServiceController : ControllerBase
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<HeloServiceController> _logger;

    public HeloServiceController(
        IConfiguration configuration,
        ILogger<HeloServiceController> logger)
    {
        _configuration = configuration;
        _logger = logger;
    }

    [HttpGet]
    [Route("helo")]
    public void Helo()
    {
        throw new NotImplementedException();
    }
}
