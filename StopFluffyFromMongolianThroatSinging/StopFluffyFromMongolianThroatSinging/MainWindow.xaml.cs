using StopFluffyFromMongolianThroatSinging.Utilities;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
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

namespace StopFluffyFromMongolianThroatSinging
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
    }
}
