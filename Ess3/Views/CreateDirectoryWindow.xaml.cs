using System;
using System.Text;
using System.Windows;

namespace Ess3.Views
{
    public partial class CreateDirectoryWindow : Window
    {
        public string DirectoryName
        {
            get
            {
                StringBuilder sb = new StringBuilder();

                if (!String.IsNullOrEmpty(tblk_Prefix.Text))
                {
                    sb.Append(tblk_Prefix.Text);
                }

                string directory = tbx_DirectoryName.Text;

                sb.Append(directory);

                if (!directory.EndsWith(@"/", StringComparison.OrdinalIgnoreCase))
                {
                    sb.Append("/");
                }

                return sb.ToString();
            }
        }

        public CreateDirectoryWindow(string prefix)
        {
            InitializeComponent();

            tblk_Prefix.Text = prefix;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
            => DialogResult = tbx_DirectoryName.Text.Length > 0;
    }
}
