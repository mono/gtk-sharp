using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;

namespace GLib
{
	static class FastActivator
	{
		const BindingFlags flags = BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance | BindingFlags.CreateInstance;

		static readonly Dictionary<Type, Delegate> cache = new Dictionary<Type, Delegate> (new TypeEqualityComparer ());
		delegate object FastCreateObjectPtr (IntPtr ptr);
		static FastCreateObjectPtr FastCtorPtr (Type t)
		{
			Delegate method;
			if (!cache.TryGetValue (t, out method)) {
				var param = Expression.Parameter (typeof (IntPtr));
				var newExpr = Expression.New (
					t.GetConstructor (flags, null, new [] { typeof (IntPtr) }, new ParameterModifier [0]),
					param
				);
				cache [t] = method = (FastCreateObjectPtr)Expression.Lambda (typeof (FastCreateObjectPtr), newExpr, param).Compile ();
			}
			return (FastCreateObjectPtr)method;
		}

		delegate object FastCreateObject ();
		static FastCreateObject FastCtor (Type t)
		{
			Delegate method;
			if (!cache.TryGetValue (t, out method)) {
				var newExpr = Expression.New (t);
				cache [t] = method = (FastCreateObject)Expression.Lambda (typeof (FastCreateObject), newExpr).Compile ();
			}
			return (FastCreateObject)method;
		}

		public static Opaque CreateOpaque (IntPtr o, Type type)
		{
			return (Opaque)FastCtorPtr (type)(o);
		}

		public static Object CreateObject (IntPtr o, Type type)
		{
			return (Object)FastCtorPtr (type)(o);
		}

		public static SignalArgs CreateSignalArgs (Type type)
		{
			return (SignalArgs)FastCtor (type)();
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
