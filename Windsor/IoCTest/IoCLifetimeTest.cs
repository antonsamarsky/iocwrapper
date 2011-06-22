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
	public class IoCLifetimeTest
	{
		[SetUp]
		public void SetUp()
		{
			IoCContainer.ResetContainer();
		}

		[Test]
		public void TransientTest()
		{
			IoCContainer.Register<ILogger, Logger>(new Transient());

			var logger1 = IoCContainer.Resolve<ILogger>();
			string result1 = logger1.Name;

			Console.WriteLine(result1);
			IoCContainer.Release(logger1);

			var logger2 = IoCContainer.Resolve<ILogger>();
			string result2 = logger2.Name;

			Console.WriteLine(result2);

			Assert.AreNotEqual(result1, result2);
		}

		[Test]
		public void SingletonTest()
		{
			IoCContainer.Register<ILogger, Logger>(new Singleton());

			var logger1 = IoCContainer.Resolve<ILogger>();
			string result1 = logger1.Name;

			Console.WriteLine(result1);
			IoCContainer.Release(logger1);

			var logger2 = IoCContainer.Resolve<ILogger>();
			string result2 = logger2.Name;

			Console.WriteLine(result2);

			Assert.AreEqual(result1, result2);
		}

		[TestCase(10)]
		public void TransientThreadTest(int numberOfThreads)
		{
			IoCContainer.Register<ILogger, Logger>(new Transient());

			var tasks = new List<Task<string>>();
			for (int i = 0; i < numberOfThreads; i++)
			{
				var task = Task<string>.Factory.StartNew(() =>
				{
					var logger1 = IoCContainer.Resolve<ILogger>();
					var result1 = logger1.Name;
					IoCContainer.Release(logger1);

					var logger2 = IoCContainer.Resolve<ILogger>();
					var result2 = logger2.Name;
					IoCContainer.Release(logger1);

					Assert.AreNotEqual(result1, result2);

					return result1;
				});
				tasks.Add(task);
			}

			var results = tasks.Select(t => t.Result).ToList();
			results.ForEach(Console.WriteLine);
			var duplicates = results.GroupBy(i => i).Where(g => g.Count() > 1).Select(g => g.Key);

			Assert.That(!duplicates.Any());
		}

		[TestCase(10)]
		public void SingletonThreadTest(int numberOfThreads)
		{
			IoCContainer.Register<ILogger, Logger>(new Singleton());

			var tasks = new List<Task<string>>();
			for (int i = 0; i < numberOfThreads; i++)
			{
				var task = Task<string>.Factory.StartNew(() =>
				{
					var logger1 = IoCContainer.Resolve<ILogger>();
					var result1 = logger1.Name;
					IoCContainer.Release(logger1);

					var logger2 = IoCContainer.Resolve<ILogger>();
					var result2 = logger2.Name;
					IoCContainer.Release(logger1);

					Assert.AreEqual(result1, result2);

					return result1;
				});
				tasks.Add(task);
			}

			var results = tasks.Select(t => t.Result).ToList();
			results.ForEach(Console.WriteLine);
			var duplicates = results.GroupBy(i => i).Where(g => g.Count() > 1).Select(g => g.Key);

			Assert.That(duplicates.Count() == 1);
		}

		[TestCase(4)]
		public void PerThreadTest(int numberOfThreads)
		{
			IoCContainer.Register<ILogger, Logger>(new PerThread());

			var tasks = new List<Task<string>>();

			for (int i = 0; i < numberOfThreads; i++)
			{
				var task = Task<string>.Factory.StartNew(() =>
				{
					var logger1 = IoCContainer.Resolve<ILogger>();
					var result1 = logger1.Name;
					IoCContainer.Release(logger1);

					var logger2 = IoCContainer.Resolve<ILogger>();
					var result2 = logger2.Name;
					IoCContainer.Release(logger1);

					Assert.AreEqual(result1, result2);

					return result1;
				});
				tasks.Add(task);
			}

			var results = tasks.Select(t => t.Result).ToList();
			results.ForEach(Console.WriteLine);
			var duplicates = results.GroupBy(i => i).Where(g => g.Count() > 1).Select(g => g.Key);

			Assert.That(!duplicates.Any());
		}

		[TestCase(2,3)]
		public void PoolTest(int initialSize, int maxSize)
		{
			IoCContainer.Register<ILogger, Logger>(new Pool(initialSize, maxSize));
			List<string> loggers = new List<string>();

			for (int i = 0; i < maxSize * 2; i++)
			{
				var logger = IoCContainer.Resolve<ILogger>();
				loggers.Add(logger.Name);
			}

			Console.WriteLine(loggers);
		}
	}
}