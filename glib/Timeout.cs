// GLib.Timeout.cs - Timeout class implementation
//
// Author: Mike Kestner <mkestner@speakeasy.net>
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
	using System.Runtime.InteropServices;

	public delegate bool TimeoutHandler ();

	public class Timeout {

		internal static class TimeoutProxy  {
			internal static readonly SourceProxy.GSourceFuncInternal SourceHandler = HandlerInternal;

			static bool HandlerInternal (IntPtr data)
			{
				try {
					var proxy = (TimeoutHandler)((GCHandle)data).Target;
					return proxy.Invoke ();
				} catch (Exception e) {
					ExceptionManager.RaiseUnhandledException (e, false);
				}
				return false;
			}
		}

		private Timeout () {}

		static readonly int defaultPriority = glibsharp_timeout_priority_default ();

		[DllImport ("glibsharpglue-2", CallingConvention = CallingConvention.Cdecl)]
		static extern int glibsharp_timeout_priority_default ();

		[DllImport("libglib-2.0-0.dll", CallingConvention=CallingConvention.Cdecl)]
		static extern uint g_timeout_add_full (int priority, uint interval, SourceProxy.GSourceFuncInternal d, IntPtr data, DestroyNotify notify);

		public static uint Add (uint interval, TimeoutHandler hndlr)
		{
			var handle = GCHandle.Alloc (hndlr);

			return Add (interval, TimeoutProxy.SourceHandler, handle);
		}

		/// <summary>
		/// The handle will be freed automatically on source removal.
		/// </summary>
		/// <returns>The add.</returns>
		/// <param name="interval">Interval.</param>
		/// <param name="handle">Handle.</param>
		internal static uint Add (uint interval, SourceProxy.GSourceFuncInternal handler, GCHandle handle)
		{
			return g_timeout_add_full (defaultPriority, interval, handler, (IntPtr)handle, DestroyHelper.NotifyHandler);
		}
	}
}
