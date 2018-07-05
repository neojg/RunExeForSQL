using Microsoft.SqlServer.Server;
using System;
using System.Diagnostics;
using System.IO;

namespace Arhat.IDExec
{
    public static class CLR
    {
        private static readonly int rcError = -200;
        private static readonly int rcTimeOut = -201;

        private static int Exec(string path, string args, int timeOut)
        {
            ProcessStartInfo psi;
            psi = new ProcessStartInfo();
            psi.Arguments = args;
            psi.FileName = path;

            var p = Process.Start(psi);
            if (p.WaitForExit(timeOut))
                return p.ExitCode;

            // Try to kill process
            try { p.Kill(); } catch { }

            throw new TimeoutException();
        }

        [SqlFunction()]
        public static int Run(string path, string args, int timeOut)
        {
            try
            {
                return Exec(path, args, timeOut);
            }
            catch (TimeoutException)
            {
                return rcTimeOut;
            }
            catch
            {
                return rcError;
            }
        }

        [SqlFunction()]
        public static string Runs(string path, string args, int timeOut)
        {
            try
            {
                return Exec(path, args, timeOut).ToString();
            }
            catch (Exception ex)
            {
                return ex.ToString();
            }
        }
    }
}
