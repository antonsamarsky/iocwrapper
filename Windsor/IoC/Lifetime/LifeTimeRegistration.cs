using Castle.MicroKernel.Registration;
using Castle.MicroKernel.Registration.Lifestyle;

namespace IoC.Lifetime
{
	public static class LifeTimeRegistration
	{
		public static ComponentRegistration<S> GetManager<S, T>(ComponentRegistration<S> componentRegistration,T definition) where T : ILifeTimeDefinition
		{
			if (definition is Pool)
			{
				var pool = definition as Pool;
				return new LifestyleGroup<S>(componentRegistration).PooledWithSize(pool.InitialSize, pool.MaxSize);
			}

			return new LifestyleGroup<S>(componentRegistration).Custom(definition.LifetimeManager);
		}
	}
}