using System;
using Castle.MicroKernel.Lifestyle;

namespace IoC.Lifetime
{
	/// <summary>
	/// The singletone.
	/// </summary>
	public class Singleton : ILifeTimeDefinition
	{
		public Type LifetimeManager
		{
			get { return typeof(SingletonLifestyleManager); }
		}
	}
}