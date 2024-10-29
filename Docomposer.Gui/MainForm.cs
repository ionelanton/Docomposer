using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Windows.Forms;
using CliWrap;
using Docomposer.Utils;

namespace Docomposer.Gui
{
    public partial class MainForm : Form
    {
        private readonly CancellationTokenSource _cancellationTokenSource;
        
        public MainForm()
        {
            _cancellationTokenSource = new CancellationTokenSource();

            using (var cancellationTokenSource = _cancellationTokenSource)
            {
                var command = Cli.Wrap( Path.Combine(ThisApp.AppDirectory(), "Docomposer.exe"))
                    .WithWorkingDirectory(ThisApp.AppDirectory())
                    .ExecuteAsync(cancellationTokenSource.Token);

                if (command.Task.IsFaulted)
                {
                    KillDocReuseProcess();
                }
            }

            InitializeComponent();
        }

        private void DocReuseForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            try
            {
                _cancellationTokenSource?.Cancel();
            }
            catch (Exception ex)
            {
                KillDocReuseProcess();
                Console.Out.WriteLine("Cancellation error = {0}", ex.Message);
            }
        }

        private void DocReuseForm_Resize(object sender, EventArgs e)
        {
            webView.Height = Height;
            webView.Width = Width;
        }

        private void KillDocReuseProcess()
        {
            foreach (var process in Process.GetProcessesByName("Docomposer"))
            {
                process.Kill();
            }
        }
    }
}