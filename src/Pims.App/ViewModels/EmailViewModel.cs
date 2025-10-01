using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using Pims.App.Commands;
using Pims.App.Services;
using Pims.Core.Models;
using Pims.Core.Services;

namespace Pims.App.ViewModels
{
    public sealed class EmailViewModel : ViewModelBase
    {
        private readonly EmailService _emailService;
        private readonly OtpWorkflowService _otpWorkflowService;
        private string? _statusMessage;
        private bool _isBusy;

        public EmailViewModel(EmailService emailService, OtpWorkflowService otpWorkflowService)
        {
            _emailService = emailService;
            _otpWorkflowService = otpWorkflowService;
            Emails = new ObservableCollection<EmailMessage>();
            RefreshCommand = new AsyncRelayCommand(_ => RefreshAsync());
            CaptureOtpCommand = new AsyncRelayCommand(_ => CaptureOtpAsync());
        }

        public ObservableCollection<EmailMessage> Emails { get; }

        public ICommand RefreshCommand { get; }

        public ICommand CaptureOtpCommand { get; }

        public string? StatusMessage
        {
            get => _statusMessage;
            private set => SetProperty(ref _statusMessage, value);
        }

        public bool IsBusy
        {
            get => _isBusy;
            private set => SetProperty(ref _isBusy, value);
        }

        public async Task RefreshAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                IsBusy = true;
                StatusMessage = "Loading emails...";
                var messages = await _emailService.GetEmailsAsync(cancellationToken);

                Emails.Clear();
                foreach (var message in messages.OrderByDescending(m => m.ReceivedAt))
                {
                    Emails.Add(message);
                }

                StatusMessage = Emails.Count == 0 ? "No emails found." : $"Loaded {Emails.Count} emails.";
            }
            catch (Exception ex)
            {
                StatusMessage = $"Failed to load emails: {ex.Message}";
            }
            finally
            {
                IsBusy = false;
            }
        }

        private async Task CaptureOtpAsync()
        {
            try
            {
                IsBusy = true;
                StatusMessage = "Retrieving OTP...";
                var otp = await _otpWorkflowService.CaptureOtpAsync();
                StatusMessage = string.IsNullOrWhiteSpace(otp) ? "No OTP found." : $"OTP {otp} copied to clipboard.";
            }
            catch (Exception ex)
            {
                StatusMessage = $"OTP fetch failed: {ex.Message}";
            }
            finally
            {
                IsBusy = false;
            }
        }
    }
}
