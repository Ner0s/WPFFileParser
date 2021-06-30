using Microsoft.Win32;
using System;
using System.ComponentModel;
using System.IO;
using System.Threading;
using System.Windows;

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
                    BrowseBtn.Visibility = Visibility.Collapsed;
                    FileNameTextBox.Visibility = Visibility.Collapsed;
                    ProgressBarStatus.Visibility = Visibility.Visible;
                    Stream myStream;
                    if ((myStream = openFileDlg.OpenFile()) != null)
                    {
                        using (myStream)
                        {
                            string filename = openFileDlg.FileName;
                            FileNameTextBox.Text = filename;
                            string[] filelines = File.ReadAllLines(filename);
                            Window_ContentRendered(sender, e);
                            foreach (string line in filelines)
                            {
                                //MessageBox.Show(line);
                            }
                        }
                    }
                    BrowseBtn.Visibility = Visibility.Visible;
                    FileNameTextBox.Visibility = Visibility.Visible;
                    ProgressBarStatus.Visibility = Visibility.Collapsed;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: Could not read file from disk. Original error: " + ex.Message);
                }
            }
        }

        private void Window_ContentRendered(object sender, EventArgs e)
        {
            BackgroundWorker worker = new BackgroundWorker
            {
                WorkerReportsProgress = true
            };
            worker.DoWork += Worker_DoWork;
            worker.ProgressChanged += Worker_ProgressChanged;

            worker.RunWorkerAsync();
        }

        private void Worker_DoWork(object sender, DoWorkEventArgs e)
        {
            for (int i = 0; i < 100; i++)
            {
                (sender as BackgroundWorker).ReportProgress(i);
                Thread.Sleep(100);
            }
        }

        private void Worker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            ProgressBarStatus.Value = e.ProgressPercentage;
        }

    }
}
