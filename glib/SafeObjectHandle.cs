using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Microsoft.Win32.SafeHandles;

namespace GLib
{
	class SafeObjectHandle : SafeHandleZeroOrMinusOneIsInvalid
	{
		public static SafeObjectHandle Zero = new SafeObjectHandle ();

		internal ToggleRef tref;

		SafeObjectHandle () : base (false)
		{
		}

		SafeObjectHandle (IntPtr handle) : base(true)
		{
			SetHandle (handle);
		}

		static readonly Dictionary<IntPtr, ToggleRef> Objects = new Dictionary<IntPtr, ToggleRef> (IntPtrEqualityComparer.Instance);
		public static SafeObjectHandle Create (Object obj, IntPtr handle)
		{
			if (handle == IntPtr.Zero)
				return Zero;

			var safeHandle = new SafeObjectHandle (handle) {
				tref = new ToggleRef (obj, handle)
			};

			lock (Objects)
				Objects [handle] = safeHandle.tref;

			return safeHandle;
		}

		public static bool TryGetObject (IntPtr o, out Object obj)
		{
			bool ret;
			ToggleRef tr;

			lock (Objects)
				ret = Objects.TryGetValue (o, out tr);
			
			obj = ret ? tr.Target : null;
			return ret;
		}

		protected override bool ReleaseHandle ()
		{
			lock (Objects)
				Objects.Remove (handle);

			if (tref != null)
				tref.Free ();
			return true;
		}
	}
}
