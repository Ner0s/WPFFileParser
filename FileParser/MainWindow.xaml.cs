using Microsoft.Win32;
using System;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Windows;
using ParseTextLib;

namespace FileParser
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void BrowseBtn_Click(object sender, RoutedEventArgs e)
        {

            // Create OpenFileDialog
            OpenFileDialog openFileDlg = new OpenFileDialog();
            bool? result = openFileDlg.ShowDialog();
            openFileDlg.DefaultExt = ".txt";
            openFileDlg.Filter = "Text documents (.txt)|*.txt";
            openFileDlg.InitialDirectory = @"C:\";

            if (result == true)
            {
                try
                {
                    //BrowseBtn.Visibility = Visibility.Collapsed;
                    //FileNameTextBox.Visibility = Visibility.Collapsed;
                    //ProgressBarStatus.Visibility = Visibility.Visible;
                    //CancelBtn.Visibility = Visibility.Visible;

                    Stream myStream;
                    if ((myStream = openFileDlg.OpenFile()) != null)
                    {
                        using (myStream)
                        {
                            string filename = openFileDlg.FileName;
                            FileNameTextBox.Text = filename;
                            string[] fileLines = File.ReadAllLines(filename);

                            BackgroundWorker worker = new BackgroundWorker
                            {
                                // tell the background worker it can report progress
                                WorkerReportsProgress = true
                            };

                            // add our event handlers
                            worker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(RunWorkerCompleted);
                            worker.ProgressChanged += new ProgressChangedEventHandler(ProgressChanged);
                            worker.DoWork += new DoWorkEventHandler(DoWork);

                            // start the worker thread
                            worker.RunWorkerAsync(fileLines);
                        }
                    }

                    //ProgressBarStatus.Visibility = Visibility.Collapsed;
                    //CancelBtn.Visibility = Visibility.Collapsed;
                    //BrowseBtn.Visibility = Visibility.Visible;
                    //FileNameTextBox.Visibility = Visibility.Visible;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: Could not read file from disk. Original error: " + ex.Message);
                }
            }

        }
        private void DoWork(object sender, DoWorkEventArgs e)
        {
            // get a reference to the worker that started this request
            BackgroundWorker workerSender = sender as BackgroundWorker;

            // get a node list from agrument passed to RunWorkerAsync
            string[] lines = e.Argument as string[];

            for (int i = 0; i < lines.Count(); i++)
            {
                new ParseLines(lines[i]);
                workerSender.ReportProgress(i != 0 ? lines.Count() / i : 0);
            }
        }

        private void RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            // do something after work is completed     
        }

        public void ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            ProgressBarStatus.Value = e.ProgressPercentage;
        }

    }

    
}
