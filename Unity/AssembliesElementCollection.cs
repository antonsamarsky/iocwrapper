﻿using System.Configuration;

namespace IoC
{
    public class AssembliesElementCollection : ConfigurationElementCollection
    {
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

        protected override ConfigurationElement CreateNewElement()
        {
            return new AssemblyElement();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((AssemblyElement)element).Assembly;
        }
    }
}
