using System;
using System.Diagnostics;
using System.IO;

namespace MSR.CVE.BackMaker
{
    public class NamedPipeServer : NamedPipeBase, IDisposable
    {
        public Process childProcess;

        public NamedPipeServer(string childProcessArguments, string pipeName)
        {
            StartChildProcess(childProcessArguments);
            Initialize(pipeName);
        }

        private void Initialize(string pipeName)
        {
            pipeHandle =
                CreateNamedPipe("\\\\.\\pipe\\" + pipeName, 3u, 6u, 255u, 4096u, 4096u, 0u, new IntPtr(0));
            if (ConnectNamedPipe(pipeHandle, new IntPtr(0)) == 0)
            {
                throw new IOException(string.Format("Unable to open pipe: {0}.", GetLastError()));
            }
        }

        public void Dispose()
        {
            pipeHandle.Close();
            try
            {
                childProcess.Kill();
            }
            catch (InvalidOperationException)
            {
            }
        }

        private void StartChildProcess(string arguments)
        {
            ProcessStartInfo processStartInfo = new ProcessStartInfo();
            processStartInfo.FileName = Path.Combine(AppDomain.CurrentDomain.SetupInformation.ApplicationBase,
                AppDomain.CurrentDomain.SetupInformation.ApplicationName);
            if (processStartInfo.FileName.EndsWith(".vshost.exe"))
            {
                processStartInfo.FileName = processStartInfo.FileName.Replace(".vshost.exe", ".exe");
            }

            processStartInfo.UseShellExecute = true;
            processStartInfo.Arguments = arguments;
            childProcess = Process.Start(processStartInfo);
        }
    }
}
