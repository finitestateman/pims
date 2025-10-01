using System.Threading;
using System.Threading.Tasks;
using Pims.Core.Services;

namespace Pims.App.Services
{
    public sealed class OtpWorkflowService
    {
        private readonly EmailService _emailService;
        private readonly ClipboardService _clipboardService;
        private readonly ToastNotificationService _toastNotificationService;

        public OtpWorkflowService(EmailService emailService, ClipboardService clipboardService, ToastNotificationService toastNotificationService)
        {
            _emailService = emailService;
            _clipboardService = clipboardService;
            _toastNotificationService = toastNotificationService;
        }

        public async Task<string?> CaptureOtpAsync(CancellationToken cancellationToken = default)
        {
            var otp = await _emailService.GetLatestOtpAsync(cancellationToken).ConfigureAwait(false);
            if (string.IsNullOrWhiteSpace(otp))
            {
                return null;
            }

            _clipboardService.SetText(otp);
            _toastNotificationService.Show("OTP Copied", $"One-time password {otp} is ready to paste.");
            return otp;
        }
    }
}
