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

            if (!settingsFile.Exists)
            {
                MessageBox.Show("Error", "Settings file not found", MessageBoxButton.OK, MessageBoxImage.Error);

                return -1;
            }

            Ess3Settings ess3Settings = LoadSettings(settingsFile);

            if (ess3Settings is null)
            {
                MessageBox.Show("Error", "Settings file didn't parse", MessageBoxButton.OK, MessageBoxImage.Error);

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

                    if (Ess3Settings.TryParse(contents, out Ess3Settings settings))
                    {
                        return settings;
                    }
                    else
                    {
                        Log.Message("failed to parse settings!");

                        return null;
                    }
                }
            }
            catch (FileNotFoundException)
            {
                Log.Message("settings file not found!");

                return null;
            }
            finally
            {
                fs?.Dispose();
            }
        }
    }
}
