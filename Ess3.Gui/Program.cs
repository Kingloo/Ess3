﻿using System;
using System.Globalization;
using Ess3.Gui.Common;

namespace Ess3.Gui
{
    public static class Program
    {
        [STAThread]
        public static int Main()
        {
            App app = new App();

            int exitCode = app.Run();

            if (exitCode != 0)
            {
                string message = string.Format(CultureInfo.CurrentCulture, "exited with code {0}", exitCode);

                LogStatic.Message(message);
            }

            return exitCode;
        }
    }
}
