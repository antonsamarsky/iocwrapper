using System;
using Castle.MicroKernel.Lifestyle;

namespace IoC.Lifetime
{
	/// <summary>
	/// The Poolable.
	/// </summary>
	public class Pool : ILifeTimeDefinition
	{
		public int InitialSize { get; private set; }

		public int MaxSize { get; private set; }

		/// <summary>
		/// Initializes a new instance of the <see cref="Pool"/> class.
		/// </summary>
		/// <param name="initialSize">The initial size.</param>
		/// <param name="maxSize">Size of the max.</param>
		public Pool(int initialSize, int maxSize)
		{
			this.InitialSize = initialSize;
			this.MaxSize = maxSize;
		}

		public Type LifetimeManager
		{
			get { return typeof(PoolableLifestyleManager); }
		}
	}
}