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
	internal class SourceProxy {
		internal Delegate real_handler;
		internal Delegate proxy_handler;
		internal uint ID;

		internal int proxyId;
		static int idCounter;
		internal static Dictionary<int, SourceProxy> proxies = new Dictionary<int, SourceProxy> ();

		protected SourceProxy ()
		{
			lock(proxies) {
				do {
					proxyId = idCounter++;
				} while (proxies.ContainsKey (proxyId));
				proxies [proxyId] = this;
			}
		}

		internal void Remove ()
		{
			lock (Source.source_handlers)
				Source.source_handlers.Remove (ID);
			real_handler = null;
			proxy_handler = null;
			lock(proxies)
				proxies.Remove (proxyId);
		}
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

			lock (Source.source_handlers) {
				SourceProxy handler;
				if (source_handlers.TryGetValue (tag, out handler)) {
					handler.Remove ();
					ret = g_source_remove (tag);
				}
			}
			return ret;
		}
	}
}
