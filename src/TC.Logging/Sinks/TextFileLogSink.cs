﻿using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading;
using TC.Logging.Formatters;

#if NET8_0_OR_GREATER
#pragma warning disable IDE0290
#pragma warning disable IDE0063
#endif

namespace TC.Logging.Sinks
{

	/// <summary>
	/// An <see cref="ILogSink"/> implementation that writes log messages to a text file.
	/// </summary>
	/// <remarks>
	/// When two TextFileLogSinks are appending to the same text file at the same time, it is possible that one of the two
	/// log messages gets lost due to a race condition between opening the file in append mode, and writing the line
	/// It is best to avoid logging to the same file from more than one logger concurrently.
	/// </remarks>
	public class TextFileLogSink : BaseTextLogSink
	{

		#region Private fields

		private readonly string filename;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new <see cref="TextFileLogSink"/> instance with the given filename, indent width and formatter.
        /// </summary>
        /// <param name="filename"></param>
        /// <param name="indentWidth"></param>
        /// <param name="formatter"></param>
#if NET8_0_OR_GREATER
        public TextFileLogSink(string filename, int indentWidth = 4, ITextLogMessageFormatter? formatter = null)
#else
		public TextFileLogSink(string filename, int indentWidth = 4, ITextLogMessageFormatter formatter = null)
#endif
            : base(indentWidth, formatter ?? new DefaultTextLogMessageFormatter())
		{
			this.filename = filename;
		}

        #endregion

        #region Public methods

        /// <inheritdoc/>
        public override void Process(LogMessage logMessage)
		{
#if NET8_0_OR_GREATER
			ObjectDisposedException.ThrowIf(IsDisposed, this);
#else
			if(IsDisposed)
				throw new ObjectDisposedException("TextFileLogSink");
#endif

            using(Stream stream = new FileStream(filename, FileMode.Append, FileAccess.Write, FileShare.ReadWrite))
			{
				using(var streamWriter = new StreamWriter(stream))
				{
					streamWriter.Write(FormatLogMessage(logMessage));

					return;
				}
			}
		}

        /// <summary>
        /// Determines whether the log directory is writeable by attempting to create a file in the directory.
        /// 
        /// This method exists only because of a legacy application and will be obsoleted at some time in the future.
        /// </summary>
        /// <param name="messages"></param>
        /// <returns></returns>
        public bool IsLogDirectoryWriteable(StringBuilder messages)
		{
#if NET8_0_OR_GREATER
            string? logDirectory = Path.GetDirectoryName(filename);
#else
			string logDirectory = Path.GetDirectoryName(filename);
#endif

            if(logDirectory == null || !IsWriteableDir(logDirectory))
			{
				messages.AppendFormat("Directory LogDirectory {0} is not writeable\n", logDirectory);
				return false;
			}

			return true;
		}

		#endregion

		#region Private methods

		private static bool IsWriteableDir(string dirname)
		{
			try
			{
				if(!Directory.Exists(dirname))
					return false;

				string filename = Path.Combine(dirname, "test_" + Guid.NewGuid().ToString("N"));
				try
				{
					using(var fs = File.Create(filename))
					{
						return true;
					}
				}
				finally
				{
					if(File.Exists(filename))
						File.Delete(filename);
				}
			}
			catch
			{
				return false;
			}
		}

        #endregion

        #region Public properties

        /// <summary>
        /// Filename of the log file
        /// </summary>
        public string LogFileName
		{
			get { return filename; }
		}

		#endregion

	}

}
