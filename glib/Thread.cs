// Thread.cs - thread awareness
//
// Author: Alp Toker <alp@atoker.com>
//
// Copyright (c) 2002 Alp Toker
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


namespace GLib
{
	using System;
	using System.Runtime.InteropServices;

	public class Thread
	{
		private Thread () {}

#if DISABLE_GTHREAD_CHECK
		public static void Init ()
		{
			// GLib automatically inits threads in 2.31 and above
			// http://developer.gnome.org/glib/unstable/glib-Deprecated-Thread-APIs.html#g-thread-init
		}

		public static bool Supported {
			get { return true; }
		}
#else
		[DllImport("libgthread-2.0-0.dll", CallingConvention=CallingConvention.Cdecl)]
		static extern void g_thread_init (IntPtr i);

		public static void Init ()
		{
			g_thread_init (IntPtr.Zero);
		}

		[DllImport("glibsharpglue-2", CallingConvention=CallingConvention.Cdecl)]
		static extern bool glibsharp_g_thread_supported ();

		public static bool Supported
		{
			get {
				return glibsharp_g_thread_supported ();
			}
		}
#endif

	}
}
