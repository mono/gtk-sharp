// GLib.Source.cs - Source class implementation
//
// Author: Duncan Mak  <duncan@ximian.com>
//
// Copyright (c) 2002 Mike Kestner
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
	using System.Collections.Generic;
	using System.Runtime.InteropServices;
	using System.Threading;

	public delegate bool GSourceFunc ();

	//
	// Base class for IdleProxy and TimeoutProxy
	//
	internal abstract class SourceProxy {
		internal uint ID;
		internal bool needsAdd = true;
		internal GCHandle handle;

		protected SourceProxy ()
		{
			handle = GCHandle.Alloc (this);
		}

		[UnmanagedFunctionPointer (CallingConvention.Cdecl)]
		internal delegate bool GSourceFuncInternal (IntPtr ptr);

		public void Remove ()
		{
			lock (Source.source_handlers) {
				if (needsAdd)
					needsAdd = false;
				else
					Source.source_handlers.Remove (ID);
				handle.Free ();
			}
		}

		static GSourceFuncInternal sourceHandler;
		public static GSourceFuncInternal SourceHandler {
			get {
				if (sourceHandler == null)
					sourceHandler = new GSourceFuncInternal (HandlerInternal);
				return sourceHandler;
			}
		}

		static bool HandlerInternal (IntPtr data)
		{
			try {
				SourceProxy proxy = (SourceProxy)((GCHandle)data).Target;
				bool cont = proxy.Invoke ();
				if (!cont)
					proxy.Remove ();
				return cont;
			} catch (Exception e) {
				ExceptionManager.RaiseUnhandledException (e, false);
			}
			return false;
		}

		protected abstract bool Invoke ();
	}
	
        public class Source {
		private Source () {}
		
		internal static Dictionary<uint, SourceProxy> source_handlers = new Dictionary<uint, SourceProxy>();
		
		[DllImport("libglib-2.0-0.dll", CallingConvention=CallingConvention.Cdecl)]
		static extern bool g_source_remove (uint tag);

		internal static void Add (SourceProxy proxy)
		{
			lock (source_handlers) {
				if (proxy.needsAdd) {
					proxy.needsAdd = false;
					source_handlers [proxy.ID] = proxy;
				}
			}
		}

		public static bool Remove (uint tag)
		{
			// g_source_remove always returns true, so we follow that
			bool ret = true;

			lock (source_handlers) {
				SourceProxy handler;
				if (source_handlers.TryGetValue (tag, out handler)) {
					ret = g_source_remove (tag);
					handler.Remove ();
				}
			}
			return ret;
		}
	}
}
