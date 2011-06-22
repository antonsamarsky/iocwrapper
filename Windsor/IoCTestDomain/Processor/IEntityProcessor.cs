using System.Collections.Generic;
using IoCTestDomain.Logger;
using IoCTestDomain.Reporting;

namespace IoCTestDomain.Processor
{
	public interface IEntityProcessor
	{
		ILogger Logger { get; set; }

		string ClassName { get; }

		void Do(string someData);

		void Do<T>(T someEntity) where T: Report;

		IEnumerable<T> Collect<T>(int numberOfElements) where T : Report;
	}
}
