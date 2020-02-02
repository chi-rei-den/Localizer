using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using Localizer.DataModel;
using Localizer.Network;
using Localizer.Package;
using Localizer.Package.Import;
using Localizer.Package.Load;
using Localizer.UIs.Components;
using Ninject;
using Squid;
using static Localizer.Lang;

namespace Localizer.UIs
{
    public class MainWindow : BasicWindow
    {
        private Panel _menuBar;

        private SplitContainer _split;

        private BasicListBox _modList;

        private BasicListBox _pkgList;

        private IPackageManageService pkgManager;

        private IFileLoadService fileLoadService;

        private IPackageImportService packageImportService;

        private IPackageLoadService<DataModel.Default.Package> packedPackageLoadServiceService;

        private IPackageLoadService<DataModel.Default.Package> sourcePackageLoadServiceService;

        private IPackageBrowserService packageBrowserService;

        private IDownloadManagerService downloadManagerService;

        private List<IPackage> onlinePackages = new List<IPackage>();

        public MainWindow()
        {
            pkgManager = Localizer.Kernel.Get<IPackageManageService>();
            sourcePackageLoadServiceService = Localizer.Kernel.Get<SourcePackageLoad<DataModel.Default.Package>>();
            packedPackageLoadServiceService = Localizer.Kernel.Get<PackedPackageLoad<DataModel.Default.Package>>();
            packageImportService = Localizer.Kernel.Get<IPackageImportService>();
            fileLoadService = Localizer.Kernel.Get<IFileLoadService>();
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
            var button = new Button
            {
                Text = _("Reload"),
                Tooltip = _("ReloadDesc"),
                Dock = DockStyle.Left
            };
            button.MouseClick += (sender, args) =>
            {
                if (args.Button == 0)
                {
                    LoadPackages();
                    RefreshPkgList(_modList.SelectedItem.Text);
                }
            };
            _menuBar.Content.Controls.Add(button);
            var button2 = new Button
            {
                Text = _("RefreshOnline"),
                Tooltip = _("RefreshOnlineDesc"),
                Dock = DockStyle.Left
            };
            button2.MouseClick += (sender, args) =>
            {
                if (args.Button == 0)
                {
                    RefreshOnlinePackages(sender);
                }
            };
            _menuBar.Content.Controls.Add(button2);
            RefreshOnlinePackages(button2);
            var button3 = new Button
            {
                Text = _("OpenFolder"),
                Tooltip = _("OpenFolderDesc"),
                Dock = DockStyle.Left
            };
            button3.MouseClick += (sender, args) =>
            {
                if (args.Button == 0)
                {
                    Process.Start($"file://{Path.Combine(Environment.CurrentDirectory, "Localizer")}");
                }
            };
            _menuBar.Content.Controls.Add(button3);
            _split = new SplitContainer
            {
                Margin = new Margin(0, 10, 0, 0),
                Dock = DockStyle.Fill
            };
            _split.SplitButton.Style = "button";
            _split.SplitFrame1.AutoSize = AutoSize.Horizontal;
            _split.SplitFrame2.AutoSize = AutoSize.Horizontal;
            Controls.Add(_split);
            _modList = new BasicListBox();
            _split.SplitFrame1.Controls.Add(_modList);
            _modList.SelectedItemChanged += (sender, value) => RefreshPkgList(value.Text);
            RefreshModList();
            _pkgList = new BasicListBox
            {
                MaxSelected = 0,
                AllowDrop = true
            };
            _split.SplitFrame2.Controls.Add(_pkgList);
            LoadPackages();
        }

        private void RefreshOnlinePackages(Control sender)
        {
            new Thread(() =>
            {
                sender.Enabled = false;
                onlinePackages?.Clear();
                Utils.SafeWrap(() => onlinePackages = packageBrowserService.GetList().ToList());
                RefreshPkgList(_modList.SelectedItem.Text);
                sender.Enabled = true;
            }).Start();
        }

        private void LoadPackages()
        {
            try
            {
                pkgManager.PackageGroups = new List<IPackageGroup>();
                foreach (var dir in new DirectoryInfo(Localizer.SourcePackageDirPath).GetDirectories())
                {
                    Utils.SafeWrap(() =>
                    {
                        var package2 = sourcePackageLoadServiceService.Load(dir.FullName, fileLoadService);
                        if (package2 != null)
                        {
                            pkgManager.AddPackage(package2);
                        }
                    });
                }
                foreach (var file in new DirectoryInfo(Localizer.DownloadPackageDirPath).GetFiles())
                {
                    Utils.SafeWrap(() =>
                    {
                        var package = packedPackageLoadServiceService.Load(file.FullName, fileLoadService);
                        if (package != null)
                        {
                            pkgManager.AddPackage(package);
                        }
                    });
                }
                pkgManager.LoadState();
            }
            catch (Exception o)
            {
                Utils.LogError(o);
            }
        }

        private void RefreshPkgList(string modName)
        {
            string TrimEndingZero(string input)
            {
                while (input.EndsWith(".0"))
                {
                    input = input.Substring(0, input.Length - 2);
                }
                return input;
            }
            try
            {
                _split.SplitFrame2.Controls.Clear();
                _pkgList.Items.Clear();
                _split.SplitFrame2.Controls.Add(_pkgList);
                var packageGroup = pkgManager.PackageGroups.FirstOrDefault(g => g.Mod.Name == modName);
                var onlineGroup = onlinePackages.Where(g => g.ModName == modName);
                if (packageGroup == null && onlineGroup == null)
                {
                    _split.SplitFrame2.Controls.Add(new Label
                    {
                        Text = _("NoPackageFound"),
                        TextWrap = true,
                        AutoSize = AutoSize.Vertical,
                        Dock = DockStyle.Fill,
                        AllowFocus = false
                    });
                }
                else
                {
                    var locals = new HashSet<string>();
                    if (packageGroup != null)
                    {
                        foreach (var p in packageGroup.Packages)
                        {
                            // TODO: Use better key for HashSet
                            locals.Add(TrimEndingZero(p.ModName + p.Author + p.Version));
                            var item = new ListBoxItem
                            {
                                Text = GetPkgLabelText(p),
                                Tooltip = (Gui.Renderer as UIRenderer).WordWrap(p.Description, 400),
                                TextWrap = true,
                                Dock = DockStyle.Top,
                                AutoSize = AutoSize.Vertical
                            };
                            item.MouseClick += (sender, args) =>
                            {
                                if (args.Button == 0)
                                {
                                    p.Enabled = !p.Enabled;
                                    item.Text = GetPkgLabelText(p);
                                    pkgManager.SaveState();
                                }
                            };
                            _pkgList.Items.Add(item);
                        }
                    }
                    if (onlineGroup != null)
                    {
                        foreach (var p in onlineGroup)
                        {
                            if (locals.Contains(TrimEndingZero(p.ModName + p.Author + p.Version)))
                            {
                                continue;
                            }
                            var item = new ListBoxItem
                            {
                                Text = _("PackageDisplay", _("PackageOnline"), p.Name, p.Version, p.Author),
                                Tooltip = (Gui.Renderer as UIRenderer).WordWrap(p.Description, 400),
                                TextWrap = true,
                                Dock = DockStyle.Top,
                                AutoSize = AutoSize.Vertical
                            };
                            item.MouseClick += (sender, args) => DownloadPackage(modName, args, p);
                            _pkgList.Items.Add(item);
                        }
                    }
                }
            }
            catch (Exception o)
            {
                Utils.LogError(o);
            }
        }

        private void DownloadPackage(string modName, MouseEventArgs args, IPackage p)
        {
            if (args.Button == 0)
            {
                new Thread(() =>
                {
                    Utils.SafeWrap(() =>
                    {
                        Utils.LogDebug($"Requesting {p.Name} download");
                        var url = packageBrowserService.GetDownloadLinkOf(p);
                        var path = Utils.EscapePath(Path.Combine(Localizer.DownloadPackageDirPath, $"{p.Name}_{p.Author}.locpack"));
                        downloadManagerService.Download(url, path);
                        Utils.LogDebug($"{p.Name} is downloaded");
                        RefreshPkgList(modName);
                    });
                }).Start();
            }
        }

        private string GetPkgLabelText(IPackage p)
        {
            return _("PackageDisplay", _(p.Enabled ? "PackageEnabled" : "PackageDisabled"), p.Name, p.Version, p.Author);
        }

        private void RefreshModList()
        {
            _modList.Items.Clear();
            foreach (var loadedMod in Utils.GetLoadedMods())
            {
                if (loadedMod != null)
                {
                    _modList.Items.Add(new ListBoxItem
                    {
                        Text = loadedMod.Name,
                        Tooltip = loadedMod.DisplayName
                    });
                }
            }
        }
    }
}