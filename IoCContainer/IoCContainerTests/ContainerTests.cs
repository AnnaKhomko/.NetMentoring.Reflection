using System;
using System.Reflection;
using IoCContainer;
using IoCContainerTests.TestData;
using IoCContainerTests.TestData.Interfaces;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace IoCContainerTests
{
	[TestClass]
	public class ContainerTests
	{
		private Container container;

		[TestInitialize]
		public void ContainerTestsInitialize()
		{
			container = new Container();
		}

		[TestMethod]
		public void CreateInstance_UsingAssemblyAttributes_ConstructorDependencies()
		{
			container.AddAssembly(Assembly.GetExecutingAssembly());

			var customerConstr = container.CreateInstance(typeof(CustomerBLL));
			var customerConstrGeneric = container.CreateInstance<CustomerBLL>();

			Assert.IsNotNull(customerConstr, "Customer instance was not created.");
			Assert.IsNotNull(customerConstrGeneric, "Customer instance was not created using generic method.");
			Assert.IsTrue(customerConstr.GetType() == typeof(CustomerBLL), "Wrong type returned by CreateInstance method.");
			Assert.IsTrue(customerConstrGeneric.GetType() == typeof(CustomerBLL), "Wrong type returned by CreateInstance generic method.");
		}

		[TestMethod]
		public void CreateInstance_UsingAssemblyAttributes_PropertyDependencies()
		{
			container.AddAssembly(Assembly.GetExecutingAssembly());

			var customerProp = container.CreateInstance(typeof(SecondCustomerBLL));
			var customerPropGeneric = container.CreateInstance<SecondCustomerBLL>();

			Assert.IsNotNull(customerProp, "Customer instance was not created.");
			Assert.IsNotNull(customerPropGeneric, "Customer instance was not created using generic method.");
			Assert.IsTrue(customerProp.GetType() == typeof(SecondCustomerBLL), "Wrong type returned by CreateInstance method.");
			Assert.IsTrue(customerPropGeneric.GetType() == typeof(SecondCustomerBLL), "Wrong type returned by CreateInstance generic method.");
		}

		[TestMethod]
		public void CreateInstance_UsingAddType_PropertyDependencies()
		{
			container.AddType(typeof(SecondCustomerBLL));
			container.AddType(typeof(Logger));
			container.AddType(typeof(ICustomerDAL), typeof(CustomerDAL));

			var customerProp = (SecondCustomerBLL)container.CreateInstance(typeof(SecondCustomerBLL));
			var customerPropGeneric = container.CreateInstance<SecondCustomerBLL>();

			Assert.IsNotNull(customerProp.Logger, "Logger instance was not created.");
			Assert.IsNotNull(customerPropGeneric.Logger, "Logger instance was not created using generic method.");
			Assert.IsTrue(customerProp.Logger.GetType() == typeof(Logger), "Wrong type returned by CreateInstance method.");
			Assert.IsTrue(customerPropGeneric.Logger.GetType() == typeof(Logger), "Wrong type returned by CreateInstance generic method.");

			Assert.IsNotNull(customerProp.CustomerDAL, "CustomerDAL instance was not created.");
			Assert.IsNotNull(customerPropGeneric.CustomerDAL, "CustomerDAL instance was not created using generic method.");
			Assert.IsTrue(customerProp.CustomerDAL.GetType() == typeof(CustomerDAL), "Wrong type returned by CreateInstance method.");
			Assert.IsTrue(customerPropGeneric.CustomerDAL.GetType() == typeof(CustomerDAL), "Wrong type returned by CreateInstance generic method.");

			Assert.IsNotNull(customerProp, "Customer instance was not created.");
			Assert.IsNotNull(customerPropGeneric, "Customer instance was not created using generic method.");
			Assert.IsTrue(customerProp.GetType() == typeof(SecondCustomerBLL), "Wrong type returned by CreateInstance method.");
			Assert.IsTrue(customerPropGeneric.GetType() == typeof(SecondCustomerBLL), "Wrong type returned by CreateInstance generic method.");
		}
	}
}
