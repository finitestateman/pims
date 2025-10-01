using System.Windows;
using Pims.App.ViewModels;

namespace Pims.App.Views
{
    public partial class FtpWindow : Window
    {
        private readonly FtpViewModel _viewModel;

        public FtpWindow(FtpViewModel viewModel)
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
        }
    }
}
