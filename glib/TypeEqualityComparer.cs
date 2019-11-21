using System;
using System.Collections.Generic;

namespace GLib
{
	sealed class TypeEqualityComparer : IEqualityComparer<Type>
	{
		public bool Equals (Type x, Type y)
		{
			return x == y;
		}

		public int GetHashCode (Type obj)
		{
			return obj == null ? 0 : obj.GetHashCode ();
		}
	}
}
