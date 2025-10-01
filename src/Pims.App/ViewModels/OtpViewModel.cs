using System;
using System.Threading.Tasks;
using System.Windows.Input;
using Pims.App.Commands;
using Pims.App.Services;

namespace Pims.App.ViewModels
{
    public sealed class OtpViewModel : ViewModelBase
    {
        private readonly OtpWorkflowService _otpWorkflowService;
        private string? _statusMessage;
        private bool _isBusy;

        public OtpViewModel(OtpWorkflowService otpWorkflowService)
        {
            _otpWorkflowService = otpWorkflowService;
            CaptureCommand = new AsyncRelayCommand(_ => CaptureAsync());
        }

        public ICommand CaptureCommand { get; }

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

        private async Task CaptureAsync()
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
