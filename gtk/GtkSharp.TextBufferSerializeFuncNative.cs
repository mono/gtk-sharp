// This file was auto-generated at one time, but is hardcoded here as part of the fix
// for the TextBufferSerializeFunc;  see https://bugzilla.novell.com/show_bug.cgi?id=555495
// The generated code may have been modified as part of this fix; see textbuffer-serializefunc.patch

namespace GtkSharp {

	using System;
	using System.Runtime.InteropServices;

	[UnmanagedFunctionPointer (CallingConvention.Cdecl)]
	internal delegate IntPtr TextBufferSerializeFuncNative(IntPtr register_buffer, IntPtr content_buffer, IntPtr start, IntPtr end, out UIntPtr length, IntPtr user_data);

	internal static class TextBufferSerializeFuncWrapper {

		public static IntPtr NativeCallback (IntPtr register_buffer, IntPtr content_buffer, IntPtr start, IntPtr end, out UIntPtr length, IntPtr user_data)
		{
			try {
				ulong mylength;

				var gch = (GCHandle)user_data;
				var managed = (Gtk.TextBufferSerializeFunc)gch.Target;

				byte [] __ret = managed (GLib.Object.GetObject(register_buffer) as Gtk.TextBuffer, GLib.Object.GetObject(content_buffer) as Gtk.TextBuffer, Gtk.TextIter.New (start), Gtk.TextIter.New (end), out mylength);

				length = new UIntPtr (mylength);

				IntPtr ret_ptr;
				if (mylength > 0) {
					ret_ptr = GLib.Marshaller.Malloc ((ulong)(sizeof (byte) * (int)mylength));
					Marshal.Copy (__ret, 0, ret_ptr, (int)mylength);
				} else {
					ret_ptr = IntPtr.Zero;
				}
				return ret_ptr;
			} catch (Exception e) {
				GLib.ExceptionManager.RaiseUnhandledException (e, true);
				// NOTREACHED: Above call does not return.
				throw e;
			}
		}

		internal static TextBufferSerializeFuncNative NativeDelegate = new TextBufferSerializeFuncNative (NativeCallback);
	}
}
