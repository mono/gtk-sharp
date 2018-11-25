// Gdk.EventClient.cs - Custom client event wrapper 
//
// Author:  Mike Kestner <mkestner@ximian.com>
//
// Copyright (c) 2004 Novell, Inc.
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


namespace Gdk {

	using System;
	using System.Runtime.InteropServices;

	public class EventClient : Event {

		[DllImport("gdksharpglue-2", CallingConvention=CallingConvention.Cdecl)]
		static extern IntPtr gtksharp_gdk_event_client_get_message_type (IntPtr evt);

		[DllImport("gdksharpglue-2", CallingConvention=CallingConvention.Cdecl)]
		static extern ushort gtksharp_gdk_event_client_get_data_format (IntPtr evt);

		[DllImport("gdksharpglue-2", CallingConvention=CallingConvention.Cdecl)]
		static extern IntPtr gtksharp_gdk_event_client_get_data (IntPtr evt);

		public EventClient (IntPtr raw) : base (raw) { }
		public EventClient (IntPtr raw, bool owned) : base (raw, owned) { }

		public Atom MessageType {
			get {
				return new Atom (gtksharp_gdk_event_client_get_message_type (Handle));
			}
		}

		public ushort DataFormat {
			get {
				return gtksharp_gdk_event_client_get_data_format (Handle);
			}
		}

		public Array Data {
			get {
				switch (DataFormat) {
				case 8:
					byte[] b = new byte [20];
					Marshal.Copy (b, 0, gtksharp_gdk_event_client_get_data (Handle), 20);
					return b;
				case 16:
					short[] s = new short [10];
					Marshal.Copy (s, 0, gtksharp_gdk_event_client_get_data (Handle), 10);
					return s;
				case 32:
					IntPtr data_ptr = gtksharp_gdk_event_client_get_data (Handle);
					long[] l = new long [5];
					for (int i = 0; i < 5; i++)
						l [i] = (long) Marshal.ReadIntPtr (data_ptr, i * IntPtr.Size);
					return l;
				default:
					throw new Exception ("Invalid Data Format: " + DataFormat);
				}
			}
		}
	}
}

