using System.Text;
using System.Threading;

/// <summary>
/// This singleton class synchronizes the write operations to the file to avoid mutli threaded concurrency issues.
/// The
/// </summary>
public class SynchronizedFile
{
    private ReaderWriterLockSlim fileLock = new ReaderWriterLockSlim();
    private int lineCounter = 0;
    private FileStream fs;

    private static readonly object singletonLock = new object();
    private static SynchronizedFile instance = null;

    /// <summary>
    /// Singleton get instance method.
    /// </summary>
    /// <param name="filename">The file to be create.</param>
    /// <returns>The SynchronizedFile object</returns>
    public static SynchronizedFile GetInstance(string filename)
    {
        if (instance == null)
        {
            lock (singletonLock)
            {
                if (instance == null)
                {
                    instance = new SynchronizedFile(filename);
                }
            }
        }
        return instance;
    }

    /// <summary>
    /// Private constructor for initializing the singleton.
    /// </summary>
    /// <param name="filename"></param>
    private SynchronizedFile(string filename)
    {
        fs = File.Create(filename);

        String line = formatLine(0);

        // Write the information to the file.
        writeToFile(line);

        // Increament the line counter
        lineCounter++;
    }

    public int Count
    {
        get { return lineCounter; }
    }

    /// <summary>
    /// Logging involves formating some data and then writing this data to the file.
    /// </summary>
    public void Log()
    {
        // Aquire the write lock for this file
        fileLock.EnterWriteLock();
        try
        {
            // Generate the line to be logged
            int threadID = Thread.CurrentThread.ManagedThreadId;
            String line = formatLine(threadID);

            // Write that line to the file
            writeToFile(line);

             // Increament the line counter
             lineCounter++;
        }
        finally
        {
            // Release the write lock for this file
            fileLock.ExitWriteLock();
        }

        // Introduce a small delay to make this more realistic and permit other theres to aquire the file lock
        Thread.Sleep(1);
    }

    /// <summary>
    /// Private method to consistenly format the data being logged.
    /// </summary>
    /// <param name="threadID">The ID of the thread performing the operation</param>
    /// <returns>The formated data</returns>
    private String formatLine(int threadID)
    {
        // Format the line using the pattern “<line_count>, <thread_id>, <current_time_stamp>”
        //   where <current_time_stamp> is a string of the form HH:MM:SS.mmm
        //   (HH = hours, MM = minutes, SS=seconds, mmm = milliseconds to 3 decimal places.)
        return lineCounter + "," + threadID + "," + DateTime.Now.ToString("hh:mm:ss:fff") + Environment.NewLine;
    }

    /// <summary>
    /// Private method to consistenly write the data to the file.
    /// </summary>
    /// <param name="data"></param>
    private void writeToFile(string data)
    {
        try
        {
            // Write the information to the file.
            byte[] info = new UTF8Encoding(true).GetBytes(data);
            fs.Write(info, 0, info.Length); ;
        }
        catch (Exception ex)
        {
            Console.WriteLine("There was an exception while writing to the SynchronizedFile:");
            Console.WriteLine(ex.ToString());
        }
    }

    public void Close()
    {
        if (fs != null)
            fs.Close();
    }

    ~SynchronizedFile()
    {
        Close();
        if (fileLock != null)
            fileLock.Dispose();
        if (fs != null)
            fs.Dispose();
    }
}