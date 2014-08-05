// AppInfoAdapter.cs - customizations to GLib.AppInfoAdapter
//
// Authors: Stephane Delcroix  <stephane@delcroix.org>
//
// Copyright (c) 2008 Novell, Inc.
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
	
	public partial class AppInfo {
		[DllImport ("libgio-2.0-0.dll", CallingConvention = CallingConvention.Cdecl)]
		static extern IntPtr g_app_info_get_all();

		public static GLib.IAppInfoBase[] GetAll () {
			IntPtr raw_ret = g_app_info_get_all();
			return (GLib.IAppInfoBase[]) GLib.Marshaller.ListPtrToArray (raw_ret, typeof (GLib.List), true, false, typeof (GLib.IAppInfoBase));
		}
	}
}
