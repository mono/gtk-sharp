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


void atksharp_object_override_get_n_children (GType gtype, gpointer cb);

int atksharp_object_base_get_n_children (AtkObject *base);

void atksharp_object_override_ref_child (GType gtype, gpointer cb);

AtkObject *atksharp_object_base_ref_child (AtkObject *base, gint i);

void atksharp_object_override_ref_state_set (GType gtype, gpointer cb);

AtkStateSet* atksharp_object_base_ref_state_set (AtkObject *base);

void atksharp_object_override_get_index_in_parent (GType gtype, gpointer cb);

gint atksharp_object_base_get_index_in_parent (AtkObject *base);

void atksharp_object_override_ref_relation_set (GType gtype, gpointer cb);

AtkRelationSet* atksharp_object_base_ref_relation_set (AtkObject *base);

void atksharp_object_override_get_attributes(GType gtype, gpointer cb);

AtkAttributeSet* atksharp_object_base_get_attributes (AtkObject *base);

void
atksharp_object_override_get_n_children (GType gtype, gpointer cb)
{
	AtkObjectClass *klass = g_type_class_peek (gtype);
	if (!klass)
		klass = g_type_class_ref (gtype);
	((AtkObjectClass *) klass)->get_n_children = cb;
}

int
atksharp_object_base_get_n_children (AtkObject *base)
{
	AtkObjectClass *parent = (AtkObjectClass *)get_threshold_class (G_OBJECT (base));
	if (parent->get_n_children)
		return (*parent->get_n_children) (base);
	return 0;
}

void
atksharp_object_override_ref_child (GType gtype, gpointer cb)
{
	AtkObjectClass *klass = g_type_class_peek (gtype);
	if (!klass)
		klass = g_type_class_ref (gtype);
	((AtkObjectClass *) klass)->ref_child = cb;
}

AtkObject *
atksharp_object_base_ref_child (AtkObject *base, gint i)
{
	AtkObjectClass *parent = (AtkObjectClass *)get_threshold_class (G_OBJECT (base));
	if (parent->ref_child)
		return (*parent->ref_child) (base, i);
	return NULL;
}

void
atksharp_object_override_ref_state_set (GType gtype, gpointer cb)
{
	AtkObjectClass *klass = g_type_class_peek (gtype);
	if (!klass)
		klass = g_type_class_ref (gtype);
	((AtkObjectClass *) klass)->ref_state_set = cb;
}

AtkStateSet*
atksharp_object_base_ref_state_set (AtkObject *atk_obj)
{
	AtkObjectClass *parent = (AtkObjectClass *)get_threshold_class (G_OBJECT (atk_obj));
	if (parent->ref_state_set)
		return (*parent->ref_state_set) (atk_obj);
	return NULL;
}

void
atksharp_object_override_get_index_in_parent (GType gtype, gpointer cb)
{
	AtkObjectClass *klass = g_type_class_peek (gtype);
	if (!klass)
		klass = g_type_class_ref (gtype);
	((AtkObjectClass *) klass)->get_index_in_parent = cb;
}

gint
atksharp_object_base_get_index_in_parent (AtkObject *atk_obj)
{
	AtkObjectClass *parent = (AtkObjectClass *)get_threshold_class (G_OBJECT (atk_obj));
	if (parent->get_index_in_parent)
		return (*parent->get_index_in_parent) (atk_obj);
	return -1;
}

void
atksharp_object_override_ref_relation_set (GType gtype, gpointer cb)
{
	AtkObjectClass *klass = g_type_class_peek (gtype);
	if (!klass)
		klass = g_type_class_ref (gtype);
	((AtkObjectClass *) klass)->ref_relation_set = cb;
}

AtkRelationSet*
atksharp_object_base_ref_relation_set (AtkObject *atk_obj)
{
	AtkObjectClass *parent = (AtkObjectClass *)get_threshold_class (G_OBJECT (atk_obj));
	if (parent->ref_relation_set)
		return (*parent->ref_relation_set) (atk_obj);
	return NULL;
}


void
atksharp_object_override_get_attributes (GType gtype, gpointer cb)
{
	AtkObjectClass *klass = g_type_class_peek (gtype);
	if (!klass)
		klass = g_type_class_ref (gtype);
	((AtkObjectClass *) klass)->get_attributes = cb;
}

AtkAttributeSet*
atksharp_object_base_get_attributes (AtkObject *base)
{
	AtkObjectClass *parent = (AtkObjectClass *)get_threshold_class (G_OBJECT (base));
	if (parent->get_attributes)
		return (*parent->get_attributes) (base);
	return NULL;
}
