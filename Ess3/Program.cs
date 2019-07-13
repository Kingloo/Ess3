using System;
using System.Globalization;
using System.IO;
using System.Windows;
using Ess3.Common;
using Ess3.Model;
using Ess3.Views;

namespace Ess3
{
    public static class Program
    {
        private static readonly string directory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
        private static readonly string filename = "Ess3Settings.json";

        [STAThread]
        public static int Main()
        {
            FileInfo settingsFile = new FileInfo(Path.Combine(directory, filename));

            Ess3Settings ess3Settings = LoadSettings(settingsFile);

            if (ess3Settings is null)
            {
                string text = "Fatal Error!";
                string caption = "No settings file found";

                MessageBox.Show(text, caption, MessageBoxButton.OK, MessageBoxImage.Error);

                return -1;
            }

            App app = new App(ess3Settings);

            int exitCode = app.Run();

            if (exitCode != 0)
            {
                Log.Message(string.Format(CultureInfo.CurrentCulture, "App exited with code: {0}", exitCode));
            }

            return exitCode;
        }

        private static Ess3Settings LoadSettings(FileInfo file)
        {
            var fs = new FileStream(
                file.FullName,
                FileMode.Open,
                FileAccess.Read,
                FileShare.None,
                4096,
                FileOptions.SequentialScan);

            try
            {
                using (StreamReader sr = new StreamReader(fs))
                {
                    fs = null;

                    string contents = sr.ReadToEnd();

                    return Ess3Settings.Parse(contents);
                }
            }
            catch (FileNotFoundException)
            {
                return null;
            }
            finally
            {
                fs?.Dispose();
            }
        }
    }
}
