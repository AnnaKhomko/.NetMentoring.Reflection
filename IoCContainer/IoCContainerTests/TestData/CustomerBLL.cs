using IoCContainer.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
