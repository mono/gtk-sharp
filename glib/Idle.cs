// GLib.Idle.cs - Idle class implementation
//
// Author: Mike Kestner <mkestner@speakeasy.net>
//         Rachel Hestilow <hestilow@ximian.com>
//
// Copyright (c) 2002 Mike Kestner
// Copyright (c) Rachel Hestilow
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

	public delegate bool IdleHandler ();

	public class Idle {

		[UnmanagedFunctionPointer (CallingConvention.Cdecl)]
		delegate bool IdleHandlerInternal (IntPtr ptr);


		internal class IdleProxy : SourceProxy {
			public IdleProxy (IdleHandler real)
			{
				real_handler = real;
				proxy_handler = new IdleHandlerInternal (Handler);
			}

			~IdleProxy ()
			{
				Dispose (false);
			}

			public void Dispose ()
			{
				Dispose (true);
				GC.SuppressFinalize (this);
			}

			protected virtual void Dispose (bool disposing)
			{
				// Both branches remove our delegate from the
				// managed list of handlers, but only
				// Source.Remove will remove it from the

				if (disposing)
					Remove ();
				else
					Source.Remove (ID);
			}

			static bool Handler (IntPtr data)
			{
				try {
					SourceProxy obj;
					lock(proxies)
						obj = proxies [(int)data];
					IdleHandler idle_handler = (IdleHandler)obj.real_handler;

					bool cont = idle_handler ();
					if (!cont)
						obj.Remove ();
					return cont;
				} catch (Exception e) {
					ExceptionManager.RaiseUnhandledException (e, false);
				}
				return false;
			}
		}
		
		private Idle ()
		{
		}
		
		[DllImport("libglib-2.0-0.dll", CallingConvention=CallingConvention.Cdecl)]
		static extern uint g_idle_add (IdleHandlerInternal d, IntPtr data);

		public static uint Add (IdleHandler hndlr)
		{
			IdleProxy p = new IdleProxy (hndlr);
			p.ID = g_idle_add ((IdleHandlerInternal)p.proxy_handler, (IntPtr)p.proxyId);
			lock (Source.source_handlers)
				Source.source_handlers [p.ID] = p;

			return p.ID;
		}
		
		[DllImport("libglib-2.0-0.dll", CallingConvention=CallingConvention.Cdecl)]
		static extern bool g_source_remove (uint id);
                                                                                
		public static bool Remove (IdleHandler hndlr)
		{
			bool result = false;
			var keys = new System.Collections.Generic.List<uint> ();

			lock (Source.source_handlers) {
				foreach (uint code in Source.source_handlers.Keys) {
					IdleProxy p = Source.source_handlers [code] as IdleProxy;
				
					if (p != null && p.real_handler == (System.Delegate) hndlr) {
						keys.Add (code);
						result = g_source_remove (code);
						p.proxy_handler = null;
						p.real_handler = null;
						lock(SourceProxy.proxies)
							SourceProxy.proxies.Remove (p.proxyId);
					}
				}

				foreach (var key in keys)
					Source.source_handlers.Remove (key);
			}

			return result;
		}
	}
}

