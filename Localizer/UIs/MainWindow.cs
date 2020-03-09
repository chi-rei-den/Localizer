using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Localizer.DataModel;
using Localizer.Helpers;
using Localizer.Network;
using Localizer.Package;
using Localizer.Package.Export;
using Localizer.Package.Load;
using Localizer.Package.Save;
using Localizer.Package.Update;
using Localizer.UIs.Components;
using Ninject;
using Squid;
using Terraria.Localization;
using static Localizer.Lang;

namespace Localizer.UIs
{
    public class MainWindow : BasicWindow
    {
        private Panel _menuBar;

        private SplitContainer _split;

        private BasicListBox _modList = new BasicListBox();

        private BasicListBox _pkgList = new BasicListBox
        {
            MaxSelected = 0,
            AllowDrop = true
        };

        private IPackageManageService pkgManager;

        private IFileLoadService fileLoadService;

        private IFileSaveService fileSaveService;

        private IPackageExportService packageExportService;

        private IPackageSaveService packageSaveService;

        private IPackageUpdateService packageUpdateService;

        private IPackageLoadService<DataModel.Default.Package> packedPackageLoadServiceService;

        private IPackageLoadService<DataModel.Default.Package> sourcePackageLoadServiceService;

        private IPackageBrowserService packageBrowserService;

        private IDownloadManagerService downloadManagerService;

        private List<IPackage> onlinePackages = new List<IPackage>();

        private bool loading = false;

        public MainWindow()
        {
            Button AddButton(string text, string tooltip, MouseEvent action)
            {
                var button = new Button
                {
                    Text = text,
                    Tooltip = tooltip,
                    Dock = DockStyle.Left
                };
                button.MouseClick += action;
                _menuBar.Content.Controls.Add(button);
                return button;
            }

            loading = true;
            pkgManager = Localizer.Kernel.Get<IPackageManageService>();
            sourcePackageLoadServiceService = Localizer.Kernel.Get<SourcePackageLoad<DataModel.Default.Package>>();
            packedPackageLoadServiceService = Localizer.Kernel.Get<PackedPackageLoad<DataModel.Default.Package>>();
            packageExportService = Localizer.Kernel.Get<IPackageExportService>();
            packageSaveService = Localizer.Kernel.Get<IPackageSaveService>();
            packageUpdateService = Localizer.Kernel.Get<IPackageUpdateService>();
            fileLoadService = Localizer.Kernel.Get<IFileLoadService>();
            fileSaveService = Localizer.Kernel.Get<IFileSaveService>();
            packageBrowserService = Localizer.Kernel.Get<IPackageBrowserService>();
            downloadManagerService = Localizer.Kernel.Get<IDownloadManagerService>();
            pkgManager.PackageGroups = new ObservableCollection<IPackageGroup>();
            Size = new Point(800, 340);
            Position = new Point(40, 200);
            Titlebar.Text = _("PackageManage");
            Titlebar.Button.MouseClick += (sender, args) => Visible = false;
            Resizable = true;

            _menuBar = new Panel
            {
                Dock = DockStyle.Top,
                Size = new Point(0, 30)
            };
            Controls.Add(_menuBar);

            AddButton(_("Reload"), _("ReloadDesc"), (sender, args) =>
            {
                if (args.Button == 0)
                {
                    LoadPackages();
                }
            });

            var refreshBtn = AddButton(_("RefreshOnline"), _("RefreshOnlineDesc"), (sender, args) =>
            {
                if (args.Button == 0)
                {
                    RefreshOnlinePackages(sender);
                }
            });

            AddButton(_("OpenFolder"), _("OpenFolderDesc"), (sender, args) =>
            {
                if (args.Button == 0)
                {
                    Process.Start($"file://{Path.Combine(Environment.CurrentDirectory, "Localizer")}");
                }
            });

            AddButton(_("Export"), _("ExportDesc"), (sender, args) =>
            {
                if (args.Button == 0)
                {
                    Export(false);
                }
            });

            AddButton(_("ExportWithTranslation"), _("ExportWithTranslationDesc"), (sender, args) =>
            {
                if (args.Button == 0)
                {
                    Export(true);
                }
            });

            _split = new SplitContainer
            {
                Margin = new Margin(0, 10, 0, 0),
                Dock = DockStyle.Fill
            };
            _split.SplitButton.Style = "button";
            _split.SplitFrame1.AutoSize = AutoSize.Horizontal;
            _split.SplitFrame2.AutoSize = AutoSize.Horizontal;
            Controls.Add(_split);
            _split.SplitFrame1.Controls.Add(_modList);
            _modList.SelectedItemChanged += (sender, value) => RefreshPkgList();
            _split.SplitFrame2.Controls.Add(_pkgList);
            RefreshModList();
            LoadPackages().ContinueWith(_ => RefreshOnlinePackages(refreshBtn));
        }

        private void Export(bool withTranslation)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(_modList.SelectedItem?.Text ?? ""))
                {
                    return;
                }

                var name = _modList.SelectedItem.Text;
                var mod = Localizer.GetWrappedMod(name);

                var enabledPackages = pkgManager.PackageGroups
                    .FirstOrDefault(g => g.Mod.Name == name)?.Packages
                    .Where(p => p.Enabled)
                    .OrderBy(p => p.ModVersion ?? new Version(0, 0, 0, 0))
                    .ToList();
                var oldPack = enabledPackages?.FirstOrDefault();

                var package = new DataModel.Default.Package
                {
                    Name = oldPack?.Name ?? name,
                    LocalizedModName = oldPack?.LocalizedModName ?? name,
                    Language = CultureInfo.GetCultureInfo(Language.ActiveCulture.Name),
                    ModName = name,
                    Files = new ObservableCollection<IFile>(),
                    Version = oldPack?.Version ?? new Version("1.0.0.0"),
                    Author = oldPack?.Author ?? "",
                    Description = oldPack?.Description ?? "",
                    Mod = mod,
                    ModVersion = Version.Parse($"{Math.Max(mod.Version.Major, 0)}.{Math.Max(mod.Version.Minor, 0)}.{Math.Max(mod.Version.Build, 0)}.{Math.Max(mod.Version.Revision, 0)}")
                };

                var dirPath = Utils.EscapePath(Path.Combine(Localizer.SourcePackageDirPath, name));

                packageExportService.Export(package, new DataModel.Default.ExportConfig
                {
                    WithTranslation = withTranslation
                });

                if (!Directory.Exists(dirPath))
                {
                    Directory.CreateDirectory(dirPath);
                }

                if (enabledPackages != null && enabledPackages.Count > 0)
                {
                    oldPack.ModVersion = package.Mod.Version;
                    var updateLogger = Localizer.Kernel.Get<IUpdateLogger>();
                    updateLogger.Init($"{package.Name}-{Utils.DateTimeToFileName(DateTime.Now)}");

                    foreach (var item in enabledPackages.Skip(1))
                    {
                        packageUpdateService.Update(oldPack, item, updateLogger);
                    }

                    packageUpdateService.Update(oldPack, package, updateLogger);

                    packageSaveService.Save(oldPack, dirPath, fileSaveService);
                }
                else
                {
                    packageSaveService.Save(package, dirPath, fileSaveService);
                }
            }
            catch (Exception e)
            {
                Localizer.Log.Error(e);
            }
        }

        private Task RefreshOnlinePackages(Control sender)
        {
            return Task.Run(() =>
            {
                try
                {
                    loading = true;
                    sender.Enabled = false;
                    onlinePackages?.Clear();
                    onlinePackages = packageBrowserService.GetList().ToList();
                }
                catch
                {
                }
                finally
                {
                    loading = false;
                    sender.Enabled = true;
                    RefreshPkgList();
                }
            });
        }

        private Task LoadPackages()
        {
            return Task.Run(() =>
            {
                try
                {
                    loading = true;
                    pkgManager.PackageGroups = new List<IPackageGroup>();
                    foreach (var dir in new DirectoryInfo(Localizer.SourcePackageDirPath).GetDirectories())
                    {
                        try
                        {
                            var package2 = sourcePackageLoadServiceService.Load(dir.FullName, fileLoadService);
                            if (package2 != null)
                            {
                                pkgManager.AddPackage(package2);
                            }
                        }
                        catch
                        {
                        }
                    }

                    var list = Directory.GetFiles(Localizer.DownloadPackageDirPath).ToList();
                    list.AddRange(Directory.GetFiles(Path.Combine(Terraria.Main.SavePath, "Mods"), "*.locpack"));
                    foreach (var file in list)
                    {
                        try
                        {
                            var package = packedPackageLoadServiceService.Load(file, fileLoadService);
                            if (package != null)
                            {
                                pkgManager.AddPackage(package);
                            }
                        }
                        catch
                        {
                        }
                    }
                    pkgManager.LoadState();
                }
                catch (Exception o)
                {
                    Utils.LogError(o);
                }
                finally
                {
                    loading = false;
                    RefreshPkgList();
                }
            });
        }

        private object refreshLock = new object();
        private void RefreshPkgList()
        {
            string TrimEndingZero(string input)
            {
                while (input.EndsWith(".0"))
                {
                    input = input.Substring(0, input.Length - 2);
                }
                return input;
            }

            lock (refreshLock)
            {
                UIModsPatch.ModsExtraInfo = pkgManager.PackageGroups.ToDictionary(key => key.Mod.Name,
                    value => string.Join(Environment.NewLine, value.Packages.Select(UI.GetPkgLabelText)));

                var modName = _modList.SelectedItem?.Text ?? "";
                try
                {
                    _split.SplitFrame2.Controls.Clear();
                    _pkgList.Items.Clear();
                    _split.SplitFrame2.Controls.Add(_pkgList);
                    var packageGroup = pkgManager.PackageGroups.FirstOrDefault(g => g.Mod.Name == modName)?.Packages.ToList();
                    var onlineGroup = onlinePackages.Where(g => g.ModName == modName).ToList();
                    var displayedPackages = new Dictionary<string, ListBoxItem>();

                    if (packageGroup != null)
                    {
                        foreach (var p in packageGroup)
                        {
                            // TODO: Use better key for HashSet
                            var item = new ListBoxItem
                            {
                                Text = UI.GetPkgLabelText(p),
                                Tooltip = (Gui.Renderer as UIRenderer).WordWrap(p.Description, 400),
                                TextWrap = true,
                                Dock = DockStyle.Top,
                                AutoSize = AutoSize.Vertical
                            };
                            item.MouseClick += (sender, args) =>
                            {
                                if (args.Button == 0)
                                {
                                    UIModsPatch.ReloadRequired = true;
                                    p.Enabled = !p.Enabled;
                                    item.Text = UI.GetPkgLabelText(p);
                                    pkgManager.SaveState();
                                }
                            };
                            displayedPackages.Add(TrimEndingZero($"{p.ModName}_{p.Author}_{p.Version}"), item);
                            _pkgList.Items.Add(item);
                        }
                    }

                    if (onlineGroup != null)
                    {
                        foreach (var p in onlineGroup)
                        {
                            if (displayedPackages.ContainsKey(TrimEndingZero($"{p.ModName}_{p.Author}_{p.Version}")))
                            {
                                continue;
                            }
                            var existing = displayedPackages.FirstOrDefault(kvp => kvp.Key.StartsWith($"{p.ModName}_{p.Author}_"));
                            var update = displayedPackages.Any(kvp => kvp.Key.StartsWith($"{p.ModName}_{p.Author}_"));
                            var item = new ListBoxItem
                            {
                                Text = _("PackageDisplay", _(update ? "PackageUpdate" : "PackageOnline"), p.Name, p.Version, p.Author),
                                Tooltip = (Gui.Renderer as UIRenderer).WordWrap(p.Description, 400),
                                TextWrap = true,
                                Dock = DockStyle.Top,
                                AutoSize = AutoSize.Vertical
                            };
                            item.MouseClick += (sender, args) => DownloadPackage(args, p);
                            displayedPackages.Add(TrimEndingZero($"{p.ModName}_{p.Author}_{p.Version}"), item);
                            _pkgList.Items.Add(item);
                        }
                    }

                    if (displayedPackages.Count == 0)
                    {
                        _split.SplitFrame2.Controls.Add(new Label
                        {
                            Text = _(loading ? "PackageLoading" : "NoPackageFound"),
                            TextWrap = true,
                            AutoSize = AutoSize.Vertical,
                            Dock = DockStyle.Fill,
                            AllowFocus = false
                        });
                    }
                }
                catch (Exception o)
                {
                    Utils.LogError(o);
                }
            }
        }

        private void DownloadPackage(MouseEventArgs args, IPackage p)
        {
            if (args.Button == 0)
            {
                Task.Run(() =>
                {
                    try
                    {
                        Utils.LogDebug($"Requesting {p.Name} download");
                        var url = packageBrowserService.GetDownloadLinkOf(p);
                        var path = Utils.EscapePath(Path.Combine(Localizer.DownloadPackageDirPath, $"{p.Name}_{p.Author}.locpack"));
                        downloadManagerService.Download(url, path);
                        UIModsPatch.ReloadRequired = true;
                        Utils.LogDebug($"{p.Name} is downloaded");
                        LoadPackages();
                    }
                    catch
                    {
                    }
                });
            }
        }

        private void RefreshModList()
        {
            _modList.Items.Clear();
            foreach (var loadedMod in Utils.GetLoadedMods())
            {
                if (loadedMod != null && loadedMod.Name != Localizer.LoadedLocalizer.Name)
                {
                    _modList.Items.Add(new ListBoxItem
                    {
                        Text = loadedMod.Name,
                        Tooltip = $"{loadedMod.DisplayName} (v{loadedMod.Version})"
                    });
                }
            }
        }
    }
}
