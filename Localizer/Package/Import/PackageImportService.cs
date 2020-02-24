using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Localizer.Attributes;
using Localizer.DataModel;
using Localizer.DataModel.Default;
using Ninject;

namespace Localizer.Package.Import
{
    public class PackageImportService : IPackageImportService
    {
        private readonly List<IPackageGroup> packageGroups = new List<IPackageGroup>();

        private Dictionary<Type, FileImporter> _importers;

        public PackageImportService()
        {
            _importers = new Dictionary<Type, FileImporter>();
        }

        public void RegisterImporter<T>(Type importerType) where T : IFile
        {
            if (importerType is null)
            {
                throw new ArgumentNullException(nameof(importerType));
            }

            Localizer.Kernel.Bind(typeof(FileImporter), importerType)
                     .To(importerType).InSingletonScope();

            var importer = Localizer.Kernel.Get(importerType) as FileImporter;
            if (importer is null)
            {
                Utils.LogError("Importer binding failed.");
                return;
            }

            if (_importers.ContainsKey(typeof(T)))
            {
                _importers[typeof(T)] = importer;
            }
            else
            {
                _importers.Add(typeof(T), importer);
            }

            Utils.LogInfo($"Importer: [{importer.GetType().FullName}] registered for file type: [{typeof(T).FullName}].");
        }

        public void UnregisterImporter<T>() where T : IFile
        {
            if (_importers.ContainsKey(typeof(T)))
            {
                _importers.Remove(typeof(T));
                Utils.LogInfo($"Unregistered importer of type [{typeof(T).FullName}]");
            }
        }

        public void Queue(IPackage package)
        {
            if (packageGroups.All(pg => pg.Mod != package.Mod))
            {
                packageGroups.Add(new PackageGroup
                {
                    Mod = package.Mod,
                    Packages = new List<IPackage> { package }
                });
            }
            else
            {
                packageGroups.FirstOrDefault(pg => pg.Mod == package.Mod)?.Packages.Add(package);
            }

            Utils.LogDebug($"Queued [{package.Name}]");
        }

        public void Import(bool preferEarly = true)
        {
            Utils.LogInfo($"Begin importing");

            foreach (var group in packageGroups)
            {
                Utils.LogInfo($"Importing Mod: [{group.Mod.Name}]");
                Utils.SafeWrap(() =>
                {
                    var merged = Merge(group);
                    foreach (var f in merged.Files)
                    {
                        Utils.SafeWrap(() =>
                        {
                            var importer = GetImporter(f);
                            if (importer is null)
                            {
                                return;
                            }

                            Import(f, group, importer, preferEarly);
                        });
                    }
                });
            }
        }

        private void Import(IFile f, IPackageGroup group, FileImporter importer, bool preferEarly)
        {
            var importerTiming = importer.GetType().GetCustomAttribute<OperationTimingAttribute>()
                                                ?.Timing ?? OperationTiming.Any;
            Utils.LogDebug($"Trying to Import [{f.GetType()}] using [{importer.GetType()}]" +
                           $"\n    ImporterTiming: [{importerTiming}]" +
                           $"\n    State: [{Localizer.State}]" +
                           $"\n    PreferEarly: [{preferEarly}]");
            if (!Localizer.CanDoOperationNow(importer.GetType())
                || (preferEarly && importerTiming > Localizer.State
                                && importerTiming == OperationTiming.Any
                                && Localizer.State != OperationTiming.BeforeModCtor))
            {
                return;
            }
            importer.Import(f, group.Mod, group.Packages.ToList()[0].Language);
            Utils.LogDebug($"Import Complete");
        }

        public void Reset()
        {
            Utils.LogInfo($"Resetting importers...");
            foreach (var importers in _importers.Values)
            {
                importers.Reset();
            }
            packageGroups.Clear();
            Utils.LogInfo($"Reset completed.");
        }

        public void Clear()
        {
            Utils.LogInfo($"Clearing import queue...");
            packageGroups.Clear();
            Utils.LogInfo($"Cleared.");
        }

        private FileImporter GetImporter(IFile file)
        {
            if (_importers.TryGetValue(file.GetType(), out var importer))
            {
                return importer;
            }

            Utils.LogWarn($"No registered importer for file type: [{file.GetType()}]");
            return null;
        }

        internal DataModel.Default.Package Merge(IPackageGroup group)
        {
            var result = new DataModel.Default.Package
            {
                Mod = group.Mod,
                ModName = group.Mod.Name,
                Files = new List<IFile>(),
                FileList = new List<string>()
            };

            foreach (var package in group.Packages)
            {
                Utils.SafeWrap(() =>
                {
                    foreach (var file in package.Files)
                    {
                        if (result.Files.All(f => f.GetType() != file.GetType()))
                        {
                            result.Files.Add(file);
                        }
                        else
                        {
                            var main = package.Files.FirstOrDefault(f => f.GetType() == file.GetType());
                            var importer = GetImporter(file);
                            if (importer is null)
                            {
                                continue;
                            }

                            var merged = importer.Merge(main, file);
                            result.Files.Remove(main);
                            result.Files.Add(merged);
                        }
                    }
                });
            }

            return result;
        }

        public void Dispose()
        {
            foreach (var importer in _importers.Values)
            {
                Utils.SafeWrap(() => importer.Dispose());
            }

            _importers = null;
        }
    }
}
