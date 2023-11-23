using System;

namespace ConsoleApp
{

    /// <summary>
    /// Console application to demonstrate a suitable method for ensuring synchronized access to a single file.
    /// 10 threads will be launched to run simultaneously.
    /// Each thread should append 10 lines to the file as fast as possible
    /// </summary>
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Multi-threaded console application that synchronizes writes to a single file.");

            const String filename = "/log/out.txt";
            const int maxThreads = 10;

            // A singleton is used to represent the single file which all threads are attempting to write to.
            SynchronizedFile syncFile = null;
            try
            {
                syncFile = SynchronizedFile.GetInstance(filename);
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.ToString());
                Environment.Exit(1);
            }

            // Create and launch the threads
            Task[] tasks = new Task[maxThreads];
            for (int i = 0; i < maxThreads; i++)
            {
                Console.WriteLine("Launching thread #" + (i+1));
                Worker worker = new Worker(syncFile);
                tasks[i] = Task.Run(() => worker.DoWork());
            }

            // Wait for all of the threads to finish
            Task.WaitAll(tasks);

            if (syncFile != null)
            {
                try
                {
                     syncFile.Close();
                }
                catch (Exception ex)
                {
                    Console.WriteLine("There was an exception while closing the SynchronizedFile:");
                    Console.WriteLine(ex.ToString());
                }
            }

            Console.WriteLine("Press enter to exit...");
            Console.ReadLine();
        }

    }
}