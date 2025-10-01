using System.Collections.Generic;
using Pims.App.ViewModels;

namespace Pims.App.Services
{
    public sealed class MenuCatalog : IMenuCatalog
    {
        private static readonly IReadOnlyList<MenuDefinition> Menus = new List<MenuDefinition>
        {
            new("email", "Email Lookup"),
            new("otp", "OTP Clipboard Helper"),
            new("ftp", "Publish JAR to FTP"),
            new("credentials", "Credentials & Settings")
        };

        public IReadOnlyList<MenuDefinition> GetMenus() => Menus;
    }
}
