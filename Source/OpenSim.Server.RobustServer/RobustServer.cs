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

using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

using OpenSim.Framework;
using OpenSim.Framework.Servers;
using OpenSim.Framework.Servers.HttpServer;
using OpenSim.Server.Base;
using OpenSim.Server.Handlers.Base;

namespace OpenSim.Server.RobustServer
{
    public class RobustServer
    {
        protected HttpServerBase m_Server = null;
        protected List<IServiceConnector> m_ServiceConnectors = new();

        private bool m_NoVerifyCertChain = false;
        private bool m_NoVerifyCertHostname = false;

        private readonly IServiceProvider m_serviceProvider;
        private readonly ILogger<RobustServer> m_logger;
        private readonly IConfiguration m_configuration;

        public RobustServer(
            IServiceProvider provider,
            IConfiguration configuration, 
            ILogger<RobustServer> logger
//            HttpServerBase server
            )
        {
            m_serviceProvider = provider;
            m_configuration = configuration;
            m_logger = logger;
//            m_Server = server;
        }

        private bool ValidateServerCertificate(
            object sender,
            X509Certificate certificate,
            X509Chain chain,
            SslPolicyErrors sslPolicyErrors
            )
        {
            if (m_NoVerifyCertChain)
                sslPolicyErrors &= ~SslPolicyErrors.RemoteCertificateChainErrors;

            if (m_NoVerifyCertHostname)
                sslPolicyErrors &= ~SslPolicyErrors.RemoteCertificateNameMismatch;

            if (sslPolicyErrors == SslPolicyErrors.None)
                return true;

            return false;
        }

        /// <summary>
        /// Opens a file and uses it as input to the console command parser.
        /// </summary>
        /// <param name="fileName">name of file to use as input to the console</param>
        private void PrintFileToConsole(string fileName)
        {
            if (File.Exists(fileName))
            {
                using (StreamReader readFile = File.OpenText(fileName))
                {
                    string currentLine;
                    while ((currentLine = readFile.ReadLine()) is not null)
                    {
                        m_logger.LogInformation("[!]" + currentLine);
                    }
                }
            }
        }

        public int Startup()
        {
            //Culture.SetCurrentCulture();
            //Culture.SetDefaultCurrentCulture();

            //ServicePointManager.DefaultConnectionLimit = 64;
            //ServicePointManager.MaxServicePointIdleTime = 30000;

            //ServicePointManager.Expect100Continue = false;
            //ServicePointManager.UseNagleAlgorithm = false;
            //ServicePointManager.ServerCertificateValidationCallback = ValidateServerCertificate;

            WebUtil.SetupHTTPClients(m_NoVerifyCertChain, m_NoVerifyCertHostname, null, 32);

            //string[] args = Environment.GetCommandLineArgs();

            //m_Server = new HttpServerBase(m_configuration, m_logger, "R.O.B.U.S.T.");

            string registryLocation;

            var serverConfig = m_configuration.GetSection("Startup");

            int dnsTimeout = serverConfig.GetValue<int>("DnsTimeout", 30000);
            try { ServicePointManager.DnsRefreshTimeout = dnsTimeout; } catch { }

            m_NoVerifyCertChain = serverConfig.GetValue<bool>("NoVerifyCertChain", m_NoVerifyCertChain);
            m_NoVerifyCertHostname = serverConfig.GetValue<bool>("NoVerifyCertHostname", m_NoVerifyCertHostname);

            string connList = serverConfig.GetValue<string>("ServiceConnectors", string.Empty);
            registryLocation = serverConfig.GetValue<string>("RegistryLocation", ".");

            var servicesConfig = m_configuration.GetSection("ServiceList");

            List<string> servicesList = new();
            if (!string.IsNullOrEmpty(connList))
                servicesList.Add(connList);
            
            foreach (var k in servicesConfig.AsEnumerable())
            {
                string[] keyparts = k.Key.Split(":");
                if ((keyparts.Length > 1) && (keyparts[0] == "ServiceList"))
                {
                    string v = servicesConfig.GetValue<string>(keyparts[1]);
                    if (!string.IsNullOrEmpty(v))
                        servicesList.Add(v);
                }
            }

            connList = string.Join(",", servicesList.ToArray());

            string[] conns = connList.Split(new char[] { ',', ' ', '\n', '\r', '\t' });

            foreach (string c in conns)
            {
                if (string.IsNullOrEmpty(c))
                    continue;

                string configName = string.Empty;
                string conn = c;
                uint port = 0;

                string[] split1 = conn.Split(new char[] { '/' });
                if (split1.Length > 1)
                {
                    conn = split1[1];

                    string[] split2 = split1[0].Split(new char[] { '@' });
                    if (split2.Length > 1)
                    {
                        configName = split2[0];
                        port = Convert.ToUInt32(split2[1]);
                    }
                    else
                    {
                        port = Convert.ToUInt32(split1[0]);
                    }
                }
                string[] parts = conn.Split(new char[] { ':' });
                string friendlyName = parts[0];
                if (parts.Length > 1)
                    friendlyName = parts[1];

                BaseHttpServer server;

                if (port != 0)
                    server = (BaseHttpServer)MainServer.GetHttpServer(port);
                else
                    server = MainServer.Instance;

                if (friendlyName == "LLLoginServiceInConnector")
                    server.AddSimpleStreamHandler(new IndexPHPHandler(server));

                m_logger.LogInformation("[SERVER]: Loading {0} on port {1}", friendlyName, server.Port);

                IServiceConnector connector = null;

                object[] modargs = new object[] { m_Server.Config, server, configName };
                connector = ServerUtils.LoadPlugin<IServiceConnector>(conn, modargs);

                if (connector == null)
                {
                    modargs = new object[] { m_Server.Config, server };
                    connector = ServerUtils.LoadPlugin<IServiceConnector>(conn, modargs);
                }

                if (connector != null)
                {
                    m_ServiceConnectors.Add(connector);
                    m_logger.LogInformation("[SERVER]: {0} loaded successfully", friendlyName);
                }
                else
                {
                    m_logger.LogError($"[SERVER]: Failed to load {conn}");
                }
            }

            PrintFileToConsole("robuststartuplogo.txt");

            return m_Server.Run();
        }

        public void Shutdown(int res)
        {
            m_Server?.Shutdown();
            Util.StopThreadPool();
            Environment.Exit(res);
        }
    }
}