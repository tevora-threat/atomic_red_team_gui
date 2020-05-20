using System;
using System.Collections.Generic;
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
using System.Windows.Forms;
using System.IO;
using System.ComponentModel;

namespace TevoraAutomatedRTGui
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    /// 

    public class SimulationStatusUpdate
    {
        public int number { get; set; }
        public string message { get; set; }
    }

    public partial class MainWindow : Window
    {
        GridViewColumnHeader currSortHeader = null;
        ListSortDirection currSortDirection = ListSortDirection.Ascending;


        public string atomic_dir { get; set; }
        public string output_dir { get; set; }
        public string config_path { get; set; }
        public bool running = false; 
        private AtomicEngine atomic_engine;
        public MainWindow()
        {
            this.atomic_dir  = @"C:\";
            this.output_dir = @"C:\";
            this.config_path = @".\config.json";

            InitializeComponent();
            ProgressBar.Minimum = 0;
        }

      
        private void ListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }
        private void LoadAtomicsClick(object sender, RoutedEventArgs e)
        {
            this.LoadAtomics(); 
        }
        private void LoadAtomics()

        {
            
                this.atomic_engine = new AtomicEngine(this.atomic_dir, this.output_dir, this.config_path);
            
      
            ProgressBar.IsIndeterminate = true;

            StartSimulationButton.IsEnabled = true;
            StartCleanupButton.IsEnabled = true; 
            AtomicsLoadedLabel.Content = this.atomic_engine.atomics.Count + " atomics loaded, " + this.atomic_engine.atomic_runnables.Count + " commands staged";
            AtomicsListView.ItemsSource = this.atomic_engine.atomic_runnables;
            ProgressBar.IsIndeterminate = false;
            ProgressDivider.Content = "0/" + this.atomic_engine.atomic_runnables.Count;
            ProgressBar.Maximum = this.atomic_engine.atomic_runnables.Count;
            ExportTestPLan.IsEnabled = true; 

        }



        private void OpenAtomicDir(object sender, RoutedEventArgs e)
        {
            FolderBrowserDialog fbd = new FolderBrowserDialog();
            if (fbd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                
                this.atomic_dir= fbd.SelectedPath;
                AtomicDirTextbox.Text = this.atomic_dir; 
            }
        }

        private void OpenOutputDir(object sender, RoutedEventArgs e)
        {
            FolderBrowserDialog fbd = new FolderBrowserDialog();
            if (fbd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {

                this.output_dir = fbd.SelectedPath;
                OutputDirTextbox.Text = this.output_dir;
            }
        }

        private void OpenConfigFile(object sender, RoutedEventArgs e)
        { 
            OpenFileDialog fbd = new OpenFileDialog();
            if (fbd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {

                this.atomic_dir = fbd.FileName;
                //ConfigPathTextbox.Text = this.atomic_dir;
            }
        }

        private async void StartSimulationButton_Click(object sender, RoutedEventArgs e)
        {
            if(this.running)
            {
                return;
            }
            this.running = true;
            LoadAtomicsButton.IsEnabled = false;
            try
            {
                Task<Report> task = Task.Run(() => this.atomic_engine.RunTests(this));
                StartSimulationButton.IsEnabled = false;
                StartCleanupButton.IsEnabled = false;


                Report report = await task;
                Utils.ReportToCsv(report, this.output_dir);
            }
            catch (Exception exception)
            {
                System.Windows.MessageBox.Show("Error: " + exception);

            }
            StartSimulationButton.IsEnabled = true;
            StartCleanupButton.IsEnabled = true;

            this.running = false;
            LoadAtomicsButton.IsEnabled = true;

        }

        private async void StartCleanupButton_Click(object sender, RoutedEventArgs e)
        {
            if (this.running)
            {
                return;
            }
            this.running = true;
            LoadAtomicsButton.IsEnabled = false;


            try
            {
                Task<Report> task = Task.Run(() => this.atomic_engine.RunCleanup(this));
                StartSimulationButton.IsEnabled = false;
                StartCleanupButton.IsEnabled = false;

                Report report = await task;
                Utils.ReportToCsv(report, this.output_dir, "cleanup_log_");
            }
            catch (Exception exception)
            {
                System.Windows.MessageBox.Show("Error: " + exception);
            
            }
            StartSimulationButton.IsEnabled = true;
            StartCleanupButton.IsEnabled = true;

            this.running = false;
            LoadAtomicsButton.IsEnabled = true;
        }
        public void UpdateStatus(SimulationStatusUpdate status)
        {

            Dispatcher.Invoke(() =>
            {
                ProgressDivider.Content = status.number + "/" + this.atomic_engine.atomic_runnables.Count + " " + status.message;
                ProgressBar.Value = status.number;

            });
        }

        private void ExportTestPLan_Click(object sender, RoutedEventArgs e)
        {
            this.atomic_engine.ExportTestPlan(this);
        }

        private void ProgressBar_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {

        }
    }
}
