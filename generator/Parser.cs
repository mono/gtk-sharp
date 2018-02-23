﻿// GtkSharp.Generation.Parser.cs - The XML Parsing engine.
//
// Author: Mike Kestner <mkestner@speakeasy.net>
//
// Copyright (c) 2001-2003 Mike Kestner
// Copyright (c) 2003 Ximian Inc.
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
using System.Collections.Generic;
using System.IO;
using System.Xml;

namespace GtkSharp.Generation {

	public class Parser  {
		
		private XmlDocument Load (string filename)
		{
			XmlDocument doc = new XmlDocument ();

			try {
				Stream stream = File.OpenRead (filename);
				doc.Load (stream);
				stream.Close ();
			} catch (XmlException e) {
				Console.WriteLine ("Invalid XML file.");
				Console.WriteLine (e);
				doc = null;
			}

			return doc;
		}

		public IGeneratable[] Parse (string filename)
		{
			XmlDocument doc = Load (filename);
			if (doc == null)
				return null;

			XmlElement root = doc.DocumentElement;

			if ((root == null) || !root.HasChildNodes) {
				Console.WriteLine ("No Namespaces found.");
				return null;
			}

			var gens = new List<IGeneratable>();

			foreach (XmlNode child in root.ChildNodes) {
				XmlElement elem = child as XmlElement;
				if (elem == null)
					continue;

				switch (child.Name) {
				case Constants.Namespace:
					gens.AddRange (ParseNamespace (elem));
					break;
				case Constants.Symbol:
					gens.Add (ParseSymbol (elem));
					break;
				default:
					Console.WriteLine ("Parser::Parse - Unexpected child node: " + child.Name);
					break;
				}
			}

			return gens.ToArray();
		}

		private List<IGeneratable> ParseNamespace (XmlElement ns)
		{
			var result = new List<IGeneratable>();

			var seen = new HashSet<string> ();
			foreach (XmlNode def in ns.ChildNodes) {

				XmlElement elem = def as XmlElement;
				if (elem == null)
					continue;

				if (elem.HasAttribute(Constants.Hidden))
					continue;

				bool is_opaque = false;
				if (elem.GetAttribute (Constants.Opaque) == "true" ||
				    elem.GetAttribute (Constants.Opaque) == "1")
					is_opaque = true;

				IGeneratable gen;
				switch (def.Name) {
				case Constants.Alias:
					gen = new AliasGen ();
					break;
				//case Constants.Boxed:
				//	if (is_opaque)
				//		gen = new OpaqueGen(ns, elem);
				//	else
				//		gen = new BoxedGen(ns, elem);
				//	break;
				//case Constants.Callback:
					//gen = new CallbackGen (ns, elem);
					//break;
				case Constants.Bitfield:
				case Constants.Enumeration:
					gen = new EnumGen (def.Name == Constants.Bitfield);
					break;
				//case Constants.Interface:
				//	gen = new InterfaceGen (ns, elem);
				//	break;
				//case Constants.Object:
				//	gen = new ObjectGen (ns, elem);
				//	break;
				//case Constants.Class:
				//	gen = new ClassGen (ns, elem);
				//	break;
				//case Constants.Struct:
					//if (is_opaque)
					//	gen = new OpaqueGen(ns, elem);
					//else
					//	gen = new StructGen (ns, elem);
					//break;
				default:
					seen.Add (def.Name);
					continue;
				}

				gen.Parse (ns, elem);
				result.Add (gen);
			}

			foreach (var nodeName in seen) {
				Console.WriteLine ("Parser::ParseNamespace - Unexpected node: " + nodeName);
			}

			return result;
		}

		private IGeneratable ParseSymbol (XmlElement symbol)
		{
			string type = symbol.GetAttribute (Constants.Type);
			string cname = symbol.GetAttribute (Constants.CName);
			string name = symbol.GetAttribute (Constants.Name);
			IGeneratable result = null;

			if (type == Constants.Simple) {
				if (symbol.HasAttribute (Constants.DefaultValue))
					result = new SimpleGen (cname, name, symbol.GetAttribute (Constants.DefaultValue));
				else {
					Console.WriteLine ("Simple type element " + cname + " has no specified default value");
					result = new SimpleGen (cname, name, String.Empty);
				}
			} else if (type == Constants.Manual)
				result = new ManualGen (cname, name);
			//else if (type == Constants.Alias)
				//result = new AliasGen (cname, name);
			else if (type == Constants.Marshal) {
				string mtype = symbol.GetAttribute (Constants.MarshalType);
				string call = symbol.GetAttribute (Constants.CallFmt);
				string from = symbol.GetAttribute (Constants.FromFmt);
				result = new MarshalGen (cname, name, mtype, call, from);
			} else
				Console.WriteLine ("Parser::ParseSymbol - Unexpected symbol type " + type);

			return result;
		}
	}
}
