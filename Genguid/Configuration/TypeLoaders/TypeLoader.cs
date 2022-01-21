using System;

namespace Genguid.Configuration.TypeLoaders
{
	/// <summary>
	/// The default loader for creating instances from their corresponding types.
	/// </summary>
	public class TypeLoader<T>
	{
		/// <summary>
		/// Creates a new instance of the specified type, where the type represents a type that
		/// implements T.
		/// </summary>
		/// <param name="type">
		/// An object representing a type which implements T.
		/// </param>
		/// <exception cref="Genguid.Configuration.TypeLoaders.ConfigurationTypeLoadException"></exception>
		public virtual T Load(Type type)
		{
			if (type == null)
			{
				throw new ArgumentNullException(nameof(type), "Argument cannot be null.");
			}

			try
			{
				return (T)Activator.CreateInstance(type)!;
			}
			catch (Exception e) when (
				e is System.ArgumentNullException ||
				e is System.ArgumentException ||
				e is System.NotSupportedException ||
				e is System.Reflection.TargetInvocationException ||
				e is System.MethodAccessException ||
				e is System.MemberAccessException ||
				e is System.Runtime.InteropServices.InvalidComObjectException ||
				e is System.MissingMethodException ||
				e is System.Runtime.InteropServices.COMException ||
				e is System.TypeLoadException)
			{
				throw new ConfigurationTypeLoadException("", e);
			}
		}
	}
}
