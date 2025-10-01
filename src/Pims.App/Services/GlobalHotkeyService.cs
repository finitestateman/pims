using System;
using System.Collections.Concurrent;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Input;
using System.Windows.Interop;

namespace Pims.App.Services
{
    public sealed class GlobalHotkeyService : IDisposable
    {
        private readonly HwndSource _hwndSource;
        private readonly ConcurrentDictionary<int, Action> _callbacks = new();
        private int _currentId;
        private bool _disposed;

        public GlobalHotkeyService()
        {
            var parameters = new HwndSourceParameters("PimsHotkeySink")
            {
                Width = 0,
                Height = 0,
                PositionX = 0,
                PositionY = 0,
                WindowStyle = unchecked((int)0x80000000) // WS_POPUP
            };

            _hwndSource = new HwndSource(parameters);
            _hwndSource.AddHook(WndProc);
        }

        public bool RegisterHotkey(ModifierKeys modifiers, Key key, Action callback)
        {
            if (_disposed)
            {
                throw new ObjectDisposedException(nameof(GlobalHotkeyService));
            }

            var id = Interlocked.Increment(ref _currentId);
            var vk = KeyInterop.VirtualKeyFromKey(key);
            if (!NativeMethods.RegisterHotKey(_hwndSource.Handle, id, (uint)modifiers, (uint)vk))
            {
                return false;
            }

            _callbacks[id] = callback;
            return true;
        }

        private IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            const int WM_HOTKEY = 0x0312;
            if (msg == WM_HOTKEY)
            {
                var id = wParam.ToInt32();
                if (_callbacks.TryGetValue(id, out var callback))
                {
                    callback();
                    handled = true;
                }
            }

            return IntPtr.Zero;
        }

        public void Dispose()
        {
            if (_disposed)
            {
                return;
            }

            foreach (var registration in _callbacks.Keys)
            {
                NativeMethods.UnregisterHotKey(_hwndSource.Handle, registration);
            }

            _hwndSource.RemoveHook(WndProc);
            _hwndSource.Dispose();
            _disposed = true;
        }

        private static class NativeMethods
        {
            [DllImport("user32.dll")]
            public static extern bool RegisterHotKey(IntPtr hWnd, int id, uint fsModifiers, uint vk);

            [DllImport("user32.dll")]
            public static extern bool UnregisterHotKey(IntPtr hWnd, int id);
        }
    }
}
