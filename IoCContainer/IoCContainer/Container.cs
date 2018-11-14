using IoCContainer.Attributes;
using IoCContainer.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace IoCContainer
{
	/// <summary>
	/// 
	/// </summary>
	public class Container
	{
		private readonly Dictionary<Type, Type> types;

		public Container()
		{
			types = new Dictionary<Type, Type>();
		}

		/// <summary>
		/// Adds the type.
		/// </summary>
		/// <param name="type">The type.</param>
		public void AddType(Type type)
		{
			types.Add(type, type);
		}

		/// <summary>
		/// Adds the type.
		/// </summary>
		/// <param name="baseType">Base type.</param>
		/// <param name="type">The type.</param>
		public void AddType(Type baseType, Type type)
		{
			types.Add(baseType, type);
		}

		/// <summary>
		/// Read dependencies from assembly by attributes.
		/// </summary>
		/// <param name="assembly">The assembly.</param>	
		public void AddAssembly(Assembly assembly)
		{
			var listOfTypes = assembly.GetTypes();
			foreach (var type in listOfTypes)
			{
				var typeImportConstrAttr = type.GetCustomAttribute<ImportConstructorAttribute>();				

				if (typeImportConstrAttr != null || HasImportProperties(type))
				{
					types.Add(type, type);
				}

				var typeExportAttributes = type.GetCustomAttributes<ExportAttribute>();
				foreach (var exportAttribute in typeExportAttributes)
				{
					if (exportAttribute.Type != null)
					{
						types.Add(exportAttribute.Type, type);
					}
					else
					{
						types.Add(type, type);
					}
				}
			}
		}
		
		/// <summary>
		/// Creates an instance of a class that was previously registered with all dependencies.
		/// </summary>
		/// <param name="type">The type.</param>
		/// <returns></returns>
		public object CreateInstance(Type type)
		{
			return CreateInstanceWithDependencies(type);
		}

		/// <summary>
		/// Creates the instance.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <returns></returns>
		public T CreateInstance<T>()
		{
			return (T)CreateInstanceWithDependencies(typeof(T));
		}

		#region Private

		/// <summary>
		/// Determines whether the specified type has import properties.
		/// </summary>
		/// <param name="type">The type.</param>
		/// <returns>
		///   <c>true</c> if the specified type has import properties; otherwise, <c>false</c>.
		/// </returns>
		private bool HasImportProperties(Type type)
		{
			var typeImportPropAttributes = type.GetProperties().Where(x => x.GetCustomAttribute<ImportAttribute>() != null);
			return typeImportPropAttributes.Any();
		}

		/// <summary>
		/// Creates the instance with dependencies.
		/// </summary>
		/// <param name="type">The type.</param>
		/// <returns></returns>
		/// <exception cref="CustomContainerException">
		/// CreateInstance method throws an exception. Type {type.Name}
		/// or
		/// CreateInstance method throws an exception. Type {type.Name}
		/// </exception>
		private object CreateInstanceWithDependencies(Type type)
		{
			if (!types.ContainsKey(type))
			{
				throw new CustomContainerException($"Cannot create instance of {type.FullName}. Dependency is not provided");
			}

			var typeToGetInstance = types[type];
			ConstructorInfo constructorInfo = GetConstructor(typeToGetInstance);

			var instance = ResolveConstructor(typeToGetInstance, constructorInfo);
			if (type.GetCustomAttribute<ImportConstructorAttribute>() != null)
			{
				return instance;
			}

			ResolveProperties(type, instance);
			return instance;
		}

		/// <summary>
		/// Gets the constructor.
		/// </summary>
		/// <param name="type">The type.</param>
		/// <returns></returns>
		/// <exception cref="DIException">There are no public constructors for type {type.FullName}</exception>
		private ConstructorInfo GetConstructor(Type type)
		{
			ConstructorInfo[] constructors = type.GetConstructors();

			if (constructors.Length == 0)
			{
				throw new CustomContainerException($"There are no public constructors for type {type.FullName}");
			}

			return constructors.First();
		}

		/// <summary>
		/// Resolves the constructor.
		/// </summary>
		/// <param name="type">The type.</param>
		/// <param name="constrInfo">The constr information.</param>
		/// <returns></returns>
		private object ResolveConstructor(Type type, ConstructorInfo constrInfo)
		{
			var currectConstrParams = constrInfo.GetParameters();
			var resolvedParams = new List<object>();
			Array.ForEach(currectConstrParams, x => resolvedParams.Add(CreateInstanceWithDependencies(x.ParameterType)));
			var instance = Activator.CreateInstance(type, resolvedParams.ToArray());
			return instance;
		}

		/// <summary>
		/// Resolves the properties.
		/// </summary>
		/// <param name="type">The type.</param>
		/// <param name="instance">The instance.</param>
		private void ResolveProperties(Type type, object instance)
		{
			var propertiesToResolve = type.GetProperties().Where(x => x.GetCustomAttribute<ImportAttribute>() != null);
			foreach (var property in propertiesToResolve)
			{
				var resolvedProp = CreateInstanceWithDependencies(property.PropertyType);
				property.SetValue(instance, resolvedProp);
			}
		}
		
		#endregion
	}
}
