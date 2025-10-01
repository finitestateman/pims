using System;
using System.Linq;
using System.Windows.Input;

namespace Pims.App.Services
{
    public static class ShortcutParser
    {
        public static bool TryParse(string? gesture, out ModifierKeys modifiers, out Key key)
        {
            modifiers = ModifierKeys.None;
            key = Key.None;
            if (string.IsNullOrWhiteSpace(gesture))
            {
                return false;
            }

            var tokens = gesture.Split('+', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
            if (tokens.Length == 0)
            {
                return false;
            }

            foreach (var token in tokens.Take(tokens.Length - 1))
            {
                modifiers |= token.ToLowerInvariant() switch
                {
                    "ctrl" or "control" => ModifierKeys.Control,
                    "alt" => ModifierKeys.Alt,
                    "shift" => ModifierKeys.Shift,
                    "win" or "windows" => ModifierKeys.Windows,
                    _ => ModifierKeys.None
                };
            }

            var keyToken = tokens.Last();
            if (!Enum.TryParse<Key>(keyToken, true, out key))
            {
                return false;
            }

            return true;
        }
    }
}
