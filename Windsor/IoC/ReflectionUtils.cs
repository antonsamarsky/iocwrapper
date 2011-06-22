using System;

namespace IoC
{
	/// <summary>
	/// The reflection helper fo IoC Container.
	/// </summary>
	public static class ReflectionUtils
	{
		/// <summary>
		/// Helper function used to create an instance of the specified type
		/// using the default constructor
		/// </summary>
		/// <typeparam name="T">the output object type.</typeparam>
		/// <param name="type">The type's full name</param>
		/// <param name="object">The @object.</param>
		/// <returns>
		/// An instance of the specified type using the defautl constructor
		/// </returns>
		public static bool TryCreateTypeFromDefaultConstructor<T>(Type type, out T @object) where T : class
		{
			bool retval = false;

			try
			{
				@object = CreateTypeFromDefaultConstructor(type, false, null) as T;

				if (@object != null)
				{
					// NOTE: It might have created it but not as'd
					retval = true;
				}
			}
			catch (Exception)
			{
				@object = null;
			}

			return retval;
		}

		/// <summary>
		/// Helper function used to create an instance of the specified type
		/// using the default constructor
		/// </summary>
		/// <typeparam name="T">The ouptu object type.</typeparam>
		/// <param name="type">The type's full name</param>
		/// <param name="object">The @object.</param>
		/// <returns>
		/// An instance of the specified type using the defautl constructor
		/// </returns>
		public static bool TryCreateTypeFromDefaultConstructor<T>(string type, out T @object) where T : class
		{
			bool retval = false;

			try
			{
				@object = CreateTypeFromDefaultConstructor(type, false, null) as T;

				if (@object != null)
				{
					// NOTE: It might have created it but not as'd might have created it but not as'd
					retval = true;
				}
			}
			catch (Exception)
			{
				@object = null;
			}

			return retval;
		}

		/// <summary>
		/// Helper function used to create an instance of the specified type
		/// using the default constructor
		/// </summary>
		/// <param name="type">The type's full name</param>
		/// <param name="object">The @object.</param>
		/// <returns>
		/// An instance of the specified type using the defautl constructor
		/// </returns>
		public static bool TryCreateTypeFromDefaultConstructor(string type, out object @object)
		{
			bool retval = false;

			try
			{
				@object = CreateTypeFromDefaultConstructor(type, false, null);
				retval = true;
			}
			catch (Exception)
			{
				@object = null;
			}

			return retval;
		}

		/// <summary>
		/// Helper function used to create an instance of the specified type
		/// using the default constructor
		/// </summary>
		/// <param name="type">The type's full name</param>
		/// <param name="object">The @object.</param>
		/// <returns>
		/// An instance of the specified type using the defautl constructor
		/// </returns>
		public static bool TryCreateTypeFromDefaultConstructor(Type type, out object @object)
		{
			bool retval = false;

			try
			{
				@object = CreateTypeFromDefaultConstructor(type, false, null);
				retval = true;
			}
			catch (Exception)
			{
				@object = null;
			}

			return retval;
		}

		/// <summary>
		/// Helper function used to create an instance of the specified type
		/// using the default constructor
		/// </summary>
		/// <param name="type">The type's full name</param>
		/// <returns>
		/// An instance of the specified type using the defautl constructor
		/// </returns>
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
			// NOTE: the target type should be in the GAC or local directory
			Type targetType = Type.GetType(type);
			object retval = targetType.GetConstructor(new Type[0]).Invoke(null);
			return retval;
		}

		/// <summary>
		/// using the default constructor
		/// </summary>
		/// <param name="type">The type's full name</param>
		/// <param name="useCache">If true the reflected information is cached</param>
		/// <param name="cacheName">The name of the cache (nnull for the default cache)</param>
		/// <returns>
		/// An instance of the specified type using the defautl constructor
		/// </returns>
		public static object CreateTypeFromDefaultConstructor(Type type, bool useCache, string cacheName)
		{
			// NOTE: the target type should be in the GAC or local directory
			object retval = type.GetConstructor(new Type[0]).Invoke(null);
			return retval;
		}

		/// <summary>
		/// Creates the type from constructor.
		/// </summary>
		/// <param name="type">The type.</param>
		/// <param name="useCache">if set to <c>true</c> [use cache].</param>
		/// <param name="signature">The signature.</param>
		/// <param name="values">The values.</param>
		/// <returns>
		/// An instance of the specified type using the default constructor.
		/// </returns>
		public static object CreateTypeFromConstructor(string type, bool useCache, Type[] signature, object[] values)
		{
			// NOTE: the target type should be in the GAC or local directory
			Type targetType = Type.GetType(type);
			object retval = targetType.GetConstructor(signature).Invoke(values);
			return retval;
		}

		/// <summary>
		/// Creates the type from constructor.
		/// </summary>
		/// <param name="type">The type.</param>
		/// <param name="useCache">if set to <c>true</c> [use cache].</param>
		/// <param name="signature">The signature.</param>
		/// <param name="values">The values.</param>
		/// <returns>
		/// An instance of the specified type using the default constructor.
		/// </returns>
		public static object CreateTypeFromConstructor(Type type, bool useCache, Type[] signature, object[] values)
		{
			// NOTE: the target type should be in the GAC or local directory            
			object retval = type.GetConstructor(signature).Invoke(values);
			return retval;
		}
	}
}