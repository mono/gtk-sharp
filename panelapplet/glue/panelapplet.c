#include <panel-applet.h>

void
panelapplet_sharp_register_applet (gchar *iid, gpointer callback)
{
	panel_applet_factory_main (iid, PANEL_TYPE_APPLET, callback, NULL);
}
