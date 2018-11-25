// NodeView.cs - a TreeView implementation that exposes ITreeNodes
//
// Author: Duncan Mak (duncan@ximian.com)
//
// Copyright (c) 2004 Novell, Inc.
//
// This program is free software; you can redistribute it and/or
// modify it under the terms of version 2 of the Lesser GNU General 
// Public License as published by the Free Software Foundation.
//
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.	 See the GNU
// Lesser General Public License for more details.
//
// You should have received a copy of the GNU Lesser General Public
// License along with this program; if not, write to the
// Free Software Foundation, Inc., 59 Temple Place - Suite 330,
// Boston, MA 02111-1307, USA.

namespace Gtk {

	using System;
	using System.Collections;
	using System.Reflection;
	using System.Runtime.InteropServices;

	public class NodeView : TreeView {

		NodeStore store;
		NodeSelection selection;

		public NodeView (NodeStore store) : base (IntPtr.Zero)
		{
			IntPtr[] names = { GLib.Marshaller.StringToPtrGStrdup ("model") };
			GLib.Value[] vals =  { new GLib.Value (store) };
			CreateNativeObject (names, vals, 0);
			this.store = store;
		}

		public NodeView () : base () {}

		[DllImport("libgtk-win32-2.0-0.dll", CallingConvention=CallingConvention.Cdecl)]
		static extern void gtk_tree_view_set_model(IntPtr raw, IntPtr model);

		[DllImport("libgobject-2.0-0.dll", CallingConvention=CallingConvention.Cdecl)]
		static extern void g_object_unref (IntPtr raw);

		public NodeStore NodeStore {
			get {
				return store;
			}
			set {
				var storeHandle = value == null ? IntPtr.Zero : value.Handle;
				gtk_tree_view_set_model (Handle, storeHandle);
				if (storeHandle != IntPtr.Zero)
					g_object_unref (storeHandle);
			}
		}

		public NodeSelection NodeSelection { 
			get {
				if (selection == null)
					selection = new NodeSelection (Selection);
				return selection;
			}
		}

		public Gtk.TreeViewColumn AppendColumn (string title, Gtk.CellRenderer cell, Gtk.NodeCellDataFunc cell_data) 
		{
			Gtk.TreeViewColumn col = new Gtk.TreeViewColumn ();
			col.Title = title;
			col.PackStart (cell, true);
			col.SetCellDataFunc (cell, cell_data);
			
			AppendColumn (col);
			return col;
		}
	}
}

