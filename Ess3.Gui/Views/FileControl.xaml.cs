using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Ess3.Gui.Interfaces;
using Ess3.Gui.ViewModels;

namespace Ess3.Gui.Views
{
    public partial class FileControl : UserControl
    {
        private readonly IFileControlViewModel viewModel;

        public FileControl()
            : this(new FileControlViewModel())
        { }

        public FileControl(IFileControlViewModel viewModel)
        {
            InitializeComponent();

            this.viewModel = viewModel;

            DataContext = this.viewModel;
        }
    }
}
