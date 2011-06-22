using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using IoC;
using IoC.Lifetime;
using IoCTestDomain.Logger;
using IoCTestDomain.Processor;
using NUnit.Framework;

namespace IoCTest
{
	[TestFixture]
	public class IoCContainerTest
	{
		[SetUp]
		public void SetUp()
		{
			IoCContainer.InitializeContainer();
		}

		[TearDown]
		public void ResetContainer()
		{
			IoCContainer.ResetContainer();
		}

		[Test]
		public void InitializeContainerFromConfigTest()
		{
			Assert.IsNotNull(IoCContainer.ContainerCore);
		}

		[Test]
		public void InitializeContainerTest()
		{
			IoCContainer.ResetContainer();
			IoCContainer.InitializeContainer(new List<string> { Assembly.GetExecutingAssembly().GetName().FullName }, null);

			var service = IoCContainer.Resolve<ILogger>();
			Assert.IsNotNull(service);
		}

		[Test]
		public void ResolveTest()
		{
			var service = IoCContainer.Resolve<ILogger>();

			Assert.IsNotNull(service);
			Assert.IsTrue(service is Logger);
		}

		[TestCase("Implementation1")]
		[TestCase("Implementation1Child")]
		public void ResolveInvokeTest(string serviceName)
		{
			var service = IoCContainer.Resolve<IEntityProcessor>(serviceName);

			Assert.IsNotNull(service);
			service.Do("some data");
		}

		[TestCase("Implementation2")]
		public void ResolveConstructorInjectionTest(string serviceName)
		{
			var service = IoCContainer.Resolve<IEntityProcessor>(serviceName, new { constructorVariable = "constructor variable" });

			Assert.IsNotNull(service);
			service.Do("some data");
		}

		[Test]
		public void ResolveAllTest()
		{
			var services = IoCContainer.ResolveAll<IEntityProcessor>(new { constructorVariable = "constructor variable" });

			Assert.IsNotNull(services);
			Assert.IsTrue(services.Any());

			services.ToList().ForEach(s => s.Do("some data"));
		}

		[Test]
		public void ResolveDependecyTest()
		{
			var service = IoCContainer.Resolve<IEntityProcessor>("Implementation1");

			Assert.IsNotNull(service);
			Assert.That(service.Logger is Logger);
		}

		[Test]
		public void RegistereNamedDependecyTest()
		{
			IoCContainer.ResetContainer();

			IoCContainer.Register<ILogger, LoggerExtended>("LoggerExtended");

			var dependsOn = new Dictionary<string, string> { { "Logger", "LoggerExtended" } };
			IoCContainer.Register<IEntityProcessor, Implementation1>(dependsOn);

			var service = IoCContainer.Resolve<IEntityProcessor>();

			Assert.IsNotNull(service);
			Assert.That(service is Implementation1);
			Assert.That(service.Logger is LoggerExtended);
		}

		[Test]
		public void RegisterInsanceTest()
		{
			IoCContainer.ResetContainer();

			ILogger logger = new Logger { Name = "TestName100500" };
			IoCContainer.RegisterInstance(logger);

			var logger2 = IoCContainer.Resolve<ILogger>();
			Assert.That(logger2.Name == logger.Name);
		}

		[Test]
		public void RegisterConstructorParamsTest()
		{
			IoCContainer.ResetContainer();

			IoCContainer.Register<ILogger, LoggerExtended>();
			IoCContainer.Register<IEntityProcessor, Implementation2>(p =>
			{
				p["constructorVariable"] = "100500";
				p["logger"] = IoCContainer.Resolve<ILogger>();
			});

			var service = IoCContainer.Resolve<IEntityProcessor>();

			Assert.IsNotNull(service);
			Assert.That(service is Implementation2);
		}

		[Test]
		public void RegisterUsingFactoryMethodTest()
		{
			IoCContainer.ResetContainer();

			IoCContainer.Register<IEntityProcessor>(() => new Implementation2(Guid.NewGuid().ToString()), new Transient());

			var service = IoCContainer.Resolve<IEntityProcessor>();
			var service2 = IoCContainer.Resolve<IEntityProcessor>();
			Assert.That(service is Implementation2);
			Assert.That(service != service2);
		}

		[Test]
		public void ResetTest()
		{
			IoCContainer.ResetContainer();
			Assert.That(() => IoCContainer.Resolve<ILogger>(), Throws.InvalidOperationException);
		}

		[Test]
		public void ReleaseTest()
		{
			var service = IoCContainer.Resolve<ILogger>();
			IoCContainer.Release(service);
		}

		[Test]
		public void RemoveGenericFromBaseTypeTest()
		{
			IoCContainer.ResetContainer();

			IoCContainer.Register<ILogger, Logger>();

			IoCContainer.Remove<ILogger>();

			Assert.That(() => IoCContainer.Resolve<ILogger>(), Throws.InvalidOperationException);
		}

		[Test]
		public void RemoveGenericFromImplementationTypeTest()
		{
			IoCContainer.ResetContainer();

			IoCContainer.Register<ILogger, Logger>();

			IoCContainer.Remove<Logger>();

			Assert.That(() => IoCContainer.Resolve<ILogger>(), Throws.InvalidOperationException);
		}

		[Test]
		public void CollectionNullTest()
		{
			var collection = Enumerable.Range(0, 100).Select(x => new object());

			foreach (var element in collection)
			{
				Assert.IsNotNull(element);
			}
		}

		[Test]
		public void RemoveGenericFromInvaImplementationTypeTest()
		{
			IoCContainer.ResetContainer();

			IoCContainer.Register<ILogger, Logger>();

			IoCContainer.Remove<LoggerExtended>();

			Assert.That(IoCContainer.Resolve<ILogger>() is Logger);
		}

		[TestCase(typeof(Logger))]
		[TestCase(typeof(ILogger))]
		public void RemoveTest(Type type)
		{
			IoCContainer.ResetContainer();

			IoCContainer.Register<ILogger, Logger>();

			IoCContainer.Remove(type);

			Assert.That(() => IoCContainer.Resolve<ILogger>(), Throws.InvalidOperationException);
		}

		[Test]
		public void RemoveIsRegisterdTypeTest()
		{
			IoCContainer.ResetContainer();

			IoCContainer.Register<ILogger, Logger>();

			IoCContainer.Remove(typeof(ILogger));

			Assert.IsFalse(IoCContainer.IsRegistered<ILogger>());
		}

		[Test]
		public void IsRegisteredWithKeyTrueTest()
		{
			IoCContainer.ResetContainer();

			IoCContainer.Register<ILogger, Logger>("logger");

			Assert.That(IoCContainer.IsRegistered<ILogger>("logger"));
		}

		[Test]
		public void IsRegisteredWithKeyFalseTest()
		{
			IoCContainer.ResetContainer();

			IoCContainer.Register<ILogger, Logger>("logger");

			Assert.IsFalse(IoCContainer.IsRegistered<ILogger>("logger2"));
		}

		[Test]
		public void IsRegisteredWithMultiKeyFalseTest()
		{
			IoCContainer.ResetContainer();

			IoCContainer.Register<ILogger, Logger>("logger");
			IoCContainer.Register<ILogger, LoggerExtended>("loggerextended");

			Assert.That(IoCContainer.Resolve<ILogger>("logger") is Logger);
			Assert.That(IoCContainer.Resolve<ILogger>("loggerextended") is LoggerExtended);
		}

		[Test]
		public void IsRegisteredTrueTest()
		{
			IoCContainer.ResetContainer();

			IoCContainer.Register<ILogger, Logger>();

			Assert.That(IoCContainer.IsRegistered<ILogger>());
		}

		[Test]
		public void IsRegisteredFalseTest()
		{
			IoCContainer.ResetContainer();

			Assert.That(!IoCContainer.IsRegistered<ILogger>());
		}

		[Test]
		public void IsRegisteredTypeTrueTest()
		{
			IoCContainer.ResetContainer();

			Assert.That(!IoCContainer.IsRegistered(typeof(ILogger)));
		}
	}
}
