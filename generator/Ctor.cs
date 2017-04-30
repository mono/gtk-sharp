// GtkSharp.Generation.Ctor.cs - The Constructor Generation Class.
//
// Author: Mike Kestner <mkestner@novell.com>
//
// Copyright (c) 2001-2003 Mike Kestner
// Copyright (c) 2004-2005 Novell, Inc.
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
	using System.Collections;
	using System.IO;
	using System.Linq;
	using System.Xml;

	public class Ctor : MethodBase  {

		private bool preferred;
		private string name;
		private bool needs_chaining = false;

		public Ctor (XmlElement elem, ClassBase implementor) : base (elem, implementor) 
		{
			if (elem.HasAttribute ("preferred"))
				preferred = true;
			if (implementor is ObjectGen)
				needs_chaining = true;
			name = implementor.Name;
		}

		public bool Preferred {
			get { return preferred; }
			set { preferred = value; }
		}

		public string StaticName {
			get {
				if (!IsStatic)
					return String.Empty;

				string[] toks = CName.Substring(CName.IndexOf("new")).Split ('_');
				string result = String.Empty;

				foreach (string tok in toks)
					result += tok.Substring(0,1).ToUpper() + tok.Substring(1);
				return result;
			}
		}

		void GenerateImport (StreamWriter sw)
		{
			sw.WriteLine("\t\t[DllImport(\"" + LibraryName + "\", CallingConvention = CallingConvention.Cdecl)]");
			sw.WriteLine("\t\tstatic extern " + Safety + "IntPtr " + CName + "(" + Parameters.ImportSignature + ");");
			sw.WriteLine();
		}

		void GenerateStatic (GenerationInfo gen_info)
		{
			StreamWriter sw = gen_info.Writer;
			sw.WriteLine("\t\t" + Protection + " static " + Safety + Modifiers +  name + " " + StaticName + "(" + Signature + ")");
			sw.WriteLine("\t\t{");

			Body.Initialize(gen_info, false, false, "", false); 

			sw.Write("\t\t\t" + name + " result = ");
			if (container_type is StructBase)
				sw.Write ("{0}.New (", name);
			else
				sw.Write ("new {0} (", name);
			sw.WriteLine (CName + "(" + Body.GetCallString (false) + "));");
			Body.Finish (sw, ""); 
			Body.HandleException (sw, ""); 
			sw.WriteLine ("\t\t\treturn result;");
		}

		static bool IsNullable (IGeneratable gen)
		{
			return gen is ClassBase && !(gen is StructBase);
		}

		public void Generate (GenerationInfo gen_info)
		{
			if (!Validate ())
				return;

			StreamWriter sw = gen_info.Writer;
			gen_info.CurrentMember = CName;

			GenerateImport (sw);
			
			if (IsStatic)
				GenerateStatic (gen_info);
			else {
				sw.WriteLine("\t\t{0} {1}{2} ({3}) {4}", Protection, Safety, name, Signature.ToString(), needs_chaining ? ": base (IntPtr.Zero)" : "");
				sw.WriteLine("\t\t{");

				if (needs_chaining) {
					sw.WriteLine ("\t\t\tif (GetType () != typeof (" + name + ")) {");
					if (gen_info.AssemblyName == "gtk-sharp")
						sw.WriteLine ("\t\t\t\tGtk.Application.AssertMainThread();");
					
					if (Parameters.Count == 0) {
						sw.WriteLine ("\t\t\t\tCreateNativeObject (Array.Empty<IntPtr> (), Array.Empty<GLib.Value> (), 0);");
						sw.WriteLine ("\t\t\t\treturn;");
					} else {
						ArrayList names = new ArrayList ();
						ArrayList values = new ArrayList ();
						for (int i = 0; i < Parameters.Count; i++) {
							Parameter p = Parameters[i];
							if (container_type.GetPropertyRecursively (p.StudlyName) != null) {
								names.Add (p.Name);
								values.Add (p.Name);
							} else if (p.PropertyName != String.Empty) {
								names.Add (p.PropertyName);
								values.Add (p.Name);
							}
						}

						if (names.Count == Parameters.Count) {
							bool genFixedDimension = true;
							foreach (Parameter par in Parameters) {
								if (IsNullable (par.Generatable)) {
									genFixedDimension = false;
									break;
								}
							}
							sw.WriteLine ("\t\t\t\tvar vals = new GLib.Value[{0}];", Parameters.Count);
							sw.WriteLine ("\t\t\t\tvar names = new IntPtr[{0}];", names.Count);
							if (!genFixedDimension)
								sw.WriteLine ("\t\t\t\tvar param_count = 0;");
							for (int i = 0; i < names.Count; i++) {
								Parameter p = Parameters [i];
								string indent = "\t\t\t\t";
								if (IsNullable (p.Generatable)) {
									sw.WriteLine (indent + "if (" + p.Name + " != null) {");
									indent += "\t";
								}
								if (genFixedDimension) {
									sw.WriteLine (indent + "names[" + i + "] = GLib.Marshaller.StringToPtrGStrdup (\"" + names [i] + "\");");
									sw.WriteLine (indent + "vals[" + i + "] = new GLib.Value (" + values [i] + ");");
								} else {
									sw.WriteLine (indent + "names[param_count] = GLib.Marshaller.StringToPtrGStrdup (\"" + names [i] + "\");");
									sw.WriteLine (indent + "vals[param_count++] = new GLib.Value (" + values [i] + ");");
								}

								if (IsNullable (p.Generatable))
									sw.WriteLine ("\t\t\t\t}");
							}

							if (genFixedDimension)
								sw.WriteLine ("\t\t\t\tCreateNativeObject (names, vals, {0});", names.Count);
							else
								sw.WriteLine ("\t\t\t\tCreateNativeObject (names, vals, param_count);");
							
							sw.WriteLine ("\t\t\t\treturn;");
						} else
							sw.WriteLine ("\t\t\t\tthrow new InvalidOperationException (\"Can't override this constructor.\");");
					}
					
					sw.WriteLine ("\t\t\t}");
				}
	
				Body.Initialize(gen_info, false, false, "", false);
				if (container_type is ObjectGen) {
					sw.WriteLine ("\t\t\towned = true;");
				}
				sw.WriteLine("\t\t\t{0} = {1}({2});", container_type.AssignToName, CName, Body.GetCallString (false));
				Body.Finish (sw, "");
				Body.HandleException (sw, "");
			}
			
			sw.WriteLine("\t\t}");
			sw.WriteLine();
			
			Statistics.CtorCount++;
		}
	}
}

