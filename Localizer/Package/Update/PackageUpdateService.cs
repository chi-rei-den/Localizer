using System;
using System.Collections.Generic;
using System.Linq;
using Localizer.DataModel;

namespace Localizer.Package.Update
{
    public sealed class PackageUpdateService : IPackageUpdateService
    {
        private Dictionary<Type, FileUpdater> _updaters;

        public PackageUpdateService()
        {
            _updaters = new Dictionary<Type, FileUpdater>();
        }

        public void RegisterUpdater<T>(FileUpdater updater) where T : IFile
        {
            if (updater is null)
            {
                throw new ArgumentNullException(nameof(updater));
            }

            if (_updaters.ContainsKey(typeof(T)))
            {
                _updaters[typeof(T)] = updater;
            }
            else
            {
                _updaters.Add(typeof(T), updater);
            }

            Utils.LogInfo($"Updater: [{updater.GetType().FullName}] registered for file type: [{typeof(T).FullName}].");
        }

        public void UnregisterUpdater<T>() where T : IFile
        {
            if (_updaters.ContainsKey(typeof(T)))
            {
                _updaters.Remove(typeof(T));
                Utils.LogInfo($"Unregistered updater of type [{typeof(T).FullName}]");
            }
        }

        public void Update(IPackage oldPackage, IPackage newPackage, IUpdateLogger logger)
        {
            Utils.LogDebug($"Updating package [{oldPackage.Name}]");

            foreach (var oldFile in oldPackage.Files)
            {
                var updater = GetUpdater(oldFile);
                if (updater is null)
                {
                    continue;
                }

                Utils.SafeWrap(() =>
                {
                    updater.Update(oldFile, newPackage.Files.FirstOrDefault(f => f.GetType() == oldFile.GetType()),
                                   logger);
                });
            }

            foreach (var file in newPackage.Files)
            {
                if (oldPackage.Files.All(f => f.GetType() != file.GetType()))
                {
                    oldPackage.AddFile(file);
                }
            }

            Utils.LogDebug($"Package [{oldPackage.Name}] updated.");
        }

        private FileUpdater GetUpdater<T>(T file) where T : IFile
        {
            if (_updaters.TryGetValue(file.GetType(), out var updater))
            {
                return updater;
            }

            Utils.LogWarn($"No registered importer for file type: [{file.GetType()}]");
            return null;
        }
    }
}
