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
	using System.Linq;
	using System.Runtime.InteropServices;

	public delegate bool IdleHandler ();

	public class Idle
	{
		internal class IdleProxy : SourceProxy
		{
			internal readonly IdleHandler real_handler;
			internal uint ID;
			internal bool needsAdd = true;

			public IdleProxy (IdleHandler real)
			{
				real_handler = real;
			}

			protected override bool Invoke ()
			{
				return real_handler ();
			}
		}

		private Idle ()
		{
		}

		static readonly int defaultPriority = glibsharp_idle_priority_default ();

		[DllImport ("glibsharpglue-2", CallingConvention = CallingConvention.Cdecl)]
		static extern int glibsharp_idle_priority_default ();

		[DllImport ("libglib-2.0-0.dll", CallingConvention = CallingConvention.Cdecl)]
		static extern uint g_idle_add_full (int priority, SourceProxy.GSourceFuncInternal d, IntPtr data, DestroyNotify notify);

		public static uint Add (IdleHandler hndlr)
		{
			IdleProxy p = new IdleProxy (hndlr);
			var handle = GCHandle.Alloc (p);

			p.ID = g_idle_add_full (defaultPriority, SourceProxy.SourceHandler, (IntPtr)handle, NotifyHandler);

			lock (idle_handlers) {
				if (p.needsAdd) {
					p.needsAdd = false;
					idle_handlers [p.ID] = p;
				}
			}

			return p.ID;
		}

		internal static Dictionary<uint, IdleProxy> idle_handlers = new Dictionary<uint, IdleProxy> ();

		[DllImport ("libglib-2.0-0.dll", CallingConvention = CallingConvention.Cdecl)]
		static extern bool g_source_remove (uint id);

		[Obsolete ("This method is inefficient, please use the GLib.Source.Remove(uint) method")]
		public static bool Remove (IdleHandler hndlr)
		{
			bool result = false;

			lock (idle_handlers) {
				// While we could delegate the removal by userdata to native,
				// that method is slower, as it would lock and iterate the whole source
				// list instead of a simple hashtable lookup.
				foreach (KeyValuePair<uint, IdleProxy> kvp in idle_handlers.ToArray ()) {
					uint code = kvp.Key;
					IdleProxy p = kvp.Value;

					if (p != null && p.real_handler == hndlr) {
						result = g_source_remove (code);
					}
				}
			}

			return result;
		}

		#region IdleProxy DestroyNotify
		static void ReleaseGCHandle (IntPtr data)
		{
			if (data == IntPtr.Zero)
				return;

			GCHandle gch = (GCHandle)data;
			IdleProxy proxy = (IdleProxy)gch.Target;

			lock (idle_handlers) {
				if (proxy.needsAdd)
					proxy.needsAdd = false;
				else
					idle_handlers.Remove (proxy.ID);
			}

			gch.Free ();
		}

		static DestroyNotify release_gchandle;

		public static DestroyNotify NotifyHandler {
			get {
				if (release_gchandle == null)
					release_gchandle = new DestroyNotify (ReleaseGCHandle);
				return release_gchandle;
			}
		}
		#endregion
	}
}

