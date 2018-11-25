// Pango.AttrFontDesc - Pango.Attribute for Pango.FontDescription
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

	public class AttrFontDesc : Attribute {

		[DllImport("libpango-1.0-0.dll", CallingConvention=CallingConvention.Cdecl)]
		static extern IntPtr pango_attr_font_desc_new (IntPtr font_desc);

		[DllImport("libpango-1.0-0.dll", CallingConvention=CallingConvention.Cdecl)]
		static extern IntPtr pango_font_description_copy(IntPtr raw);

		public AttrFontDesc (Pango.FontDescription font_desc) : this (pango_attr_font_desc_new (pango_font_description_copy (font_desc.Handle)), true) {}

		internal AttrFontDesc (IntPtr raw, bool owned) : base (raw, owned) {}

		[DllImport("pangosharpglue-2", CallingConvention=CallingConvention.Cdecl)]
		static extern IntPtr pangosharp_attr_font_desc_get_desc (IntPtr raw);

		public Pango.FontDescription Desc {
			get {
				IntPtr raw_ret = pangosharp_attr_font_desc_get_desc (Handle);
				return new Pango.FontDescription (raw_ret);
			}
		}
	}
}
