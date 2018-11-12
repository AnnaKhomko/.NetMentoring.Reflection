using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IoCContainer.Attributes
{
	[AttributeUsage(AttributeTargets.Class)]
	public class ExportAttribute : Attribute
	{
		public ExportAttribute()
		{ }

		public ExportAttribute(Type type)
		{
			Type = type;
		}

		public Type Type { get; private set; }
	}
}
