/* misc.c : Glue for overriding vms of AtkMisc
 *
 * Author: Mike Kestner  <mkestner@novell.com>
 * 
 * Copyright (c) 2008 Novell, Inc.
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

#include <atk/atk.h>

static const gchar *__prefix = "__gtksharp_";

#define HAS_PREFIX(a) (*((guint64 *)(a)) == *((guint64 *) __prefix))

static GObjectClass *
get_threshold_class (GObject *obj)
{
	GType gtype = G_TYPE_FROM_INSTANCE (obj);
	while (HAS_PREFIX (g_type_name (gtype)))
		gtype = g_type_parent (gtype);
	GObjectClass *klass = g_type_class_peek (gtype);
	if (klass == NULL) klass = g_type_class_ref (gtype);
	return klass;
}

void atksharp_misc_override_threads_enter (GType gtype, gpointer cb);
void atksharp_misc_base_threads_enter (AtkMisc *misc);

void 
atksharp_misc_override_threads_enter (GType gtype, gpointer cb)
{
	AtkMiscClass *klass = g_type_class_peek (gtype);
	if (!klass)
		klass = g_type_class_ref (gtype);
	((AtkMiscClass *) klass)->threads_enter = cb;
}

void 
atksharp_misc_base_threads_enter (AtkMisc *misc)
{
	AtkMiscClass *parent = (AtkMiscClass *)get_threshold_class (G_OBJECT (misc));
	if (parent->threads_enter)
		(*parent->threads_enter) (misc);
}

void atksharp_misc_override_threads_leave (GType gtype, gpointer cb);
void atksharp_misc_base_threads_leave (AtkMisc *misc);

void 
atksharp_misc_override_threads_leave (GType gtype, gpointer cb)
{
	AtkMiscClass *klass = g_type_class_peek (gtype);
	if (!klass)
		klass = g_type_class_ref (gtype);
	((AtkMiscClass *) klass)->threads_leave = cb;
}

void 
atksharp_misc_base_threads_leave (AtkMisc *misc)
{
	AtkMiscClass *parent = (AtkMiscClass *)get_threshold_class (G_OBJECT (misc));
	if (parent->threads_leave)
		(*parent->threads_leave) (misc);
}

void atksharp_misc_set_singleton_instance (AtkMisc *misc);

void 
atksharp_misc_set_singleton_instance (AtkMisc *misc)
{
	atk_misc_instance = misc;
}
