using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IoCContainer.Exceptions
{
	public class CustomContainerException : Exception
	{
		public CustomContainerException() : base()
		{ }

		public CustomContainerException(string message) : base(message)
		{ }
	}
}
