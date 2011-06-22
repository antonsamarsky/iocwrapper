using System.Configuration;

namespace IoC
{
	/// <summary>
	/// The assembly registration.
	/// </summary>
	public sealed class IoCAssemblyRegistration : ConfigurationSection
	{
		/// <summary>
		/// The configuration section name.
		/// </summary>
		public const string ConfigurationSectionName = "iocAssemblyConfiguration";

		/// <summary>
		/// Gets the assemblies.
		/// </summary>
		[ConfigurationProperty("assemblies")]
		public AssembliesElementCollection Assemblies
		{
			get
			{
				return (AssembliesElementCollection)this["assemblies"] ?? new AssembliesElementCollection();
			}
		}

		/// <summary>
		/// Returns an configuration instance
		/// </summary>
		/// <returns>
		/// The get configuration.
		/// </returns>
		public static IoCAssemblyRegistration GetConfig()
		{
			return (IoCAssemblyRegistration)ConfigurationManager.GetSection(ConfigurationSectionName) ?? new IoCAssemblyRegistration();
		}
	}
}
