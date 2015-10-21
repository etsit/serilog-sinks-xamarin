// Copyright 2015 Serilog Contributors
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//     http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using System;
using Serilog.Configuration;
using Serilog.Events;
using Serilog.Formatting.Display;
using Serilog.Sinks.MonoAndroid;
using Serilog.Sinks.RollingFile;

namespace Serilog
{
	/// <summary>
	/// Adds WriteTo.AndroidLog() to the logger configuration.
	/// </summary>
	public static class LoggerConfigurationMonoAndroidExtensions
	{
		const string DefaultAndroidLogOutputTemplate = "[{Level}] {Message:l{NewLine:l}{Exception:l}";
        const string DefaultOutputTemplate = "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level}] {Message}{NewLine}{Exception}";
        const string DefaultConsoleOutputTemplate = "{Timestamp:yyyy-MM-dd HH:mm:ss} [{Level}] {Message}{NewLine}{Exception}";
        const long DefaultFileSizeLimitBytes = 1L * 1024 * 1024 * 1024;
        const int DefaultRetainedFileCountLimit = 31; // A long month of logs

	    /// <summary>
	    /// Write to the built-in Android log.
	    /// </summary>
	    /// <param name="sinkConfiguration">The configuration this applies to.</param>
	    /// <param name="restrictedToMinimumLevel">The minimum log event level required in order to write an event to the sink.</param>
	    /// <param name="outputTemplate">Output template providing the format for events</param>
	    /// <param name="formatProvider">Supplies culture-specific formatting information, or null.</param>
	    /// <returns>Logger configuration, allowing configuration to continue.</returns>
	    /// <exception cref="ArgumentNullException">A required parameter is null.</exception>
	    public static LoggerConfiguration AndroidLog(this LoggerSinkConfiguration sinkConfiguration,
			LogEventLevel restrictedToMinimumLevel = LevelAlias.Minimum,
			string outputTemplate = DefaultAndroidLogOutputTemplate,
			IFormatProvider formatProvider = null)
		{

			if (sinkConfiguration == null)
				throw new ArgumentNullException("sinkConfiguration");

			if (outputTemplate == null)
				throw new ArgumentNullException("outputTemplate");

			var formatter = new MessageTemplateTextFormatter(outputTemplate, formatProvider);
			return sinkConfiguration.Sink(new AndroidLogSink(formatter), restrictedToMinimumLevel);
		}

        /// <summary>
        /// Write log events to a series of files. Each file will be named according to
        /// the date of the first log entry written to it. Only simple date-based rolling is
        /// currently supported.
        /// </summary>
        /// <param name="sinkConfiguration">Logger sink configuration.</param>
        /// <param name="pathFormat">String describing the location of the log files,
        /// with {Date} in the place of the file date. E.g. "Logs\myapp-{Date}.log" will result in log
        /// files such as "Logs\myapp-2013-10-20.log", "Logs\myapp-2013-10-21.log" and so on.</param>
        /// <param name="restrictedToMinimumLevel">The minimum level for
        /// events passed through the sink.</param>
        /// <param name="outputTemplate">A message template describing the format used to write to the sink.
        /// the default is "{Timestamp} [{Level}] {Message}{NewLine}{Exception}".</param>
        /// <param name="formatProvider">Supplies culture-specific formatting information, or null.</param>
        /// <param name="fileSizeLimitBytes">The maximum size, in bytes, to which any single log file will be allowed to grow.
        /// For unrestricted growth, pass null. The default is 1 GB.</param>
        /// <param name="retainedFileCountLimit">The maximum number of log files that will be retained,
        /// including the current log file. For unlimited retention, pass null. The default is 31.</param>
        /// <returns>Configuration object allowing method chaining.</returns>
        /// <remarks>The file will be written using the UTF-8 character set.</remarks>
        public static LoggerConfiguration RollingFile(
            this LoggerSinkConfiguration sinkConfiguration,
            string pathFormat,
            LogEventLevel restrictedToMinimumLevel = LevelAlias.Minimum,
            string outputTemplate = DefaultOutputTemplate,
            IFormatProvider formatProvider = null,
            long? fileSizeLimitBytes = DefaultFileSizeLimitBytes,
            int? retainedFileCountLimit = DefaultRetainedFileCountLimit)
        {
            if (sinkConfiguration == null) throw new ArgumentNullException("sinkConfiguration");
            if (outputTemplate == null) throw new ArgumentNullException("outputTemplate");
            var formatter = new MessageTemplateTextFormatter(outputTemplate, formatProvider);
            var sink = new RollingFileSink(pathFormat, formatter, fileSizeLimitBytes, retainedFileCountLimit);
            return sinkConfiguration.Sink(sink, restrictedToMinimumLevel);
        }
	}
}