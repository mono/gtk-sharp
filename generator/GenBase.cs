// GtkSharp.Generation.GenBase.cs - The Generatable base class.
//
// Author: Mike Kestner <mkestner@novell.com>
//
// Copyright (c) 2001-2002 Mike Kestner
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


namespace GtkSharp.Generation {

	using System;
	using System.IO;
	using System.Xml;

	public abstract class GenBase : IGeneratable {
		
		string Documentation;

		public void Parse (XmlElement ns, XmlElement elem)
		{
			ParseElement (ns, elem);
		}

		protected virtual void ParseElement (XmlElement ns, XmlElement elem)
		{
			Name = elem.GetName ();
			CName = elem.GetCType ();

			Namespace = ns;
			// TODO: Grab the correct library name to pinvoke
			LibraryName = ns.GetAttribute (Constants.Library);
			NS = ns.GetAttribute (Constants.Name);

			if (elem.HasAttribute (Constants.Internal)) {
				string attr = elem.GetAttribute (Constants.Internal);
				IsInternal = attr == "1" || attr == "true";
			}

			foreach (XmlElement child in elem.ChildNodes) {
				ParseChildElement (ns, child);
			}
		}

		protected virtual void ParseChildElement (XmlElement ns, XmlElement childElement)
		{
			if (childElement.Name == Constants.Documentation) {
				Documentation = childElement.InnerText;
				return;
			}

			Console.WriteLine ("{0} - Unexpected node {1} in {2}", GetType().Name, childElement.Name, Name);
		}

		public string CName { get; private set; }

		public bool IsInternal { get; private set; }

		public string LibraryName { get; private set; }

		public string Name { get; private set; }

		public virtual string MarshalReturnType { 
			get {
				return MarshalType;
			}
		}

		public virtual string MarshalCallbackType {
			get {
				return MarshalType;
			}
		}

		public abstract string MarshalType { get; }

		public string NS { get; private set; }

		public abstract string DefaultValue { get; }

		public string QualifiedName {
			get {
				return NS + "." + Name;
			}
		}

		public virtual string ToNativeReturnType { 
			get {
				return MarshalType;
			}
		}

		protected void AppendCustom (StreamWriter sw, string custom_dir)
		{
			AppendCustom (sw, custom_dir, Name);
		}

		protected void AppendCustom (StreamWriter sw, string custom_dir, string type_name)
		{
			char sep = Path.DirectorySeparatorChar;
			string custom = custom_dir + sep + type_name + ".custom";
			if (File.Exists(custom)) {
				sw.WriteLine ("#region Customized extensions");
				sw.WriteLine ("#line 1 \"" + type_name + ".custom\"");
				FileStream custstream = new FileStream(custom, FileMode.Open, FileAccess.Read);
				StreamReader sr = new StreamReader(custstream);
				sw.WriteLine (sr.ReadToEnd ());
				sw.WriteLine ("#endregion");
				sr.Close ();
			}
		}

		public abstract string CallByName (string var);

		public abstract string FromNative (string var);

		public virtual string FromNativeReturn (string var)
		{
			return FromNative (var);
		}

		public virtual string ToNativeReturn (string var)
		{
			return CallByName (var);
		}

		public abstract bool Validate ();

		internal XmlElement Namespace { get; private set; }

		public void Generate ()
		{
			GenerationInfo geninfo = new GenerationInfo (Namespace);
			Generate (geninfo);
		}

		public void GenerateDocumentation (GenerationInfo info)
		{
			if (string.IsNullOrEmpty (Documentation))
				return;

			// FIXME: Indent
			info.Writer.WriteLine ("/// <summary>");
			foreach (var line in Documentation.Split ('\n')) {
				info.Writer.Write ("/// ");
				info.Writer.WriteLine (line);
			}
			info.Writer.WriteLine ("/// </summary>");
		}


		public abstract void Generate (GenerationInfo geninfo);
	}
}

