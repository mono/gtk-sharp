//
// Application.cs
//
// Author(s):
//	Antonius Riha <antoniusriha@gmail.com>
//
// Copyright (c) 2014 Antonius Riha
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

using System;
using System.Runtime.InteropServices;

namespace GLib
{
	public partial class Application
	{
		public Application () : this (null, ApplicationFlags.None)
		{
		}

		[DllImport (GioGlobal.GioNativeDll, CallingConvention = CallingConvention.Cdecl)]
		static extern int g_application_run (IntPtr raw, int argc, IntPtr argv);

		public int Run ()
		{
			return Run (null, null);
		}

		public int Run (string progName, string[] args)
		{
			var argc = 0;
			var argv = IntPtr.Zero;
			if (progName != null) {
				if (progName.Trim () == string.Empty) {
					throw new ArgumentException ("progName must not be empty.", "progName");
				}

				if (args == null) {
					throw new ArgumentNullException ("args");
				}

				var progArgs = new string[args.Length + 1];
				progArgs [0] = progName;
				args.CopyTo (progArgs, 1);

				argc = progArgs.Length;
				argv = new Argv (progArgs).Handle;
			}

			return g_application_run (Handle, argc, argv);
		}
	}
}
