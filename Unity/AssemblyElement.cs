using System.Configuration;

namespace IoC
{
    public class AssemblyElement : ConfigurationElement
    {
        [ConfigurationProperty("assembly", IsRequired = true)]
        public string Assembly { get { return this["assembly"] as string; } }
    }
}
