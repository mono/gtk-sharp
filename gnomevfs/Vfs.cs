//
// .cs: Bindings for gnome-vfs.
//
// Author:
//   Jeroen Zwartepoorte <jeroen@xs4all.nl>
//
// (C) Copyright Jeroen Zwartepoorte 2004
//

using System;
using System.Runtime.InteropServices;

namespace Gnome.Vfs {
	public class Vfs {
		[DllImport ("gnomevfs-2")]
		static extern bool gnome_vfs_init ();
		
		[DllImport ("gnomevfs-2")]
		static extern bool gnome_vfs_initialized ();		
	
		static Vfs ()
		{
			if (!gnome_vfs_initialized ())
				gnome_vfs_init ();
		}
		
		[DllImport ("gnomevfs-2")]
		private static extern string gnome_vfs_get_mime_type (string uri);
		
		public static string GetMimeType (string uri)
		{
			return gnome_vfs_get_mime_type (uri);
		}

		[DllImport ("gnomevfs-2")]
		private static extern string gnome_vfs_mime_get_icon (string mime_type);
		
		public static string GetIcon (string mime_type)
		{
			return gnome_vfs_mime_get_icon (mime_type);
		}

		[DllImport ("gnomevfs-2")]
		private static extern string gnome_vfs_mime_get_description (string mime_type);
		
		public static string GetDescription (string mime_type)
		{
			return gnome_vfs_mime_get_description (mime_type);
		}

		[DllImport ("gnomevfs-2")]
		private static extern void gnome_vfs_async_cancel (IntPtr handle);
		
		public static void CancelAsync (Handle handle)
		{
			gnome_vfs_async_cancel (handle.Raw);
		}

		[DllImport ("gnomevfs-2")]
		private static extern void gnome_vfs_async_close (IntPtr handle, AsyncCallbackNative callback, IntPtr data);
		
		public static void CloseAsync (Handle handle, AsyncCallback callback)
		{
			AsyncCallbackWrapper wrapper = new AsyncCallbackWrapper (callback, null);
			gnome_vfs_async_close (handle.Raw, wrapper.NativeDelegate, IntPtr.Zero);
		}

		[DllImport ("gnomevfs-2")]
		private static extern void gnome_vfs_async_create (out IntPtr handle, string uri, OpenMode mode, bool exclusive, int perm, int priority, AsyncCallbackNative callback, IntPtr data);
		
		public static Handle CreateAsync (string uri, OpenMode mode, bool exclusive, int perm, int priority, AsyncCallback callback)
		{
			IntPtr handle = IntPtr.Zero;
			AsyncCallbackWrapper wrapper = new AsyncCallbackWrapper (callback, null);
			gnome_vfs_async_create (out handle, uri, mode, exclusive, perm, priority, wrapper.NativeDelegate, IntPtr.Zero);
			return new Handle (handle);
		}

		[DllImport ("gnomevfs-2")]
		private static extern void gnome_vfs_async_open (out IntPtr handle, string uri, OpenMode mode, int priority, AsyncCallbackNative callback, IntPtr data);
		
		public static Handle OpenAsync (string uri, OpenMode mode, int priority, AsyncCallback callback)
		{
			IntPtr handle = IntPtr.Zero;
			AsyncCallbackWrapper wrapper = new AsyncCallbackWrapper (callback, null);
			gnome_vfs_async_open (out handle, uri, mode, priority, wrapper.NativeDelegate, IntPtr.Zero);
			return new Handle (handle);
		}

		[DllImport ("gnomevfs-2")]
		private static extern void gnome_vfs_async_read (IntPtr handle, out byte buffer, uint bytes, AsyncReadCallbackNative callback, IntPtr data);
		
		public static void ReadAsync (Handle handle, out byte buffer, uint bytes, AsyncReadCallback callback)
		{
			AsyncReadCallbackWrapper wrapper = new AsyncReadCallbackWrapper (callback, null);
			gnome_vfs_async_read (handle.Raw, out buffer, bytes, wrapper.NativeDelegate, IntPtr.Zero);
		}
		
		[DllImport ("gnomevfs-2")]
		private static extern void gnome_vfs_async_seek (IntPtr handle, SeekPosition whence, long offset, AsyncCallbackNative callback, IntPtr data);
		
		public static void SeekAsync (Handle handle, SeekPosition whence, long offset, AsyncCallback callback)
		{
			AsyncCallbackWrapper wrapper = new AsyncCallbackWrapper (callback, null);
			gnome_vfs_async_seek (handle.Raw, whence, offset, wrapper.NativeDelegate, IntPtr.Zero);
		}

		[DllImport ("gnomevfs-2")]
		private static extern void gnome_vfs_async_write (IntPtr handle, out byte buffer, uint bytes, AsyncWriteCallbackNative callback, IntPtr data);
		
		public static void WriteAsync (Handle handle, out byte buffer, uint bytes, AsyncWriteCallback callback)
		{
			AsyncWriteCallbackWrapper wrapper = new AsyncWriteCallbackWrapper (callback, null);
			gnome_vfs_async_write (handle.Raw, out buffer, bytes, wrapper.NativeDelegate, IntPtr.Zero);
		}
		
		[DllImport ("gnomevfs-2")]
		private static extern string gnome_vfs_result_to_string (int result);
		
		public static string ResultToString (Result result)
		{
			return gnome_vfs_result_to_string ((int)result);
		}
		
		public static string ResultToString (int result)
		{
			return gnome_vfs_result_to_string (result);
		}
	}
}
