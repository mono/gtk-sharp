//
// Vfs.cs: Bindings for gnome-vfs.
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
	
		public static bool Initialized {
			get {
				return gnome_vfs_initialized ();
			}
		}
		
		static Vfs ()
		{
			if (!gnome_vfs_initialized ())
				gnome_vfs_init ();
		}
		
		public static bool Initialize ()
		{
			return gnome_vfs_init ();
		}
		
		[DllImport ("gnomevfs-2")]
		static extern bool gnome_vfs_shutdown ();
		
		public static bool Shutdown ()
		{
			return gnome_vfs_shutdown ();
		}
		
		[DllImport ("gnomevfs-2")]
		private static extern void gnome_vfs_async_cancel (IntPtr handle);
		
		public static void CancelAsync (Handle handle)
		{
			gnome_vfs_async_cancel (handle.Raw);
		}

		[DllImport ("gnomevfs-2")]
		private static extern Result gnome_vfs_close (IntPtr handle);
		
		public static Result Close (Handle handle)
		{
			return gnome_vfs_close (handle.Raw);
		}

		[DllImport ("gnomevfs-2")]
		private static extern void gnome_vfs_async_close (IntPtr handle, AsyncCallbackNative callback, IntPtr data);
		
		public static void CloseAsync (Handle handle, AsyncCallback callback)
		{
			AsyncCallbackWrapper wrapper = new AsyncCallbackWrapper (callback, null);
			gnome_vfs_async_close (handle.Raw, wrapper.NativeDelegate, IntPtr.Zero);
		}

		[DllImport ("gnomevfs-2")]
		private static extern Result gnome_vfs_create (out IntPtr handle, string uri, OpenMode mode, bool exclusive, uint perm);
		
		public static Handle Create (string uri, OpenMode mode, bool exclusive, uint perm)
		{
			IntPtr handle = IntPtr.Zero;
			Result result = gnome_vfs_create (out handle, uri, mode, exclusive, perm);
			if (result != Result.Ok)
				return null; // Throw Exception!
			else
				return new Handle (handle);
		}

		[DllImport ("gnomevfs-2")]
		private static extern void gnome_vfs_async_create (out IntPtr handle, string uri, OpenMode mode, bool exclusive, uint perm, int priority, AsyncCallbackNative callback, IntPtr data);
		
		public static Handle CreateAsync (string uri, OpenMode mode, bool exclusive, uint perm, int priority, AsyncCallback callback)
		{
			IntPtr handle = IntPtr.Zero;
			AsyncCallbackWrapper wrapper = new AsyncCallbackWrapper (callback, null);
			gnome_vfs_async_create (out handle, uri, mode, exclusive, perm, priority, wrapper.NativeDelegate, IntPtr.Zero);
			return new Handle (handle);
		}

		[DllImport ("gnomevfs-2")]
		private static extern Result gnome_vfs_open (out IntPtr handle, string uri, OpenMode mode);
		
		public static Handle Open (string uri, OpenMode mode)
		{
			IntPtr handle = IntPtr.Zero;
			Result result = gnome_vfs_open (out handle, uri, mode);
			if (result != Result.Ok)
				return null; // Throw Exception!
			else
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
		private static extern Result gnome_vfs_read (IntPtr handle, out byte buffer, ulong bytes, out ulong bytes_read);
		
		public static Result Read (Handle handle, out byte buffer, ulong bytes, out ulong bytes_read)
		{
			return gnome_vfs_read (handle.Raw, out buffer, bytes, out bytes_read);
		}

		[DllImport ("gnomevfs-2")]
		private static extern void gnome_vfs_async_read (IntPtr handle, out byte buffer, uint bytes, AsyncReadCallbackNative callback, IntPtr data);
		
		public static void ReadAsync (Handle handle, out byte buffer, uint bytes, AsyncReadCallback callback)
		{
			AsyncReadCallbackWrapper wrapper = new AsyncReadCallbackWrapper (callback, null);
			gnome_vfs_async_read (handle.Raw, out buffer, bytes, wrapper.NativeDelegate, IntPtr.Zero);
		}
		
		[DllImport ("gnomevfs-2")]
		private static extern Result gnome_vfs_seek (IntPtr handle, SeekPosition whence, long offset);
		
		public static Result Seek (Handle handle, SeekPosition whence, long offset)
		{
			return gnome_vfs_seek (handle.Raw, whence, offset);
		}
		
		[DllImport ("gnomevfs-2")]
		private static extern void gnome_vfs_async_seek (IntPtr handle, SeekPosition whence, long offset, AsyncCallbackNative callback, IntPtr data);
		
		public static void SeekAsync (Handle handle, SeekPosition whence, long offset, AsyncCallback callback)
		{
			AsyncCallbackWrapper wrapper = new AsyncCallbackWrapper (callback, null);
			gnome_vfs_async_seek (handle.Raw, whence, offset, wrapper.NativeDelegate, IntPtr.Zero);
		}

		[DllImport ("gnomevfs-2")]
		private static extern Result gnome_vfs_tell (IntPtr handle, out ulong offset);
		
		public static Result Tell (Handle handle, out ulong offset)
		{
			return gnome_vfs_tell (handle.Raw, out offset);
		}

		[DllImport ("gnomevfs-2")]
		private static extern Result gnome_vfs_write (IntPtr handle, out byte buffer, ulong bytes, out ulong bytes_written);
		
		public static Result Write (Handle handle, out byte buffer, ulong bytes, out ulong bytes_written)
		{
			return gnome_vfs_write (handle.Raw, out buffer, bytes, out bytes_written);
		}

		[DllImport ("gnomevfs-2")]
		private static extern void gnome_vfs_async_write (IntPtr handle, out byte buffer, uint bytes, AsyncWriteCallbackNative callback, IntPtr data);
		
		public static void WriteAsync (Handle handle, out byte buffer, uint bytes, AsyncWriteCallback callback)
		{
			AsyncWriteCallbackWrapper wrapper = new AsyncWriteCallbackWrapper (callback, null);
			gnome_vfs_async_write (handle.Raw, out buffer, bytes, wrapper.NativeDelegate, IntPtr.Zero);
		}
		
		[DllImport ("gnomevfs-2")]
		private static extern Result gnome_vfs_truncate (string uri, ulong length);
		
		public static Result Truncate (string uri, ulong length)
		{
			return gnome_vfs_truncate (uri, length);
		}
		
		[DllImport ("gnomevfs-2")]
		private static extern Result gnome_vfs_truncate_handle (IntPtr handle, ulong length);
		
		public static Result Truncate (Handle handle, ulong length)
		{
			return gnome_vfs_truncate_handle (handle.Raw, length);
		}
		
		[DllImport ("gnomevfs-2")]
		private static extern Result gnome_vfs_move (string old_uri, string new_uri, bool force_replace);
		
		public static Result Move (string old_uri, string new_uri, bool force_replace)
		{
			return gnome_vfs_move (old_uri, new_uri, force_replace);
		}
		
		[DllImport ("gnomevfs-2")]
		private static extern Result gnome_vfs_make_directory (string uri, uint perm);
		
		public static Result MakeDirectory (string uri, uint perm)
		{
			return gnome_vfs_make_directory (uri, perm);
		}
		
		[DllImport ("gnomevfs-2")]
		private static extern Result gnome_vfs_remove_directory (string uri);
		
		public static Result RemoveDirectory (string uri)
		{
			return gnome_vfs_remove_directory (uri);
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
