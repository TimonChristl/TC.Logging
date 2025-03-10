﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace TC.Logging.Sinks
{

	/// <summary>
	/// An <see cref="ILogSink"/> implementation that writes log messages to a binary log file. The file format is
	/// specific to TC.Logging and may change between releases. To read a binary log file back, a <see cref="TC.Logging.Sources.BinaryFileLogSource"/>
	/// can be used.
	/// </summary>
	/// <remarks>
	/// The extra data field of log messages is converted to a string before it is written to the file. Writing a binary log file and reading it back using <see cref="TC.Logging.Sources.BinaryFileLogSource"/>
	/// therefore does not generate an identical stream of log messages.
	/// </remarks>
	public class BinaryFileLogSink : BaseLogSink
	{

		#region Private fields

		private readonly BinaryWriter binaryWriter;
		private readonly string filename;

		#endregion

		#region Constructor

		/// <summary>
		/// Initializes a new instance of <see cref="BinaryFileLogSink"/> with the given filename.
		/// </summary>
		/// <param name="filename"></param>
		public BinaryFileLogSink(string filename)
		{
			this.filename = filename;

			var stream = new FileStream(filename, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.Read);
			stream.Seek(0, SeekOrigin.End);

			binaryWriter = new BinaryWriter(stream);
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
				throw new ObjectDisposedException("BinaryFileLogSink");
#endif

            binaryWriter.Write((byte)1); // LogMessage Version
			binaryWriter.Write(logMessage.TimeStamp.ToBinary());
			binaryWriter.Write((byte)logMessage.Severity);
			binaryWriter.Write(logMessage.Text);
			binaryWriter.Write(logMessage.NestingDepth);
#if NET8_0_OR_GREATER
            string? extraDataAsString = logMessage.GetExtraDataAsString();
#else
			string extraDataAsString = logMessage.GetExtraDataAsString();
#endif
            if(extraDataAsString != null)
				binaryWriter.Write(extraDataAsString);
		}

		#endregion

		#region Protected methods

		/// <summary>
		/// Closes the underlying file.
		/// </summary>
		/// <param name="disposing"></param>
		protected override void Dispose(bool disposing)
		{
			if(disposing)
			{
				binaryWriter.Close();
			}

			base.Dispose(disposing);
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
