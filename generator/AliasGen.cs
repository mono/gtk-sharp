// GtkSharp.Generation.AliasGen.cs - The Alias type Generatable.
//
// Author: Mike Kestner <mkestner@speakeasy.net>
//
// Copyright (c) 2003 Mike Kestner
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
	using System.Xml;

	public class AliasGen : SimpleBase {
		
		protected override void ParseElement(XmlElement ns, XmlElement elem)
		{
			base.ParseElement(ns, elem);

			Name = elem.GetName ();
		}

		protected override void ParseChildElement(XmlElement ns, XmlElement childElement)
		{
			if (childElement.Name == Constants.Type) {
				CName = childElement.GetCType ();
				return;
			}
			base.ParseChildElement(ns, childElement);
		}
	}
}

