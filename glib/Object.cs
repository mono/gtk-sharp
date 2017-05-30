// Object.cs - GObject class wrapper implementation
//
// Authors: Mike Kestner <mkestner@speakeasy.net>
//
// Copyright (c) 2001-2003 Mike Kestner
// Copyright (c) 2004-2005 Novell, Inc.
//
// This program is free software; you can redistribute it and/or
// modify it under the terms of version 2 of the Lesser GNU General
// Public License as published by the Free Software Foundation.
//
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
// Lesser General Public License for more details.
//
// You should have received a copy of the GNU Lesser General Public
// License along with this program; if not, write to the
// Free Software Foundation, Inc., 59 Temple Place - Suite 330,
// Boston, MA 02111-1307, USA.


namespace GLib {

	using System;
	using System.Collections;
	using System.Collections.Generic;
	using System.ComponentModel;
	using System.Diagnostics;
	using System.Reflection;
	using System.Runtime.InteropServices;
	using System.Text;

	public class Object : IWrapper, IDisposable {

		IntPtr handle;
		ToggleRef tref;
		internal protected bool owned = false;
		Hashtable data;
		static Dictionary<IntPtr, ToggleRef> Objects = new Dictionary<IntPtr, ToggleRef>(IntPtrEqualityComparer.Instance);
		static object lockObject = new object ();
		static List<ToggleRef> PendingDestroys = new List<ToggleRef> ();
		static bool idle_queued;

		~Object ()
		{
			lock (lockObject) {
				lock (Objects) {
					if (tref != null)
						PendingDestroys.Add (tref);
					Objects.Remove (Handle);
				}
				if (!idle_queued){
					Timeout.Add (50, PerformQueuedUnrefs);
					idle_queued = true;
				}
			}
		}

		[DllImport("libgobject-2.0-0.dll", CallingConvention=CallingConvention.Cdecl)]
		static extern void g_object_unref (IntPtr raw);

		static bool PerformQueuedUnrefs ()
		{
			List<ToggleRef> references;

			lock (lockObject) {
				references = PendingDestroys;
				PendingDestroys = new List<ToggleRef> ();
				idle_queued = false;
			}

			foreach (ToggleRef r in references)
				r.Free ();

			return false;
		}

		public virtual void Dispose ()
		{
			if (Handle == IntPtr.Zero)
				return;
			
			lock(Objects) {
				Objects.Remove (Handle);
				try {
					if (tref != null) {
						tref.Free ();
						tref = null;
					}
				} catch (Exception e) {
					Console.WriteLine ("Exception while disposing a " + this + " in Gtk#");
					throw e;
				}
			}
			handle = IntPtr.Zero;
			GC.SuppressFinalize (this);
		}

		[DllImport("libgobject-2.0-0.dll", CallingConvention=CallingConvention.Cdecl)]
		static extern IntPtr g_object_ref (IntPtr raw);

		public static Object TryGetObject (IntPtr o)
		{
			if (o == IntPtr.Zero)
				return null;

			ToggleRef tr;
			lock(Objects)
				Objects.TryGetValue (o, out tr);
			if (tr != null) {
				return tr.Target;
			}

			return null;
		}

		public static Object GetObject(IntPtr o, bool owned_ref)
		{
			if (o == IntPtr.Zero)
				return null;

			Object obj = null;
			ToggleRef toggle_ref = null;
			lock(Objects)
				Objects.TryGetValue (o, out toggle_ref);

			if (toggle_ref != null)
				obj = toggle_ref.Target;

			if (obj != null && obj.Handle == o)
				return obj;

			obj = GLib.ObjectManager.CreateObject (o);
			if (obj == null) {
				g_object_unref (o);
				return null;
			}

			if (owned_ref)
				g_object_unref (o);
			return obj;
		}

		public static Object GetObject(IntPtr o)
		{
			return GetObject (o, false);
		}

		private static void ConnectDefaultHandlers (GType gtype, System.Type t)
		{
			foreach (MethodInfo minfo in t.GetMethods(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.DeclaredOnly)) {
				MethodInfo baseinfo = minfo.GetBaseDefinition ();
				if (baseinfo == minfo)
					continue;

				foreach (object attr in baseinfo.GetCustomAttributes (typeof (DefaultSignalHandlerAttribute), false)) {
					DefaultSignalHandlerAttribute sigattr = attr as DefaultSignalHandlerAttribute;
					MethodInfo connector = sigattr.Type.GetMethod (sigattr.ConnectionMethod, BindingFlags.Static | BindingFlags.NonPublic);
					object[] parms = new object [1];
					parms [0] = gtype;
					connector.Invoke (null, parms);
					break;
				}
			}

		}

		private static void InvokeClassInitializers (GType gtype, System.Type t)
		{
			object[] parms = {gtype, t};

			BindingFlags flags = BindingFlags.Static | BindingFlags.NonPublic;

			foreach (TypeInitializerAttribute tia in t.GetCustomAttributes (typeof (TypeInitializerAttribute), true)) {
				MethodInfo m = tia.Type.GetMethod (tia.MethodName, flags);
				if (m != null)
					m.Invoke (null, parms);
			}

			for (Type curr = t; curr != typeof(GLib.Object); curr = curr.BaseType) {

				if (curr.Assembly.IsDefined (typeof (IgnoreClassInitializersAttribute), false))
					continue;

				foreach (MethodInfo minfo in curr.GetMethods(flags))
					if (minfo.IsDefined (typeof (ClassInitializerAttribute), true))
						minfo.Invoke (null, parms);
			}
 		}

		//  Key: The pointer to the ParamSpec of the property
		//  Value: The corresponding PropertyInfo object
		static Dictionary<IntPtr, PropertyInfo> properties;
		static Dictionary<IntPtr, PropertyInfo> Properties {
			get {
				if (properties == null)
					properties = new Dictionary<IntPtr, PropertyInfo> (IntPtrEqualityComparer.Instance);
				return properties;
			}
		}

		[DllImport ("glibsharpglue-2", CallingConvention=CallingConvention.Cdecl)]
		static extern void gtksharp_override_property_handlers (IntPtr type, GetPropertyDelegate get_cb, SetPropertyDelegate set_cb);

		[DllImport ("glibsharpglue-2", CallingConvention=CallingConvention.Cdecl)]
		static extern IntPtr gtksharp_register_property (IntPtr type, IntPtr name, IntPtr nick, IntPtr blurb, uint property_id, IntPtr property_type, bool can_read, bool can_write);

		static void AddProperties (GType gtype, System.Type t)
		{
			uint idx = 1;

			bool handlers_overridden = false;
			foreach (PropertyInfo pinfo in t.GetProperties (BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly)) {
				foreach (object attr in pinfo.GetCustomAttributes (typeof (PropertyAttribute), false)) {
					if(pinfo.GetIndexParameters().Length > 0)
						throw(new InvalidOperationException(String.Format("GLib.RegisterPropertyAttribute cannot be applied to property {0} of type {1} because the property expects one or more indexed parameters", pinfo.Name, t.FullName)));

					PropertyAttribute property_attr = attr as PropertyAttribute;
					if (!handlers_overridden) {
						gtksharp_override_property_handlers (gtype.Val, GetPropertyHandler, SetPropertyHandler);
						handlers_overridden = true;
					}

					IntPtr native_name = GLib.Marshaller.StringToPtrGStrdup (property_attr.Name);
					IntPtr native_nick = GLib.Marshaller.StringToPtrGStrdup (property_attr.Nickname);
					IntPtr native_blurb = GLib.Marshaller.StringToPtrGStrdup (property_attr.Blurb);

					IntPtr param_spec = gtksharp_register_property (gtype.Val, native_name, native_nick, native_blurb, idx, ((GType) pinfo.PropertyType).Val, pinfo.CanRead, pinfo.CanWrite);

					GLib.Marshaller.Free (native_name);
					GLib.Marshaller.Free (native_nick);
					GLib.Marshaller.Free (native_blurb);

					if (param_spec == IntPtr.Zero)
						// The GType of the property is not supported
						throw new InvalidOperationException (String.Format ("GLib.PropertyAttribute cannot be applied to property {0} of type {1} because the return type of the property is not supported", pinfo.Name, t.FullName));

					Properties.Add (param_spec, pinfo);
					idx++;
				}
			}
		}

		[UnmanagedFunctionPointer (CallingConvention.Cdecl)]
		delegate void GetPropertyDelegate (IntPtr GObject, uint property_id, ref GLib.Value value, IntPtr pspec);

		static void GetPropertyCallback (IntPtr handle, uint property_id, ref GLib.Value value, IntPtr param_spec)
		{
			GLib.Object obj = GLib.Object.GetObject (handle, false);
			value.Val = (Properties [param_spec] as PropertyInfo).GetValue (obj);
		}

		static GetPropertyDelegate get_property_handler;
		static GetPropertyDelegate GetPropertyHandler {
			get {
				if (get_property_handler == null)
					get_property_handler = new GetPropertyDelegate (GetPropertyCallback);
				return get_property_handler;
			}
		}

		[UnmanagedFunctionPointer (CallingConvention.Cdecl)]
		delegate void SetPropertyDelegate (IntPtr GObject, uint property_id, ref GLib.Value value, IntPtr pspec);

		static void SetPropertyCallback(IntPtr handle, uint property_id, ref GLib.Value value, IntPtr param_spec)
		{
			GLib.Object obj = GLib.Object.GetObject (handle, false);
			(Properties [param_spec] as PropertyInfo).SetValue (obj, value.Val);
		}

		static SetPropertyDelegate set_property_handler;
		static SetPropertyDelegate SetPropertyHandler {
			get {
				if (set_property_handler == null)
					set_property_handler = new SetPropertyDelegate (SetPropertyCallback);
				return set_property_handler;
			}
		}

		[DllImport("libgobject-2.0-0.dll", CallingConvention=CallingConvention.Cdecl)]
		static extern void g_type_add_interface_static (IntPtr gtype, IntPtr iface_type, ref GInterfaceInfo info);

		static void AddInterfaces (GType gtype, Type t)
		{
			foreach (Type iface in t.GetInterfaces ()) {
				if (!iface.IsDefined (typeof (GInterfaceAttribute), true) || iface.IsAssignableFrom (t.BaseType))
					continue;

				GInterfaceAttribute attr = iface.GetCustomAttributes (typeof (GInterfaceAttribute), false) [0] as GInterfaceAttribute;
				GInterfaceAdapter adapter = Activator.CreateInstance (attr.AdapterType, null) as GInterfaceAdapter;

				GInterfaceInfo info = adapter.Info;
				g_type_add_interface_static (gtype.Val, adapter.GType.Val, ref info);
			}
		}

		[DllImport("glibsharpglue-2", CallingConvention=CallingConvention.Cdecl)]
		static extern IntPtr gtksharp_register_type (IntPtr name, IntPtr parent_type);

		static int type_uid;
		static string BuildEscapedName (System.Type t)
		{
			string qn = t.FullName;
			// Just a random guess
			StringBuilder sb = new StringBuilder (20 + qn.Length);
			sb.Append ("__gtksharp_");
			sb.Append (type_uid++);
			sb.Append ("_");
			foreach (char c in qn) {
				if ((c >= '0' && c <= '9') || (c >= 'a' && c <= 'z') || (c >= 'A' && c <= 'Z'))
					sb.Append (c);
				else if (c == '.')
					sb.Append ('_');
				else if ((uint) c <= byte.MaxValue) {
					sb.Append ('+');
					sb.Append (((byte) c).ToString ("x2"));
				} else {
					sb.Append ('-');
					sb.Append (((uint) c).ToString ("x4"));
				}
			}
			return sb.ToString ();
		}

		protected static GType RegisterGType (System.Type t)
		{
			GType parent_gtype = LookupGType (t.BaseType);
			string name = BuildEscapedName (t);
			IntPtr native_name = GLib.Marshaller.StringToPtrGStrdup (name);
			GType gtype = new GType (gtksharp_register_type (native_name, parent_gtype.Val));
			GLib.Marshaller.Free (native_name);
			GLib.GType.Register (gtype, t);
			AddProperties (gtype, t);
			ConnectDefaultHandlers (gtype, t);
			InvokeClassInitializers (gtype, t);
			AddInterfaces (gtype, t);
			g_types[t] = gtype;
			return gtype;
		}


		static Dictionary<Type, GType> g_types = new Dictionary<Type, GType> ();

		protected GType LookupGType ()
		{
			return LookupGType (GetType ());
		}

		protected internal static GType LookupGType (System.Type t)
		{
			GType res;
			if (g_types.TryGetValue (t, out res))
				return res;

			GTypeTypeAttribute geattr;
			if ((geattr = (GTypeTypeAttribute)Attribute.GetCustomAttribute (t, typeof (GTypeTypeAttribute), false)) != null) {
				var val = geattr.Type;
				g_types [t] = val;
				return val;
			}

			PropertyInfo pi = t.GetProperty ("GType", BindingFlags.DeclaredOnly | BindingFlags.Static | BindingFlags.Public);
			if (pi != null) {
				var val = (GType)pi.GetValue (null, null);
				g_types [t] = val;
				return val;
			}

			return RegisterGType (t);
		}

		protected Object (IntPtr raw)
		{
			Raw = raw;
		}

		protected Object ()
		{
			CreateNativeObject (new string [0], new GLib.Value [0]);
		}

		[DllImport("libgobject-2.0-0.dll", CallingConvention=CallingConvention.Cdecl)]
		static extern IntPtr g_object_new (IntPtr gtype, IntPtr dummy);

		[Obsolete]
		protected Object (GType gtype)
		{
			Raw = g_object_new (gtype.Val, IntPtr.Zero);
		}

		[DllImport("glibsharpglue-2", CallingConvention=CallingConvention.Cdecl)]
		static extern IntPtr gtksharp_object_newv (IntPtr gtype, int n_params, IntPtr[] names, GLib.Value[] vals);

		protected virtual void CreateNativeObject (string[] names, GLib.Value[] vals)
		{
			CreateNativeObject (names, vals, names.Length);
		}

		protected void CreateNativeObject (string [] names, GLib.Value [] vals, int count)
		{
			IntPtr[] native_names = new IntPtr [count];
			for (int i = 0; i < count; i++)
				native_names [i] = GLib.Marshaller.StringToPtrGStrdup (names [i]);
			CreateNativeObject (native_names, vals, count);
		}

		protected void CreateNativeObject (IntPtr [] native_names, GLib.Value [] vals, int count)
		{
			owned = true;
			Raw = gtksharp_object_newv (LookupGType ().Val, count, native_names, vals);
			for (int i = 0; i < count; ++i) {
				GLib.Marshaller.Free (native_names [i]);
				vals [i].Dispose ();
			}
		}

		protected virtual IntPtr Raw {
			get {
				return handle;
			}
			set {
				if (handle == value)
					return;

				lock(Objects) {
					if (handle != IntPtr.Zero) {
						Objects.Remove (handle);
						if (tref != null) {
							tref.Free ();
							tref = null;
						}
					}
					handle = value;
					if (value != IntPtr.Zero) {
						tref = new ToggleRef (this);
						Objects [value] = tref;
					}
				}
			}
        	}

		public static GLib.GType GType {
			get {
				return GType.Object;
			}
		}

		[DllImport("glibsharpglue-2", CallingConvention=CallingConvention.Cdecl)]
		static extern IntPtr gtksharp_get_type_name (IntPtr raw);

		protected string TypeName {
			get {
				return Marshaller.Utf8PtrToString (gtksharp_get_type_name (Raw));
			}
		}

		internal GLib.GType NativeType {
			get {
				return LookupGType ();
			}
		}

		internal ToggleRef ToggleRef {
			get {
				return tref;
			}
		}

		public IntPtr Handle {
			get {
				return handle;
			}
		}

		[DebuggerBrowsable (DebuggerBrowsableState.Never)]
		public IntPtr OwnedHandle {
			get {
				return g_object_ref (handle);
			}
		}

		Hashtable before_signals;
		[Obsolete ("Replaced by GLib.Signal marshaling mechanism.")]
		protected internal Hashtable BeforeSignals {
			get {
				if (before_signals == null)
					before_signals = new Hashtable ();
				return before_signals;
			}
		}

		Hashtable after_signals;
		[Obsolete ("Replaced by GLib.Signal marshaling mechanism.")]
		protected internal Hashtable AfterSignals {
			get {
				if (after_signals == null)
					after_signals = new Hashtable ();
				return after_signals;
			}
		}

		EventHandlerList before_handlers;
		[Obsolete ("Replaced by GLib.Signal marshaling mechanism.")]
		protected EventHandlerList BeforeHandlers {
			get {
				if (before_handlers == null)
					before_handlers = new EventHandlerList ();
				return before_handlers;
			}
		}

		EventHandlerList after_handlers;
		[Obsolete ("Replaced by GLib.Signal marshaling mechanism.")]
		protected EventHandlerList AfterHandlers {
			get {
				if (after_handlers == null)
					after_handlers = new EventHandlerList ();
				return after_handlers;
			}
		}

		[UnmanagedFunctionPointer (CallingConvention.Cdecl)]
		delegate void NotifyDelegate (IntPtr handle, IntPtr pspec, IntPtr gch);

		void NotifyCallback (IntPtr handle, IntPtr pspec, IntPtr gch)
		{
			try {
				GLib.Signal sig = ((GCHandle) gch).Target as GLib.Signal;
				if (sig == null)
					throw new Exception("Unknown signal GC handle received " + gch);

				NotifyArgs args = new NotifyArgs ();
				args.Args = new object[1];
				args.Args[0] = pspec;
				NotifyHandler handler = (NotifyHandler) sig.Handler;
				handler (GLib.Object.GetObject (handle), args);
			} catch (Exception e) {
				ExceptionManager.RaiseUnhandledException (e, false);
			}
		}

		void ConnectNotification (string signal, NotifyHandler handler)
		{
			Signal sig = Signal.Lookup (this, signal, new NotifyDelegate (NotifyCallback));
			sig.AddDelegate (handler);
		}

		public void AddNotification (string property, NotifyHandler handler)
		{
			ConnectNotification ("notify::" + property, handler);
		}

		public void AddNotification (NotifyHandler handler)
		{
			ConnectNotification ("notify", handler);
		}

		void DisconnectNotification (string signal, NotifyHandler handler)
		{
			Signal sig = Signal.Lookup (this, signal, new NotifyDelegate (NotifyCallback));
			sig.RemoveDelegate (handler);
		}

		public void RemoveNotification (string property, NotifyHandler handler)
		{
			DisconnectNotification ("notify::" + property, handler);
		}

		public void RemoveNotification (NotifyHandler handler)
		{
			DisconnectNotification ("notify", handler);
		}

		public override int GetHashCode ()
		{
			return Handle.GetHashCode ();
		}

		public Hashtable Data {
			get {
				if (data == null)
					data = new Hashtable ();

				return data;
			}
		}

		Hashtable persistent_data;
		protected Hashtable PersistentData {
			get {
				if (persistent_data == null)
					persistent_data = new Hashtable ();

				return persistent_data;
			}
		}

		[DllImport("libgobject-2.0-0.dll", CallingConvention=CallingConvention.Cdecl)]
		static extern void g_object_get_property (IntPtr obj, IntPtr name, ref GLib.Value val);

		protected GLib.Value GetProperty (string name)
		{
			IntPtr native_name = GLib.Marshaller.StringToPtrGStrdup (name);
			Value val = new Value (this, native_name);
			g_object_get_property (Raw, native_name, ref val);
			GLib.Marshaller.Free (native_name);
			return val;
		}

		[DllImport("libgobject-2.0-0.dll", CallingConvention=CallingConvention.Cdecl)]
		static extern void g_object_set_property (IntPtr obj, IntPtr name, ref GLib.Value val);

		protected void SetProperty (string name, GLib.Value val)
		{
			IntPtr native_name = GLib.Marshaller.StringToPtrGStrdup (name);
			g_object_set_property (Raw, native_name, ref val);
			GLib.Marshaller.Free (native_name);
		}

		[DllImport("libgobject-2.0-0.dll", CallingConvention=CallingConvention.Cdecl)]
		static extern void g_object_notify (IntPtr obj, IntPtr property_name);

		protected void Notify (string property_name)
		{
			IntPtr native_name = GLib.Marshaller.StringToPtrGStrdup (property_name);
			g_object_notify (Handle, native_name);
			GLib.Marshaller.Free (native_name);
		}

		[DllImport("glibsharpglue-2", CallingConvention=CallingConvention.Cdecl)]
		static extern void gtksharp_override_virtual_method (IntPtr gtype, IntPtr name, Delegate cb);

		protected static void OverrideVirtualMethod (GType gtype, string name, Delegate cb)
		{
			IntPtr native_name = GLib.Marshaller.StringToPtrGStrdup (name);
			gtksharp_override_virtual_method (gtype.Val, native_name, cb);
			GLib.Marshaller.Free (native_name);
		}

		[DllImport("libgobject-2.0-0.dll", CallingConvention=CallingConvention.Cdecl)]
		protected static extern void g_signal_chain_from_overridden (IntPtr args, ref GLib.Value retval);

		[DllImport("glibsharpglue-2", CallingConvention=CallingConvention.Cdecl)]
		static extern bool gtksharp_is_object (IntPtr obj);

		internal static bool IsObject (IntPtr obj)
		{
			return gtksharp_is_object (obj);
		}

		[DllImport("glibsharpglue-2", CallingConvention=CallingConvention.Cdecl)]
		static extern int gtksharp_object_get_ref_count (IntPtr obj);

		protected int RefCount {
			get {
				return gtksharp_object_get_ref_count (Handle);
			}
		}

		internal void Harden ()
		{
			tref.Harden ();
		}

		static Object ()
		{
			if (Environment.GetEnvironmentVariable ("GTK_SHARP_DEBUG") != null)
				GLib.Log.SetLogHandler ("GLib-GObject", GLib.LogLevelFlags.All, new GLib.LogFunc (GLib.Log.PrintTraceLogFunction));
		}
	}
}
