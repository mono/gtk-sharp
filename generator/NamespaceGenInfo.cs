// NamespaceGenInfo.cs
//
// Author: Antonius Riha <antoniusriha@gmail.com>
//
// Copyright (c) 2014 Antonius Riha
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
using System;
using System.Xml;

namespace GtkSharp.Generation
{
	public class NamespaceGenInfo
	{
		readonly XmlElement nsElem;

		readonly AssemblyMetadataClassGenerator assemblyMetadataClassGen;

		readonly LibraryNameHandle libNameHandle;

		string libName;

		public NamespaceGenInfo (XmlElement nsElem) : this (nsElem, null) {}

		public NamespaceGenInfo (XmlElement nsElem, AssemblyMetadataClassGenerator assemblyMetadataClassGen)
		{
			if (nsElem == null) {
				throw new ArgumentNullException ("nsElem");
			}

			this.nsElem = nsElem;
			this.assemblyMetadataClassGen = assemblyMetadataClassGen;

			var xmlLibName = NamespaceElement.GetAttribute ("library");
			if (string.IsNullOrEmpty (xmlLibName)) {
				libName = "LIB_NAME_NOT_SET";
			} else {
				if (assemblyMetadataClassGen != null) {
					libNameHandle = assemblyMetadataClassGen.AddLibrary (xmlLibName);
				} else {
					libName = "\"" + xmlLibName + "\"";
				}
			}
		}

		public XmlElement NamespaceElement {
			get { return nsElem; }
		}

		public AssemblyMetadataClassGenerator AssemblyMetadataClassGenerator {
			get { return assemblyMetadataClassGen; }
		}

		public string LibraryName {
			get {
				if (libName == null) {
					libName = libNameHandle.GetLibraryNameExpression ();
				}

				return libName;
			}
		}
	}
}
