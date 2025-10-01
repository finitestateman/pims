using System;
using System.Windows;

namespace Pims.App.Services
{
    public sealed class ClipboardService
    {
        public void SetText(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
            {
                return;
            }

            Application.Current.Dispatcher.Invoke(() => Clipboard.SetText(text));
        }
    }
}
