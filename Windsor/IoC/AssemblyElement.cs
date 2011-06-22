using System.Configuration;

namespace IoC
{
	/// <summary>
	/// The assemble element of the configuration.
	/// </summary>
	public class AssemblyElement : ConfigurationElement
	{
		/// <summary>
		/// Gets the assembly.
		/// </summary>
		[ConfigurationProperty("assembly", IsRequired = true)]
		public string Assembly
		{
			get
			{
				return this["assembly"] as string;
			}
		}
	}
}
