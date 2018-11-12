using IoCContainer.Attributes;
using IoCContainerTests.TestData.Interfaces;

namespace IoCContainerTests.TestData
{
	[Export(typeof(ICustomerDAL))]
	public class CustomerDAL : ICustomerDAL
	{
		public CustomerDAL() { }
	}
}
