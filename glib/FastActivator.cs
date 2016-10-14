using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;

namespace GLib
{
	static class FastActivator
	{
		const BindingFlags flags = BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance | BindingFlags.CreateInstance;
		delegate object FastCreateObject (IntPtr ptr);
		static FastCreateObject FastCtor (Type t)
		{
			FastCreateObject method;
			if (!cache.TryGetValue (t, out method)) {
				var param = Expression.Parameter (typeof (IntPtr));
				var newExpr = Expression.New (
					t.GetConstructor (flags, null, new [] { typeof (IntPtr) }, new ParameterModifier [0]),
					param
				);
				cache [t] = method = (FastCreateObject)Expression.Lambda (typeof (FastCreateObject), newExpr, param).Compile ();
			}
			return method;
		}

		static readonly Dictionary<Type, FastCreateObject> cache = new Dictionary<Type, FastCreateObject> (new TypeEqualityComparer ());
		public static Opaque CreateOpaque (IntPtr o, Type type)
		{
			return (Opaque)FastCtor (type)(o);
		}

		public static Object CreateObject (IntPtr o, Type type)
		{
			return (Object)FastCtor (type)(o);
		}

		class TypeEqualityComparer : IEqualityComparer<Type>
		{
			public bool Equals (Type x, Type y)
			{
				return x == y;
			}
			public int GetHashCode (Type obj)
			{
				if (obj == null)
					return 0;
				return obj.GetHashCode ();
			}
		}
	}
	}
}
