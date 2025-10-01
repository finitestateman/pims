using System;
using System.Threading.Tasks;
using System.Windows.Input;
using Pims.App.Commands;
using Pims.Core.Models;
using Pims.Core.Services;

namespace Pims.App.ViewModels
{
    public sealed class CredentialsViewModel : ViewModelBase
    {
        private readonly CredentialStore _credentialStore;
        private readonly AuthenticationService _authenticationService;
        private string? _userId;
        private string? _primaryPassword;
        private string? _secondaryPassword;
        private string? _statusMessage;

        public CredentialsViewModel(CredentialStore credentialStore, AuthenticationService authenticationService)
        {
            _credentialStore = credentialStore;
            _authenticationService = authenticationService;
            SaveCommand = new AsyncRelayCommand(_ => SaveAsync());
        }

        public ICommand SaveCommand { get; }

        public string? UserId
        {
            get => _userId;
            set => SetProperty(ref _userId, value);
        }

        public string? PrimaryPassword
        {
            get => _primaryPassword;
            set => SetProperty(ref _primaryPassword, value);
        }

        public string? SecondaryPassword
        {
            get => _secondaryPassword;
            set => SetProperty(ref _secondaryPassword, value);
        }

        public string? StatusMessage
        {
            get => _statusMessage;
            private set => SetProperty(ref _statusMessage, value);
        }

        public async Task InitializeAsync()
        {
            var credentials = await _credentialStore.LoadAsync();
            if (credentials != null)
            {
                UserId = credentials.UserId;
                PrimaryPassword = credentials.PrimaryPassword;
                SecondaryPassword = credentials.SecondaryPassword;
            }
        }

        private async Task SaveAsync()
        {
            try
            {
                if (string.IsNullOrWhiteSpace(UserId) || string.IsNullOrWhiteSpace(PrimaryPassword) || string.IsNullOrWhiteSpace(SecondaryPassword))
                {
                    StatusMessage = "All fields are required.";
                    return;
                }

                var credentials = new UserCredentials(UserId, PrimaryPassword, SecondaryPassword);
                await _credentialStore.SaveAsync(credentials);
                _authenticationService.UpdateCachedCredentials(credentials);
                StatusMessage = "Credentials saved.";
            }
            catch (Exception ex)
            {
                StatusMessage = $"Save failed: {ex.Message}";
            }
        }
    }
}
