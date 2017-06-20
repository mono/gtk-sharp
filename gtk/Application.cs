// GTK.Application.cs - GTK Main Event Loop class implementation
//
// Author: Mike Kestner <mkestner@speakeasy.net>
//
// Copyright (c) 2001 Mike Kestner
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

namespace Gtk {

	using System;
	using System.Reflection;
	using System.Runtime.InteropServices;
	using Gdk;

	public class Application {
		static System.Threading.Thread MainThread;

		//
		// Disables creation of instances.
		//
		private Application ()
		{
		}

		const int WS_EX_TOOLWINDOW = 0x00000080;
		const int WS_OVERLAPPEDWINDOW = 0x00CF0000;

		[DllImport ("user32.dll", EntryPoint="CreateWindowExW", CharSet=CharSet.Unicode, CallingConvention=CallingConvention.StdCall)]
		static extern IntPtr Win32CreateWindow (int dwExStyle, string lpClassName, string lpWindowName,int dwStyle, int x, int y, int nWidth, int nHeight, IntPtr hWndParent, IntPtr hMenu, IntPtr hInstance, IntPtr lParam);

		[DllImport ("user32.dll", EntryPoint="DestroyWindow", CharSet=CharSet.Unicode, CallingConvention=CallingConvention.StdCall)]
		static extern bool Win32DestroyWindow (IntPtr window);

		static Application ()
		{
			if (!GLib.Thread.Supported)
				GLib.Thread.Init ();

			switch (Environment.OSVersion.Platform) {
			case PlatformID.Win32NT:
			case PlatformID.Win32S:
			case PlatformID.Win32Windows:
			case PlatformID.WinCE:
				// No idea why we need to create that window, but it enables visual styles on the Windows platform
				IntPtr window = Win32CreateWindow (WS_EX_TOOLWINDOW, "static", "gtk-sharp visual styles window", WS_OVERLAPPEDWINDOW, 0, 0, 0, 0, IntPtr.Zero, IntPtr.Zero, IntPtr.Zero, IntPtr.Zero);
				Win32DestroyWindow (window);
				break;
			default:
				break;
			}
		}

		[DllImport("libgtk-win32-2.0-0.dll", CallingConvention=CallingConvention.Cdecl)]
		static extern void gtk_init (ref int argc, ref IntPtr argv);

		[DllImport("libgtk-win32-2.0-0.dll", CallingConvention=CallingConvention.Cdecl)]
		static extern bool gtk_init_check (ref int argc, ref IntPtr argv);

		static void SetPrgname ()
		{
			GLib.Global.ProgramName = System.IO.Path.GetFileNameWithoutExtension (Environment.GetCommandLineArgs () [0]);
		}

		public static void Init ()
		{
			SetPrgname ();
			IntPtr argv = new IntPtr(0);
			int argc = 0;

			gtk_init (ref argc, ref argv);
		}

		static bool do_init (string progname, ref string[] args, bool check)
		{
			SetPrgname ();
			bool res = false;
			string[] progargs = new string[args.Length + 1];

			progargs[0] = progname;
			args.CopyTo (progargs, 1);

			GLib.Argv argv = new GLib.Argv (progargs);
			IntPtr buf = argv.Handle;
			int argc = progargs.Length;

			if (check)
				res = gtk_init_check (ref argc, ref buf);
			else
				gtk_init (ref argc, ref buf);

			if (buf != argv.Handle)
				throw new Exception ("init returned new argv handle");

			// copy back the resulting argv, minus argv[0], which we're
			// not interested in.

			if (argc <= 1)
				args = new string[0];
			else {
				progargs = argv.GetArgs (argc);
				args = new string[argc - 1];
				Array.Copy (progargs, 1, args, 0, argc - 1);
			}

			return res;
		}

		internal static void AssertMainThread ()
		{
			if (MainThread != null && System.Threading.Thread.CurrentThread != MainThread) {
				GLib.Log.Write (null, GLib.LogLevelFlags.Warning, "Gtk operations should be done on the main Thread\n" + Environment.StackTrace);
			}
		}

		public static void Init (string progname, ref string[] args)
		{
			MainThread = System.Threading.Thread.CurrentThread;
			do_init (progname, ref args, false);
		}

		public static bool InitCheck (string progname, ref string[] args)
		{
			MainThread = System.Threading.Thread.CurrentThread;
			return do_init (progname, ref args, true);
		}

		[DllImport("libgtk-win32-2.0-0.dll", CallingConvention=CallingConvention.Cdecl)]
		static extern void gtk_main ();

		public static void Run ()
		{
			gtk_main ();
		}

		[DllImport("libgtk-win32-2.0-0.dll", CallingConvention=CallingConvention.Cdecl)]
		static extern bool gtk_events_pending ();

		public static bool EventsPending ()
		{
			return gtk_events_pending ();
		}

		[DllImport("libgtk-win32-2.0-0.dll", CallingConvention=CallingConvention.Cdecl)]
		static extern void gtk_main_iteration ();

		[DllImport("libgtk-win32-2.0-0.dll", CallingConvention=CallingConvention.Cdecl)]
		static extern bool gtk_main_iteration_do (bool blocking);

		public static void RunIteration ()
		{
			gtk_main_iteration ();
		}

		public static bool RunIteration (bool blocking)
		{
			return gtk_main_iteration_do (blocking);
		}

		[DllImport("libgtk-win32-2.0-0.dll", CallingConvention=CallingConvention.Cdecl)]
		static extern void gtk_main_quit ();

		public static void Quit ()
		{
			gtk_main_quit ();
		}


		[DllImport("libgtk-win32-2.0-0.dll", CallingConvention=CallingConvention.Cdecl)]
		static extern IntPtr gtk_get_current_event ();

		public static Gdk.Event CurrentEvent {
			get {
				var raw_ret = gtk_get_current_event ();
				var ret = Gdk.Event.GetEvent (raw_ret, true);
				return ret;
			}
		}

		internal class InvokeProxy : GLib.SourceProxy {
			readonly protected EventHandler d;

			internal InvokeProxy (EventHandler d)
			{
				this.d = d;
			}

			protected override bool Invoke ()
			{
				d (this, EventArgs.Empty);
				return false;
			}
		}

		internal class InvokeProxyWithArgs : InvokeProxy
		{
			readonly object sender;
			readonly EventArgs args;

			internal InvokeProxyWithArgs (EventHandler d, object sender, EventArgs args) : base (d)
			{
				this.args = args;
				this.sender = sender;
			}

			protected override bool Invoke ()
			{
				d (sender, args);
				return false;
			}
		}

		internal class InvokeProxyAction : GLib.SourceProxy
		{
			readonly System.Action act;

			internal InvokeProxyAction (System.Action act)
			{
				this.act = act;
			}

			protected override bool Invoke ()
			{
				act ();
				return false;
			}
		}

		internal class InvokeProxyAction<T> : GLib.SourceProxy
		{
			readonly Action<T> act;
			readonly T arg;

			internal InvokeProxyAction (Action<T> act, T arg)
			{
				this.act = act;
				this.arg = arg;
			}

			protected override bool Invoke ()
			{
				act (arg);
				return false;
			}
		}

		public static void Invoke (EventHandler d)
		{
			var p = new InvokeProxy (d);
			var handle = GCHandle.Alloc (p);

			GLib.Timeout.Add (0, handle);
		}

		public static void Invoke (object sender, EventArgs args, EventHandler d)
		{
			var p = new InvokeProxyWithArgs (d, sender, args);
			var handle = GCHandle.Alloc (p);

			GLib.Timeout.Add (0, handle);
		}

		public static void Invoke (System.Action act)
		{
			var p = new InvokeProxyAction (act);
			var handle = GCHandle.Alloc (p);

			GLib.Timeout.Add (0, handle);
		}

		public static void Invoke<T> (Action<T> act, T arg)
		{
			var p = new InvokeProxyAction<T> (act, arg);
			var handle = GCHandle.Alloc (p);

			GLib.Timeout.Add (0, handle);
		}
	}
}
