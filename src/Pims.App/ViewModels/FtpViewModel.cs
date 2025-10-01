using System;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Input;
using Microsoft.Win32;
using Pims.App.Commands;
using Pims.Core.Models;
using Pims.Core.Services;

namespace Pims.App.ViewModels
{
    public sealed class FtpViewModel : ViewModelBase
    {
        private readonly FtpTransferService _ftpTransferService;
        private readonly PreferenceStore _preferenceStore;
        private readonly AuthenticationService _authenticationService;
        private string? _localFilePath;
        private string? _ftpDirectory;
        private string? _statusMessage;
        private bool _isBusy;

        public FtpViewModel(FtpTransferService ftpTransferService, PreferenceStore preferenceStore, AuthenticationService authenticationService)
        {
            _ftpTransferService = ftpTransferService;
            _preferenceStore = preferenceStore;
            _authenticationService = authenticationService;

            BrowseCommand = new RelayCommand(_ => BrowseForJar());
            UploadCommand = new AsyncRelayCommand(_ => UploadAsync(), _ => !IsBusy && !string.IsNullOrWhiteSpace(LocalFilePath));
        }

        public ICommand BrowseCommand { get; }

        public ICommand UploadCommand { get; }

        public string? LocalFilePath
        {
            get => _localFilePath;
            set
            {
                SetProperty(ref _localFilePath, value);
                (UploadCommand as AsyncRelayCommand)?.RaiseCanExecuteChanged();
            }
        }

        public string? FtpDirectory
        {
            get => _ftpDirectory;
            set => SetProperty(ref _ftpDirectory, value);
        }

        public string? StatusMessage
        {
            get => _statusMessage;
            private set => SetProperty(ref _statusMessage, value);
        }

        public bool IsBusy
        {
            get => _isBusy;
            private set
            {
                SetProperty(ref _isBusy, value);
                (UploadCommand as AsyncRelayCommand)?.RaiseCanExecuteChanged();
            }
        }

        public async Task InitializeAsync()
        {
            var preferences = await _preferenceStore.LoadAsync();
            LocalFilePath = preferences.LastLocalJarPath;
            FtpDirectory = preferences.LastFtpDirectory;
        }

        private void BrowseForJar()
        {
            var dialog = new OpenFileDialog
            {
                Filter = "JAR files (*.jar)|*.jar|All files (*.*)|*.*",
                CheckFileExists = true,
                InitialDirectory = !string.IsNullOrWhiteSpace(LocalFilePath) ? Path.GetDirectoryName(LocalFilePath) : Environment.CurrentDirectory
            };

            if (dialog.ShowDialog() == true)
            {
                LocalFilePath = dialog.FileName;
                StatusMessage = $"Selected {Path.GetFileName(dialog.FileName)}";
            }
        }

        private async Task UploadAsync()
        {
            if (string.IsNullOrWhiteSpace(LocalFilePath))
            {
                StatusMessage = "Select a JAR file first.";
                return;
            }

            try
            {
                IsBusy = true;
                StatusMessage = "Uploading...";

                var credentials = await _authenticationService.GetOrCreateCredentialsAsync(() => throw new InvalidOperationException("Credentials are required before uploading."));

                await _ftpTransferService.UploadJarAsync(LocalFilePath, FtpDirectory ?? string.Empty, credentials);

                var preferences = await _preferenceStore.LoadAsync();
                preferences.LastLocalJarPath = LocalFilePath;
                preferences.LastFtpDirectory = FtpDirectory;
                await _preferenceStore.SaveAsync(preferences);

                StatusMessage = "Upload complete.";
            }
            catch (Exception ex)
            {
                StatusMessage = $"Upload failed: {ex.Message}";
            }
            finally
            {
                IsBusy = false;
            }
        }
    }
}
