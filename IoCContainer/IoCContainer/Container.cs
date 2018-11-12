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
		/// <param name="baseType">Type of the base.</param>
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
				var typeImportPropAttr = type.GetProperties().Where(x => x.GetCustomAttribute<ImportAttribute>() != null);

				if (typeImportConstrAttr != null || typeImportPropAttr.Count() > 0)
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
				throw new CustomContainerException($"CreateInstance method throws an exception. Type {type.Name} is not registered.");
			}

			var typeToGetInstance = types[type];
			var constrOfType = typeToGetInstance.GetConstructors();
			if (constrOfType.Count() == 0)
			{
				throw new CustomContainerException($"CreateInstance method throws an exception. Type {type.Name} doesn't have a public constructor.");
			}

			var currectConstr = constrOfType.First();
			var instance = ResolveConstructor(typeToGetInstance, currectConstr);
			if (type.GetCustomAttribute<ImportConstructorAttribute>() != null)
			{
				return instance;
			}

			ResolveProperties(type, instance);
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

		#endregion
	}
}
