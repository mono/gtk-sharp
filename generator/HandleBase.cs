// HandleBase.cs - Base class for Handle types
//
// Authors:  Mike Kestner <mkestner@novell.com>
//
// Copyright (c) 2005 Novell, Inc.
//
// This program is free software; you can redistribute it and/or
// modify it under the terms of version 2 of the GNU General Public
// License as published by the Free Software Foundation.
//
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
// General Public License for more details.
//
// You should have received a copy of the GNU General Public
// License along with this program; if not, write to the
// Free Software Foundation, Inc., 59 Temple Place - Suite 330,
// Boston, MA 02111-1307, USA.


namespace GtkSharp.Generation {

	using System;
	using System.IO;
	using System.Xml;

	public abstract class HandleBase : ClassBase, IAccessor, IOwnable {

		protected HandleBase (XmlElement ns, XmlElement elem, AssemblyMetadataClassGenerator assemblyMetadataClassGen)
			: base (ns, elem, assemblyMetadataClassGen) {}
					
		public override string AssignToName {
			get {
				return "Raw";
			}
		}

		public override string MarshalType {
			get {
				return "IntPtr";
			}
		}

		public override string CallByName (string name)
		{
			return name + " == null ? IntPtr.Zero : " + name + ".Handle";
		}

		public override string CallByName ()
		{
			return "Handle";
		}

		public abstract string FromNative (string var, bool owned);

		public override string FromNative (string var)
		{
			return FromNative (var, false);
		}

		public void WriteAccessors (StreamWriter sw, string indent, string var)
		{
			sw.WriteLine (indent + "get {");
			sw.WriteLine (indent + "\treturn " + FromNative (var, false) + ";");
			sw.WriteLine (indent + "}");
			sw.WriteLine (indent + "set {");
			sw.WriteLine (indent + "\t" + var + " = " + CallByName ("value") + ";");
			sw.WriteLine (indent + "}");
		}
	}
}
