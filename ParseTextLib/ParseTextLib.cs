using System.ComponentModel;
using System.IO;
using System.Collections.Generic;
using System;

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
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                exception = ex;
            }
        }
    }
}
