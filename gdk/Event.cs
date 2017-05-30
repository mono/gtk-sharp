// Gdk.Event.cs - Custom event wrapper 
//
// Authors: Rachel Hestilow <hestilow@ximian.com> 
//          Mike Kestner <mkestner@ximian.com>
//
// Copyright (c) 2002 Rachel Hestilow
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

	public class Event : GLib.IWrapper {

		[DllImport("gdksharpglue-2", CallingConvention=CallingConvention.Cdecl)]
		static extern EventType gtksharp_gdk_event_get_event_type (IntPtr evt);

		[DllImport("gdksharpglue-2", CallingConvention=CallingConvention.Cdecl)]
		static extern IntPtr gtksharp_gdk_event_get_window (IntPtr evt);

		[DllImport("gdksharpglue-2", CallingConvention=CallingConvention.Cdecl)]
		static extern sbyte gtksharp_gdk_event_get_send_event (IntPtr evt);

		[DllImport("libgdk-win32-2.0-0.dll", CallingConvention=CallingConvention.Cdecl)]
		static extern IntPtr gdk_event_get_type ();

		IntPtr raw;
		bool owned;

		[DllImport ("libgdk-win32-2.0-0.dll", CallingConvention = CallingConvention.Cdecl)]
		static extern void gdk_event_free (IntPtr raw);

		~Event()
		{
			gdk_event_free (raw);
		}

		public Event(IntPtr raw) : this(raw, false)
		{
		}

		public Event(IntPtr raw, bool owned) 
		{
			this.raw = raw;
			this.owned = owned;
			if (!owned)
				System.GC.SuppressFinalize (this);
		}

		public IntPtr Handle {
			get {
				return raw;
			}
		}

		static GLib.GType _gtype = new GLib.GType (gdk_event_get_type ());
		public static GLib.GType GType {
			get {
				return _gtype;
			}
		}

		public EventType Type {
			get {
				return gtksharp_gdk_event_get_event_type (Handle);
			}
		}

		public Window Window {
			get {
				return GLib.Object.GetObject (gtksharp_gdk_event_get_window (Handle)) as Window;
			}
		}

		public bool SendEvent {
			get {
				return gtksharp_gdk_event_get_send_event (Handle) == 0 ? false : true;
			}
		}

		public static Event New (IntPtr raw)
		{
			return GetEvent (raw);
		}

		public static Event GetEvent (IntPtr raw)
		{
			return GetEvent (raw, false);
		}

		public static Event GetEvent (IntPtr raw, bool owned)
		{
			if (raw == IntPtr.Zero)
				return null;

			switch (gtksharp_gdk_event_get_event_type (raw)) {
			case EventType.Expose:
				return new EventExpose (raw, owned);
			case EventType.MotionNotify:
				return new EventMotion (raw, owned);
			case EventType.ButtonPress:
			case EventType.TwoButtonPress:
			case EventType.ThreeButtonPress:
			case EventType.ButtonRelease:
				return new EventButton (raw, owned);
			case EventType.KeyPress:
			case EventType.KeyRelease:
				return new EventKey (raw, owned);
			case EventType.EnterNotify:
			case EventType.LeaveNotify:
				return new EventCrossing (raw, owned);
			case EventType.FocusChange:
				return new EventFocus (raw, owned);
			case EventType.Configure:
				return new EventConfigure (raw, owned);
			case EventType.PropertyNotify:
				return new EventProperty (raw, owned);
			case EventType.SelectionClear:
			case EventType.SelectionRequest:
			case EventType.SelectionNotify:
				return new EventSelection (raw, owned);
			case EventType.ProximityIn:
			case EventType.ProximityOut:
				return new EventProximity (raw, owned);
			case EventType.DragEnter:
			case EventType.DragLeave:
			case EventType.DragMotion:
			case EventType.DragStatus:
			case EventType.DropStart:
			case EventType.DropFinished:
				return new EventDND (raw, owned);
			case EventType.ClientEvent:
				return new EventClient (raw, owned);
			case EventType.VisibilityNotify:
				return new EventVisibility (raw, owned);
			case EventType.Scroll:
				return new EventScroll (raw, owned);
			case EventType.WindowState:
				return new EventWindowState (raw, owned);
			case EventType.Setting:
				return new EventSetting (raw, owned);
#if GTK_SHARP_2_6
			case EventType.OwnerChange:
				return new EventOwnerChange (raw, owned);
#endif
#if GTK_SHARP_2_8
			case EventType.GrabBroken:
				return new EventGrabBroken (raw, owned);
#endif
			case EventType.Map:
			case EventType.Unmap:
			case EventType.NoExpose:
			case EventType.Delete:
			case EventType.Destroy:
			default:
				return new Gdk.Event (raw, owned);
			}
		}
	}
}

