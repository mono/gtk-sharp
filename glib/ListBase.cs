// ListBase.cs - List base class implementation
//
// Authors: Mike Kestner <mkestner@novell.com>
//
// Copyright (c) 2002 Mike Kestner
// Copyright (c) 2005 Novell, Inc.
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
	using System.Collections;
	using System.Runtime.InteropServices;

	public delegate void ListElementFree (IntPtr ptr);
	public abstract class ListBase : IDisposable, ICollection, GLib.IWrapper, ICloneable {

		private IntPtr list_ptr = IntPtr.Zero;
		private bool managed = false;
		internal bool elements_owned = false;
		protected System.Type element_type = null;
		private ListElementFree free_func;

		abstract internal IntPtr NthData (uint index);
		abstract internal int Length (IntPtr list);
		abstract internal void Free (IntPtr list);
		abstract internal IntPtr Append (IntPtr current, IntPtr raw);
		abstract internal IntPtr Prepend (IntPtr current, IntPtr raw);



		[DllImport ("libglib-2.0-0.dll", CallingConvention = CallingConvention.Cdecl)]
		static extern void g_free (IntPtr item);

		static ListElementFree gFreeFunc;
		protected static ListElementFree GFreeFunc {
			get {
				if (gFreeFunc == null)
					gFreeFunc = new ListElementFree (g_free);;
				return gFreeFunc;
			}
		}

		internal ListBase (IntPtr list, System.Type element_type, bool owned, bool elements_owned, ListElementFree free_func)
		{
			list_ptr = list;
			this.element_type = element_type;
			managed = owned;
			this.elements_owned = elements_owned;
			this.free_func = free_func;
		}

		~ListBase ()
		{
			Dispose (false);
		}

		[Obsolete ("Replaced by owned parameter on ctor.")]
		public bool Managed {
			set { managed = value; }
		}

		public IntPtr Handle {
			get {
				return list_ptr;
			}
		}

		public void Append (IntPtr raw)
		{
			list_ptr = Append (list_ptr, raw);
		}

		public void Append (string item)
		{
			this.Append (Marshaller.StringToPtrGStrdup (item));
		}

		public void Append (object item)
		{
			this.Append (AllocNativeElement (item));
		}

		public void Prepend (IntPtr raw)
		{
			list_ptr = Prepend (list_ptr, raw);
		}

		// ICollection
		public int Count {
			get {
				return Length (list_ptr);
			}
		}

		public object this [int index] {
			get {
				IntPtr data = NthData ((uint) index);
				object ret = null;
				ret = DataMarshal (data);
				return ret;
			}
		}

		// Synchronization could be tricky here. Hmm.
		public bool IsSynchronized {
			get { return false; }
		}

		public object SyncRoot {
			get { return null; }
		}

		public void CopyTo (Array array, int index)
		{
			CopyTo (array, index, Count);
		}

		internal void CopyTo (Array array, int index, int count)
		{
			object[] orig = new object[count];
			int i = 0;
			foreach (object o in this)
				orig [i++] = o;

			orig.CopyTo (array, index);
		}

		public void CopyTo<T> (T[] array, int index)
		{
			int i = index;
			foreach (T o in this)
				array [i++] = o;
		}

		public class FilenameString {
			private FilenameString () {}
		}

		IntPtr AllocNativeElement (object element)
		{
			if (element_type == null) {
				if (element is IWrapper)
					return (element as IWrapper).Handle;
				else
					return (IntPtr) GCHandle.Alloc (element);
			} else {
				if (element_type == typeof (string))
					return Marshaller.StringToPtrGStrdup (element as string);
				else if (element_type == typeof (FilenameString))
					return Marshaller.StringToFilenamePtr (element as string);
				else if (element_type == typeof (IntPtr))
					return (IntPtr) GCHandle.Alloc (element);
				else if (typeof (IWrapper).IsAssignableFrom (element_type))
					return (element as IWrapper).Handle;
				else if (element_type == typeof (int))
					return new IntPtr ((int) element);
				else if (element_type.IsValueType)
					return Marshaller.StructureToPtrAlloc (element);
			}
			return IntPtr.Zero;
		}

		internal object DataMarshal (IntPtr data)
		{
			object ret = null;
			if (element_type != null) {
				if (element_type == typeof (string))
					ret = Marshaller.Utf8PtrToString (data);
				else if (element_type == typeof (FilenameString))
					ret = Marshaller.FilenamePtrToString (data);
				else if (element_type == typeof (IntPtr))
					ret = data;
				else if (element_type.IsSubclassOf (typeof (GLib.Object)))
					ret = GLib.Object.GetObject (data, false);
				else if (element_type.IsSubclassOf (typeof (GLib.Opaque)))
					ret = GLib.Opaque.GetOpaque (data, element_type, elements_owned);
				else if (element_type == typeof (int))
					ret = (int) data;
				else if (element_type.IsValueType)
					ret = Marshal.PtrToStructure (data, element_type);
				else if (element_type.IsInterface) {
					Type adapter_type = element_type.Assembly.GetType (element_type.FullName + "Adapter");
					System.Reflection.MethodInfo method = adapter_type.GetMethod ("GetObject", new Type[] {typeof(IntPtr), typeof(bool)});
					ret = method.Invoke (null, new object[] {data, false});
				} else
					ret = Activator.CreateInstance (element_type, new object[] {data});

			} else if (Object.IsObject (data))
				ret = GLib.Object.GetObject (data, false);

			return ret;
		}

		[DllImport ("libgobject-2.0-0.dll", CallingConvention=CallingConvention.Cdecl)]
		static extern void g_object_unref (IntPtr item);

		public void Empty ()
		{
			if (elements_owned) {
				var current = list_ptr;
				if (typeof (GLib.Object).IsAssignableFrom (element_type)) {
					while (current != IntPtr.Zero) {
						var temp = current;
						current = Next (temp);
						g_object_unref (GetData (temp));
					}
				} else if (typeof (GLib.Opaque).IsAssignableFrom (element_type)) {
					while (current != IntPtr.Zero) {
						var temp = current;
						current = Next (temp);
						GLib.Opaque.GetOpaque (GetData (temp), element_type, true).Dispose ();
					}
				} else {
					while (current != IntPtr.Zero) {
						var temp = current;
						current = Next (temp);
						free_func (GetData (temp));
					}
				}
			}

			if (managed)
				FreeList ();
		}

		IntPtr GetData (IntPtr current)
		{
			// data field is at offset 0 for GList and GSList
			return Marshal.ReadIntPtr (current);
		}

		IntPtr Next (IntPtr current)
		{
			// next field follows gpointer data field for GList and GSList
			return Marshal.ReadIntPtr (current, IntPtr.Size);
		}

		private class ListEnumerator : IEnumerator
		{
			private IntPtr current = IntPtr.Zero;
			private ListBase list;

			public ListEnumerator (ListBase list)
			{
				this.list = list;
			}

			public object Current {
				get {
					IntPtr data = list.GetData (current);
					object ret = null;
					ret = list.DataMarshal (data);
					return ret;
				}
			}

			public bool MoveNext ()
			{
				if (current == IntPtr.Zero)
					current = list.list_ptr;
				else
					current = list.Next (current);
				return (current != IntPtr.Zero);
			}

			public void Reset ()
			{
				current = IntPtr.Zero;
			}
		}

		// IEnumerable
		public IEnumerator GetEnumerator ()
		{
			return new ListEnumerator (this);
		}

		// IDisposable
		public void Dispose ()
		{
			Dispose (true);
			GC.SuppressFinalize (this);
		}

		protected virtual void Dispose (bool disposing)
		{
			Empty ();
		}

		void FreeList ()
		{
			if (list_ptr != IntPtr.Zero)
				Free (list_ptr);
			list_ptr = IntPtr.Zero;
		}

		// ICloneable
		abstract public object Clone ();
	}
}
