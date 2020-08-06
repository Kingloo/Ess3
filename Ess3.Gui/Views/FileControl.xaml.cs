using System.Windows.Controls;
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
