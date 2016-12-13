// Copyright (c) FastQuant Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.IO;

namespace FastQuant
{
    // FIXME: Will be remove when netstandard2.0 is ready
    internal static class InstallationUtils
    {
        public static string GetApplicationDataPath()
        {
#if NET451 || NET452 || NET46 || NET461 || NET462
            return Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
#else
            return Path.Combine(Environment.GetEnvironmentVariable("HOME") ?? Environment.GetEnvironmentVariable("USERPROFILE"), "AppData", "Roaming");
#endif
        }
    }

    public static class Installation
    {
        public static DirectoryInfo ApplicationDataDir => Directory.CreateDirectory(Path.Combine(InstallationUtils.GetApplicationDataPath(), "SmartQuant Ltd", "OpenQuant 2014"));

        public static DirectoryInfo DataDir => Directory.CreateDirectory(Path.Combine(ApplicationDataDir.FullName, "data"));

        public static DirectoryInfo ConfigDir => Directory.CreateDirectory(Path.Combine(ApplicationDataDir.FullName, "config"));

        public static DirectoryInfo LogsDir => Directory.CreateDirectory(Path.Combine(ApplicationDataDir.FullName, "logs"));
    }
}
