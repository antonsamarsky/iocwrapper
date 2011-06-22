using System;

namespace IoCTestDomain.Logger
{
	public class LoggerExtended: ILogger
	{
		public string Name
		{
			get { return "LoggerExtended"; }
			set {}
		}

		public void Log(string message)
		{
			Console.WriteLine(this.Name + ": " + message);
		}
		
	}
}