// GLib.Signal.cs - signal marshaling class
//
// Authors: Mike Kestner <mkestner@novell.com>
//          Andrés G. Aragoneses <aaragoneses@novell.com>
//
// Copyright (c) 2005,2008 Novell, Inc.
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
	using System.Runtime.InteropServices;

	[Flags]
	public enum ConnectFlags {
		After = 1 << 0,
		Swapped = 1 << 1,
	}

	public partial class Signal {

		[Flags]
		public enum Flags {
			RunFirst = 1 << 0,
			RunLast = 1 << 1,
			RunCleanup = 1 << 2,
			NoRecurse = 1 << 3,
			Detailed = 1 << 4,
			Action = 1 << 5,
			NoHooks = 1 << 6
		}

		[StructLayout (LayoutKind.Sequential)]
		public struct InvocationHint {
			public uint signal_id;
			public uint detail;
			public Flags run_type;
		}

		[UnmanagedFunctionPointer (CallingConvention.Cdecl)]
		public delegate bool EmissionHookNative (ref InvocationHint hint, uint n_pvals, IntPtr pvals, IntPtr data);

		public delegate bool EmissionHook (InvocationHint ihint, object[] inst_and_param_values);

		public class EmissionHookMarshaler {

			EmissionHook handler;
			EmissionHookNative cb;
			IntPtr user_data;
			GCHandle gch;

			public EmissionHookMarshaler (EmissionHook handler)
			{
				this.handler = handler;
				cb = new EmissionHookNative (NativeCallback);
				gch = GCHandle.Alloc (this);
			}

			public EmissionHookMarshaler (EmissionHookNative callback, IntPtr user_data)
			{
				cb = callback;
				this.user_data = user_data;
				handler = new EmissionHook (NativeInvoker);
			}

			bool NativeCallback (ref InvocationHint hint, uint n_pvals, IntPtr pvals_ptr, IntPtr data)
			{
				object[] pvals = new object [n_pvals];

				unsafe {
					Value* vals = (Value *)pvals_ptr;
					for (int i = 0; i < n_pvals; ++i)
						pvals [i] = vals [i].Val;
				}

				bool result = handler (hint, pvals);
				if (!result)
					gch.Free ();
				return result;
			}

			public EmissionHookNative Callback {
				get {
					return cb;
				}
			}

			bool NativeInvoker (InvocationHint ihint, object[] pvals)
			{
				int val_sz = Marshal.SizeOf (typeof (Value));
				unsafe
				{
					Value* vals = stackalloc Value [pvals.Length];
					for (int i = 0; i < pvals.Length; i++) {
						vals [i] = new Value (pvals [i]);
					}

					bool result = cb (ref ihint, (uint)pvals.Length, (IntPtr)vals, user_data);

					for (int i = 0; i < pvals.Length; ++i)
						vals[i].Dispose ();
					return result;
				}
			}

			public EmissionHook Invoker {
				get {
					return handler;
				}
			}
		}

		ToggleRef tref;
		string name;
		Type args_type;
		SignalClosure before_closure;
		SignalClosure after_closure;
		Delegate marshaler;

		private Signal (GLib.Object obj, string signal_name, Delegate marshaler)
		{
			tref = obj.ToggleRef;
			name = signal_name;
			tref.Signals [name] = this;
			this.marshaler = marshaler;
		}

		private Signal (GLib.Object obj, string signal_name, Type args_type)
		{
			tref = obj.ToggleRef;
			name = signal_name;
			this.args_type = args_type;
			tref.Signals [name] = this;
		}

		internal void Free ()
		{
			if (before_closure != null) {
				before_closure.Dispose();
				before_closure = null;
			}
			if (after_closure != null) {
				after_closure.Dispose();
				after_closure = null;
			}
			GC.SuppressFinalize (this);
		}

		static void ClosureDisposedCB (object o, EventArgs args)
		{
			var closure = (SignalClosure)o;
			var signal = closure.signal;
			var tref = signal.tref;
			var name = signal.name;

			closure.Disposed -= ClosureDisposedHandler;
			closure.Invoked -= ClosureInvokedHandler;

			if (closure == signal.before_closure) {
				if (tref.Target != null)
					tref.Target.BeforeSignals.Remove (name);
				signal.before_closure = null;
			} else {
				if (tref.Target != null)
					tref.Target.AfterSignals.Remove (name);
				signal.after_closure = null;
			}

			if (signal.before_closure == null && signal.after_closure == null)
				tref.RemoveSignal (name);
		}

		static EventHandler closure_disposed_cb;
		static EventHandler ClosureDisposedHandler {
			get {
				if (closure_disposed_cb == null)
					closure_disposed_cb = new EventHandler (ClosureDisposedCB);
				return closure_disposed_cb;
			}
		}

		static void ClosureInvokedCB (object o, ClosureInvokedArgs args)
		{
			var closure = (SignalClosure)o;
			Delegate handler;
			if (closure == closure.signal.before_closure)
				handler = args.Target.BeforeSignals [closure.signal.name] as Delegate;
			else
				handler = args.Target.AfterSignals [closure.signal.name] as Delegate;

			if (handler != null)
				handler.DynamicInvoke (new object [] { args.Target, args.Args });
		}

		static ClosureInvokedHandler closure_invoked_cb;
		static ClosureInvokedHandler ClosureInvokedHandler {
			get {
				if (closure_invoked_cb == null)
					closure_invoked_cb = new ClosureInvokedHandler (ClosureInvokedCB);
				return closure_invoked_cb;
			}
		}

		public static Signal Lookup (GLib.Object obj, string name)
		{
			return Lookup (obj, name, typeof (EventArgs));
		}

		public static Signal Lookup (GLib.Object obj, string name, Delegate marshaler)
		{
			Signal result = obj.ToggleRef.Signals [name] as Signal;
			if (result == null)
				result = new Signal (obj, name, marshaler);
			return result;
		}

		public static Signal Lookup (GLib.Object obj, string name, Type args_type)
		{
			Signal result = obj.ToggleRef.Signals [name] as Signal;
			if (result == null)
				result = new Signal (obj, name, args_type);
			return result;
		}


		public Delegate Handler {
			get {
				unsafe
				{
					InvocationHint *hint = (InvocationHint*)g_signal_get_invocation_hint (tref.Handle);
					if (hint->run_type == Flags.RunFirst)
						return tref.Target.BeforeSignals [name] as Delegate;
					else
						return tref.Target.AfterSignals [name] as Delegate;
				}
			}
		}

		public void AddDelegate (Delegate d)
		{
			if (args_type == null)
				args_type = d.Method.GetParameters ()[1].ParameterType;

			if (d.Method.IsDefined (typeof (ConnectBeforeAttribute), false)) {
				tref.Target.BeforeSignals [name] = Delegate.Combine (tref.Target.BeforeSignals [name] as Delegate, d);
				if (before_closure == null) {
					if (marshaler == null)
						before_closure = new SignalClosure (this);
					else
						before_closure = new SignalClosure (this, marshaler);
					before_closure.Disposed += ClosureDisposedHandler;
					before_closure.Invoked += ClosureInvokedHandler;
					before_closure.Connect (false);
				}
			} else {
				tref.Target.AfterSignals [name] = Delegate.Combine (tref.Target.AfterSignals [name] as Delegate, d);
				if (after_closure == null) {
					if (marshaler == null)
						after_closure = new SignalClosure (this);
					else
						after_closure = new SignalClosure (this, marshaler);
					after_closure.Disposed += ClosureDisposedHandler;
					after_closure.Invoked += ClosureInvokedHandler;
					after_closure.Connect (true);
				}
			}
		}

		public void RemoveDelegate (Delegate d)
		{
			if (tref.Target == null)
				return;

			bool before = d.Method.IsDefined (typeof (ConnectBeforeAttribute), false);
			Hashtable hash = before ? tref.Target.BeforeSignals : tref.Target.AfterSignals;

			Delegate del = Delegate.Remove (hash [name] as Delegate, d);
			if (del != null) {
				hash [name] = del;
				return;
			}

			hash.Remove (name);
			if (before) {
				if (before_closure != null) {
					before_closure.Dispose ();
					before_closure = null;
				}
			} else {
				if (after_closure != null) {
					after_closure.Dispose ();
					after_closure = null;
				}
			}
		}

		// format: children-changed::add
		private static void ParseSignalDetail (string signal_detail, out string signal_name, out uint gquark)
		{
			//can't use String.Split because it doesn't accept a string arg (only char) in the 1.x profile
			int link_pos = signal_detail.IndexOf ("::");
			if (link_pos < 0) {
				gquark = 0;
				signal_name = signal_detail;
			} else if (link_pos == 0) {
				throw new FormatException ("Invalid detailed signal: " + signal_detail);
			} else {
				signal_name = signal_detail.Substring (0, link_pos);
				gquark = GetGQuarkFromString (signal_detail.Substring (link_pos + 2));
			}
		}

		public static object Emit (GLib.Object instance, string detailed_signal, params object[] args)
		{
			uint gquark, signal_id;
			string signal_name;
			ParseSignalDetail (detailed_signal, out signal_name, out gquark);
			signal_id = GetSignalId (signal_name, instance);
			if (signal_id <= 0)
				throw new ArgumentException ("Invalid signal name: " + signal_name);
			unsafe
			{
				int valsLength = args.Length + 1;
				GLib.Value *vals = stackalloc GLib.Value [valsLength];

				vals [0] = new GLib.Value (instance);
				for (int i = 1; i < valsLength; i++)
					vals [i] = new GLib.Value (args [i - 1]);

				object ret_obj = null;
				GType retType = glibsharp_signal_get_return_type (signal_id);
				if (retType != GType.None) {
					GLib.Value ret = new GLib.Value (retType);
					g_signal_emitv (vals, signal_id, gquark, ref ret);
					ret_obj = ret.Val;
					ret.Dispose ();
				} else
					g_signal_emitv (vals, signal_id, gquark, IntPtr.Zero);

				for (int i = 0; i < valsLength; ++i)
					vals[i].Dispose ();

				return ret_obj;
			}
		}

		private static uint GetGQuarkFromString (string str) {
			IntPtr native_string = GLib.Marshaller.StringToPtrGStrdup (str);
			uint ret = g_quark_from_string (native_string);
			GLib.Marshaller.Free (native_string);
			return ret;
		}

		private static uint GetSignalId (string signal_name, GLib.Object obj)
		{
			IntPtr typeid = gtksharp_get_type_id (obj.Handle);
			return GetSignalId (signal_name, typeid);
		}

		private static uint GetSignalId (string signal_name, IntPtr typeid)
		{
			IntPtr native_name = GLib.Marshaller.StringToPtrGStrdup (signal_name);
			uint signal_id = g_signal_lookup (native_name, typeid);
			GLib.Marshaller.Free (native_name);
			return signal_id;
		}

		public static ulong AddEmissionHook (string detailed_signal, GLib.GType type, EmissionHook handler_func)
		{
			uint gquark;
			string signal_name;
			ParseSignalDetail (detailed_signal, out signal_name, out gquark);
			uint signal_id = GetSignalId (signal_name, type.Val);
			if (signal_id <= 0)
				throw new Exception ("Invalid signal name: " + signal_name);
			return g_signal_add_emission_hook (signal_id, gquark, new EmissionHookMarshaler (handler_func).Callback, IntPtr.Zero, IntPtr.Zero);
		}

		[DllImport("libgobject-2.0-0.dll", CallingConvention=CallingConvention.Cdecl)]
		static extern IntPtr g_signal_get_invocation_hint (IntPtr instance);

		[DllImport("libgobject-2.0-0.dll", CallingConvention=CallingConvention.Cdecl)]
		unsafe static extern void g_signal_emitv (GLib.Value* instance_and_params, uint signal_id, uint gquark_detail, ref GLib.Value return_value);

		[DllImport("libgobject-2.0-0.dll", CallingConvention=CallingConvention.Cdecl)]
		unsafe static extern void g_signal_emitv (GLib.Value* instance_and_params, uint signal_id, uint gquark_detail, IntPtr return_value);

		[DllImport("glibsharpglue-2", CallingConvention=CallingConvention.Cdecl)]
		static extern GType glibsharp_signal_get_return_type (uint signal_id);

		[DllImport("libgobject-2.0-0.dll", CallingConvention=CallingConvention.Cdecl)]
		static extern uint g_signal_lookup (IntPtr name, IntPtr itype);

		//better not to expose g_quark_from_static_string () due to memory allocation issues
		[DllImport("libglib-2.0-0.dll", CallingConvention=CallingConvention.Cdecl)]
		static extern uint g_quark_from_string (IntPtr str);

		[DllImport("glibsharpglue-2", CallingConvention=CallingConvention.Cdecl)]
		static extern IntPtr gtksharp_get_type_id (IntPtr raw);

		[DllImport("libgobject-2.0-0.dll", CallingConvention=CallingConvention.Cdecl)]
		static extern ulong g_signal_add_emission_hook (uint signal_id, uint gquark_detail, EmissionHookNative hook_func, IntPtr hook_data, IntPtr data_destroy);

	}
}
