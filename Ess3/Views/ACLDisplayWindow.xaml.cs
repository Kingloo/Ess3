using System.Windows;
using System.Windows.Input;
using Amazon.S3.Model;

namespace Ess3.Views
{
    public partial class AclDisplayWindow : Window
    {
        public AclDisplayWindow(GetObjectAclResponse acls)
        {
            InitializeComponent();

            DataContext = acls.Grants;
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                Close();
            }
        }
    }
}
