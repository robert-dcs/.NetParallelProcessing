﻿using Npgsql;
using ClosedXML.Excel;

namespace LerPlanilhaExcel
{
    class Program
    {
        static void Main(string[] args)
        {
            int sampleSize = 1000000;
            var xls = new XLWorkbook(@"C:\Users\Robertson\Documents\projetos\rt_iot\dotNeParalelismProcessor\sample\data"+sampleSize+".xlsx"); 
            var file = xls.Worksheets.First(w => w.Name == "Sheet1");
            var totalRows = file.Rows().Count();
            var listOfPeople = new List<string>();
            for (int l = 1; l <= totalRows; l++)
            {
                var name = file.Cell($"A{l}").Value.ToString();
                listOfPeople.Add(name);
            }
            Console.WriteLine("First record from sample: " + listOfPeople[0]);
            Console.WriteLine("Las record from sample: " + listOfPeople[listOfPeople.Count() - 1]);
               
            SynchronousProcessing(listOfPeople);
            ParallelProcessing(listOfPeople);
            ParallelProcessing2(listOfPeople);
            ParallelProcessing3(listOfPeople);
        }

        static void ParallelProcessing(List<string> listOfPeople) 
        {
            var dataBaseCon = new DBconnection.Database();
            using var con = dataBaseCon.getConnection();
            dataBaseCon.dropAndCreateTable();
            var watch = System.Diagnostics.Stopwatch.StartNew();
            Parallel.ForEach(listOfPeople, person =>
            {
                dataBaseCon.inserPersonToDB(person);
            });
            watch.Stop();
            var elapsedMs = watch.ElapsedMilliseconds;
            Console.WriteLine("[C#] Parallel implementation 1 took " + elapsedMs + " milliseconds.");
            Console.WriteLine("[C#] Processed " + listOfPeople.Count() + " records.");
        }

        static void ParallelProcessing2(List<string> listOfPeople) 
        {
            var dataBaseCon = new DBconnection.Database();
            using var con = dataBaseCon.getConnection();
            dataBaseCon.dropAndCreateTable();
            var watch = System.Diagnostics.Stopwatch.StartNew();
            Parallel.ForEach(listOfPeople, dataBaseCon.inserPersonToDB);
            watch.Stop();
            var elapsedMs = watch.ElapsedMilliseconds;
            Console.WriteLine("[C#] Parallel implementation 2 took " + elapsedMs + " milliseconds.");
            Console.WriteLine("[C#] Processed " + listOfPeople.Count() + " records.");
        }

        static void ParallelProcessing3(List<string> listOfPeople) 
        {
            var dataBaseCon = new DBconnection.Database();
            List<Thread> threads = new List<Thread>();
            using var con = dataBaseCon.getConnection();
            dataBaseCon.dropAndCreateTable();
            var watch = System.Diagnostics.Stopwatch.StartNew();

            int maxConcurrency =  Environment.ProcessorCount;
            SemaphoreSlim semaphore = new SemaphoreSlim(maxConcurrency);

            foreach (string person in listOfPeople)
            {
                Thread thread = new Thread(() =>
                {
                    semaphore.Wait();

                    try
                    {
                        dataBaseCon.inserPersonToDB(person);
                    }
                    finally
                    {
                        semaphore.Release(); 
                    }
                });

                thread.Start();
                threads.Add(thread); 
            }

            foreach (Thread thread in threads)
            {
                thread.Join();
            }

            watch.Stop();
            var elapsedMs = watch.ElapsedMilliseconds;
            Console.WriteLine("[C#] Parallel implementation 3 took " + elapsedMs + " milliseconds.");
            Console.WriteLine("[C#] Processed " + listOfPeople.Count() + " records.");
        }
        
        static void SynchronousProcessing(List<string> listOfPeople) 
        {
            var dataBaseCon = new DBconnection.Database();
            using var con = dataBaseCon.getConnection();
            dataBaseCon.dropAndCreateTable();
            var watch = System.Diagnostics.Stopwatch.StartNew();
            foreach (string person in listOfPeople)
            {
                dataBaseCon.inserPersonToDB(person);
            };
            watch.Stop();

            var elapsedMs = watch.ElapsedMilliseconds;
            Console.WriteLine("[C#] Synchronous implementation took " + elapsedMs + " milliseconds.");
            Console.WriteLine("[C#] Processed " + listOfPeople.Count() + " records.");
        }
    }
}

