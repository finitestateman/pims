using System;
using System.Drawing;
using System.Windows;
using System.Windows.Forms;
using Pims.Core.Configuration;

namespace Pims.App.Services
{
    public sealed class TrayIconService : IDisposable
    {
        private readonly NotifyIcon _notifyIcon;
        private readonly IMenuCatalog _menuCatalog;
        private readonly IMenuNavigationService _navigationService;
        private bool _disposed;

        public TrayIconService(IMenuCatalog menuCatalog, IMenuNavigationService navigationService, AppConfiguration configuration)
        {
            _menuCatalog = menuCatalog;
            _navigationService = navigationService;
            _notifyIcon = new NotifyIcon
            {
                Icon = SystemIcons.Application,
                Visible = true,
                Text = string.IsNullOrWhiteSpace(configuration.TrayTooltip) ? "Pims Utility" : configuration.TrayTooltip
            };

            BuildContextMenu();
            _notifyIcon.DoubleClick += (_, _) => ShowMainWindow();
        }

        private void BuildContextMenu()
        {
            var menu = new ContextMenuStrip();
            var showItem = new ToolStripMenuItem("Show Main Window", null, (_, _) => ShowMainWindow());
            menu.Items.Add(showItem);
            menu.Items.Add(new ToolStripSeparator());

            foreach (var entry in _menuCatalog.GetMenus())
            {
                var item = new ToolStripMenuItem(entry.DisplayName, null, (_, _) => _navigationService.OpenMenu(entry));
                menu.Items.Add(item);
            }

            menu.Items.Add(new ToolStripSeparator());
            var exitItem = new ToolStripMenuItem("Exit", null, (_, _) => Application.Current.Shutdown());
            menu.Items.Add(exitItem);

            _notifyIcon.ContextMenuStrip = menu;
        }

        private static void ShowMainWindow()
        {
            if (Application.Current.MainWindow == null)
            {
                return;
            }

            Application.Current.Dispatcher.Invoke(() =>
            {
                Application.Current.MainWindow.Show();
                Application.Current.MainWindow.WindowState = WindowState.Normal;
                Application.Current.MainWindow.Activate();
            });
        }

        public void Dispose()
        {
            if (_disposed)
            {
                return;
            }

            _notifyIcon.Visible = false;
            _notifyIcon.Dispose();
            _disposed = true;
        }
    }
}
