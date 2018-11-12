using IoCContainer.Attributes;
using IoCContainerTests.TestData.Interfaces;

namespace IoCContainerTests.TestData
{
	[ImportConstructor]
	public class CustomerBLL
	{
		public CustomerBLL(ICustomerDAL dal, Logger logger)
		{
		}
	}

	public class SecondCustomerBLL
	{
		[Import]
		public ICustomerDAL CustomerDAL { get; set; }
		[Import]
		public Logger Logger { get; set; }
	}
}
