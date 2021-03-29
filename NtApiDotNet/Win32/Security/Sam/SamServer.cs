﻿//  Copyright 2021 Google Inc. All Rights Reserved.
//
//  Licensed under the Apache License, Version 2.0 (the "License");
//  you may not use this file except in compliance with the License.
//  You may obtain a copy of the License at
//
//  http://www.apache.org/licenses/LICENSE-2.0
//
//  Unless required by applicable law or agreed to in writing, software
//  distributed under the License is distributed on an "AS IS" BASIS,
//  WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//  See the License for the specific language governing permissions and
//  limitations under the License.

using NtApiDotNet.Win32.SafeHandles;
using NtApiDotNet.Win32.Security.Native;

namespace NtApiDotNet.Win32.Security.Sam
{
    /// <summary>
    /// Class to represent a connection to a SAM server.
    /// </summary>
    public sealed class SamServer : SamObject
    {
        #region Private Members
        private readonly string _server_name;

        private SamServer(SafeSamHandle handle, SamServerAccessRights granted_access, string server_name)
            : base(handle, granted_access, SamUtils.SAM_SERVER_NT_TYPE_NAME, string.IsNullOrEmpty(server_name) ? "SAM Server" : $"SAM Server ({server_name})")
        {
            _server_name = server_name;
        }
        #endregion

        /// <summary>
        /// Connect to a SAM server.
        /// </summary>
        /// <param name="server_name">The name of the server. Set to null for local connection.</param>
        /// <param name="desired_access">The desired access on the SAM server.</param>
        /// <param name="throw_on_error">True to throw on error.</param>
        /// <returns>The server connection.</returns>
        public static NtResult<SamServer> Connect(string server_name, SamServerAccessRights desired_access, bool throw_on_error)
        {
            UnicodeString name = string.IsNullOrEmpty(server_name) ? null : new UnicodeString(server_name);

            return SecurityNativeMethods.SamConnect(name, out SafeSamHandle handle, desired_access, 
                null).CreateResult(throw_on_error, () => new SamServer(handle, desired_access, server_name));
        }

        /// <summary>
        /// Connect to a SAM server.
        /// </summary>
        /// <param name="server_name">The name of the server. Set to null for local connection.</param>
        /// <param name="desired_access">The desired access on the SAM server.</param>
        /// <returns>The server connection.</returns>
        public static SamServer Connect(string server_name, SamServerAccessRights desired_access)
        {
            return Connect(server_name, desired_access, true).Result;
        }

        /// <summary>
        /// Connect to a SAM server.
        /// </summary>
        /// <param name="desired_access">The desired access on the SAM server.</param>
        /// <returns>The server connection.</returns>
        public static SamServer Connect(SamServerAccessRights desired_access)
        {
            return Connect(null, desired_access);
        }

        /// <summary>
        /// Connect to a SAM server with maximum access.
        /// </summary>
        /// <returns>The server connection.</returns>
        public static SamServer Connect()
        {
            return Connect(SamServerAccessRights.MaximumAllowed);
        }
    }
}