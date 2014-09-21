// TextAdapter.cs - Atk TextAdapter class customizations
//
// Author: Brad Taylor <brad@getcoded.net>
//
// Copyright (c) 2008 Novell, Inc.
//
// This code is inserted after the automatically generated code.
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

namespace Atk {
	public partial class Text {

		public void EmitTextChanged (TextChangedDetail detail, int position, int length)
		{
			GLib.Signal.Emit (GLib.Object.GetObject (Handle),
			                  "text_changed::" + detail.ToString ().ToLower (),
			                  position, length);
		}
	}
}
