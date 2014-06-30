// Copyright (c) Philip McGarvey 2011

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.IO;

namespace Utilities {
  public interface ILog {
    void Write(string line);
  }


  public abstract class CustomLog<T> {

    // Thread the log queue is running on
    Thread logThread;
    object locker;
    // Queue of entries to be written
    Queue<T> queue;
    
    // Delay in miliseconds between writes
    int delay;
    T lastEntry;

    #region Properties

    /// <summary>
    /// The interval, in milliseconds, between writes to the log file.
    /// </summary>
    public int Interval {
      get {
        lock (locker) { return delay; }
      }
      set {
        lock (locker) {
          if (value < 0)
            throw new ArgumentException("Interval must be non-negative");
          else
            delay = value;
        }
      }
    }

    /// <summary>
    /// Gets whether the writing thread has been started.
    /// </summary>
    public bool Started { get; private set; }

    #endregion


    /// <summary>
    /// Creates a new LogFile which will write to the given path.
    /// </summary>
    /// <param name="path">The destination path for the log file to be written to.</param>
    public CustomLog() {
      delay = 100;
      queue = new Queue<T>();
      locker = new object();
    }

    /// <summary>
    /// Starts the writing thread.
    /// </summary>
    public void Start() {
      if (Started)
        throw new InvalidOperationException("Log has already been started");
      BeforeStart();
      logThread = new Thread(LogLoop);
      logThread.Start();
      Started = true;
    }

    protected void BeforeStart() {

    }

    /// <summary>
    /// Stops the writing thread.
    /// </summary>
    public void Stop() {
      logThread.Abort();
    }

    /// <summary>
    /// Writes a line to the log file.
    /// </summary>
    /// <param name="str">The line to be written.</param>
    public void Write(T entry) {
      lock (locker) {
        queue.Enqueue(entry);
        lastEntry = entry;
      }
    }

    #region Private Implementation
    /// <summary>
    /// The main loop for the writing thread.
    /// </summary>
    private void LogLoop() {
      while (true) {
        T[] current = null;
        lock (locker) {
          if (queue.Count > 0) {
            current = new T[queue.Count];
            queue.CopyTo(current, 0);
            queue.Clear();
          }
        }
        if (current != null) {
          WriteAll(current);
        }

        Thread.Sleep(delay);
      }
    }
    /// <summary>
    /// Writes strings to the log file, and closes the file.
    /// </summary>
    protected abstract void WriteAll(T[] entries);

    #endregion

  }
  /// <summary>
  /// Provides a simple interface for writing to a log file asyncronously.  
  /// </summary>
  public class LogFile : ILog {
    static readonly DateTime unixEpoch = new DateTime(1970, 1, 1, 0, 0, 0);

    #region Fields

    // Thread the log queue is running on
    Thread logThread;
    // Path to the log file being written to
    string path;
    object locker;
    // Queue of strings to be written
    List<string> queue;

    bool showTimestampAsSpan;

    // Delay in miliseconds between writes
    int delay;
    // Maximum file length before wrapping around to the beginning
    long fileLength;
    string lineEnding;
    // Format string for the timestamp on each line
    string timeFormat;
    FileMode mode;
    // Timestamp of last entry, for when we are recording differences.
    double lastTimeStamp;
    string lastLine;

    #endregion

    #region Properties

    /// <summary>
    /// Gets or sets the format string used to format the unix timestamp at the beginning of each line of the log file.  Example: "00000000.000 ".  Default is null.
    /// </summary>
    public string TimestampFormat {
      get {
        lock (locker) { return timeFormat; }
      }
      set {
        lock (locker) { timeFormat = value; }
      }
    }

    /// <summary>
    /// Gets or sets the string to append to each line of the log file.  Default is "\r\n".
    /// </summary>
    public string Newline {
      get {
        lock (locker) { return lineEnding; }
      }
      set {
        lock (locker) { lineEnding = value; }
      }
    }

    /// <summary>
    /// If true, the timestamp prefixing each line will contain the length in seconds since the last entry, rather than the absolute unix time.
    /// </summary>
    public bool ShowTimestampAsSpan {
      get {
        lock (locker) { return showTimestampAsSpan; }
      }
      set {
        lock (locker) { showTimestampAsSpan = value; }
      }
    }

    /// <summary>
    /// Gets or sets the maximum length of the log file, in bytes.  If MaximumLength is 0, there is no limit.  Otherwise, the log will begin overwriting the file each time it reaches the limit.
    /// </summary>
    public long MaximumLength {
      get {
        lock (locker) { return fileLength; }
      }
      set {
        lock (locker) {
          if (value < 0)
            throw new ArgumentException("MaximumLength must be non-negative");
          else
            fileLength = value;
        }
      }
    }

    /// <summary>
    /// The interval, in milliseconds, between writes to the log file.
    /// </summary>
    public int Interval {
      get {
        lock (locker) { return delay; }
      }
      set {
        lock (locker) {
          if (value < 0)
            throw new ArgumentException("Interval must be non-negative");
          else
            delay = value;
        }
      }
    }

    /// <summary>
    /// Gets whether the writing thread has been started.
    /// </summary>
    public bool Started { get; private set; }

    #endregion


    /// <summary>
    /// Creates a new LogFile which will write to the given path.
    /// </summary>
    /// <param name="path">The destination path for the log file to be written to.</param>
    public LogFile(string path, FileMode mode = FileMode.Create) {
      timeFormat = null;
      lineEnding = "\r\n";
      fileLength = 0;
      delay = 100;
      this.mode = mode;
      this.path = path;
      queue = new List<string>();
      locker = new object();
    }

    /// <summary>
    /// Starts the writing thread.
    /// </summary>
    public void Start() {
      if (Started)
        throw new InvalidOperationException("Log file has already been started");

      if (mode != FileMode.Append)
        File.Delete(path);

      logThread = new Thread(LogLoop);
      logThread.Start();
      Started = true;
    }

    /// <summary>
    /// Stops the writing thread.
    /// </summary>
    public void Stop() {
      logThread.Abort();
    }

    /// <summary>
    /// Writes a line to the log file.
    /// </summary>
    /// <param name="str">The line to be written.</param>
    public void Write(string line) {
      lock (locker) {
        if (showTimestampAsSpan)
          queue.Add(GetTimestamp() + lastLine);
        else
          queue.Add(GetTimestamp() + line);
        lastLine = line;
      }
    }

    #region Private Implementation

    /// <summary>
    /// Returns the current timestamp formatted with timeFormat.
    /// </summary>
    private string GetTimestamp() {
      if (string.IsNullOrEmpty(timeFormat)) {
        return "";
      } else {
        double last = lastTimeStamp;
        lastTimeStamp = (DateTime.UtcNow - unixEpoch).TotalSeconds;
        if (showTimestampAsSpan)
          return (lastTimeStamp - last).ToString(timeFormat);
        return lastTimeStamp.ToString(timeFormat);
      }
    }

    /// <summary>
    /// Writes strings to the log file, and closes the file.
    /// </summary>
    private void WriteStrings(string[] strings) {
      File.AppendAllLines(path, strings);
    }

    /// <summary>
    /// The main loop for the writing thread.
    /// </summary>
    private void LogLoop() {
      while (true) {
        string[] current = null;
        lock (locker) {
          if (queue.Count > 0) {
            current = new string[queue.Count];
            queue.CopyTo(current);
            queue.Clear();
          }
        }
        if (current != null) {
          WriteStrings(current);
        }

        Thread.Sleep(delay);
      }
    }

    #endregion
  }
}
