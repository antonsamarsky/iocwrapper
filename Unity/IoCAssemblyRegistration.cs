using System.Configuration;

namespace IoC
{
    public sealed class IoCAssemblyRegistration : ConfigurationSection
    {
        public const string ConfigurationSectionName = "iocAssemblyConfiguration";

        /// <summary>
        /// Returns an configuration instance
        /// </summary>
        public static IoCAssemblyRegistration GetConfig()
        {

            return (IoCAssemblyRegistration)ConfigurationManager.
               GetSection(ConfigurationSectionName) ??
               new IoCAssemblyRegistration();

        }

        [ConfigurationProperty("assemblies")]
        public AssembliesElementCollection Assemblies
        {
            get
            {
                return (AssembliesElementCollection)this["assemblies"] ??
                   new AssembliesElementCollection();
            }
        }

    }
}
