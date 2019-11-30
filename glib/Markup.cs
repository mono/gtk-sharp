// Markup.cs: Wrapper for the Markup code in Glib
//
// Authors:
//    Miguel de Icaza (miguel@ximian.com)
//
// Copyright (c) 2003 Ximian, Inc.
//
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

namespace GLib {


	public class Markup {
		private Markup () {}

		delegate IntPtr EscapeTextDelegate (IntPtr text, IntPtr len);
		
		[DllImport("libglib-2.0-0.dll", CallingConvention = CallingConvention.Cdecl)]
		static extern IntPtr g_markup_escape_text (IntPtr text, IntPtr len);

		[DllImport ("libglib-2.0-0.dll", EntryPoint="g_markup_escape_text", CallingConvention = CallingConvention.Cdecl)]
		static extern IntPtr g_markup_escape_text_win32 (IntPtr text, int len);

		static IntPtr EscapeTextWindows (IntPtr text, IntPtr len)
		{
			return g_markup_escape_text_win32 (text, (int)len);
		}

		static IntPtr EscapeTextUnix (IntPtr text, IntPtr len)
		{
			return g_markup_escape_text (text, len);
		}

		static readonly EscapeTextDelegate escapeText = System.IO.Path.DirectorySeparatorChar == '\\'
			? new EscapeTextDelegate (EscapeTextWindows) : EscapeTextUnix;

		static public string EscapeText (string s)
		{
			if (s == null)
				return string.Empty;

			IntPtr len;
			IntPtr native = Marshaller.StringToPtrGStrdup (s, out len);
			string result = Marshaller.PtrToStringGFree (escapeText (native, len));
			Marshaller.Free (native);
			return result;
		}
	}
}
