using System;
using System.Windows;
using Microsoft.Extensions.DependencyInjection;
using Pims.App.ViewModels;
using Pims.App.Views;

namespace Pims.App.Services
{
    public sealed class MenuNavigationService : IMenuNavigationService
    {
        private readonly IServiceProvider _serviceProvider;

        public MenuNavigationService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public void OpenMenu(MenuDefinition menuDefinition)
        {
            var window = ResolveWindow(menuDefinition.Key);
            if (window == null)
            {
                MessageBox.Show($"No menu registered for key '{menuDefinition.Key}'", "Pims", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            window.Owner = Application.Current?.MainWindow;
            window.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            window.Show();
        }

        private Window? ResolveWindow(string key) => key switch
        {
            "email" => _serviceProvider.GetService<EmailWindow>(),
            "otp" => _serviceProvider.GetService<OtpWindow>(),
            "ftp" => _serviceProvider.GetService<FtpWindow>(),
            "credentials" => _serviceProvider.GetService<CredentialsWindow>(),
            _ => null
        };
    }
}
