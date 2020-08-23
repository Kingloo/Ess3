using System.Diagnostics;
using System.Windows.Controls;
using Ess3.Gui.Interfaces;
using Ess3.Gui.ViewModels;
using Ess3.Library.Model;

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

        private void TabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count == 1)
            {
                if (e.AddedItems[0] is Ess3Bucket newSelectedBucket)
                {
                    viewModel.SelectedBucket = newSelectedBucket;
                }
            }
        }
    }
}
