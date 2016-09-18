// Pango.Attribute - Attribute "base class"
//
// Copyright (c) 2005, 2007 Novell, Inc.
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

	public class Attribute : GLib.IWrapper, IDisposable {

		IntPtr raw;
		bool owned;

		internal Attribute (IntPtr raw)
		{
			this.raw = raw;
		}

		[DllImport("pangosharpglue-2", CallingConvention=CallingConvention.Cdecl)]
		static extern Pango.AttrType pangosharp_attribute_get_attr_type (IntPtr raw);

		public static Attribute GetAttribute (IntPtr raw, bool owned)
		{
			Attribute attr;
			switch (pangosharp_attribute_get_attr_type (raw)) {
			case Pango.AttrType.Language:
				attr = new AttrLanguage (raw);
				break;
			case Pango.AttrType.Family:
				attr = new AttrFamily (raw);
				break;
			case Pango.AttrType.Style:
				attr = new AttrStyle (raw);
				break;
			case Pango.AttrType.Weight:
				attr = new AttrWeight (raw);
				break;
			case Pango.AttrType.Variant:
				attr = new AttrVariant (raw);
				break;
			case Pango.AttrType.Stretch:
				attr = new AttrStretch (raw);
				break;
			case Pango.AttrType.Size:
				attr = new AttrSize (raw);
				break;
			case Pango.AttrType.FontDesc:
				attr = new AttrFontDesc (raw);
				break;
			case Pango.AttrType.Foreground:
				attr = new AttrForeground (raw);
				break;
			case Pango.AttrType.Background:
				attr = new AttrBackground (raw);
				break;
			case Pango.AttrType.Underline:
				attr = new AttrUnderline (raw);
				break;
			case Pango.AttrType.Strikethrough:
				attr = new AttrStrikethrough (raw);
				break;
			case Pango.AttrType.Rise:
				attr = new AttrRise (raw);
				break;
			case Pango.AttrType.Shape:
				attr = new AttrShape (raw);
				break;
			case Pango.AttrType.Scale:
				attr = new AttrScale (raw);
				break;
			case Pango.AttrType.Fallback:
				attr = new AttrFallback (raw);
				break;
#if GTK_SHARP_2_6
			case Pango.AttrType.LetterSpacing:
				attr = new AttrLetterSpacing (raw);
				break;
			case Pango.AttrType.UnderlineColor:
				attr = new AttrUnderlineColor (raw);
				break;
			case Pango.AttrType.StrikethroughColor:
				attr = new AttrStrikethroughColor (raw);
				break;
#endif
#if GTK_SHARP_2_12
			case Pango.AttrType.Gravity:
				attr = new AttrGravity (raw);
				break;
			case Pango.AttrType.GravityHint:
				attr = new AttrGravityHint (raw);
				break;
#endif
			default:
				attr = new Attribute (raw);
				break;
			}
			attr.owned = owned;
			return attr;
		}

		public static Attribute GetAttribute (IntPtr raw)
		{
			return GetAttribute (raw, false);
		}

		[DllImport("libpango-1.0-0.dll", CallingConvention=CallingConvention.Cdecl)]
		static extern void pango_attribute_destroy (IntPtr raw);

		~Attribute ()
		{
			Dispose ();
		}

		public void Dispose ()
		{
			if (!owned)
				return;
			
			if (raw != IntPtr.Zero) {
				pango_attribute_destroy (raw);
				raw = IntPtr.Zero;
			}
		}

		public IntPtr Handle {
			get {
				return raw;
			}
		}

		public static GLib.GType GType {
			get {
				return GLib.GType.Pointer;
			}
		}

		public Pango.AttrType Type {
			get {
				return pangosharp_attribute_get_attr_type (raw);
			}
		}

		[DllImport("pangosharpglue-2", CallingConvention=CallingConvention.Cdecl)]
		static extern uint pangosharp_attribute_get_start_index (IntPtr raw);

		[DllImport("pangosharpglue-2", CallingConvention=CallingConvention.Cdecl)]
		static extern void pangosharp_attribute_set_start_index (IntPtr raw, uint index);

		public uint StartIndex {
			get {
				return pangosharp_attribute_get_start_index (raw);
			}
			set {
				pangosharp_attribute_set_start_index (raw, value);
			}
		}

		[DllImport("pangosharpglue-2", CallingConvention=CallingConvention.Cdecl)]
		static extern uint pangosharp_attribute_get_end_index (IntPtr raw);

		[DllImport("pangosharpglue-2", CallingConvention=CallingConvention.Cdecl)]
		static extern void pangosharp_attribute_set_end_index (IntPtr raw, uint index);

		public uint EndIndex {
			get {
				return pangosharp_attribute_get_end_index (raw);
			}
			set {
				pangosharp_attribute_set_end_index (raw, value);
			}
		}

		[DllImport("libpango-1.0-0.dll", CallingConvention=CallingConvention.Cdecl)]
		static extern IntPtr pango_attribute_copy (IntPtr raw);

		public Pango.Attribute Copy () {
			return GetAttribute (pango_attribute_copy (raw), true);
		}

		[DllImport("libpango-1.0-0.dll", CallingConvention=CallingConvention.Cdecl)]
		static extern bool pango_attribute_equal (IntPtr raw1, IntPtr raw2);

		public bool Equal (Pango.Attribute attr2) {
			return pango_attribute_equal (raw, attr2.raw);
		}
	}
}
