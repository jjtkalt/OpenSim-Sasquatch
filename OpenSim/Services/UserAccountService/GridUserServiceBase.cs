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


using OpenSim.Data;
using OpenSim.Services.Base;
using Microsoft.Extensions.Configuration;

namespace OpenSim.Services.UserAccountService
{
    public class GridUserServiceBase : ServiceBase
    {
        protected IGridUserData m_Database = null;

        public GridUserServiceBase(IConfiguration config) : base(config)
        {
            string dllName = String.Empty;
            string connString = String.Empty;
            string realm = "GridUser";

            //
            // Try reading the [DatabaseService] section, if it exists
            //
            var dbConfig = config.GetSection("DatabaseService");
            if (dbConfig.Exists())
            {
                if (string.IsNullOrEmpty(dllName))
                    dllName = dbConfig.GetValue("StorageProvider", String.Empty);
                if (string.IsNullOrEmpty(connString))
                    connString = dbConfig.GetValue("ConnectionString", String.Empty);
            }

            //
            // [GridUsetService] section overrides [DatabaseService], if it exists
            //
            var usersConfig = config.GetSection("GridUserService");
            if (usersConfig.Exists())
            {
                dllName = usersConfig.GetValue("StorageProvider", dllName);
                connString = usersConfig.GetValue("ConnectionString", connString);
                realm = usersConfig.GetValue("Realm", realm);
            }

            //
            // We tried, but this doesn't exist. We can't proceed.
            //
            if (string.IsNullOrEmpty(dllName))
                throw new Exception("No StorageProvider configured");

            m_Database = LoadPlugin<IGridUserData>(dllName, new Object[] { connString, realm });
            
            if (m_Database == null)
                throw new Exception("Could not find a storage interface in the given module " + dllName);
        }
    }
}