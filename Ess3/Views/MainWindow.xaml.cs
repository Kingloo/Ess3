using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using Ess3.Model;
using Ess3.ViewModels;

namespace Ess3.Views
{
    public partial class MainWindow : Window
    {
        private readonly MainWindowViewModel viewModel = null;

        public MainWindow(MainWindowViewModel viewModel)
        {
            InitializeComponent();

            this.viewModel = viewModel ?? throw new ArgumentNullException(nameof(viewModel));
            DataContext = viewModel;

            viewModel.ActivityProgressChanged += MainWindowViewModel_ActivityProgressChanged;

            tabControl.SelectionChanged += TabControl_SelectionChanged;
        }

        private async void MainWindow_Loaded(object sender, RoutedEventArgs e)
            => await viewModel.ListAllBucketsAsync().ConfigureAwait(false);

        private async void TabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count > 0)
            {
                if (e.AddedItems[0] is Ess3Bucket bucket)
                {
                    await viewModel.RefreshBucketAsync(bucket).ConfigureAwait(false);
                }
            }
        }

        private void MainWindowViewModel_ActivityProgressChanged(object sender, ActivityProgressChangedEventArgs e)
        {
            if (!String.IsNullOrEmpty(e.Title))
            {
                actPrg.SetTitle(e.Title);
            }

            actPrg.SetPercent(e.Percent);
            actPrg.SetProgress(e.Progress);
        }

        private void MainWindow_Closing(object sender, CancelEventArgs e)
        {
            string text = "Do you really want to quit?";
            string caption = "Wait...";

            var result = MessageBox.Show(text, caption, MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (result == MessageBoxResult.No)
            {
                e.Cancel = true;
            }
        }
    }
}
