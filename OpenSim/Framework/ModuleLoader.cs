/*
 * Copyright (c) Contributors, http://whitecore-sim.org/, http://aurora-sim.org
 * See CONTRIBUTORS.TXT for a full list of copyright holders.
 *
 * Redistribution and use in source and binary forms, with or without
 * modification, are permitted provided that the following conditions are met:
 *     * Redistributions of source code must retain the above copyright
 *       notice, this list of conditions and the following disclaimer.
 *     * Redistributions in binary form must reproduce the above copyright
 *       notice, this list of conditions and the following disclaimer in the
 *       documentation and/or other materials provided with the distribution.
 *     * Neither the name of the WhiteCore-Sim Project nor the
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

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace OpenSim.Framework
{
    public static class ModuleLoader
    {
        #region Module Loaders

        /// <summary>
        ///     Find all T modules in the current directory
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static List<T> FindModules<T>()
        {
            return LoadModules<T>(".");
        }

        /// <summary>
        ///     Gets all modules found in the given directory.
        ///     Identifier is the name of the interface.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="moduleDir"></param>
        /// <returns></returns>
        public static List<T> LoadModules<T>(string moduleDir)
        {
            PluginLoadContext loadContext = new PluginLoadContext(moduleDir);
            List<T> modules = new List<T>();

            DirectoryInfo dir = new DirectoryInfo(moduleDir);

            foreach (FileInfo fileInfo in dir.GetFiles("*.dll"))
                modules.AddRange(LoadModulesFromDLL<T>(moduleDir, fileInfo.FullName));

            //try
            //{
            //    List<Type> loadedDllModules;
            //            LoadedDlls.TryGetValue(moduleDir, out loadedDllModules);
            //            foreach (Type pluginType in loadedDllModules)
            //            {
            //                try
            //                {
            //                    if (pluginType.IsPublic)
            //                    {
            //                        if (!pluginType.IsAbstract)
            //                        {
            //                            if(typeof (T).IsAssignableFrom(pluginType))
            //                            {
            //                                modules.Add((T) Activator.CreateInstance(pluginType));
            //                            }
            //                        }
            //                    }
            //                }
            //                catch (Exception)
            //                {
            //                }
            //            }
            //        }
            //        catch (Exception)
            //        {
            //        }

            return modules;
        }

        /// <summary>
        ///     Load all T modules from dllname
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="moduleDir"></param>
        /// <param name="dllName"></param>
        /// <returns></returns>
        private static List<T> LoadModulesFromDLL<T>(string moduleDir, string dllName)
        {
            List<T> modules = new List<T>();

            Assembly pluginAssembly = Assembly.Load(AssemblyName.GetAssemblyName(dllName));

            if (pluginAssembly != null)
            {
                try
                {
                    List<Type> loadedTypes = new List<Type>();
                    foreach (Type pluginType in pluginAssembly.GetTypes().Where((p) => p.IsPublic && !p.IsAbstract))
                    {
                        try
                        {
                            if (typeof(T).IsAssignableFrom(pluginType))
                            {
                                modules.Add((T) Activator.CreateInstance(pluginType));
                            }
                        }
                        catch (Exception ex)
                        {
                            //MainConsole.Instance.Warn("[MODULELOADER]: Error loading module " + pluginType.Name +
                            //                          " in file " + dllName +
                            //                          " : " + ex);
                        }
                    }
                }
                catch (Exception)
                {
                }
            }

            return modules;
        }

        #endregion

        /// <summary>
        ///     Load all plugins from the given .dll file with the interface 'type'
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dllName"></param>
        /// <returns></returns>
        public static List<T> LoadPlugins<T>(string dllName)
        {
            List<T> plugins = new List<T>();
            try
            {
                Assembly pluginAssembly = Assembly.Load(AssemblyName.GetAssemblyName(dllName));
                foreach (Type pluginType in pluginAssembly.GetTypes().Where(pluginType => pluginType.IsPublic))
                {
                    try
                    {
                        if (typeof(T).IsAssignableFrom(pluginType))
                        {
                            plugins.Add((T) Activator.CreateInstance(pluginType));
                        }
                    }
                    catch (Exception)
                    {
                    }
                }
            }
            catch (ReflectionTypeLoadException e)
            {
                foreach (Exception e2 in e.LoaderExceptions)
                {
                    //MainConsole.Instance.Error(e2.ToString());
                }
                throw e;
            }
            return plugins;
        }
    }
}
