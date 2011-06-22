using System;

namespace IoC.Lifetime
{
	public class PerSession : ILifeTimeDefinition
	{
		public Type LifetimeManager
		{
			get { return typeof(PerSessionManager); }
		}
	}
}