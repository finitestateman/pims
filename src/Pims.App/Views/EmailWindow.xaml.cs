using System.Threading.Tasks;
using System.Windows;
using Pims.App.ViewModels;

namespace Pims.App.Views
{
    public partial class EmailWindow : Window
    {
        private readonly EmailViewModel _viewModel;

        public EmailWindow(EmailViewModel viewModel)
        {
            InitializeComponent();
            _viewModel = viewModel;
            DataContext = _viewModel;
            Loaded += OnLoaded;
        }

        private async void OnLoaded(object sender, RoutedEventArgs e)
        {
            Loaded -= OnLoaded;
            await _viewModel.RefreshAsync().ConfigureAwait(false);
        }
    }
}
