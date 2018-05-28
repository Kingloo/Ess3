using System;
using System.Windows.Controls;

namespace Ess3.Views
{
    public partial class ActivityProgress : UserControl
    {
        public ActivityProgress()
        {
            InitializeComponent();
        }

        public void SetTitle(string title)
        {
            Title.Text = title;
        }

        public void SetPercent(string percent)
        {
            Percent.Content = percent;
        }

        public void SetProgress(double progress)
        {
            Progress.Value = progress;
        }

        public void Clear()
        {
            Title.Text = string.Empty;
            Percent.Content = string.Empty;
            Progress.Value = 0d;
        }
    }

    public class ActivityProgressChangedEventArgs : EventArgs
    {
        private readonly string _title = string.Empty;
        public string Title => _title;

        private readonly string _percent = string.Empty;
        public string Percent => _percent;

        private readonly double _progress = 0;
        public double Progress => _progress;

        public ActivityProgressChangedEventArgs(string title, string percent, double progress)
        {
            _title = title;
            _percent = percent;
            _progress = progress;
        }
    }
}
