using MonitorFluffy.Utilities;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace MonitorFluffy
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        // Utilities

        private DeoLogger deologger;
        private Recorder recorder;
        private AudioPlayer audioPlayer;

        // Monitoring

        private bool isMonitoring;
        private Thread monitor = null;
        private int currentMonitorSession = 1;

        public MainWindow()
        {
            InitializeComponent();

            txtOutput.Text = "Starting App. Checking Health:" + Environment.NewLine;
            if (SetupUtilities())
            {
                txtOutput.Text += Environment.NewLine + "------" + Environment.NewLine + "Ready To Roll! Click Start Monitoring!";
                btnMonitorTrigger.IsEnabled = true;
                btnViewRecordings.IsEnabled = true;
            }
            else
            {
                txtOutput.Text += Environment.NewLine + "------" + Environment.NewLine + "Annnnd it's broken. See above";
            }
            svOutput.ScrollToVerticalOffset(10);
        }

        private bool SetupUtilities()
        {
            System.IO.Directory.CreateDirectory(Pathing.InTodayPath);
            // DeoLogger
            try
            {
                deologger = new DeoLogger();
                txtOutput.Text += Environment.NewLine + "DeoLogger: Works!";
            }
            catch (Exception ex)
            {
                txtOutput.Text += Environment.NewLine + "DeoLogger: Broken: " + ex.Message;
                return false;
            }
            // Recorder
            try
            {
                recorder = new Recorder();
                txtOutput.Text += Environment.NewLine + "Recorder: Works!";
            }
            catch (Exception ex)
            {
                txtOutput.Text += Environment.NewLine + "Recorder: Broken: " + ex.Message;
                return false;
            }
            // AudioPlayer
            try
            {
                audioPlayer = new AudioPlayer();
                txtOutput.Text += Environment.NewLine + "AudioPlayer: Works!";
            }
            catch (Exception ex)
            {
                txtOutput.Text += Environment.NewLine + "AudioPlayer: Broken: " + ex.Message;
                return false;
            }
            txtOutput.Text += Environment.NewLine + "Writing to: " + Pathing.InTodayPath;
            return true;
        }

        private void btnViewRecordings_Click(object sender, RoutedEventArgs e)
        {
            Process.Start("explorer.exe", Pathing.InTodayPath);
        }

        private void btnMonitorTrigger_Click(object sender, RoutedEventArgs e)
        {
            // Update Status
            bool previousIsMonitoring = isMonitoring;
            isMonitoring = !isMonitoring;

            if (previousIsMonitoring)
            {
                btnMonitorTrigger.Content = "Monitor being shutdown!";
                btnMonitorTrigger.IsEnabled = false;
                Task t = new Task(() =>
                {
                    while (this.monitor.IsAlive)
                    {
                        // Wait.
                        Thread.Sleep(500);
                    }
                    this.Dispatcher.Invoke(() => btnMonitorTrigger.Content = "Begin Monitor");
                    this.Dispatcher.Invoke(() => btnMonitorTrigger.Background = new SolidColorBrush(Color.FromRgb(90, 201, 56)));
                    this.Dispatcher.Invoke(() => btnMonitorTrigger.IsEnabled = true);
                    currentMonitorSession = 1;
                });
                t.Start();
            }
            else
            {
                txtOutput.Text = "Starting Monitoring Thread @ " + DateTime.Now.ToShortTimeString() + Environment.NewLine;
                btnMonitorTrigger.Content = "Stop Monitor";
                btnMonitorTrigger.Background = new SolidColorBrush(Color.FromRgb(255, 153, 153));
                monitor = new Thread(() => this.MonitorThread());
                monitor.Start();
            }
        }

        #region Monitoring Thread

        private void MonitorThread()
        {
            while (isMonitoring)
            {
                // Setup next Monitor Session
                DateTime startTime = DateTime.Now.Add(new TimeSpan(0, new Random().Next(9, 19), 0));
                this.Dispatcher.Invoke(() => txtOutput.Text += Environment.NewLine + "----------------");

                // The +1 is to trigger the initial text output
                int minutesDiff = (int)(startTime - DateTime.Now).TotalMinutes + 1;
                //  Prevent Full on Thread Sleep so that we are checking on Stop monitor events.
                while (startTime > DateTime.Now && isMonitoring)
                {
                    Thread.Sleep(1390);
                    int minutesDiff_new = (int)(startTime - DateTime.Now).TotalMinutes;
                    if (minutesDiff != minutesDiff_new)
                    {
                        this.Dispatcher.Invoke(() => txtOutput.Text += Environment.NewLine + $"Monitor Session #{currentMonitorSession} begins in {minutesDiff} Minutes");
                        minutesDiff = minutesDiff_new;
                    }
                }

                // Recording before Audio

                deologger.StartLog();
                try
                {
                    // Recording Before Audio
                    if (isMonitoring)
                    {
                        this.Dispatcher.Invoke(() => txtOutput.Text += Environment.NewLine + " Recording 60 second audio @ " + DateTime.Now.ToShortTimeString());
                        recorder.Record(60, Recorder.RECORDING_TYPE.BEFORE_AUDIO);
                    }
                    // Play Audio
                    if (isMonitoring)
                    {
                        string inAudioPlayed = audioPlayer.PlayRandomAudio();
                        this.Dispatcher.Invoke(() => txtOutput.Text += Environment.NewLine + inAudioPlayed + " Audio Play @ " + DateTime.Now.ToShortTimeString());
                        deologger.AddLine(inAudioPlayed + " Audio Play @ " + DateTime.Now.ToShortTimeString());
                    }
                    // Recording after Audio
                    if (isMonitoring)
                    {
                        this.Dispatcher.Invoke(() => txtOutput.Text += Environment.NewLine + " Recording 60 second audio @ " + DateTime.Now.ToShortTimeString());
                        recorder.Record(60, Recorder.RECORDING_TYPE.AFTER_AUDIO);
                    }
                }
                catch (Exception ex)
                {
                    this.Dispatcher.Invoke(() => txtOutput.Text += Environment.NewLine + "Monitor Failed! " + ex.Message + Environment.NewLine);
                    deologger.AddLine("Failure while Monitoring: ");
                    deologger.AddLine(ex.Message);
                    deologger.AddLine(ex.StackTrace ?? "No StackTrace Provided");
                }
                this.Dispatcher.Invoke(() => txtOutput.Text += Environment.NewLine + "-------------------------");
                deologger.StopLog();
                ++currentMonitorSession;
            }
            this.Dispatcher.Invoke(() => txtOutput.Text += Environment.NewLine + "Monitoring has ended.");
        }

        #endregion Monitoring Thread
    }
}
