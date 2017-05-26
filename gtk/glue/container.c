/* container.c : Glue for GtkContainer
 *
 * Author:  Mike Kestner (mkestner@ximian.com)
 *
 * Copyright (C) 2004 Novell, Inc.
 *
 * This program is free software; you can redistribute it and/or
 * modify it under the terms of version 2 of the Lesser GNU General 
 * Public License as published by the Free Software Foundation.
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
 * Lesser General Public License for more details.
 *
 * You should have received a copy of the GNU Lesser General Public
 * License along with this program; if not, write to the
 * Free Software Foundation, Inc., 59 Temple Place - Suite 330,
 * Boston, MA 02111-1307, USA.
 */

#include <string.h>
#include <gtk/gtkcontainer.h>

//if this can be invoked without an EntryPointNotFoundExfeption, the caller knows the container leak leak is fixed
void gtksharp_container_leak_fixed_marker (void);

void gtksharp_container_leak_fixed_marker (void)
{
}

void gtksharp_container_base_forall (GtkContainer *container, gboolean include_internals, GtkCallback cb, gpointer data);

void 
gtksharp_container_base_forall (GtkContainer *container, gboolean include_internals, GtkCallback cb, gpointer data)
{
	// Find and call the first base callback that's not the GTK# callback. The GTK# callback calls down the whole
	// managed override chain, so calling it on a subclass-of-a-managed-container-subclass causes a stack overflow.
	GtkContainerClass *parent = (GtkContainerClass *) G_OBJECT_GET_CLASS (container);
	while ((parent = g_type_class_peek_parent (parent))) {
		if (strncmp (G_OBJECT_CLASS_NAME (parent), "__gtksharp_", 11) != 0) {
			if (parent->forall) {
				(*parent->forall) (container, include_internals, cb, data);
			}
			return;
		}
	}
}

void gtksharp_container_override_forall (GType gtype, gpointer cb);

void 
gtksharp_container_override_forall (GType gtype, gpointer cb)
{
	GtkContainerClass *klass = g_type_class_peek (gtype);
	if (!klass)
		klass = g_type_class_ref (gtype);
	((GtkContainerClass *) klass)->forall = cb;
}

void gtksharp_container_invoke_gtk_callback (GtkCallback cb, GtkWidget *widget, gpointer data);

void 
gtksharp_container_invoke_gtk_callback (GtkCallback cb, GtkWidget *widget, gpointer data)
{
	cb (widget, data);
}

void gtksharp_container_override_child_type (GType gtype, gpointer cb);

void
gtksharp_container_override_child_type (GType gtype, gpointer cb)
{
	GtkContainerClass *klass = g_type_class_peek (gtype);
	if (!klass)
		klass = g_type_class_ref (gtype);
	((GtkContainerClass *) klass)->child_type = cb;
}

void gtksharp_container_child_get_property (GtkContainer *container, GtkWidget *child,
					    const gchar* property, GValue *value);

void
gtksharp_container_child_get_property (GtkContainer *container, GtkWidget *child,
				       const gchar* property, GValue *value)
{
	GParamSpec *spec = gtk_container_class_find_child_property (G_OBJECT_GET_CLASS (container), property);
	g_value_init (value, spec->value_type);
	gtk_container_child_get_property (container, child, property, value);
}

