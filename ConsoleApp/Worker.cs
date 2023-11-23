using System;

namespace ConsoleApp
{
    /// <summary>
    /// Worker class representing a thread of execution with this example.
    /// </summary>
    internal class Worker
    {
        private SynchronizedFile syncFile;
        private const int numLinesToAppend = 10;

        /// <summary>
        /// Constructor given a SynchronizedFile as input
        /// </summary>
        /// <param name="file">SynchronizedFile object</param>
        public Worker(SynchronizedFile file)
        {
            syncFile = file;
        }

        /// <summary>
        /// The operations to be performed when this worker class (thread) runs.
        /// </summary>
        internal void DoWork()
        {
            for (int i = 0; i < numLinesToAppend; i++)
            {
                syncFile.Log();
            }
        }
    }
}
