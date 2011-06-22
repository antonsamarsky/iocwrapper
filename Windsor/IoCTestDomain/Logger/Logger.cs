using System;

namespace IoCTestDomain.Logger
{
	public class Logger : ILogger
	{
		private string name;

		public Logger()
		{
			this.name = Guid.NewGuid().ToString();
			Console.WriteLine(this.name);
		}

		public string Name
		{
			get { return this.name; }
			set { this.name = value; }
		}

		public void Log(string message)
		{
			Console.WriteLine(this.Name + ": " + message);
		}
	}
}