// This file was auto-generated at one time, but is hardcoded here as part of the fix
// for the TextBufferSerializeFunc;  see https://bugzilla.novell.com/show_bug.cgi?id=555495
// The generated code may have been modified as part of this fix; see textbuffer-serializefunc.patch

namespace GtkSharp {

	using System;
	using System.Runtime.InteropServices;

	[UnmanagedFunctionPointer (CallingConvention.Cdecl)]
	internal delegate IntPtr TextBufferSerializeFuncNative(IntPtr register_buffer, IntPtr content_buffer, IntPtr start, IntPtr end, out UIntPtr length, IntPtr user_data);

	internal class TextBufferSerializeFuncInvoker {

		TextBufferSerializeFuncNative native_cb;
		IntPtr __data;
		GLib.DestroyNotify __notify;

		~TextBufferSerializeFuncInvoker ()
		{
			if (__notify == null)
				return;
			__notify (__data);
		}

		internal TextBufferSerializeFuncInvoker (TextBufferSerializeFuncNative native_cb) : this (native_cb, IntPtr.Zero, null) {}

		internal TextBufferSerializeFuncInvoker (TextBufferSerializeFuncNative native_cb, IntPtr data) : this (native_cb, data, null) {}

		internal TextBufferSerializeFuncInvoker (TextBufferSerializeFuncNative native_cb, IntPtr data, GLib.DestroyNotify notify)
		{
			this.native_cb = native_cb;
			__data = data;
			__notify = notify;
		}

		internal Gtk.TextBufferSerializeFunc Handler {
			get {
				return new Gtk.TextBufferSerializeFunc(InvokeNative);
			}
		}

		private static readonly byte [] empty_byte_array = new byte[0];
		byte [] InvokeNative (Gtk.TextBuffer register_buffer, Gtk.TextBuffer content_buffer, Gtk.TextIter start, Gtk.TextIter end, out ulong length)
		{
			IntPtr native_start = GLib.Marshaller.StructureToPtrAlloc (start);
			IntPtr native_end = GLib.Marshaller.StructureToPtrAlloc (end);
			UIntPtr native_length;
			IntPtr result_ptr = native_cb (register_buffer == null ? IntPtr.Zero : register_buffer.Handle, content_buffer == null ? IntPtr.Zero : content_buffer.Handle, native_start, native_end, out native_length, __data);
			start = Gtk.TextIter.New (native_start);
			Marshal.FreeHGlobal (native_start);
			end = Gtk.TextIter.New (native_end);
			Marshal.FreeHGlobal (native_end);
			length = (ulong) native_length;

			byte [] result = null;
			if (length > 0 && result_ptr != IntPtr.Zero) {
					result = new byte [length];
					Marshal.Copy (result_ptr, result, 0, (int)length);
			}

			if (result_ptr != IntPtr.Zero) {
				GLib.Marshaller.Free (result_ptr);
			}

			return result == null ? empty_byte_array : result;
		}
	}

	internal class TextBufferSerializeFuncWrapper {

		public IntPtr NativeCallback (IntPtr register_buffer, IntPtr content_buffer, IntPtr start, IntPtr end, out UIntPtr length, IntPtr user_data)
		{
			try {
				ulong mylength;

				byte [] __ret = managed (GLib.Object.GetObject(register_buffer) as Gtk.TextBuffer, GLib.Object.GetObject(content_buffer) as Gtk.TextBuffer, Gtk.TextIter.New (start), Gtk.TextIter.New (end), out mylength);

				length = new UIntPtr (mylength);

				IntPtr ret_ptr;
				if (mylength > 0) {
					ret_ptr = GLib.Marshaller.Malloc ((ulong)(sizeof (byte) * (int)mylength));
					Marshal.Copy (__ret, 0, ret_ptr, (int)mylength);
				} else {
					ret_ptr = IntPtr.Zero;
				}

				if (release_on_call)
					gch.Free ();
				return ret_ptr;
			} catch (Exception e) {
				GLib.ExceptionManager.RaiseUnhandledException (e, true);
				// NOTREACHED: Above call does not return.
				throw e;
			}
		}

		bool release_on_call = false;
		GCHandle gch;

		public void PersistUntilCalled ()
		{
			release_on_call = true;
			gch = GCHandle.Alloc (this);
		}

		internal TextBufferSerializeFuncNative NativeDelegate;
		Gtk.TextBufferSerializeFunc managed;

		public TextBufferSerializeFuncWrapper (Gtk.TextBufferSerializeFunc managed)
		{
			this.managed = managed;
			if (managed != null)
				NativeDelegate = new TextBufferSerializeFuncNative (NativeCallback);
		}

		public static Gtk.TextBufferSerializeFunc GetManagedDelegate (TextBufferSerializeFuncNative native)
		{
			if (native == null)
				return null;
			TextBufferSerializeFuncWrapper wrapper = (TextBufferSerializeFuncWrapper) native.Target;
			if (wrapper == null)
				return null;
			return wrapper.managed;
		}
	}
}
