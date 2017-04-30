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
		internal readonly GSourceFuncInternal Handler;
		internal bool needsAdd = true;

		protected SourceProxy ()
		{
			Handler = new GSourceFuncInternal (HandlerInternal);
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
			}
		}

		bool HandlerInternal (IntPtr data)
		{
			try {
				bool cont = Invoke (data);
				if (!cont)
					Remove ();
				return cont;
			} catch (Exception e) {
				ExceptionManager.RaiseUnhandledException (e, false);
			}
			return false;
		}

		protected abstract bool Invoke (IntPtr data);
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
