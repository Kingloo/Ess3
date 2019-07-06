using System;
using System.Globalization;
using System.IO;
using System.Windows;
using Amazon;
using Ess3.Common;
using Ess3.Model;
using Ess3.Views;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Ess3
{
    public static class Program
    {
        [STAThread]
        public static int Main()
        {
            string directory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            string filename = "Ess3Settings.json";

            FileInfo settingsFile = new FileInfo(Path.Combine(directory, filename));

            Ess3Settings ess3Settings = LoadSettings(settingsFile);

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

                    return Parse(contents);
                }
            }
            catch (FileNotFoundException)
            {
                string text = "Fatal Error!";
                string caption = "No settings file found";

                MessageBox.Show(text, caption, MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                fs?.Dispose();
            }

            return null;
        }

        private static Ess3Settings Parse(string contents)
        {
            try
            {
                JObject json = JObject.Parse(contents);

                string accessKey = (string)json["AWSAccessKey"];
                string secretKey = (string)json["AWSSecretKey"];

                RegionEndpoint endpoint = RegionEndpoint.GetBySystemName((string)json["EndpointSystemName"]);

                return new Ess3Settings(accessKey, secretKey, endpoint);
            }
            catch (JsonReaderException)
            {
                return null;
            }
        }
    }
}
