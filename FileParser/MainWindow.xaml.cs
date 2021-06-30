using Microsoft.Win32;
using System;
using System.ComponentModel;
using System.IO;
using System.Windows;
using ParseTextLib;
using System.Collections.Generic;

namespace FileParser
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private WordCounter wordCounter;
        public MainWindow()
        {
            InitializeComponent();
        }

        private void BrowseBtn_Click(object sender, RoutedEventArgs e)
        {
            if (wordCounter != null)
            {
                MessageBox.Show("Error: Already running.");
                return;
            }

            // Create OpenFileDialog
            OpenFileDialog openFileDlg = new OpenFileDialog();
            openFileDlg.DefaultExt = ".txt";
            openFileDlg.Filter = "Text documents (.txt)|*.txt";

            if (openFileDlg.ShowDialog() != true)
            {
                return;
            }

            BrowseBtn.Visibility = Visibility.Collapsed;
            //FileNameTextBox.Visibility = Visibility.Collapsed;
            ProgressBarStatus.Visibility = Visibility.Visible;
            CancelBtn.Visibility = Visibility.Visible;

            string fileName = openFileDlg.FileName;
            FileNameTextBox.Text = new FileInfo(fileName).Name;
            wordCounter = new WordCounter();

            // add our event handlers
            wordCounter.RunWorkerCompleted += new RunWorkerCompletedEventHandler(RunWorkerCompleted);
            wordCounter.ProgressChanged += new ProgressChangedEventHandler(ProgressChanged);
                            
            // start the worker thread
            wordCounter.RunWorkerAsync(fileName);
                       
        }

        
        private void RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (wordCounter.result != null)
            {
                SaveFileDialog saveFileDlg = new SaveFileDialog();
                saveFileDlg.DefaultExt = ".txt";
                saveFileDlg.Filter = "Text documents (.txt)|*.txt";

                if (saveFileDlg.ShowDialog() == true)
                {
                    try
                    {
                        using (StreamWriter writer = File.CreateText(saveFileDlg.FileName))
                        {
                            foreach (KeyValuePair<string, uint> pair in wordCounter.result)
                            {
                                writer.WriteLine("{0}: {1}", pair.Key, pair.Value);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Error: Could not save file to disk. Original error: " + ex.Message);
                    }
                }
            }
            else if (wordCounter.exception != null)
            {
                MessageBox.Show("Error: Could not process file. Original Error: " + wordCounter.exception.Message);
            }
            wordCounter = null;
            ProgressBarStatus.Visibility = Visibility.Collapsed;
            CancelBtn.Visibility = Visibility.Collapsed;
            BrowseBtn.Visibility = Visibility.Visible;
            //FileNameTextBox.Visibility = Visibility.Visible;
        }

        public void ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            ProgressBarStatus.Value = e.ProgressPercentage;
        }

        private void CancelBtn_Click(object sender, RoutedEventArgs e)
        {
            if (wordCounter == null)
            {
                MessageBox.Show("Error: Not running.");
                return;
            }
            wordCounter.CancelAsync();
        }
    }

    
}
