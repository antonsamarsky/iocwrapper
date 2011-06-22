using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IoC;
using IoC.Lifetime;
using IoCTestDomain.Logger;
using NUnit.Framework;

namespace IoCTest
{
	[TestFixture]
	public class IoCConcurrencyTest
	{
		[SetUp]
		public void SetUp()
		{
			IoCContainer.ResetContainer();
		}

		[TestCase(10000)]
		public void ConcurrenRegisterResolveTest(int numberOfThreads)
		{
			IoCContainer.Register<ILogger, Logger>();
			var tasks = new List<Task<string>>();
			for (int i = 0; i < numberOfThreads; i++)
			{
				var task = Task<string>.Factory.StartNew(() =>
				{
					IoCContainer.Resolve<ILogger>();

					var key = Guid.NewGuid().ToString();
					IoCContainer.Register<ILogger, Logger>(key, new Transient());

					var logger = IoCContainer.Resolve<ILogger>(key);
					var result = logger.Name;

					Task.Factory.StartNew(() => IoCContainer.Resolve<ILogger>());
					
					return result;
				});
				tasks.Add(task);
			}

			var results = tasks.AsParallel().Select(t => t.Result).ToList();
		}
	}
}