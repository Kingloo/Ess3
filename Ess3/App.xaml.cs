using System;
using System.Windows;
using System.Windows.Threading;
using Ess3.Model;
using Ess3.ViewModels;
using Ess3.Views;

namespace Ess3
{
    public partial class App : Application
    {
        public App(Ess3Settings settings)
        {
            if (settings == null) { throw new ArgumentNullException(nameof(settings)); }

            InitializeComponent();

            MainWindow = new MainWindow(new MainWindowViewModel(settings));

            MainWindow.Show();
        }

        private void Application_DispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            Log.LogException(e.Exception);
        }
    }
}
