﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TC.Logging.Formatters;

namespace TC.Logging.Sinks
{

	/// <summary>
	/// Abstract base class for text-format implementations of <see cref="ILogSink"/>.
	/// </summary>
	public abstract class BaseTextLogSink : BaseLogSink
	{

		#region Private fields

		private readonly int indentWidth;
		private readonly ITextLogMessageFormatter formatter;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of <see cref="BaseTextLogSink"/> with the given indent width.
        /// </summary>
        /// <param name="indentWidth"></param>
        /// <param name="formatter"></param>
#if NET8_0_OR_GREATER
        public BaseTextLogSink(int indentWidth, ITextLogMessageFormatter? formatter = null)
#else
		public BaseTextLogSink(int indentWidth, ITextLogMessageFormatter formatter = null)
#endif
        {
            this.indentWidth = indentWidth;
			this.formatter = formatter ?? new DefaultShortTextLogMessageFormatter();
		}

		#endregion

		#region Protected methods

		/// <summary>
		/// Formats the log message <paramref name="logMessage"/> into a string.
		/// </summary>
		/// <param name="logMessage"></param>
		/// <returns></returns>
		protected string FormatLogMessage(LogMessage logMessage)
		{
			return formatter.Format(logMessage, indentWidth);
		}

		#endregion

		#region Public properties

		/// <summary>
		/// Width of indentation for one nesting level.
		/// </summary>
		public int IndentWidth
		{
			get { return indentWidth; }
		}

		#endregion

	}

}
