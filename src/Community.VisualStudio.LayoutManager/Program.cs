﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;

namespace Community.VisualStudio.LayoutManager
{
    internal static class Program
    {
        public static void Main(string[] args)
        {
            var assembly = typeof(Program).GetTypeInfo().Assembly;

            Console.WriteLine($"{assembly.GetCustomAttribute<AssemblyProductAttribute>().Product} {assembly.GetName().Version}");
            Console.WriteLine();

            var configurationBuilder = new ConfigurationBuilder()
                .AddCommandLine(args);

            try
            {
                var configuration = configurationBuilder.Build();

                var layoutPath = configuration["layout-path"];

                if (layoutPath == null)
                {
                    throw new InvalidOperationException("Layout path is not specified");
                }

                var command = configuration["command"];

                if (command == null)
                {
                    throw new InvalidOperationException("Command is not specified");
                }

                var catalogPath = Path.Combine(layoutPath, "Catalog.json");

                if (!File.Exists(catalogPath))
                {
                    throw new InvalidOperationException("Catalog is not found");
                }

                var catalog = default(CatalogInfo);

                using (var stream = new FileStream(catalogPath, FileMode.Open, FileAccess.Read, FileShare.Read))
                {
                    using (var reader = new StreamReader(stream, Encoding.UTF8))
                    {
                        catalog = JsonConvert.DeserializeObject<CatalogInfo>(reader.ReadToEnd());
                    }
                }

                Console.WriteLine($"Found layout for Visual Studio {catalog.Product.ProductDisplayVersion}");
                Console.WriteLine();

                var layoutPackages = new List<CatalogPackageInfo>();
                var packageNameRegex = new Regex("^(?<id>[^,]+),version=(?<version>[^,]+)(?:,chip=(?<chip>[^,]+))?(?:,language=(?<language>[^,]+))?$", RegexOptions.Compiled);

                foreach (var directoryPath in Directory.GetDirectories(layoutPath))
                {
                    var match = packageNameRegex.Match(Path.GetFileName(directoryPath));

                    if (!match.Success)
                    {
                        continue;
                    }

                    var package = new CatalogPackageInfo
                    {
                        ID = match.Groups["id"].Value,
                        Version = match.Groups["version"].Value,
                        Chip = match.Groups["chip"].Success ? match.Groups["chip"].Value : null,
                        Language = match.Groups["language"].Success ? match.Groups["language"].Value : null
                    };

                    layoutPackages.Add(package);
                }

                var obsoletePackages = layoutPackages.Except(catalog.Packages, new CatalogPackageEqualityComparer())
                    .OrderBy(x => x.ID)
                    .ThenBy(x => x.Version)
                    .ToArray();

                if (obsoletePackages.Length == 0)
                {
                    Console.WriteLine("There are no obsolete packages");
                }

                switch (command)
                {
                    case "list-obsolete":
                        {
                            foreach (var package in obsoletePackages)
                            {
                                Console.WriteLine(package);
                            }
                        }
                        break;
                    case "remove-obsolete":
                        {
                            foreach (var package in obsoletePackages)
                            {
                                Console.WriteLine($"Removing {package}...");

                                var directoryPath = Path.Combine(layoutPath, package.ToString());

                                if (Directory.Exists(directoryPath))
                                {
                                    Directory.Delete(directoryPath, true);
                                }
                            }
                        }
                        break;
                    default:
                        throw new InvalidOperationException("Invalid command");
                }
            }
            catch (Exception ex)
            {
                Environment.ExitCode = 1;

                Console.WriteLine($"ERROR: {ex.Message}");
                Console.WriteLine();

                var assemblyFile = Path.GetFileName(assembly.Location);

                Console.WriteLine($"Usage: dotnet {assemblyFile} --layout-path <value> --command <value>");
                Console.WriteLine();
                Console.WriteLine("Available commands:");
                Console.WriteLine("    list-obsolete      List obsolete packages which are not included in the catalog");
                Console.WriteLine("    remove-obsolete    Remove obsolete packages which are not included in the catalog");
            }
        }
    }
}