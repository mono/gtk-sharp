using System;
namespace GLib
{
	[AttributeUsage (AttributeTargets.Class | AttributeTargets.Assembly, Inherited = false)]
	public sealed class IgnoreRegistrationAttribute : Attribute
	{
		public IgnoreRegistrationAttribute()
		{
			Properties = true;
			DefaultHandlers = true;
			Interfaces = true;
		}
		public bool Properties { get; set; }
		public bool DefaultHandlers { get; set; }
		public bool Interfaces { get; set; }
	}
}
