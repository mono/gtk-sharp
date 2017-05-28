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
		static object fastCtorPtrLock = new object ();
		volatile static Dictionary<Type, FastCreateObjectPtr> cacheFastCtorPtr = new Dictionary<Type, FastCreateObjectPtr> (new TypeEqualityComparer ()); 
		static FastCreateObjectPtr FastCtorPtr (Type t)
		{
			FastCreateObjectPtr method;
			var local = cacheFastCtorPtr;
			if (!local.TryGetValue (t, out method)) {
				lock (fastCtorPtrLock) {
					var param = Expression.Parameter (typeof (IntPtr));
					var ctor = t.GetConstructor (flags, null, new [] { typeof (IntPtr) }, new ParameterModifier [0]);
					if (ctor == null)
						throw new MissingMethodException ();

					var newExpr = Expression.New (ctor, param);
					var copy = new Dictionary<Type, FastCreateObjectPtr> (local);
					copy [t] = method = Expression.Lambda<FastCreateObjectPtr> (newExpr, param).Compile ();
					cacheFastCtorPtr = copy;
				}
			}
			return method;
		}

		delegate object FastCreateObject ();
		static object fastCtorLock = new object ();
		volatile static Dictionary<Type, FastCreateObject> cacheFastCtor = new Dictionary<Type, FastCreateObject> (new TypeEqualityComparer ());
		static FastCreateObject FastCtor (Type t)
		{
			FastCreateObject method;
			var local = cacheFastCtor;
			if (!local.TryGetValue (t, out method)) {
				lock (fastCtorLock) {
					var newExpr = Expression.New (t);
					var copy = new Dictionary<Type, FastCreateObject> (local);
					copy [t] = method = Expression.Lambda<FastCreateObject> (newExpr).Compile ();
					cacheFastCtor = copy;
				}
			}
			return method;
		}

		delegate object FastCreateBoxed (IntPtr ptr);
		static object fastBoxedLock = new object ();
		volatile static Dictionary<Type, FastCreateBoxed> cacheBoxed = new Dictionary<Type, FastCreateBoxed> (new TypeEqualityComparer ());
		static FastCreateBoxed FastBoxed (Type t)
		{
			FastCreateBoxed method;
			var local = cacheBoxed;
			if (!local.TryGetValue (t, out method)) {
				lock (fastBoxedLock) {
					var newMethod = t.GetMethod ("New", BindingFlags.Static | BindingFlags.Public | BindingFlags.FlattenHierarchy);
					var copy = new Dictionary<Type, FastCreateBoxed> (local);
					if (newMethod != null) {
						var param = Expression.Parameter (typeof (IntPtr));
						var call = Expression.Call (newMethod, param);
						var callWithConvert = Expression.Convert (call, typeof (object));
						copy [t] = method = Expression.Lambda<FastCreateBoxed> (callWithConvert, param).Compile ();
					} else {
						copy [t] = method = ptr => System.Runtime.InteropServices.Marshal.PtrToStructure (ptr, t);
					}
					cacheBoxed = copy;
				}
			}
			return method;
		}


		public static Opaque CreateOpaque (IntPtr o, Type type)
		{
			return (Opaque)FastCtorPtr (type) (o);
		}

		public static Object CreateObject (IntPtr o, Type type)
		{
			return (Object)FastCtorPtr (type) (o);
		}

		public static SignalArgs CreateSignalArgs (Type type)
		{
			return (SignalArgs)FastCtor (type) ();
		}

		public static object CreateBoxed (IntPtr o, Type type)
		{
			return FastBoxed (type) (o);
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
