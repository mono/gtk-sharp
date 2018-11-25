// gen-intptr-ctor-docs.cs - Generate documentation for Constructor (IntPtr).
//
// Author: John Luke  <john.luke@gmail.com>
//
// Copyright (c) 2004 Novell, Inc.
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

namespace GtkSharp.Docs
{
	using System;
	using System.Collections;
	using System.IO;
	using System.Reflection;
	using System.Xml;
	using System.Xml.XPath;

	public class GenIntPtrConstructorDocs
	{
		public static int Main (string[] args)
		{
			XmlDocument api_doc = new XmlDocument ();

			foreach (string arg in args)
			{
				Assembly assembly;
				try
				{
					assembly = Assembly.LoadFile (arg);
				}
				catch (XmlException e)
				{
					Console.WriteLine (e);
					return 1;
				}

				foreach (Type t in assembly.GetTypes ())
				{
					if (!t.IsSubclassOf (typeof (GLib.Object)))
						continue;

					ConstructorInfo ci = t.GetConstructor (new Type[] {typeof (System.IntPtr)});
					if (ci == null)
						continue;

					string filename = "en/" + t.Namespace + "/" + t.Name + ".xml";

					try
					{
						Stream stream = File.OpenRead (filename);
						api_doc.Load (stream);
						stream.Close ();
					}
					catch (XmlException e)
					{
						Console.WriteLine (e);
						return 1;
					}

					XPathNavigator api_nav = api_doc.CreateNavigator ();

					XPathNodeIterator iter = api_nav.Select ("/Type/Members/Member[@MemberName='.ctor']/Parameters/Parameter[@Type='System.IntPtr']");
					if (iter.MoveNext ())
					{
						XmlElement elem = ((IHasXmlNode)iter.Current).GetNode ().ParentNode.ParentNode as XmlElement;
						XmlElement summ = elem ["Docs"] ["summary"];
						XmlElement rem = elem ["Docs"] ["remarks"];
						XmlElement param = elem ["Docs"] ["param"];
						string summary = summ.InnerXml;
						string remarks = rem.InnerXml;
						if (summary == "To be added." && remarks == "To be added.")
						{
							Console.WriteLine (filename + ": documenting IntPtr ctor");
							summ.InnerXml = "Internal constructor";
							rem.InnerXml = "This is not typically used by C# code.  Exposed primarily for use by language bindings to wrap native object instances.";
							param.InnerXml = "Native object pointer.";
						}
					}
					api_doc.Save (filename);
				}
			}
		return 0;
		}
	}
}
