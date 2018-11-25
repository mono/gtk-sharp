/* object.c : Glue for overriding vms of AtkObject
 *
 * Author: Andres G. Aragoneses  <aaragoneses@novell.com>
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


void atksharp_object_factory_override_create_accessible (GType gtype, gpointer cb);
AtkObject *atksharp_object_factory_base_create_accessible (AtkObjectFactory *factory, GObject *obj);

void atksharp_object_factory_override_invalidate (GType gtype, gpointer cb);
void atksharp_object_factory_base_invalidate (AtkObjectFactory *factory);

void atksharp_object_factory_override_get_accessible_type (GType gtype, gpointer cb);
GType atksharp_object_factory_base_get_accessible_type (AtkObjectFactory *factory);

void
atksharp_object_factory_override_create_accessible (GType gtype, gpointer cb)
{
	AtkObjectFactoryClass *klass = g_type_class_peek (gtype);
	if (!klass)
		klass = g_type_class_ref (gtype);
	klass->create_accessible = cb;
}

AtkObject *
atksharp_object_factory_base_create_accessible (AtkObjectFactory *factory, GObject *obj)
{
	AtkObjectFactoryClass *parent = (AtkObjectFactoryClass *)get_threshold_class (G_OBJECT (factory));
	if (parent->create_accessible)
		return (*parent->create_accessible) (obj);
	return NULL;
}

void
atksharp_object_factory_override_invalidate (GType gtype, gpointer cb)
{
	AtkObjectFactoryClass *klass = g_type_class_peek (gtype);
	if (!klass)
		klass = g_type_class_ref (gtype);
	klass->invalidate = cb;
}

void
atksharp_object_factory_base_invalidate (AtkObjectFactory *factory)
{
	AtkObjectFactoryClass *parent = (AtkObjectFactoryClass *)get_threshold_class (G_OBJECT (factory));
	if (parent->invalidate)
		(*parent->invalidate) (factory);
}


void
atksharp_object_factory_override_get_accessible_type (GType gtype, gpointer cb)
{
	AtkObjectFactoryClass *klass = g_type_class_peek (gtype);
	if (!klass)
		klass = g_type_class_ref (gtype);
	klass->get_accessible_type = cb;
}

GType
atksharp_object_factory_base_get_accessible_type (AtkObjectFactory *factory)
{
	AtkObjectFactoryClass *parent = (AtkObjectFactoryClass *)get_threshold_class (G_OBJECT (factory));
	if (parent->get_accessible_type)
		return (*parent->get_accessible_type) ();

	return G_TYPE_INVALID;
}
