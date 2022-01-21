using Genguid.Formatters;

namespace Genguid.Configuration.TypeLoaders
{
	/// <summary>
	/// The default loader for creating GUID formatter instances from their corresponding types.
	/// This class cannot be inherited.
	/// </summary>
	public sealed class FormatterTypeLoader : IFormatterTypeLoader
	{
		private static readonly object instanceLock = new();
		private static IFormatterTypeLoader instance;

		private readonly TypeLoader<GuidFormatter> baseTypeLoader;

		private FormatterTypeLoader(TypeLoader<GuidFormatter> baseTypeLoader)
		{
			this.baseTypeLoader = baseTypeLoader;
		}

		static FormatterTypeLoader()
		{
			instance = new FormatterTypeLoader(new TypeLoader<GuidFormatter>());
		}

		/// <summary>
		/// Gets the current formatter type loader for the app.
		/// </summary>
		public static IFormatterTypeLoader Current
		{
			get
			{
				return instance;
			}
		}

		/// <summary>
		/// Changes the current formatter type loader for the app to the one specified.
		/// </summary>
		/// <param name="typeLoader">The new formatter type loader.</param>
		/// <exception cref="System.ArgumentNullException"></exception>
		public static void SetCurrent(IFormatterTypeLoader typeLoader)
		{
			lock (instanceLock)
			{
				instance = typeLoader ?? throw new ArgumentNullException(nameof(typeLoader), "Argument cannot be null."); ;
			}
		}

		/// <summary>
		/// Creates a new instance of a GUID formatter of the specified type.
		/// </summary>
		/// <param name="formatterType">
		/// The GUID formatter type, which must represent a type that implements the
		/// Genguid.Formatters.GuidFormatterDecorator abstract class.
		/// </param>
		/// <exception cref="System.ArgumentNullException"></exception>
		/// <exception cref="System.ArgumentException"></exception>
		/// <exception cref="System.NotSupportedException"></exception>
		/// <exception cref="System.Reflection.TargetInvocationException"></exception>
		/// <exception cref="System.MethodAccessException"></exception>
		/// <exception cref="System.MemberAccessException"></exception>
		/// <exception cref="System.Runtime.InteropServices.InvalidComObjectException"></exception>
		/// <exception cref="System.MissingMethodException"></exception>
		/// <exception cref="System.Runtime.InteropServices.COMException"></exception>
		/// <exception cref="System.TypeLoadException"></exception>
		public GuidFormatter Load(Type[] formatterTypes)
		{
			if (formatterTypes is null)
			{
				throw new ArgumentNullException(nameof(formatterTypes), "Argument cannot be null.");
			}			

			if (formatterTypes.Length == 0)
			{
				throw new ArgumentException("Array must not be empty.", nameof(formatterTypes));
			}

			// The overall, decorated formatter
			GuidFormatter formatter;

			// Instantiate the base formatter
			Type baseFormatterType = formatterTypes[0];			

			if (baseFormatterType.IsSubclassOf(typeof(GuidFormatter)) && !baseFormatterType.IsSubclassOf(typeof(GuidFormatterDecorator)))
			{
				formatter = baseTypeLoader.Load(baseFormatterType);
			}
			else
			{
				throw new TypeLoadException(
					String.Format(
						"Base formatter type {0} must implement the abstract class {1} and must not implement the abstract class {2}.",
						baseFormatterType.FullName,
						typeof(GuidFormatter).FullName,
						typeof(GuidFormatterDecorator).FullName
					)
				);
			}

			// Instantiate decorator formatters
			for (int i = 1; i < formatterTypes.Length; i++)
			{
				Type type = formatterTypes[i];

				if (type.IsSubclassOf(typeof(GuidFormatterDecorator)))
				{
					formatter = (GuidFormatterDecorator)Activator.CreateInstance(type, formatter)!;
				}
				else
				{
					throw new TypeLoadException(
						String.Format(
							"Decorator formatter type {0} must implement the abstract class {1}.",
							type.FullName,
							typeof(GuidFormatterDecorator).FullName
						)
					);
				}
			}

			return formatter!;
		}
	}
}
