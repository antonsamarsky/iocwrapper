using System;
using System.Web;
using Castle.MicroKernel.Context;
using Castle.MicroKernel.Lifestyle;

namespace IoC.Lifetime
{
	/// <summary>
	/// Tbe per session lifetime manager for the Castle Windsor.
	/// </summary>
	public class PerSessionManager : AbstractLifestyleManager
	{
		/// <summary>
		/// The object id in the session.
		/// </summary>
		private readonly string perRequestObjectId = string.Format("PerSession_{0}", Guid.NewGuid().ToString());

		/// <summary>
		/// Resolves the specified context.
		/// </summary>
		/// <param name="context">The context.</param>
		/// <returns>The object instance.</returns>
		public override object Resolve(CreationContext context)
		{
			if (HttpContext.Current == null || HttpContext.Current.Session == null)
			{
				return base.Resolve(context);
			}

			return HttpContext.Current.Session[this.perRequestObjectId] ?? (HttpContext.Current.Session[this.perRequestObjectId] = base.Resolve(context));
		}

		/// <summary>
		/// Releases unmanaged and - optionally - managed resources
		/// </summary>
		public override void Dispose()
		{
		}
	}
}