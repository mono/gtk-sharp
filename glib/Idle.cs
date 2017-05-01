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
	using System.Collections.Generic;
	using System.Runtime.InteropServices;

	public delegate bool IdleHandler ();

	public class Idle {

		internal class IdleProxy : SourceProxy {
			internal readonly IdleHandler real_handler;
			public IdleProxy (IdleHandler real)
			{
				real_handler = real;
			}

			protected override bool Invoke (IntPtr data)
			{
				return real_handler ();
			}
		}
		
		private Idle ()
		{
		}
		
		[DllImport("libglib-2.0-0.dll", CallingConvention=CallingConvention.Cdecl)]
		static extern uint g_idle_add (SourceProxy.GSourceFuncInternal d, IntPtr data);

		public static uint Add (IdleHandler hndlr)
		{
			IdleProxy p = new IdleProxy (hndlr);
			p.ID = g_idle_add (p.Handler, IntPtr.Zero);
			Source.Add (p);

			return p.ID;
		}
		
		[DllImport("libglib-2.0-0.dll", CallingConvention=CallingConvention.Cdecl)]
		static extern bool g_source_remove (uint id);
                                                                                
		public static bool Remove (IdleHandler hndlr)
		{
			bool result = false;

			lock (Source.source_handlers) {
				foreach (KeyValuePair<uint, SourceProxy> kvp in Source.source_handlers) {
					uint code = kvp.Key;
					IdleProxy p = kvp.Value as IdleProxy;
				
					if (p != null && p.real_handler == hndlr) {
						result = g_source_remove (code);
						p.Remove ();
					}
				}
			}

			return result;
		}
	}
}

