﻿// GtkSharp.Generation.ClassBase.cs - Common code between object
// and interface wrappers
//
// Authors: Rachel Hestilow <hestilow@ximian.com>
//          Mike Kestner <mkestner@speakeasy.net>
//
// Copyright (c) 2002 Rachel Hestilow
// Copyright (c) 2001-2003 Mike Kestner 
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

using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;

namespace GtkSharp.Generation {

	public abstract class ClassBase : GenBase {
		protected Dictionary<string, Property> props = new Dictionary<string, Property>();
		protected Dictionary<string, ObjectField> fields = new Dictionary<string, ObjectField>();
		protected Dictionary<string, Signal> sigs = new Dictionary<string, Signal>();
		protected Dictionary<string, Method> methods = new Dictionary<string, Method>();
		protected List<string> interfaces = new List<string>();
		protected List<string> managed_interfaces = new List<string>();
		protected List<Ctor> ctors = new List<Ctor>();

		private bool ctors_initted = false;
		private Dictionary<string, Ctor> clash_map;
		private bool deprecated = false;
		private bool isabstract = false;

		public Dictionary<string, Method> Methods {
			get {
				return methods;
			}
		}	

		public Dictionary<string, Signal> Signals {
			get {
				return sigs;
			}
		}	

		public ClassBase Parent {
			get {
				string parent = Elem.GetAttribute(Constants.Parent);

				if (parent == "")
					return null;
				else
					return SymbolTable.Table.GetClassGen(parent);
			}
		}

		protected ClassBase (XmlElement ns, XmlElement elem) : base (ns, elem) {
					
			if (elem.HasAttribute (Constants.Deprecated)) {
				string attr = elem.GetAttribute (Constants.Deprecated);
				deprecated = attr == "1" || attr == "true";
			}
			
			if (elem.HasAttribute (Constants.Abstract)) {
				string attr = elem.GetAttribute (Constants.Abstract);
				isabstract = attr == "1" || attr == "true";
			}

			foreach (XmlNode node in elem.ChildNodes) {
				if (!(node is XmlElement)) continue;
				XmlElement member = (XmlElement) node;
				if (member.HasAttribute (Constants.Hidden))
					continue;
				
				string name;
				switch (node.Name) {
				case Constants.Method:
					name = member.GetAttribute(Constants.Name);
					while (methods.ContainsKey(name))
						name += "mangled";
					methods.Add (name, new Method (member, this));
					break;

				case Constants.Property:
					name = member.GetAttribute(Constants.Name);
					while (props.ContainsKey(name))
						name += "mangled";
					props.Add (name, new Property (member, this));
					break;

				case Constants.Field:
					name = member.GetAttribute(Constants.Name);
					while (fields.ContainsKey (name))
						name += "mangled";
					fields.Add (name, new ObjectField (member, this));
					break;

				case Constants.Signal:
					name = member.GetAttribute(Constants.Name);
					while (sigs.ContainsKey(name))
						name += "mangled";
					sigs.Add (name, new Signal (member, this));
					break;

				case Constants.Implements:
					ParseImplements (member);
					break;

				case Constants.Constructor:
					ctors.Add (new Ctor (member, this));
					break;

				default:
					break;
				}
			}
		}

		public override bool Validate ()
		{
			if (Parent != null && !Parent.ValidateForSubclass ())
				return false;
			foreach (string iface in interfaces) {
				InterfaceGen igen = SymbolTable.Table[iface] as InterfaceGen;
				if (igen == null) {
					Console.WriteLine (QualifiedName + " implements unknown GInterface " + iface);
					return false;
				}
				if (!igen.ValidateForSubclass ()) {
					Console.WriteLine (QualifiedName + " implements invalid GInterface " + iface);
					return false;
				}
			}

			var invalidProperties = new List<Property>();
			foreach (Property prop in props.Values) {
				if (!prop.Validate ()) {
					Console.WriteLine ("in type " + QualifiedName);
					invalidProperties.Add (prop);
				}
			}
			foreach (Property prop in invalidProperties)
				props.Remove (prop.Name);

			var invalidSignals = new List<Signal>();
			foreach (Signal sig in sigs.Values) {
				if (!sig.Validate ()) {
					Console.WriteLine ("in type " + QualifiedName);
					invalidSignals.Add (sig);
				}
			}
			foreach (Signal sig in invalidSignals)
				sigs.Remove (sig.Name);

			var invalidObjectFields = new List<ObjectField>();
			foreach (ObjectField field in fields.Values) {
				if (!field.Validate ()) {
					Console.WriteLine ("in type " + QualifiedName);
					invalidObjectFields.Add (field);
				}
			}
			foreach (ObjectField field in invalidObjectFields)
				fields.Remove (field.Name);

			var invalidMethods = new List<Method>();
			foreach (Method method in methods.Values) {
				if (!method.Validate ()) {
					Console.WriteLine ("in type " + QualifiedName);
					invalidMethods.Add (method);
				}
			}
			foreach (Method method in invalidMethods)
				methods.Remove (method.Name);

			var invalidCtors = new List<Ctor>();
			foreach (Ctor ctor in ctors) {
				if (!ctor.Validate ()) {
					Console.WriteLine ("in type " + QualifiedName);
					invalidCtors.Add (ctor);
				}
			}
			foreach (Ctor ctor in invalidCtors)
				ctors.Remove (ctor);

			return true;
		}

		public virtual bool ValidateForSubclass ()
		{
			var invalids = new List<Signal>();

			foreach (Signal sig in sigs.Values) {
				if (!sig.Validate ()) {
					Console.WriteLine ("in type " + QualifiedName);
					invalids.Add (sig);
				}
			}
			foreach (Signal sig in invalids)
				sigs.Remove (sig.Name);
			invalids.Clear ();

			return true;
		}

		public bool IsDeprecated {
			get {
				return deprecated;
			}
		}

		public bool IsAbstract {
			get {
				return isabstract;
			}
		}

		public abstract string AssignToName { get; }

		public abstract string CallByName ();

		public override string DefaultValue {
			get {
				return "null";
			}
		}

		protected bool IsNodeNameHandled (string name)
		{
			switch (name) {
			case Constants.Method:
			case Constants.Property:
			case Constants.Field:
			case Constants.Signal:
			case Constants.Implements:
			case Constants.Constructor:
			case Constants.DisableDefaultConstructor:
				return true;
				
			default:
				return false;
			}
		}

		public void GenProperties (GenerationInfo gen_info, ClassBase implementor)
		{		
			if (props.Count == 0)
				return;

			foreach (Property prop in props.Values)
				prop.Generate (gen_info, "\t\t", implementor);
		}

		public void GenSignals (GenerationInfo gen_info, ClassBase implementor)
		{		
			if (sigs == null)
				return;

			foreach (Signal sig in sigs.Values)
				sig.Generate (gen_info, implementor);
		}

		protected void GenFields (GenerationInfo gen_info)
		{
			foreach (ObjectField field in fields.Values)
				field.Generate (gen_info, "\t\t");
		}

		private void ParseImplements (XmlElement member)
		{
			foreach (XmlNode node in member.ChildNodes) {
				if (node.Name != Constants.Interface)
					continue;
				XmlElement element = (XmlElement) node;
				if (element.HasAttribute (Constants.Hidden))
					continue;
				if (element.HasAttribute (Constants.CName))
					interfaces.Add (element.GetAttribute (Constants.CName));
				else if (element.HasAttribute (Constants.Name))
					managed_interfaces.Add (element.GetAttribute (Constants.Name));
			}
		}
		
		protected bool IgnoreMethod (Method method, ClassBase implementor)
		{	
			if (implementor != null && implementor.QualifiedName != this.QualifiedName && method.IsStatic)
				return true;

			string mname = method.Name;
			return ((method.IsSetter || (method.IsGetter && mname.StartsWith("Get"))) &&
				((props != null) && props.ContainsKey(mname.Substring(3)) ||
				 (fields != null) && fields.ContainsKey(mname.Substring(3))));
		}

		public void GenMethods (GenerationInfo gen_info, Dictionary<string, bool> collisions, ClassBase implementor)
		{		
			if (methods == null)
				return;

			foreach (Method method in methods.Values) {
				if (IgnoreMethod (method, implementor))
				    	continue;

				string oname = null, oprotection = null;
				if (collisions != null && collisions.ContainsKey (method.Name)) {
					oname = method.Name;
					oprotection = method.Protection;
					method.Name = QualifiedName + "." + method.Name;
					method.Protection = "";
				}
				method.Generate (gen_info, implementor);
				if (oname != null) {
					method.Name = oname;
					method.Protection = oprotection;
				}
			}
		}

		public Method GetMethod (string name)
		{
			Method method;
			methods.TryGetValue (name, out method);
			return method;
		}

		public Property GetProperty (string name)
		{
			Property prop;
			props.TryGetValue (name, out prop);
			return prop;
		}

		public Signal GetSignal (string name)
		{
			Signal sig;
			sigs.TryGetValue (name, out sig);
			return sig;
		}

		public Method GetMethodRecursively (string name)
		{
			return GetMethodRecursively (name, false);
		}
		
		public virtual Method GetMethodRecursively (string name, bool check_self)
		{
			Method p = null;
			if (check_self)
				p = GetMethod (name);
			if (p == null && Parent != null) 
				p = Parent.GetMethodRecursively (name, true);
			
			if (check_self && p == null) {
				foreach (string iface in interfaces) {
					ClassBase igen = SymbolTable.Table.GetClassGen (iface);
					if (igen == null)
						continue;
					p = igen.GetMethodRecursively (name, true);
					if (p != null)
						break;
				}
			}

			return p;
		}

		public virtual Property GetPropertyRecursively (string name)
		{
			ClassBase klass = this;
			Property p = null;
			while (klass != null && p == null) {
				p = klass.GetProperty(name);
				klass = klass.Parent;
			}

			return p;
		}

		public Signal GetSignalRecursively (string name)
		{
			return GetSignalRecursively (name, false);
		}
		
		public virtual Signal GetSignalRecursively (string name, bool check_self)
		{
			Signal p = null;
			if (check_self)
				p = GetSignal (name);
			if (p == null && Parent != null) 
				p = Parent.GetSignalRecursively (name, true);
			
			if (check_self && p == null) {
				foreach (string iface in interfaces) {
					ClassBase igen = SymbolTable.Table.GetClassGen (iface);
					if (igen == null)
						continue;
					p = igen.GetSignalRecursively (name, true);
					if (p != null)
						break;
				}
			}

			return p;
		}

		public bool Implements (string iface)
		{
			if (interfaces.Contains (iface))
				return true;
			else if (Parent != null)
				return Parent.Implements (iface);
			else
				return false;
		}

		public List<Ctor> Ctors { get { return ctors; } }

		bool HasStaticCtor (string name) 
		{
			if (Parent != null && Parent.HasStaticCtor (name))
				return true;

			foreach (Ctor ctor in Ctors)
				if (ctor.StaticName == name)
					return true;

			return false;
		}

		private void InitializeCtors ()
		{
			if (ctors_initted)
				return;

			if (Parent != null)
				Parent.InitializeCtors ();

			List<Ctor> valid_ctors = new List<Ctor>();
			clash_map = new Dictionary<string, Ctor>();

			foreach (Ctor ctor in ctors) {
				Ctor clash;
				if (clash_map.TryGetValue (ctor.Signature.Types, out clash)) {
					Ctor alter = ctor.Preferred ? clash : ctor;
					alter.IsStatic = true;
					if (Parent != null && Parent.HasStaticCtor (alter.StaticName))
						alter.Modifiers = "new ";
				} else
					clash_map [ctor.Signature.Types] = ctor;

				valid_ctors.Add (ctor);
			}

			ctors = valid_ctors;
			ctors_initted = true;
		}

		protected virtual void GenCtors (GenerationInfo gen_info)
		{
			InitializeCtors ();
			foreach (Ctor ctor in ctors)
				ctor.Generate (gen_info);
		}

		public virtual void Finish (StreamWriter sw, string indent)
		{
		}

		public virtual void Prepare (StreamWriter sw, string indent)
		{
		}
	}
}
