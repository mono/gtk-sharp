//
// WidgetFlags.cs: GtkWidgetFlags enum.
//
// Author:
//   Jeroen Zwartepoorte <jeroen@xs4all.nl>
//
// (C) Copyright Jeroen Zwartepoorte 2004
//

namespace Gtk {
	public enum WidgetFlags {
		Toplevel        = 1 << 4,
		NoWindow        = 1 << 5,
		Realized        = 1 << 6,
		Mapped          = 1 << 7,
		Visible         = 1 << 8,
		Sensitive       = 1 << 9,
		ParentSensitive = 1 << 10,
		CanFocus        = 1 << 11,
		HasFocus        = 1 << 12,

		/* widget is allowed to receive the default via gtk_widget_grab_default
		 * and will reserve space to draw the default if possible
		 */
		CanDefault      = 1 << 13,

		/* the widget currently is receiving the default action and should be drawn
		 * appropriately if possible
		 */
		HasDefault      = 1 << 14,

		HasGrab	        = 1 << 15,
		RcStyle	        = 1 << 16,
		CompositeChild  = 1 << 17,
		NoReparent      = 1 << 18,
		AppPaintable    = 1 << 19,

		/* the widget when focused will receive the default action and have
		 * HAS_DEFAULT set even if there is a different widget set as default
		 */
		ReceivesDefault = 1 << 20,

		DoubleBuffered  = 1 << 21,
		NoShowAll       = 1 << 22
	}
}
