using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using Microsoft.Extensions.DependencyInjection;
using Pims.App.Services;
using Pims.App.Views;
using Pims.Core.Configuration;
using Pims.Core.Services;

namespace Pims.App
{
    public partial class App : Application
    {
        private ServiceProvider? _serviceProvider;
        private TrayIconService? _trayIconService;
        private GlobalHotkeyService? _hotkeyService;
        private HttpClientFactory? _httpClientFactory;
        private System.Net.Http.HttpClient? _httpClient;

        protected override async void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            var configurationProvider = new AppConfigurationProvider();
            var configuration = configurationProvider.Load();
            _httpClientFactory = new HttpClientFactory();
            var (client, cookieContainer) = _httpClientFactory.Create(configuration);
            _httpClient = client;

            var services = new ServiceCollection();
            services.AddSingleton(configuration);
            services.AddSingleton(configurationProvider);
            services.AddSingleton(cookieContainer);
            services.AddSingleton(client);
            services.AddSingleton<CredentialStore>();
            services.AddSingleton<PreferenceStore>();
            services.AddSingleton<AuthenticationService>();
            services.AddSingleton<SessionHttpClient>();
            services.AddSingleton<EmailService>();
            services.AddSingleton<FtpTransferService>();
            services.AddSingleton<ClipboardService>();
            services.AddSingleton<ToastNotificationService>();
            services.AddSingleton<OtpWorkflowService>();
            services.AddSingleton<IMenuCatalog, MenuCatalog>();
            services.AddSingleton<IMenuNavigationService, MenuNavigationService>();
            services.AddSingleton<TrayIconService>();
            services.AddSingleton<GlobalHotkeyService>();

            services.AddSingleton<ViewModels.MainWindowViewModel>();
            services.AddTransient<ViewModels.EmailViewModel>();
            services.AddTransient<ViewModels.OtpViewModel>();
            services.AddTransient<ViewModels.FtpViewModel>();
            services.AddTransient<ViewModels.CredentialsViewModel>();

            services.AddTransient<MainWindow>();
            services.AddTransient<EmailWindow>();
            services.AddTransient<OtpWindow>();
            services.AddTransient<FtpWindow>();
            services.AddTransient<CredentialsWindow>();

            _serviceProvider = services.BuildServiceProvider();

            await EnsureCredentialsAsync();
            await EnsureLoginAsync();

            _trayIconService = _serviceProvider.GetRequiredService<TrayIconService>();
            _hotkeyService = _serviceProvider.GetRequiredService<GlobalHotkeyService>();
            await RegisterOtpHotkeyAsync();

            var mainWindow = _serviceProvider.GetRequiredService<MainWindow>();
            MainWindow = mainWindow;
            mainWindow.Show();
        }

        protected override void OnExit(ExitEventArgs e)
        {
            _trayIconService?.Dispose();
            _hotkeyService?.Dispose();
            _httpClient?.Dispose();
            _serviceProvider?.Dispose();
            base.OnExit(e);
        }

        private async Task EnsureCredentialsAsync()
        {
            var credentialStore = _serviceProvider!.GetRequiredService<CredentialStore>();
            var credentials = await credentialStore.LoadAsync();
            if (credentials != null)
            {
                return;
            }

            var dialog = _serviceProvider.GetRequiredService<CredentialsWindow>();
            dialog.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            dialog.ShowDialog();
        }

        private async Task EnsureLoginAsync()
        {
            var authenticationService = _serviceProvider!.GetRequiredService<AuthenticationService>();
            try
            {
                await authenticationService.EnsureAuthenticatedAsync();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Login failed: {ex.Message}", "Pims", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async Task RegisterOtpHotkeyAsync()
        {
            var preferenceStore = _serviceProvider!.GetRequiredService<PreferenceStore>();
            var preferences = await preferenceStore.LoadAsync();
            if (!ShortcutParser.TryParse(preferences.OtpShortcutGesture, out var modifiers, out var key) || key == Key.None)
            {
                return;
            }

            var registered = _hotkeyService!.RegisterHotkey(modifiers, key, () =>
            {
                _ = Dispatcher.InvokeAsync(async () =>
                {
                    var workflow = _serviceProvider.GetRequiredService<OtpWorkflowService>();
                    await workflow.CaptureOtpAsync();
                });
            });

            if (!registered)
            {
                MessageBox.Show($"Failed to register global shortcut {preferences.OtpShortcutGesture}.", "Pims", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }
    }
}
