using System;

namespace IoC
{
	public class ReflectionHelper
	{
		/// <summary>
		/// Helper function used to create an instance of the specified type
		/// using the default constructor
		/// </summary>
		/// <param name="type">The type's full name</param>
		/// <param name="useCache">If true the reflected information is cached in the default cache</param>
		/// <returns>An instance of the specified type using the defautl constructor</returns>
		public static bool TryCreateTypeFromDefaultConstructor<T>(Type type, out T obj) where T : class
		{
			bool retval = false;

			try
			{
				obj = CreateTypeFromDefaultConstructor(type, false, null) as T;

				if (null != obj) // might have created it but not as'd
					retval = true;
			}
			catch (Exception)
			{
				obj = null;
			}

			return retval;
		}

		/// <summary>
		/// Helper function used to create an instance of the specified type
		/// using the default constructor
		/// </summary>
		/// <param name="type">The type's full name</param>
		/// <param name="useCache">If true the reflected information is cached in the default cache</param>
		/// <returns>An instance of the specified type using the defautl constructor</returns>
		public static bool TryCreateTypeFromDefaultConstructor<T>(string type, out T obj) where T : class
		{
			bool retval = false;

			try
			{
				obj = CreateTypeFromDefaultConstructor(type, false, null) as T;

				if (null != obj) // might have created it but not as'd
					retval = true;
			}
			catch (Exception)
			{
				obj = null;
			}

			return retval;
		}

		/// <summary>
		/// Helper function used to create an instance of the specified type
		/// using the default constructor
		/// </summary>
		/// <param name="type">The type's full name</param>
		/// <param name="useCache">If true the reflected information is cached in the default cache</param>
		/// <returns>An instance of the specified type using the defautl constructor</returns>
		public static bool TryCreateTypeFromDefaultConstructor(string type, out object obj)
		{
			bool retval = false;

			try
			{
				obj = CreateTypeFromDefaultConstructor(type, false, null);
				retval = true;
			}
			catch (Exception)
			{
				obj = null;
			}

			return retval;
		}

		/// <summary>
		/// Helper function used to create an instance of the specified type
		/// using the default constructor
		/// </summary>
		/// <param name="type">The type's full name</param>
		/// <param name="useCache">If true the reflected information is cached in the default cache</param>
		/// <returns>An instance of the specified type using the defautl constructor</returns>
		public static bool TryCreateTypeFromDefaultConstructor(Type type, out object obj)
		{
			bool retval = false;

			try
			{
				obj = CreateTypeFromDefaultConstructor(type, false, null);
				retval = true;
			}
			catch (Exception)
			{
				obj = null;
			}

			return retval;
		}

		/// <summary>
		/// Helper function used to create an instance of the specified type
		/// using the default constructor
		/// </summary>
		/// <param name="type">The type's full name</param>
		/// <param name="useCache">If true the reflected information is cached in the default cache</param>
		/// <returns>An instance of the specified type using the defautl constructor</returns>
		public static object CreateTypeFromDefaultConstructor(string type)
		{
			object retval = CreateTypeFromDefaultConstructor(type, false, null);
			return retval;
		}

		/// <summary>
		/// Helper function used to create an instance of the specified type
		/// using the default constructor
		/// </summary>
		/// <param name="type">The type's full name</param>
		/// <param name="useCache">If true the reflected information is cached in the default cache</param>
		/// <returns>An instance of the specified type using the defautl constructor</returns>
		public static object CreateTypeFromDefaultConstructor(string type, bool useCache)
		{
			object retval = CreateTypeFromDefaultConstructor(type, useCache, null);
			return retval;
		}

		/// <summary>
		/// Helper function used to create an instance of the specified type
		/// using the default constructor
		/// </summary>
		/// <param name="type">The type's full name</param>
		/// <param name="useCache">If true the reflected information is cached</param>
		/// <param name="cacheName">The name of the cache (nnull for the default cache)</param>
		/// <returns>An instance of the specified type using the defautl constructor</returns>
		public static object CreateTypeFromDefaultConstructor(string type, bool useCache, string cacheName)
		{
			object retval;
			// the target type should be in the GAC or local directory
			Type targetType = Type.GetType(type);
			retval = targetType.GetConstructor(new Type[0]).Invoke(null);
			return retval;
		}

		/// using the default constructor
		/// </summary>
		/// <param name="type">The type's full name</param>
		/// <param name="useCache">If true the reflected information is cached</param>
		/// <param name="cacheName">The name of the cache (nnull for the default cache)</param>
		/// <returns>An instance of the specified type using the defautl constructor</returns>
		public static object CreateTypeFromDefaultConstructor(Type type, bool useCache, string cacheName)
		{
			object retval;
			// the target type should be in the GAC or local directory
			retval = type.GetConstructor(new Type[0]).Invoke(null);
			return retval;
		}

		public static object CreateTypeFromConstructor(string type, bool useCache, Type[] signature, object[] values)
		{
			object retval;
			// the target type should be in the GAC or local directory
			Type targetType = Type.GetType(type);
			retval = targetType.GetConstructor(signature).Invoke(values);
			return retval;
		}

		public static object CreateTypeFromConstructor(Type type, bool useCache, Type[] signature, object[] values)
		{
			object retval;
			// the target type should be in the GAC or local directory            
			retval = type.GetConstructor(signature).Invoke(values);
			return retval;
		}
	}
}