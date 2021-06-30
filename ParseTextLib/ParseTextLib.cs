using System.ComponentModel;
using System.IO;
using System.Collections.Generic;
using System;
using System.Threading;
using System.Diagnostics;

namespace ParseTextLib
{
    public class WordCounter : BackgroundWorker
    {
        public Dictionary<string, uint> result;
        public Exception exception;
        public WordCounter()
        {
            // tell the background worker it can report progress
            WorkerReportsProgress = true;
            WorkerSupportsCancellation = true;
            DoWork += new DoWorkEventHandler(DoMyWork);
        }

        private void DoMyWork(object sender, DoWorkEventArgs e)
        {
            // get a reference to the worker that started this request
            BackgroundWorker workerSender = sender as BackgroundWorker;

            try
            {
                // get a node list from agrument passed to RunWorkerAsync
                string fileName = e.Argument as string;

                using (StreamReader reader = File.OpenText(fileName))
                {
                    result = new Dictionary<string, uint>();
                    string line;
                    DateTime lastUpdate = DateTime.Now;
                    while ((line = reader.ReadLine()) != null)
                    {
                        foreach (string word in line.Split(' '))
                        {
                            string trimmedWord = word.Trim();
                            if (trimmedWord == "")
                            {
                                continue;
                            }
                            if (result.ContainsKey(trimmedWord))
                            {
                                result[trimmedWord]++;
                            }
                            else
                            {
                                result.Add(trimmedWord, 1);
                            }
                            if ((DateTime.Now - lastUpdate).TotalMilliseconds >= 100)
                            {
                                workerSender.ReportProgress((int)(
                                    (double)reader.BaseStream.Position /
                                    (double)reader.BaseStream.Length *
                                    100.0d
                                ));
                                lastUpdate = DateTime.Now;
                            }
                            //Debug.WriteLine("{0} {1}", reader.BaseStream.Position, reader.BaseStream.Length);
                        }
                        if (CancellationPending)
                        {
                            result = null;
                            workerSender.ReportProgress(0);
                            return;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                exception = ex;
            }
            workerSender.ReportProgress(100);
        }
    }
}
