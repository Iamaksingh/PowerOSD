using System;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Media;
using System.Threading.Tasks;

namespace PowerOSD
{
    public partial class MainWindow : Window
    {
        private NotifyIcon trayIcon;
        private System.Windows.Forms.PowerLineStatus lastPowerStatus;

        public MainWindow()
        {
            InitializeComponent();
            PositionOSD();
            SetupSystemTray();
            StartPowerMonitoring();
        }

        private void PositionOSD()
        {
            this.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            this.Opacity = 0; // Start hidden
        }

        private void SetupSystemTray()
        {
            trayIcon = new NotifyIcon
            {
                Icon = System.Drawing.SystemIcons.Information,
                Text = "PowerOSD",
                Visible = true
            };

            trayIcon.ContextMenuStrip = new ContextMenuStrip();
            trayIcon.ContextMenuStrip.Items.Add("Exit", null, (s, e) => CloseApp());
        }

        private void CloseApp()
        {
            trayIcon.Dispose();
            System.Windows.Application.Current.Shutdown();
        }

        private async void StartPowerMonitoring()
        {
            lastPowerStatus = SystemInformation.PowerStatus.PowerLineStatus;

            while (true)
            {
                System.Windows.Forms.PowerLineStatus currentStatus = SystemInformation.PowerStatus.PowerLineStatus;

                if (currentStatus != lastPowerStatus)
                {
                    lastPowerStatus = currentStatus;
                    ShowOSD(currentStatus);
                }

                await Task.Delay(2000); // Check every 2 seconds
            }
        }

        private async void ShowOSD(System.Windows.Forms.PowerLineStatus status)
        {
            osdText.Text = (status == System.Windows.Forms.PowerLineStatus.Offline) ? "🔋 On Battery" : "🔌 Plugged In";

            this.Dispatcher.Invoke(() =>
            {
                this.Opacity = 0;  // Start transparent
                this.Show();
            });

            // Fade in
            for (double i = 0; i <= 1; i += 0.1)
            {
                this.Dispatcher.Invoke(() => this.Opacity = i);
                await Task.Delay(50);
            }

            await Task.Delay(2000); // Show for 2 seconds

            // Fade out
            for (double i = 1; i >= 0; i -= 0.1)
            {
                this.Dispatcher.Invoke(() => this.Opacity = i);
                await Task.Delay(50);
            }

            this.Dispatcher.Invoke(() => this.Hide());
        }
    }
}