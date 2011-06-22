using System;
using IoCTestDomain.Logger;

namespace IoCTestDomain.Processor
{
	public class Implementation1Child : Implementation1
	{
		public Implementation1Child(ILogger logger) : base(logger)
		{
		}

		public override string ClassName
		{
			get { return "Implementation1Child"; }
		}

		public override void Do(string someData)
		{
			Console.WriteLine(ClassName + " >> " + someData);
			base.Do(someData);
		}

		public override void Do<T>(T someEntity)
		{
			Console.WriteLine(ClassName + ">> ID: " + someEntity.Id + "; Name" + someEntity.Name);
			base.Do(someEntity);
		}

	}
}