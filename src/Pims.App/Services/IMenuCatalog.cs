using System.Collections.Generic;
using Pims.App.ViewModels;

namespace Pims.App.Services
{
    public interface IMenuCatalog
    {
        IReadOnlyList<MenuDefinition> GetMenus();
    }
}
