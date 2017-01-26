using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;

namespace GLib
{
	static class FastActivator
	{
		const BindingFlags flags = BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance | BindingFlags.CreateInstance;

		delegate object FastCreateObjectPtr (IntPtr ptr);
		static FastCreateObjectPtr FastCtorPtr (Type t, Dictionary<Type, FastCreateObjectPtr> cache)
		{
			FastCreateObjectPtr method;
			lock (cache) {
				if (!cache.TryGetValue (t, out method)) {
					var param = Expression.Parameter (typeof (IntPtr));
					var ctor = t.GetConstructor (flags, null, new [] { typeof (IntPtr) }, new ParameterModifier [0]);
					if (ctor == null)
						throw new MissingMethodException ();

					var newExpr = Expression.New (ctor, param);
					cache [t] = method = (FastCreateObjectPtr)Expression.Lambda (typeof (FastCreateObjectPtr), newExpr, param).Compile ();
				}
			}
			return method;
		}

		delegate object FastCreateObject ();
		static FastCreateObject FastCtor (Type t, Dictionary<Type, FastCreateObject> cache)
		{
			FastCreateObject method;
			lock (cache) {
				if (!cache.TryGetValue (t, out method)) {
					var newExpr = Expression.New (t);
					cache [t] = method = (FastCreateObject)Expression.Lambda (typeof (FastCreateObject), newExpr).Compile ();
				}
			}
			return method;
		}

		static readonly Dictionary<Type, FastCreateObjectPtr> cacheOpaque = new Dictionary<Type, FastCreateObjectPtr> (new TypeEqualityComparer ());
		public static Opaque CreateOpaque (IntPtr o, Type type)
		{
			return (Opaque)FastCtorPtr (type, cacheOpaque)(o);
		}

		static readonly Dictionary<Type, FastCreateObjectPtr> cacheObject = new Dictionary<Type, FastCreateObjectPtr> (new TypeEqualityComparer ());
		public static Object CreateObject (IntPtr o, Type type)
		{
			return (Object)FastCtorPtr (type, cacheObject)(o);
		}

		static readonly Dictionary<Type, FastCreateObject> cacheSignalArgs = new Dictionary<Type, FastCreateObject> (new TypeEqualityComparer ());
		public static SignalArgs CreateSignalArgs (Type type)
		{
			return (SignalArgs)FastCtor (type, cacheSignalArgs)();
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
