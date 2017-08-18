﻿using System;
using JetBrains.Annotations;
using System.IO;
using System.Reflection;

namespace AudioWorks.Commands.Tests
{
    [UsedImplicitly]
    public class ExtensionsFixture
    {
        public ExtensionsFixture()
        {
            var configuration = new DirectoryInfo(Directory.GetCurrentDirectory()).Parent.Name;
            var extensionsInstallDir = new DirectoryInfo(
                    Path.GetDirectoryName(new Uri(Assembly.GetExecutingAssembly().CodeBase).LocalPath))
                .CreateSubdirectory("Module").CreateSubdirectory("Extensions");

            foreach (var extensionProjectDir in new DirectoryInfo(Path.Combine(
                new DirectoryInfo(Directory.GetCurrentDirectory()).Parent.Parent.Parent.Parent.FullName,
                "Extensions")).GetDirectories())
            {
                DotNetUtility.Publish(
                    extensionProjectDir.FullName,
                    configuration,
                    extensionsInstallDir.CreateSubdirectory(extensionProjectDir.Name).FullName);
            }
        }
    }
}
