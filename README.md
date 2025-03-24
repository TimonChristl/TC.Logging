# TC.Logging

A simple logging system for NetStandard2.0 and Net8.0 or higher. The net8.0 version has nullability annotations.

## Features

* Log messages can be written to files in text or binary format, to the console, or to System.Diagnostics.Trace.
* Log messages written to text files can be formatted using an implementation of ITextLogMessageFormatter.
* Log messages can be written to files in a binary format, which is faster than writing to text files, but in
  order to do anything with these log messages they first have to be read back using a BinaryFileLogSource.
  The file format used for binary files is proprietary to this library.
* Log messages have a nesting level, which is transformed to an indentation when the log message is formatted
  as text 
* Exception logging: Logging an exception also logs its InnerException (recursively), and also treats
  AggregateException specially (logs all InnerExceptions)
* Method to log not-too-large amounts of binary data as hex dump

## Getting started

Create a new instance of class Logger, add one or more log sinks (for example, a TextFileLogSink), and call
the various Log() methods.

## Why another log system

This logging library is based on ideas of several other loggers I wrote at work and at home. At some point I
got sick of each project having its own logger and decided to merge them, which resulted in a total rewrite
and this library.

## TC.Logging.Json

Add-On for writing log messages to a JSON file.
