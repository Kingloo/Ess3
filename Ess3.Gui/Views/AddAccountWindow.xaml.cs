using System.Windows;
using Ess3.Gui.Interfaces;
using Ess3.Gui.ViewModels;

namespace Ess3.Gui.Views
{
    public partial class AddAccountWindow : Window
    {
        private readonly IAddAccountWindowViewModel viewModel;

        public AddAccountWindow()
            : this(new AddAccountWindowViewModel())
        { }

        public AddAccountWindow(IAddAccountWindowViewModel viewModel)
        {
            InitializeComponent();

            this.viewModel = viewModel;

            DataContext = this.viewModel;
        }

        private async void validateButton_Click(object sender, RoutedEventArgs e)
        {
            string awsAccessKey = awsAccessKeyTextBox.Text;
            string awsSecretKey = awsSecretKeyTextBox.Text;

            validateButton.IsEnabled = false;

            bool isValid = await viewModel.ValidateAsync(awsAccessKey, awsSecretKey);

            validateButton.IsEnabled = true;

            if (isValid)
            {
                viewModel.AWSAccessKey = awsAccessKey;
                viewModel.AWSSecretKey = awsSecretKey;

                addButton.IsEnabled = true;
            }
        }

        private void addButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }
    }
}
