// GLibSharp.Marshaller.cs : Marshalling utils 
//
// Author: Rachel Hestilow <rachel@nullenvoid.com>
//         Mike Kestner  <mkestner@ximian.com>
//
// Copyright (c) 2002, 2003 Rachel Hestilow
// Copyright (c) 2004 Novell, Inc.
//
// This program is free software; you can redistribute it and/or
// modify it under the terms of version 2 of the Lesser GNU General 
// Public License as published by the Free Software Foundation.
//
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
// Lesser General Public License for more details.
//
// You should have received a copy of the GNU Lesser General Public
// License along with this program; if not, write to the
// Free Software Foundation, Inc., 59 Temple Place - Suite 330,
// Boston, MA 02111-1307, USA.


namespace GLib {
	using System;
	using System.Runtime.InteropServices;
	
	public class Marshaller {

		private Marshaller () {}

		[DllImport("libglib-2.0-0.dll", CallingConvention=CallingConvention.Cdecl)]
		static extern void g_free (IntPtr mem);

		public static void Free (IntPtr ptr)
		{
			g_free (ptr);
		}

		public static void Free (IntPtr[] ptrs)
		{
			if (ptrs == null)
				return;

			for (int i = 0; i < ptrs.Length; i++)
				g_free (ptrs [i]);
		}

		[DllImport("libglib-2.0-0.dll", CallingConvention=CallingConvention.Cdecl)]
		static extern IntPtr g_filename_to_utf8 (IntPtr mem, int len, IntPtr read, out IntPtr written, out IntPtr error);

		[DllImport("libglib-2.0-0.dll", CallingConvention=CallingConvention.Cdecl)]
		static extern IntPtr g_filename_to_utf8_utf8 (IntPtr mem, int len, IntPtr read, out IntPtr written, out IntPtr error);

		public static string FilenamePtrToString (IntPtr ptr) 
		{
			if (ptr == IntPtr.Zero) return null;
			
			IntPtr dummy, error;
			IntPtr utf8;

			if (Global.IsWindowsPlatform)
				utf8 = g_filename_to_utf8_utf8 (ptr, -1, IntPtr.Zero, out dummy, out error);
			else
				utf8 = g_filename_to_utf8 (ptr, -1, IntPtr.Zero, out dummy, out error);

			if (error != IntPtr.Zero)
				throw new GLib.GException (error);
			return Utf8PtrToString (utf8);
		}

		public static string FilenamePtrToStringGFree (IntPtr ptr) 
		{
			string ret = FilenamePtrToString (ptr);
			g_free (ptr);
			return ret;
		}

		[DllImport ("libglib-2.0-0.dll", CallingConvention = CallingConvention.Cdecl)]
		unsafe static extern char* g_utf8_to_utf16 (IntPtr native_str, IntPtr len, IntPtr items_read, ref IntPtr items_written, out IntPtr error);

		[DllImport ("libglib-2.0-0.dll", CallingConvention = CallingConvention.Cdecl)]
		unsafe static extern IntPtr g_utf16_to_utf8 (char* native_str, IntPtr len, IntPtr items_read, out IntPtr items_written, out IntPtr error);

		[DllImport("glibsharpglue-2", CallingConvention=CallingConvention.Cdecl)]
		static extern UIntPtr glibsharp_strlen (IntPtr mem);
		public static string Utf8PtrToString (IntPtr ptr) 
		{
			if (ptr == IntPtr.Zero)
				return null;
			unsafe
			{
				IntPtr written = IntPtr.Zero;
				IntPtr error;
				char *utf16 = g_utf8_to_utf16 (ptr, new IntPtr (-1), IntPtr.Zero, ref written, out error);
				if (error != IntPtr.Zero)
					throw new GLib.GException (error);
				
				var result = new string (utf16, 0, (int)written);
				g_free ((IntPtr)utf16);
				return result;
			}
		}

		public static string[] Utf8PtrToString (IntPtr[] ptrs) {
			// The last pointer is a null terminator.
			string[] ret = new string[ptrs.Length - 1];
			for (int i = 0; i < ret.Length; i++)
				ret[i] = Utf8PtrToString (ptrs[i]);
			return ret;
		}

		public static string PtrToStringGFree (IntPtr ptr) 
		{
			string ret = Utf8PtrToString (ptr);
			g_free (ptr);
			return ret;
		}

		public static string[] PtrToStringGFree (IntPtr[] ptrs) {
			// The last pointer is a null terminator.
			string[] ret = new string[ptrs.Length - 1];
			for (int i = 0; i < ret.Length; i++) {
				ret[i] = Utf8PtrToString (ptrs[i]);
				g_free (ptrs[i]);
			}
			return ret;
		}

		[DllImport("libglib-2.0-0.dll", CallingConvention=CallingConvention.Cdecl)]
		static extern IntPtr g_filename_from_utf8 (IntPtr mem, int len, IntPtr read, out IntPtr written, out IntPtr error);

		[DllImport("libglib-2.0-0.dll", CallingConvention=CallingConvention.Cdecl)]
		static extern IntPtr g_filename_from_utf8_utf8 (IntPtr mem, int len, IntPtr read, out IntPtr written, out IntPtr error);

		public static IntPtr StringToFilenamePtr (string str) 
		{
			if (str == null)
				return IntPtr.Zero;

			IntPtr dummy, error;
			IntPtr utf8 = StringToPtrGStrdup (str);

			IntPtr result;

			if (Global.IsWindowsPlatform)
				result = g_filename_from_utf8_utf8 (utf8, -1, IntPtr.Zero, out dummy, out error);
			else
				result = g_filename_from_utf8 (utf8, -1, IntPtr.Zero, out dummy, out error);

			g_free (utf8);
			if (error != IntPtr.Zero)
				throw new GException (error);

			return result;
		}

		public static IntPtr StringToPtrGStrdup (string str) {
			IntPtr len;
			return StringToPtrGStrdup (str, out len);
		}

		internal static IntPtr StringToPtrGStrdup (string str, out IntPtr len)
		{
			if (str == null) {
				len = IntPtr.Zero;
				return IntPtr.Zero;
			}

			unsafe {
				fixed (char* p = str) {
					IntPtr error;
					var result = g_utf16_to_utf8 (p, new IntPtr (str.Length), IntPtr.Zero, out len, out error);

					if (error != IntPtr.Zero)
						throw new GLib.GException (error);

					return result;
				}
			}
		}

		public static string StringFormat (string format, params object[] args) {
			string ret = String.Format (format, args);
			if (ret.IndexOf ('%') == -1)
				return ret;
			else
				return ret.Replace ("%", "%%");
		}

		public static IntPtr[] StringArrayToNullTermPointer (string[] strs)
		{
			if (strs == null)
				return null;
			IntPtr[] result = new IntPtr [strs.Length + 1];
			for (int i = 0; i < strs.Length; i++)
				result [i] = StringToPtrGStrdup (strs [i]);
			result [strs.Length] = IntPtr.Zero;
			return result;
		}

		[DllImport("libglib-2.0-0.dll", CallingConvention=CallingConvention.Cdecl)]
		static extern void g_strfreev (IntPtr mem);

		public static void StrFreeV (IntPtr null_term_array)
		{
			g_strfreev (null_term_array);
		}

		public static string[] NullTermPtrToStringArray (IntPtr null_term_array, bool owned)
		{
			if (null_term_array == IntPtr.Zero)
				return Array.Empty<string> ();

			int count = 0;
			var result = new System.Collections.Generic.List<string> ();
			IntPtr s = Marshal.ReadIntPtr (null_term_array, count++ * IntPtr.Size);
			while (s != IntPtr.Zero) {
				result.Add (Utf8PtrToString (s));
				s = Marshal.ReadIntPtr (null_term_array, count++ * IntPtr.Size);
			}

			if (owned)
				g_strfreev (null_term_array);

			return result.ToArray ();
		}

		public static string[] PtrToStringArrayGFree (IntPtr string_array)
		{
			if (string_array == IntPtr.Zero)
				return Array.Empty<string> ();
	
			int count = 0;
			while (Marshal.ReadIntPtr (string_array, count*IntPtr.Size) != IntPtr.Zero)
				++count;
	
			string[] members = new string[count];
			for (int i = 0; i < count; ++i) {
				IntPtr s = Marshal.ReadIntPtr (string_array, i * IntPtr.Size);
				members[i] = GLib.Marshaller.PtrToStringGFree (s);
			}
			GLib.Marshaller.Free (string_array);
			return members;
		}

		// Argv marshalling -- unpleasantly complex, but
		// don't know of a better way to do it.
		//
		// Currently, the 64-bit cleanliness is
		// hypothetical. It's also ugly, but I don't know of a
		// construct to handle both 32 and 64 bitness
		// transparently, since we need to alloc buffers of
		// [native pointer size] * [count] bytes.

		[DllImport("libglib-2.0-0.dll", CallingConvention=CallingConvention.Cdecl)]
		static extern IntPtr g_malloc(UIntPtr size);

		public static IntPtr Malloc (ulong size)
		{
			return g_malloc (new UIntPtr (size));
		}

		static bool check_sixtyfour () {
			int szint = sizeof (int);
			int szlong = sizeof (long);
			int szptr = IntPtr.Size;

			if (szptr == szint)
				return false;
			if (szptr == szlong)
				return true;

			throw new Exception ("Pointers are neither int- nor long-sized???");
		}

		static IntPtr make_buf_32 (string[] args) 
		{
			int[] ptrs = new int[args.Length];

			for (int i = 0; i < args.Length; i++)
				ptrs[i] = (int) Marshal.StringToHGlobalAuto (args[i]);

			IntPtr buf = g_malloc (new UIntPtr ((ulong) sizeof (int) * 
					       (ulong) args.Length));
			Marshal.Copy (ptrs, 0, buf, ptrs.Length);
			return buf;
		}

		static IntPtr make_buf_64 (string[] args) 
		{
			long[] ptrs = new long[args.Length];

			for (int i = 0; i < args.Length; i++)
				ptrs[i] = (long) Marshal.StringToHGlobalAuto (args[i]);
				
			IntPtr buf = g_malloc (new UIntPtr ((ulong) sizeof (long) * 
					       (ulong) args.Length));
			Marshal.Copy (ptrs, 0, buf, ptrs.Length);
			return buf;
		}

		[Obsolete ("Use GLib.Argv instead to avoid leaks.")]
		public static IntPtr ArgvToArrayPtr (string[] args)
		{
			if (args.Length == 0)
				return IntPtr.Zero;

			if (check_sixtyfour ())
				return make_buf_64 (args);

			return make_buf_32 (args);
		}

		// should we be freeing these pointers? they're marshalled
		// from our own strings, so I think not ...

		static string[] unmarshal_32 (IntPtr buf, int argc)
		{
			int[] ptrs = new int[argc];
			string[] args = new string[argc];

			Marshal.Copy (buf, ptrs, 0, argc);

			for (int i = 0; i < ptrs.Length; i++)
				args[i] = Marshal.PtrToStringAuto ((IntPtr) ptrs[i]);
			
			return args;
		}

		static string[] unmarshal_64 (IntPtr buf, int argc)
		{
			long[] ptrs = new long[argc];
			string[] args = new string[argc];

			Marshal.Copy (buf, ptrs, 0, argc);

			for (int i = 0; i < ptrs.Length; i++)
				args[i] = Marshal.PtrToStringAuto ((IntPtr) ptrs[i]);
			
			return args;
		}		

		[Obsolete ("Use GLib.Argv instead to avoid leaks.")]
		public static string[] ArrayPtrToArgv (IntPtr array, int argc)
		{
			if (argc == 0)
				return Array.Empty<string> ();

			if (check_sixtyfour ())
				return unmarshal_64 (array, argc);

			return unmarshal_32 (array, argc);
		}

		static DateTime local_epoch = new DateTime (1970, 1, 1, 0, 0, 0);
		static int utc_offset = (int) (TimeZone.CurrentTimeZone.GetUtcOffset (DateTime.Now)).TotalSeconds;

		public static IntPtr DateTimeTotime_t (DateTime time)
		{
			return new IntPtr (((long)time.Subtract (local_epoch).TotalSeconds) - utc_offset);
		}

		public static DateTime time_tToDateTime (IntPtr time_t)
		{
			return local_epoch.AddSeconds (time_t.ToInt64 () + utc_offset);
		}

		[DllImport("glibsharpglue-2", CallingConvention=CallingConvention.Cdecl)]
		static extern IntPtr gtksharp_unichar_to_utf8_string (uint c);

		public static char GUnicharToChar (uint ucs4_char)
		{ 
			if (ucs4_char == 0)
				return (char) 0;

			IntPtr raw_ret = gtksharp_unichar_to_utf8_string (ucs4_char);
			string ret = GLib.Marshaller.PtrToStringGFree(raw_ret);
			if (ret.Length > 1)
				throw new ArgumentOutOfRangeException ("ucs4char is not representable by a char.");

			return ret [0];
		}

		[DllImport("glibsharpglue-2", CallingConvention=CallingConvention.Cdecl)]
		static extern uint glibsharp_utf16_to_unichar (ushort c);

		public static uint CharToGUnichar (char c)
		{
			return glibsharp_utf16_to_unichar ((ushort) c);
		}

		public static IntPtr StructureToPtrAlloc (object o)
		{
			IntPtr result = Marshal.AllocHGlobal (Marshal.SizeOf (o));
			Marshal.StructureToPtr (o, result, false);
			return result;
		}

		public static IntPtr StructureToPtrAlloc<T> (T o)
		{
			IntPtr result = Marshal.AllocHGlobal (Marshal.SizeOf (o));
			Marshal.StructureToPtr (o, result, false);
			return result;
		}

		[Obsolete ("Use the ListPtrToArray<T> overload.")]
		public static Array ListPtrToArray (IntPtr list_ptr, Type list_type, bool owned, bool elements_owned, Type elem_type)
		{
			ListBase list;
			if (list_type == typeof(GLib.List))
				list = new GLib.List (list_ptr, elem_type, owned, elements_owned);
			else
				list = new GLib.SList (list_ptr, elem_type, owned, elements_owned);

			using (list)
				return ListToArray (list, elem_type);
		}

		public static T [] ListPtrToArray<T> (IntPtr list_ptr, Type list_type, bool owned, bool elements_owned, ListElementFree free_func)
		{
			ListBase list;
			if (list_type == typeof (GLib.List))
				list = new GLib.List (list_ptr, typeof (T), owned, elements_owned, free_func);
			else
				list = new GLib.SList (list_ptr, typeof (T), owned, elements_owned, free_func);

			using (list)
				return ListToArray<T> (list);
		}

		public static T [] ListPtrToArray<T> (IntPtr list_ptr, Type list_type, bool owned, bool elements_owned)
		{
			ListBase list;
			if (list_type == typeof (GLib.List))
				list = new GLib.List (list_ptr, typeof(T), owned, elements_owned);
			else
				list = new GLib.SList (list_ptr, typeof (T), owned, elements_owned);

			using (list)
				return ListToArray<T> (list);
		}

		[Obsolete ("Use the PtrArrayToArray<T> overload.")]
		public static Array PtrArrayToArray (IntPtr list_ptr, bool owned, bool elements_owned, Type elem_type)
		{
			GLib.PtrArray array = new GLib.PtrArray (list_ptr, elem_type, owned, elements_owned);
			Array ret = Array.CreateInstance (elem_type, array.Count);
			array.CopyTo (ret, 0);
			array.Dispose ();
			return ret;
		}

		public static T [] PtrArrayToArray<T> (IntPtr list_ptr, bool owned, bool elements_owned)
		{
			var elem_type = typeof (T);
			GLib.PtrArray array = new GLib.PtrArray (list_ptr, elem_type, owned, elements_owned);
			T [] ret = new T [array.Count];
			array.CopyTo (ret, 0);
			array.Dispose ();
			return ret;
		}

		public static T [] PtrArrayToArray<T> (IntPtr list_ptr, bool owned, bool elements_owned, ListElementFree free_func)
		{
			var elem_type = typeof (T);
			GLib.PtrArray array = new GLib.PtrArray (list_ptr, elem_type, owned, elements_owned, free_func);
			T [] ret = new T [array.Count];
			array.CopyTo (ret, 0);
			array.Dispose ();
			return ret;
		}

		[Obsolete ("Use the ListToArray<T> overload")]
		public static Array ListToArray (ListBase list, System.Type type)
		{
			int count = list.Count;
			Array result = Array.CreateInstance (type, count);
			if (count > 0)
				list.CopyTo (result, 0, count);

			if (type.IsSubclassOf (typeof (GLib.Opaque)))
				list.elements_owned = false;

			return result;
		}

		public static T [] ListToArray<T> (ListBase list)
		{
			int count = list.Count;
			var result = new T [count];
			if (count > 0)
				list.CopyTo (result, 0);

			if (typeof(T).IsSubclassOf (typeof (GLib.Opaque)))
				list.elements_owned = false;

			return result;
		}
	}
}

