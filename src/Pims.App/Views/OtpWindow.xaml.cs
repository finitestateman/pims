using System.Windows;
using Pims.App.ViewModels;

namespace Pims.App.Views
{
    public partial class OtpWindow : Window
    {
        public OtpWindow(OtpViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
        }
    }
}
