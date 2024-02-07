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
 *     * Neither the name of the OpenSimulator Project nor the
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

using OpenSim.Server.Base;
using OpenSim.Services.Interfaces;
using OpenSim.Framework.Servers.HttpServer;
using OpenSim.Server.Handlers.Base;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace OpenSim.Server.Handlers.Authentication
{
    public class OpenIdServerConnector : IServiceConnector
    {
        private IAuthenticationService m_AuthenticationService;
        private IUserAccountService m_UserAccountService;

        public OpenIdServerConnector(
            IConfiguration configuration,
            ILogger<OpenIdServerConnector> logger
            )
        {
            Config = configuration;
            Logger = Logger;
        }

        public string ConfigName => "OpenIdService";

        public IConfiguration Config { get; private set; }
        public ILogger Logger { get; private set; }
        public IHttpServer HttpServer { get; private set; }

        public void Initialize(IHttpServer httpServer)
        {
            HttpServer = httpServer;

            var serverConfig = Config.GetSection(ConfigName);
            if (serverConfig.Exists() is false)
                throw new Exception($"No section {ConfigName} in config file");

            string authService = serverConfig.GetValue<string>("AuthenticationServiceModule", String.Empty);
            string userService = serverConfig.GetValue<string>("UserAccountServiceModule", String.Empty);

            if (string.IsNullOrEmpty(authService))
                throw new Exception("No AuthenticationServiceModule in config file for OpenId authentication");

            if (string.IsNullOrEmpty(userService))
                throw new Exception("No UserAccountServiceModule in config file for OpenId authentication");

            Object[] args = new Object[] { Config };

            m_AuthenticationService = ServerUtils.LoadPlugin<IAuthenticationService>(authService, args);
            m_UserAccountService = ServerUtils.LoadPlugin<IUserAccountService>(userService, args);

            // Handler for OpenID user identity pages
            HttpServer.AddStreamHandler(new OpenIdStreamHandler("GET", "/users", m_UserAccountService, m_AuthenticationService));
            
            // Handlers for the OpenID endpoint server
            HttpServer.AddStreamHandler(new OpenIdStreamHandler("POST", "/openid/server", m_UserAccountService, m_AuthenticationService));
            HttpServer.AddStreamHandler(new OpenIdStreamHandler("GET", "/openid/server", m_UserAccountService, m_AuthenticationService));
        }

    }
}
