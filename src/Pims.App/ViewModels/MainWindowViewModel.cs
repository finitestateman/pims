using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using Pims.App.Commands;
using Pims.App.Services;

namespace Pims.App.ViewModels
{
    public sealed class MainWindowViewModel : ViewModelBase
    {
        private readonly IMenuNavigationService _navigationService;

        public MainWindowViewModel(IMenuNavigationService navigationService, IMenuCatalog catalog)
        {
            _navigationService = navigationService;
            MenuDefinitions = new ObservableCollection<MenuDefinition>(catalog.GetMenus());
            OpenMenuCommand = new RelayCommand(OnOpenMenu);
        }

        public ObservableCollection<MenuDefinition> MenuDefinitions { get; }

        public ICommand OpenMenuCommand { get; }

        private void OnOpenMenu(object? parameter)
        {
            if (parameter is MenuDefinition menu)
            {
                _navigationService.OpenMenu(menu);
            }
        }
    }
}
