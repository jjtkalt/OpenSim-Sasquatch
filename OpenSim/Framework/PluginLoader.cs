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

using System.Reflection;
using System.Runtime.Loader;
using log4net;

namespace OpenSim.Framework
{
    /// <summary>
    /// Generic Plugin Loader
    /// </summary>
    public class PluginLoader<T> where T : IPlugin
    {
        private static readonly ILog log
            = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public PluginInitialiserBase Initialiser { get; set; } = new PluginInitialiserBase();

        private string pluginBaseDir = String.Empty;

        public PluginLoader(PluginInitialiserBase init)
        {
            Initialiser = init;
        }

        public PluginLoader(PluginInitialiserBase init, string dir)
        {
            Initialiser = init;
            pluginBaseDir = dir;
        }

        public List<T> Load()
        {
            List<T> loadedPlugins = new List<T>();
            if (string.IsNullOrEmpty(pluginBaseDir))
            {
                pluginBaseDir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            }

            DirectoryInfo dir = new DirectoryInfo(pluginBaseDir);
            AssemblyLoadContext loadContext = new AssemblyLoadContext(dir.FullName);

            foreach (FileInfo fileInfo in dir.GetFiles("OpenSim.ApplicationPlugins.*.dll"))
            {
                try
                {
                    AssemblyName assemblyName = AssemblyName.GetAssemblyName(fileInfo.FullName);

                    log.Info($"[PLUGINLOADER] Searching {assemblyName.FullName} for modules to load");
                    Assembly pluginAssembly = loadContext.LoadFromAssemblyPath(fileInfo.FullName);

                    if (pluginAssembly != null)
                    {
                        foreach (var pluginType in pluginAssembly.GetTypes().Where((p) => p.IsPublic && !p.IsAbstract))
                        {
                            try
                            {
                                if (typeof(T).IsAssignableFrom(pluginType))
                                {
                                    log.Info($"[PLUGINLOADER] Initializing {pluginType.FullName}");
                                    loadedPlugins.Add((T)Activator.CreateInstance(pluginType));
                                }
                            }
                            catch (Exception ex)
                            {
                                log.Warn($"[MODULELOADER]: Error loading module {pluginType.Name} in file {fileInfo.FullName} : {ex}");
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    log.Warn($"[MODULELOADER]: Error loading assembly {fileInfo.FullName} : {ex}");
                    continue;
                }
            }

            foreach (T plugin in loadedPlugins)
            {
                Initialiser.Initialise(plugin);
            }

            return loadedPlugins;
        }
    }
}
