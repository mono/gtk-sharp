// GtkSharp.Generation.InterfaceGen.cs - The Interface Generatable.
//
// Author: Mike Kestner <mkestner@speakeasy.net>
//
// Copyright (c) 2001-2003 Mike Kestner
// Copyright (c) 2004, 2007 Novell, Inc.
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
	using System.Xml;

	public class InterfaceGen : ObjectBase {

		bool consume_only;
		ArrayList vms = new ArrayList ();
		ArrayList members = new ArrayList ();

		public InterfaceGen (XmlElement ns, XmlElement elem) : base (ns, elem) 
		{
			consume_only = elem.HasAttribute ("consume_only");
			foreach (XmlNode node in elem.ChildNodes) {
				switch (node.Name) {
				case "virtual_method":
					VirtualMethod vm = new VirtualMethod (node as XmlElement, this);
					vms.Add (vm);
					members.Add (vm);
					break;
				case "signal":
					object sig = sigs [(node as XmlElement).GetAttribute ("name")];
					if (sig == null)
						sig = new Signal (node as XmlElement, this);
					members.Add (sig);
					break;
				default:
					if (!IsNodeNameHandled (node.Name))
						Console.WriteLine ("Unexpected node " + node.Name + " in " + CName);
					break;
				}
			}
		}

		public bool IsConsumeOnly {
			get {
				return consume_only;
			}
		}

		public override string FromNative (string var, bool owned)
		{
			if (IsConsumeOnly)
				return "GLib.Object.GetObject (" + var + ", " + (owned ? "true" : "false") + ") as " + QualifiedName;
			else
				return QualifiedName + "Adapter.GetObject (" + var + ", " + (owned ? "true" : "false") + ")";
		}

		public override bool ValidateForSubclass ()
		{
			ArrayList invalids = new ArrayList ();

			foreach (Method method in methods.Values) {
				if (!method.Validate ()) {
					Console.WriteLine ("in type " + QualifiedName);
					invalids.Add (method);
				}
			}
			foreach (Method method in invalids)
				methods.Remove (method.Name);
			invalids.Clear ();

			return base.ValidateForSubclass ();
		}

		string IfaceName {
			get {
				return Name + "Iface";
			}
		}

		void GenerateIfaceStruct (StreamWriter sw)
		{
			sw.WriteLine ("\t\tstatic " + IfaceName + " iface;");
			sw.WriteLine ();
			sw.WriteLine ("\t\tstruct " + IfaceName + " {");
			sw.WriteLine ("\t\t\tpublic IntPtr gtype;");
			sw.WriteLine ("\t\t\tpublic IntPtr itype;");
			sw.WriteLine ();

			foreach (object member in members) {
				if (member is Signal) {
					Signal sig = member as Signal;
					sw.WriteLine ("\t\t\tpublic IntPtr {0};", sig.CName.Replace ("\"", "").Replace ("-", "_"));
				} else if (member is VirtualMethod) {
					VirtualMethod vm = member as VirtualMethod;
					bool has_target = methods [vm.Name] != null;
					if (!has_target)
						Console.WriteLine ("Interface " + QualifiedName + " virtual method " + vm.Name + " has no matching method to invoke.");
					string type = has_target && vm.IsValid ? vm.Name + "Delegate" : "IntPtr";
					sw.WriteLine ("\t\t\tpublic " + type + " " + vm.CName + ";");
				}
			}

			sw.WriteLine ("\t\t}");
			sw.WriteLine ();
		}

		void GenerateStaticCtor (StreamWriter sw)
		{
			sw.WriteLine ("\t\tstatic " + Name + "Adapter ()");
			sw.WriteLine ("\t\t{");
			sw.WriteLine ("\t\t\tGLib.GType.Register (_gtype, typeof({0}Adapter));", Name);
			foreach (VirtualMethod vm in vms) {
				bool has_target = methods [vm.Name] != null;
				if (has_target && vm.IsValid)
					sw.WriteLine ("\t\t\tiface.{0} = new {1}Delegate ({1}Callback);", vm.CName, vm.Name);
			}
			sw.WriteLine ("\t\t}");
			sw.WriteLine ();
		}

		void GenerateInitialize (StreamWriter sw)
		{
			sw.WriteLine ("\t\tstatic void Initialize (IntPtr ifaceptr, IntPtr data)");
			sw.WriteLine ("\t\t{");
			sw.WriteLine ("\t\t\t" + IfaceName + " native_iface = Marshal.PtrToStructure<" + IfaceName + "> (ifaceptr);");
			foreach (VirtualMethod vm in vms)
				sw.WriteLine ("\t\t\tnative_iface." + vm.CName + " = iface." + vm.CName + ";");
			sw.WriteLine ("\t\t\tMarshal.StructureToPtr<" + IfaceName + "> (native_iface, ifaceptr, false);");
			sw.WriteLine ("\t\t\tGCHandle gch = (GCHandle) data;");
			sw.WriteLine ("\t\t\tgch.Free ();");
			sw.WriteLine ("\t\t}");
			sw.WriteLine ();
		}

		void GenerateCallbacks (StreamWriter sw)
		{
			foreach (VirtualMethod vm in vms) {
				if (methods [vm.Name] != null) {
					sw.WriteLine ();
					vm.GenerateCallback (sw);
				}
			}
		}

		void GenerateCtors (StreamWriter sw)
		{
			sw.WriteLine ("\t\tpublic " + Name + "Adapter ()");
			sw.WriteLine ("\t\t{");
			sw.WriteLine ("\t\t\tInitHandler = new GLib.GInterfaceInitHandler (Initialize);");
			sw.WriteLine ("\t\t}");
			sw.WriteLine ();
			sw.WriteLine ("\t\t{0}Implementor implementor;", Name);
			sw.WriteLine ();
			sw.WriteLine ("\t\tpublic {0}Adapter ({0}Implementor implementor)", Name);
			sw.WriteLine ("\t\t{");
			sw.WriteLine ("\t\t\tif (implementor == null)");
			sw.WriteLine ("\t\t\t\tthrow new ArgumentNullException (\"implementor\");");
			sw.WriteLine ("\t\t\tthis.implementor = implementor;");
			sw.WriteLine ("\t\t}");
			sw.WriteLine ();
			sw.WriteLine ("\t\tpublic " + Name + "Adapter (IntPtr handle)");
			sw.WriteLine ("\t\t{");
			sw.WriteLine ("\t\t\tthis.handle = handle;");
			sw.WriteLine ("\t\t}");
			sw.WriteLine ();
		}

		void GenerateGType (StreamWriter sw)
		{
			Method m = GetMethod ("GetType");
			m.GenerateImport (sw);
			sw.WriteLine ("\t\tprivate static GLib.GType _gtype = new GLib.GType ({0} ());", m.CName);
			sw.WriteLine ();
			sw.WriteLine ("\t\tpublic override GLib.GType GType {");
			sw.WriteLine ("\t\t\tget {");
			sw.WriteLine ("\t\t\t\treturn _gtype;");
			sw.WriteLine ("\t\t\t}");
			sw.WriteLine ("\t\t}");
			sw.WriteLine ();
		}

		void GenerateHandleProp (StreamWriter sw)
		{
			sw.WriteLine ("\t\tIntPtr handle;");
			sw.WriteLine ("\t\tpublic override IntPtr Handle {");
			sw.WriteLine ("\t\t\tget {");
			sw.WriteLine ("\t\t\t\tif (handle != IntPtr.Zero)");
			sw.WriteLine ("\t\t\t\t\treturn handle;");
			sw.WriteLine ("\t\t\t\treturn implementor == null ? IntPtr.Zero : implementor.Handle;");
			sw.WriteLine ("\t\t\t}");
			sw.WriteLine ("\t\t}");
			sw.WriteLine ();
		}

		void GenerateGetObject (StreamWriter sw)
		{
			sw.WriteLine ("\t\tpublic static " + Name + " GetObject (IntPtr handle, bool owned)");
			sw.WriteLine ("\t\t{");
			sw.WriteLine ("\t\t\tGLib.Object obj = GLib.Object.GetObject (handle, owned);");
			sw.WriteLine ("\t\t\treturn GetObject (obj);");
			sw.WriteLine ("\t\t}");
			sw.WriteLine ();
			sw.WriteLine ("\t\tpublic static " + Name + " GetObject (GLib.Object obj)");
			sw.WriteLine ("\t\t{");
			sw.WriteLine ("\t\t\tif (obj == null)");
			sw.WriteLine ("\t\t\t\treturn null;");
			sw.WriteLine ("\t\t\telse if (obj is " + Name + "Implementor)");
			sw.WriteLine ("\t\t\t\treturn new {0}Adapter (obj as {0}Implementor);", Name);
			sw.WriteLine ("\t\t\telse if (obj as " + Name + " == null)");
			sw.WriteLine ("\t\t\t\treturn new {0}Adapter (obj.Handle);", Name);
			sw.WriteLine ("\t\t\telse");
			sw.WriteLine ("\t\t\t\treturn obj as {0};", Name);
			sw.WriteLine ("\t\t}");
			sw.WriteLine ();
		}

		void GenerateImplementorProp (StreamWriter sw)
		{
			sw.WriteLine ("\t\tpublic " + Name + "Implementor Implementor {");
			sw.WriteLine ("\t\t\tget {");
			sw.WriteLine ("\t\t\t\treturn implementor;");
			sw.WriteLine ("\t\t\t}");
			sw.WriteLine ("\t\t}");
			sw.WriteLine ();
		}

		void GenerateAdapter (GenerationInfo gen_info)
		{
			if (IsConsumeOnly)
				return;

			StreamWriter sw = gen_info.Writer = gen_info.OpenStream (Name + "Adapter");

			sw.WriteLine ("namespace " + NS + " {");
			sw.WriteLine ();
			sw.WriteLine ("\tusing System;");
			sw.WriteLine ("\tusing System.Runtime.InteropServices;");
			sw.WriteLine ();
			sw.WriteLine ("#region Autogenerated code");
			sw.WriteLine ("\tpublic class " + Name + "Adapter : GLib.GInterfaceAdapter, " + QualifiedName + " {");
			sw.WriteLine ();

			GenerateIfaceStruct (sw);
			GenerateStaticCtor (sw);
			GenerateCallbacks (sw);
			GenerateInitialize (sw);
			GenerateCtors (sw);
			GenerateGType (sw);
			GenerateHandleProp (sw);
			GenerateGetObject (sw);
			GenerateImplementorProp (sw);

			GenProperties (gen_info, null);

			foreach (Signal sig in sigs.Values)
				sig.GenEvent (sw, null, "GLib.Object.GetObject (Handle)");

			Method temp = methods ["GetType"] as Method;
			if (temp != null)
				methods.Remove ("GetType");
			GenMethods (gen_info, new Hashtable (), this);
			if (temp != null)
				methods ["GetType"] = temp;

			sw.WriteLine ("#endregion");

			AppendCustom (sw, gen_info.CustomDir, Name + "Adapter");

			sw.WriteLine ("\t}");
			sw.WriteLine ("}");
			sw.Close ();
			gen_info.Writer = null;
		}

		void GenerateImplementorIface (GenerationInfo gen_info)
		{
			StreamWriter sw = gen_info.Writer;
			if (IsConsumeOnly)
				return;

			sw.WriteLine ();
			sw.WriteLine ("\t[GLib.GInterface (typeof (" + Name + "Adapter))]");
			string access = IsInternal ? "internal" : "public";
			sw.WriteLine ("\t" + access + " interface " + Name + "Implementor : GLib.IWrapper {");
			sw.WriteLine ();
			Hashtable vm_table = new Hashtable ();
			foreach (VirtualMethod vm in vms)
				vm_table [vm.Name] = vm;
			foreach (VirtualMethod vm in vms) {
				if (vm_table [vm.Name] == null)
					continue;
				else if (!vm.IsValid) {
					vm_table.Remove (vm.Name);
					continue;
				} else if (vm.IsGetter || vm.IsSetter) {
					string cmp_name = (vm.IsGetter ? "Set" : "Get") + vm.Name.Substring (3);
					VirtualMethod cmp = vm_table [cmp_name] as VirtualMethod;
					if (cmp != null && (cmp.IsGetter || cmp.IsSetter)) {
						if (vm.IsSetter)
							cmp.GenerateDeclaration (sw, vm);
						else
							vm.GenerateDeclaration (sw, cmp);
						vm_table.Remove (cmp.Name);
					} else 
						vm.GenerateDeclaration (sw, null);
					vm_table.Remove (vm.Name);
				} else {
					vm.GenerateDeclaration (sw, null);
					vm_table.Remove (vm.Name);
				}
			}

			AppendCustom (sw, gen_info.CustomDir, Name + "Implementor");

			sw.WriteLine ("\t}");
		}

		public override void Generate (GenerationInfo gen_info)
		{
			GenerateAdapter (gen_info);
			StreamWriter sw = gen_info.Writer = gen_info.OpenStream (Name);

			sw.WriteLine ("namespace " + NS + " {");
			sw.WriteLine ();
			sw.WriteLine ("\tusing System;");
			sw.WriteLine ();
			sw.WriteLine ("#region Autogenerated code");
			string access = IsInternal ? "internal" : "public";
			sw.WriteLine ("\t" + access + " interface " + Name + " : GLib.IWrapper {");
			sw.WriteLine ();
			
			foreach (Signal sig in sigs.Values) {
				sig.GenerateDecl (sw);
				sig.GenEventHandler (gen_info);
			}

			foreach (Method method in methods.Values) {
				if (IgnoreMethod (method, this))
					continue;
				method.GenerateDecl (sw);
			}

			foreach (Property prop in props.Values)
				prop.GenerateDecl (sw, "\t\t");

			AppendCustom (sw, gen_info.CustomDir);

			sw.WriteLine ("\t}");
			GenerateImplementorIface (gen_info);
			sw.WriteLine ("#endregion");
			sw.WriteLine ("}");
			sw.Close ();
			gen_info.Writer = null;
			Statistics.IFaceCount++;
		}
	}
}

