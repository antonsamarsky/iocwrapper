using System.Configuration;

namespace IoC
{
	/// <summary>
	/// The configuration assemblies elements.
	/// </summary>
	public class AssembliesElementCollection : ConfigurationElementCollection
	{
		/// <summary>
		/// Gets or sets a property, attribute, or child element of this configuration element.
		/// </summary>
		/// <param name="index">The parameter index</param>
		/// <returns>The specified property, attribute, or child element</returns>
		/// <exception cref="T:System.Configuration.ConfigurationErrorsException"></exception>
		public AssemblyElement this[int index]
		{
			get
			{
				return BaseGet(index) as AssemblyElement;
			}

			set
			{
				if (BaseGet(index) != null)
				{
					BaseRemoveAt(index);
				}

				BaseAdd(index, value);
			}
		}

		/// <summary>
		/// When overridden in a derived class, creates a new <see cref="T:System.Configuration.ConfigurationElement"/>.
		/// </summary>
		/// <returns>
		/// A new <see cref="T:System.Configuration.ConfigurationElement"/>.
		/// </returns>
		protected override ConfigurationElement CreateNewElement()
		{
			return new AssemblyElement();
		}

		/// <summary>
		/// Gets the element key for a specified configuration element when overridden in a derived class.
		/// </summary>
		/// <param name="element">The <see cref="T:System.Configuration.ConfigurationElement"/> to return the key for.</param>
		/// <returns>
		/// An <see cref="T:System.Object"/> that acts as the key for the specified <see cref="T:System.Configuration.ConfigurationElement"/>.
		/// </returns>
		protected override object GetElementKey(ConfigurationElement element)
		{
			return ((AssemblyElement)element).Assembly;
		}
	}
}
