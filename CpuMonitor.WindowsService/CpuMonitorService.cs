using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

namespace CpuMonitor.WindowsService
{
    using System.Configuration;
    using System.Timers;

    public partial class CpuMonitorService : ServiceBase
    {
        private readonly Timer updateTimer;
        private readonly Timer saveTimer;

        private readonly Monitor monitor;

        private readonly string filePath;

        public CpuMonitorService()
        {
            this.InitializeComponent();
            this.monitor = new Monitor(new[] { 0, 80, 90, 95, 99 });
            this.filePath = ConfigurationManager.AppSettings["Persistence.Filepath"];
            this.updateTimer = new Timer();
            this.saveTimer = new Timer();
        }

        protected override void OnStart(string[] args)
        {
            this.Log("Starting");
            this.updateTimer.Interval = 1000;
            this.updateTimer.Enabled = true;
            this.updateTimer.Elapsed += this.UpdateTimer_Tick;
            this.updateTimer.Start();

            this.saveTimer.Interval = 10 * 1000;
            this.saveTimer.Enabled = true;
            this.saveTimer.Elapsed += this.SaveTimer_Tick;
            this.saveTimer.Start();

            this.Log($"Archiving file {this.filePath}");
            Persistence.archive(this.filePath);
            this.Log("Finished startup");
        }

        private void UpdateTimer_Tick(object sender, EventArgs e)
        {
            this.monitor.Update();
        }

        private void SaveTimer_Tick(object sender, EventArgs e)
        {
            this.Log($"Saving to {this.filePath}");
            this.monitor.Save(this.filePath);
        }

        protected override void OnStop()
        {
            this.updateTimer.Stop();
            this.saveTimer.Stop();
        }

        private void Log(string text)
        {
            var filename = @"C:\temp\CpuMonitor\log.txt";
            System.IO.File.AppendAllText(filename, $"[{DateTime.Now}] {text}\r\n");
        }
    }
}
