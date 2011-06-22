namespace IoCTestDomain.Logger
{
	public interface ILogger
	{
		string Name { get; set; }

		void Log(string message);
	}
}