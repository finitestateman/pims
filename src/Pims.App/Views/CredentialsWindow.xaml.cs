using System.Windows;
using Pims.App.ViewModels;

namespace Pims.App.Views
{
    public partial class CredentialsWindow : Window
    {
        private readonly CredentialsViewModel _viewModel;

        public CredentialsWindow(CredentialsViewModel viewModel)
        {
            InitializeComponent();
            _viewModel = viewModel;
            DataContext = _viewModel;
            Loaded += OnLoaded;
        }

        private async void OnLoaded(object sender, RoutedEventArgs e)
        {
            Loaded -= OnLoaded;
            await _viewModel.InitializeAsync();
            PrimaryPasswordBox.Password = _viewModel.PrimaryPassword ?? string.Empty;
            SecondaryPasswordBox.Password = _viewModel.SecondaryPassword ?? string.Empty;
        }

        private void OnPrimaryPasswordChanged(object sender, RoutedEventArgs e)
        {
            _viewModel.PrimaryPassword = PrimaryPasswordBox.Password;
        }

        private void OnSecondaryPasswordChanged(object sender, RoutedEventArgs e)
        {
            _viewModel.SecondaryPassword = SecondaryPasswordBox.Password;
        }
    }
}
