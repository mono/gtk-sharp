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
	internal abstract class SourceProxy : IDisposable {
		internal uint ID;

		internal int proxyId;
		static int idCounter;
		internal static Dictionary<int, SourceProxy> proxies = new Dictionary<int, SourceProxy> ();

		[UnmanagedFunctionPointer (CallingConvention.Cdecl)]
		internal delegate bool GSourceFuncInternal (IntPtr ptr);

		protected SourceProxy ()
		{
			lock(proxies) {
				do {
					proxyId = idCounter++;
				} while (proxies.ContainsKey (proxyId));
				proxies [proxyId] = this;
			}
		}

		public void Dispose ()
		{
			lock (Source.source_handlers)
				Source.source_handlers.Remove (ID);
			lock(proxies)
				proxies.Remove (proxyId);
		}

		internal static bool Handler (IntPtr data)
		{
			try {
				SourceProxy obj;
				lock (proxies)
					obj = proxies [(int)data];

				bool cont = obj.Invoke ();
				if (!cont)
					obj.Dispose ();
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

		public static bool Remove (uint tag)
		{
			// g_source_remove always returns true, so we follow that
			bool ret = true;

			lock (source_handlers) {
				SourceProxy handler;
				if (source_handlers.TryGetValue (tag, out handler)) {
					ret = g_source_remove (tag);
					handler.Dispose ();
				}
			}
			return ret;
		}
	}
}
