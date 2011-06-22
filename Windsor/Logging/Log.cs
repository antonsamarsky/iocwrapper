using System;
using Diagnostics;
using log4net;

namespace Logging
{
	/// <summary>
	/// The logging manager.
	/// </summary>
	public class Log
	{
		/// <summary>
		/// Gets or sets the get logging info.
		/// </summary>
		/// <value>The get logging info.</value>
		public static Action SetAdditionalLoggingInfo { get; set; }

		/// <summary>
		/// Errors the specified message.
		/// </summary>
		/// <param name="message">The message.</param>
		/// <param name="instance">The instance.</param>
		public static void Error(string message, object instance)
		{
			Error(message, null, instance.GetType());
		}

		/// <summary>
		/// Errors the specified message.
		/// </summary>
		/// <param name="message">The message.</param>
		/// <param name="ownerType">Type of the owner.</param>
		public static void Error(string message, Type ownerType)
		{
			Error(message, null, ownerType);
		}

		/// <summary>
		/// Errors the specified message.
		/// </summary>
		/// <param name="message">The message.</param>
		/// <param name="exception">The exception.</param>
		/// <param name="instance">The instance.</param>
		public static void Error(string message, Exception exception, object instance)
		{
			Error(message, exception, instance.GetType());
		}

		/// <summary>
		/// Errors the specified message.
		/// </summary>
		/// <param name="message">The message.</param>
		/// <param name="exception">The exception.</param>
		/// <param name="ownerType">The owner type.</param>
		public static void Error(string message, Exception exception, Type ownerType)
		{
			LogMessage(message, exception, ownerType, LogLevel.Error);
		}

		/// <summary>
		/// Infoes the specified message.
		/// </summary>
		/// <param name="message">The message.</param>
		/// <param name="instance">The instance.</param>
		public static void Info(string message, object instance)
		{
			Info(message, null, instance.GetType());
		}

		/// <summary>
		/// Infoes the specified message.
		/// </summary>
		/// <param name="message">The message.</param>
		/// <param name="ownerType">Type of the owner.</param>
		public static void Info(string message, Type ownerType)
		{
			Info(message, null, ownerType);
		}

		/// <summary>
		/// Infoes the specified message.
		/// </summary>
		/// <param name="message">The message.</param>
		/// <param name="exception">The exception.</param>
		/// <param name="instance">The instance.</param>
		public static void Info(string message, Exception exception, object instance)
		{
			Info(message, exception, instance.GetType());
		}

		/// <summary>
		/// Infoes the specified message.
		/// </summary>
		/// <param name="message">The message.</param>
		/// <param name="exception">The exception.</param>
		/// <param name="ownerType">Type of the owner.</param>
		public static void Info(string message, Exception exception, Type ownerType)
		{
			LogMessage(message, exception, ownerType, LogLevel.Info);
		}

		/// <summary>
		/// Warns the specified message.
		/// </summary>
		/// <param name="message">The message.</param>
		/// <param name="instance">The instance.</param>
		public static void Warn(string message, object instance)
		{
			Warn(message, null, instance.GetType());
		}

		/// <summary>
		/// Warns the specified message.
		/// </summary>
		/// <param name="message">The message.</param>
		/// <param name="ownerType">Type of the owner.</param>
		public static void Warn(string message, Type ownerType)
		{
			Warn(message, null, ownerType);
		}

		/// <summary>
		/// Warns the specified message.
		/// </summary>
		/// <param name="message">The message.</param>
		/// <param name="exception">The exception.</param>
		/// <param name="instance">The instance.</param>
		public static void Warn(string message, Exception exception, object instance)
		{
			Warn(message, exception, instance.GetType());
		}

		/// <summary>
		/// Warns the specified message.
		/// </summary>
		/// <param name="message">The message.</param>
		/// <param name="exception">The exception.</param>
		/// <param name="ownerType">Type of the owner.</param>
		public static void Warn(string message, Exception exception, Type ownerType)
		{
			LogMessage(message, exception, ownerType, LogLevel.Warning);
		}

		/// <summary>
		/// Warns the specified message.
		/// </summary>
		/// <param name="message">The message.</param>
		/// <param name="instance">The instance.</param>
		public static void Fatal(string message, object instance)
		{
			Fatal(message, null, instance.GetType());
		}

		/// <summary>
		/// Warns the specified message.
		/// </summary>
		/// <param name="message">The message.</param>
		/// <param name="ownerType">Type of the owner.</param>
		public static void Fatal(string message, Type ownerType)
		{
			Fatal(message, null, ownerType);
		}

		/// <summary>
		/// Warns the specified message.
		/// </summary>
		/// <param name="message">The message.</param>
		/// <param name="exception">The exception.</param>
		/// <param name="instance">The instance.</param>
		public static void Fatal(string message, Exception exception, object instance)
		{
			Fatal(message, exception, instance.GetType());
		}

		/// <summary>
		/// Warns the specified message.
		/// </summary>
		/// <param name="message">The message.</param>
		/// <param name="exception">The exception.</param>
		/// <param name="ownerType">Type of the owner.</param>
		public static void Fatal(string message, Exception exception, Type ownerType)
		{
			LogMessage(message, exception, ownerType, LogLevel.Fatal);
		}

		/// <summary>
		/// Warns the specified message.
		/// </summary>
		/// <param name="message">The message.</param>
		/// <param name="instance">The instance.</param>
		public static void Debug(string message, object instance)
		{
			Debug(message, null, instance.GetType());
		}

		/// <summary>
		/// Warns the specified message.
		/// </summary>
		/// <param name="message">The message.</param>
		/// <param name="ownerType">Type of the owner.</param>
		public static void Debug(string message, Type ownerType)
		{
			Debug(message, null, ownerType);
		}

		/// <summary>
		/// Warns the specified message.
		/// </summary>
		/// <param name="message">The message.</param>
		/// <param name="exception">The exception.</param>
		/// <param name="instance">The instance.</param>
		public static void Debug(string message, Exception exception, object instance)
		{
			Debug(message, exception, instance.GetType());
		}

		/// <summary>
		/// Warns the specified message.
		/// </summary>
		/// <param name="message">The message.</param>
		/// <param name="exception">The exception.</param>
		/// <param name="ownerType">Type of the owner.</param>
		public static void Debug(string message, Exception exception, Type ownerType)
		{
			LogMessage(message, exception, ownerType, LogLevel.Debug);
		}

		/// <summary>
		/// Logs the message.
		/// </summary>
		/// <param name="message">The message.</param>
		/// <param name="ownerType">Type of the owner.</param>
		/// <param name="logNotificationLevel">The log notification level.</param>
		protected static void LogMessage(string message, Type ownerType, LogLevel logNotificationLevel)
		{
			LogMessage(message, null, ownerType, logNotificationLevel);
		}

		/// <summary>
		/// Logs the message.
		/// </summary>
		/// <param name="message">The message.</param>
		/// <param name="exception">The exception.</param>
		/// <param name="ownerType">Type of the owner.</param>
		/// <param name="logNotificationLevel">The log notification level.</param>
		protected static void LogMessage(string message, Exception exception, Type ownerType, LogLevel logNotificationLevel)
		{
			Assert.ArgumentNotNull(message, "message");
			Assert.ArgumentNotNull(ownerType, "ownerType");

			ILog logger = LogManager.GetLogger(ownerType);

			if (logger == null)
			{
				return;
			}

			ThreadContext.Properties["logLevelNumber"] = (int)Enum.Parse(typeof(LogLevel), Enum.GetName(typeof(LogLevel), logNotificationLevel));

			if (SetAdditionalLoggingInfo != null)
			{
				SetAdditionalLoggingInfo.Invoke();
			}

			if (exception != null)
			{
				switch (logNotificationLevel)
				{
					case LogLevel.Debug:
						logger.Debug(message, exception);
						break;
					case LogLevel.Info:
						logger.Info(message, exception);
						break;
					case LogLevel.Warning:
						logger.Warn(message, exception);
						break;
					case LogLevel.Error:
						logger.Error(message, exception);
						break;
					case LogLevel.Fatal:
						logger.Fatal(message, exception);
						break;
				}
			}
			else
			{
				switch (logNotificationLevel)
				{
					case LogLevel.Debug:
						logger.Debug(message);
						break;
					case LogLevel.Info:
						logger.Info(message);
						break;
					case LogLevel.Warning:
						logger.Warn(message);
						break;
					case LogLevel.Error:
						logger.Error(message);
						break;
					case LogLevel.Fatal:
						logger.Fatal(message);
						break;
				}
			}
		}
	}
}