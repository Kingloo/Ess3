using System;
using System.Windows;
using System.Windows.Threading;
using Ess3.Gui.Common;
using Ess3.Gui.Views;

namespace Ess3.Gui
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();
        }

        private void Application_Startup(object sender, StartupEventArgs e)
        {
            MainWindow = new MainWindow();
            MainWindow.Show();
        }

        private void Application_DispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            if (e.Exception is Exception ex)
            {
                LogStatic.Exception(ex);
            }
            else
            {
                LogStatic.Message("an empty DispatcherUnhandledException was thrown");
            }
        }
    }
}
