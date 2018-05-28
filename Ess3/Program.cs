using System;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using Amazon;
using Ess3.Model;
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

            Ess3Settings ess3Settings = LoadSettingsAsync(settingsFile).GetAwaiter().GetResult();

            App app = new App(ess3Settings);

            int exitCode = app.Run();

            if (exitCode != 0)
            {
                Log.LogMessage($"App exited with code: {exitCode}");
            }

            return exitCode;
        }

        private static async Task<Ess3Settings> LoadSettingsAsync(FileInfo file)
        {
            FileStream fsAsync = new FileStream(
                file.FullName,
                FileMode.Open,
                FileAccess.Read,
                FileShare.None,
                4096,
                FileOptions.Asynchronous | FileOptions.SequentialScan);

            try
            {
                using (StreamReader sr = new StreamReader(fsAsync))
                {
                    fsAsync = null;

                    string fileContents = await sr.ReadToEndAsync().ConfigureAwait(false);

                    return Parse(fileContents);
                }
            }
            catch (FileNotFoundException)
            {
                string text = "Fatal Error!";
                string caption = "No settings file found";

                MessageBox.Show(text, caption, MessageBoxButton.OK, MessageBoxImage.Error);

                return null;
            }
            finally
            {
                fsAsync?.Dispose();
            }
        }

        private static Ess3Settings Parse(string fileContents)
        {
            try
            {
                JObject json = JObject.Parse(fileContents);

                return new Ess3Settings(
                    (string)json["AWSAccessKey"],
                    (string)json["AWSSecretKey"],
                    RegionEndpoint.GetBySystemName((string)json["EndpointSystemName"]));
            }
            catch (JsonReaderException) { }

            return null;
        }
    }
}
