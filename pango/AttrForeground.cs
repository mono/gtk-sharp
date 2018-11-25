// Pango.AttrForeground - Pango.Attribute for foreground color
//
// Copyright (c) 2005 Novell, Inc.
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

namespace Pango {

	using System;
	using System.Runtime.InteropServices;

	public class AttrForeground : Attribute {

		[DllImport("libpango-1.0-0.dll", CallingConvention=CallingConvention.Cdecl)]
		static extern IntPtr pango_attr_foreground_new (ushort red, ushort green, ushort blue);

		public AttrForeground (ushort red, ushort green, ushort blue) : this (pango_attr_foreground_new (red, green, blue), true) {}

		public AttrForeground (Pango.Color color) : this (pango_attr_foreground_new (color.Red, color.Green, color.Blue), true) {}

		internal AttrForeground (IntPtr raw, bool owned) : base (raw, owned) {}

		[DllImport("pangosharpglue-2", CallingConvention=CallingConvention.Cdecl)]
		static extern Pango.Color pangosharp_attr_color_get_color (IntPtr raw);

		public Pango.Color Color {
			get {
				return pangosharp_attr_color_get_color (Handle);
			}
		}
	}
}
