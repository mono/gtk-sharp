// GtkSharp.Generation.MarshalGen.cs - Simple marshaling Generatable.
//
// Author: Mike Kestner <mkestner@novell.com>
//
// Copyright (c) 2004 Novell, Inc.
//
// This program is free software; you can redistribute it and/or
// modify it under the terms of version 2 of the GNU General Public
// License as published by the Free Software Foundation.
//
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
// General Public License for more details.
//
// You should have received a copy of the GNU General Public
// License along with this program; if not, write to the
// Free Software Foundation, Inc., 59 Temple Place - Suite 330,
// Boston, MA 02111-1307, USA.


namespace GtkSharp.Generation {

	using System;

	public class MarshalGen : SimpleBase {
		
		string mtype;
		string call_fmt;
		string from_fmt;
		bool freeAfterUse;

		public MarshalGen (string ctype, string type, string mtype, string call_fmt, string from_fmt) : this (ctype, type, mtype, call_fmt, from_fmt, true)
		{
		}

		public MarshalGen (string ctype, string type, string mtype, string call_fmt, string from_fmt, bool freeAfterUse) : base (ctype, type, "null")
		{
			this.mtype = mtype;
			this.call_fmt = call_fmt;
			this.from_fmt = from_fmt;
			FreeAfterUse = freeAfterUse;
		}

		public bool FreeAfterUse { get; private set; }

		public override string MarshalType {
			get {
				return mtype;
			}
		}

		public override string CallByName (string var)
		{
			return String.Format (call_fmt, var);
		}
		
		public override string FromNative (string var)
		{
			return String.Format (from_fmt, var);
		}
	}

	public class StringMarshalGen : MarshalGen, IManualMarshaler
	{
		public StringMarshalGen (string ctype, string type, string mtype, string call_fmt, string from_fmt) : base (ctype, type, mtype, call_fmt, from_fmt, true)
		{
		}

		public string AllocNative (string managed_var)
		{
			return CallByName (managed_var);
		}

		public string ReleaseNative (string native_var)
		{
			return string.Format ("GLib.Marshaller.Free ({0})", native_var);
		}
	}
}

