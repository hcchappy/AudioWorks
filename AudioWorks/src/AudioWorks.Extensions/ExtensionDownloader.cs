﻿using System;
using System.IO;
using System.Linq;
using System.Threading;
using AudioWorks.Common;
using JetBrains.Annotations;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using NuGet.Common;
using NuGet.Configuration;
using NuGet.PackageManagement;
using NuGet.Protocol;
using NuGet.Protocol.Core.Types;
using NuGet.Resolver;
using ILogger = Microsoft.Extensions.Logging.ILogger;

namespace AudioWorks.Extensions
{
    static class ExtensionDownloader
    {
        [NotNull] static readonly string _projectRoot = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "AudioWorks",
            "Extensions");

        [NotNull] static readonly string _customUrl = ConfigurationManager.Configuration.GetValue("ExtensionRepository",
            "https://www.myget.org/F/audioworks-extensions/api/v3/index.json");

        [NotNull] static readonly string _defaultUrl = ConfigurationManager.Configuration.GetValue("DefaultRepository",
            "https://api.nuget.org/v3/index.json");

        [NotNull] static readonly object _syncRoot = new object();
        static bool _alreadyDownloaded;

        internal static void DownloadOnce()
        {
            if (_alreadyDownloaded) return;

            lock (_syncRoot)
            {
                try
                {
                    var logger = LoggingManager.CreateLogger(typeof(ExtensionDownloader).FullName);

                    if (!ConfigurationManager.Configuration.GetValue("AutomaticExtensionDownloads", true))
                        logger.LogInformation("Automatic extension downloads are disabled.");
                    else
                    {
                        logger.LogInformation("Beginning automatic extension updates.");

                        Directory.CreateDirectory(_projectRoot);

                        var customRepository =
                            new SourceRepository(new PackageSource(_customUrl), Repository.Provider.GetCoreV3());
                        var defaultRepository =
                            new SourceRepository(new PackageSource(_defaultUrl), Repository.Provider.GetCoreV3());

                        var settings = Settings.LoadDefaultSettings(_projectRoot);
                        var packageManager = new NuGetPackageManager(
                            new SourceRepositoryProvider(settings, Repository.Provider.GetCoreV3()),
                            settings,
                            _projectRoot);

                        var cancelSource = new CancellationTokenSource(
                            ConfigurationManager.Configuration.GetValue("AutomaticExtensionDownloadTimeout", 30) *
                            1000);

                        try
                        {
                            var packageSearchResourceTask =
                                customRepository.GetResourceAsync<PackageSearchResource>(cancelSource.Token);

                            // Workaround - GetResourceAsync.Result can block even when the token is canceled
                            packageSearchResourceTask.Wait(cancelSource.Token);
                            if (packageSearchResourceTask.IsCanceled)
                            {
                                logger.LogError("Timed out retrieving resource at '{0}'.", _customUrl);
                                return;
                            }

                            var packageSearchResource = packageSearchResourceTask.Result;

                            var publishedPackages = packageSearchResource.SearchAsync("AudioWorks.Extensions",
                                    new SearchFilter(true), 0, 100, NullLogger.Instance, cancelSource.Token)
                                .Result.ToArray();

                            logger.LogInformation("Discovered {0} packages published at '{1}'.",
                                publishedPackages.Length, _customUrl);

                            foreach (var publishedPackage in publishedPackages)
                            {
                                var extensionDir =
                                    new DirectoryInfo(Path.Combine(_projectRoot, publishedPackage.Identity.ToString()));
                                if (extensionDir.Exists)
                                {
                                    logger.LogInformation("Package '{0}' is already installed.",
                                        publishedPackage.Identity.ToString());

                                    continue;
                                }

                                logger.LogInformation("Installing package '{0}'.",
                                    publishedPackage.Identity.ToString());

                                extensionDir.Create();
                                var stagingRootDir = extensionDir.CreateSubdirectory("Staging");

                                var project = new ExtensionNuGetProject(stagingRootDir.FullName);

                                packageManager.InstallPackageAsync(
                                        project,
                                        publishedPackage.Identity,
                                        new ResolutionContext(DependencyBehavior.Lowest, true, false,
                                            VersionConstraints.None),
                                        new ExtensionProjectContext(),
                                        customRepository,
                                        new[] { defaultRepository },
                                        cancelSource.Token)
                                    .Wait(cancelSource.Token);

                                // Move newly installed packages into the extension folder
                                foreach (var installedPackage in project.GetInstalledPackagesAsync(cancelSource.Token)
                                    .Result)
                                {
                                    var packageDir =
                                        new DirectoryInfo(project.GetInstalledPath(installedPackage.PackageIdentity));

                                    foreach (var subDir in packageDir.GetDirectories())
                                        switch (subDir.Name)
                                        {
                                            case "lib":
                                                MoveContents(subDir
                                                        .GetDirectories("netstandard*")
                                                        .OrderByDescending(dir => dir.Name)
                                                        .FirstOrDefault(),
                                                    extensionDir, logger);
                                                break;
                                            case "contentFiles":
                                                MoveContents(subDir
                                                        .GetDirectories("any").FirstOrDefault()?
                                                        .GetDirectories("netstandard*")
                                                        .OrderByDescending(dir => dir.Name)
                                                        .FirstOrDefault(),
                                                    extensionDir, logger);
                                                break;
                                        }
                                }

                                stagingRootDir.Delete(true);
                            }

                            // Remove any extensions that aren't published
                            foreach (var obsoleteExtension in new DirectoryInfo(_projectRoot).GetDirectories()
                                .Select(dir => dir.Name)
                                .Except(publishedPackages.Select(package => package.Identity.ToString()),
                                    StringComparer.OrdinalIgnoreCase))
                            {
                                Directory.Delete(Path.Combine(_projectRoot, obsoleteExtension), true);

                                logger.LogInformation("Deleted unlisted extension in '{0}'.",
                                    obsoleteExtension);
                            }
                        }
                        catch (Exception e)
                        {
                            if (e is AggregateException aggregate)
                                foreach (var inner in aggregate.InnerExceptions)
                                    logger.LogError(inner, e.Message);
                            else
                                logger.LogError(e, e.Message);
                        }
                    }
                }
                finally
                {
                    // Regardless of success or failure, don't try again
                    _alreadyDownloaded = true;
                }
            }
        }

        static void MoveContents(
            [CanBeNull] DirectoryInfo source,
            [NotNull] DirectoryInfo destination,
            [NotNull] ILogger logger)
        {
            if (source == null || !source.Exists) return;

            foreach (var file in source.GetFiles()
                .Where(file => file.Extension.Equals(".dll", StringComparison.OrdinalIgnoreCase)))
            {
                logger.LogInformation("Moving '{0}' to '{1}'.",
                    file.FullName, destination.FullName);

                file.MoveTo(Path.Combine(destination.FullName, file.Name));
            }

            foreach (var subdir in source.GetDirectories())
                MoveContents(subdir, destination.CreateSubdirectory(subdir.Name), logger);

            source.Delete(true);
        }
    }
}