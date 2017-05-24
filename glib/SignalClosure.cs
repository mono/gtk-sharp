// SignalClosure.cs - signal marshaling class
//
// Authors: Mike Kestner <mkestner@novell.com>
//
// Copyright (c) 2008 Novell, Inc.
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
	using System.Runtime.InteropServices;

	internal class ClosureInvokedArgs : EventArgs {

		EventArgs args;
		GLib.Object obj;
		object result;

		public ClosureInvokedArgs (GLib.Object obj, EventArgs args)
		{
			this.obj = obj;
			this.args = args;
		}

		public EventArgs Args {
			get {
				return args;
			}
		}

		public GLib.Object Target {
			get {
				return obj;
			}
		}
	}

	internal delegate void ClosureInvokedHandler (object o, ClosureInvokedArgs args);

	public partial class Signal
	{
		class SignalClosure : IDisposable
		{
			internal Signal signal;
			IntPtr raw_closure;
			uint id = UInt32.MaxValue;
			GCHandle? gch;

			static Dictionary<IntPtr, SignalClosure> closures = new Dictionary<IntPtr, SignalClosure> (IntPtrEqualityComparer.Instance);

			public SignalClosure (Signal sig, System.Type args_type)
			{
				raw_closure = glibsharp_closure_new (Marshaler, Notify, IntPtr.Zero);
				closures [raw_closure] = this;
				signal = sig;
			}

			public SignalClosure (Signal sig, Delegate custom_marshaler)
			{
				gch = GCHandle.Alloc (sig);
				raw_closure = g_cclosure_new (custom_marshaler, (IntPtr)gch, Notify);
				closures [raw_closure] = this;
				signal = sig;
			}

			public event EventHandler Disposed;
			public event ClosureInvokedHandler Invoked;

			public void Connect (bool is_after)
			{
				IntPtr native_name = GLib.Marshaller.StringToPtrGStrdup (signal.name);
				id = g_signal_connect_closure (signal.tref.Handle, native_name, raw_closure, is_after);
				GLib.Marshaller.Free (native_name);
			}

			public void Disconnect ()
			{
				if (id != UInt32.MaxValue && g_signal_handler_is_connected (signal.tref.Handle, id))
					g_signal_handler_disconnect (signal.tref.Handle, id);
			}

			public void Dispose ()
			{
				Disconnect ();
				closures.Remove (raw_closure);
				if (gch != null) {
					gch.Value.Free ();
					gch = null;
				}
				if (Disposed != null)
					Disposed (this, EventArgs.Empty);
				GC.SuppressFinalize (this);
			}

			public void Invoke (ClosureInvokedArgs args)
			{
				if (Invoked == null)
					return;
				Invoked (this, args);
			}

			static ClosureMarshal marshaler;
			static ClosureMarshal Marshaler {
				get {
					if (marshaler == null) {
						unsafe
						{
							marshaler = new ClosureMarshal (MarshalCallback);
						}
					}
					return marshaler;
				}
			}

			[UnmanagedFunctionPointer (CallingConvention.Cdecl)]
			unsafe delegate void ClosureMarshal (IntPtr closure, Value* return_val, uint n_param_vals, Value* param_values, IntPtr invocation_hint, IntPtr marshal_data);

			unsafe static void MarshalCallback (IntPtr raw_closure, Value* return_val, uint n_param_vals, Value* param_values, IntPtr invocation_hint, IntPtr marshal_data)
			{
				SignalClosure closure = null;
				try {
					closure = closures [raw_closure] as SignalClosure;
					GLib.Object __obj = param_values [0].Val as GLib.Object;
					if (__obj == null)
						return;

					if (closure.signal.args_type == typeof (EventArgs)) {
						closure.Invoke (new ClosureInvokedArgs (__obj, EventArgs.Empty));
						return;
					}

					SignalArgs args = FastActivator.CreateSignalArgs (closure.signal.args_type);
					args.Args = new object [n_param_vals - 1];
					for (int i = 1; i < n_param_vals; i++) {
						args.Args [i - 1] = param_values [i].Val;
					}
					ClosureInvokedArgs ci_args = new ClosureInvokedArgs (__obj, args);
					closure.Invoke (ci_args);
					for (int i = 1; i < n_param_vals; i++) {
						param_values [i].Update (args.Args [i - 1]);
					}
					if (return_val == null || args.RetVal == null)
						return;

					return_val->Val = args.RetVal;
				} catch (Exception e) {
					if (closure != null)
						Console.WriteLine ("Marshaling {0} signal", closure.signal.name);
					ExceptionManager.RaiseUnhandledException (e, false);
				}
			}

			[UnmanagedFunctionPointer (CallingConvention.Cdecl)]
			delegate void ClosureNotify (IntPtr data, IntPtr closure);

			static void NotifyCallback (IntPtr data, IntPtr raw_closure)
			{
				SignalClosure closure;
				if (closures.TryGetValue (raw_closure, out closure))
					closure.Dispose ();
			}

			static ClosureNotify notify_handler;
			static ClosureNotify Notify {
				get {
					if (notify_handler == null)
						notify_handler = new ClosureNotify (NotifyCallback);
					return notify_handler;
				}
			}

			[DllImport ("glibsharpglue-2", CallingConvention = CallingConvention.Cdecl)]
			static extern IntPtr glibsharp_closure_new (ClosureMarshal marshaler, ClosureNotify notify, IntPtr gch);

			[DllImport ("libgobject-2.0-0.dll", CallingConvention = CallingConvention.Cdecl)]
			static extern IntPtr g_cclosure_new (Delegate cb, IntPtr user_data, ClosureNotify notify);

			[DllImport ("libgobject-2.0-0.dll", CallingConvention = CallingConvention.Cdecl)]
			static extern uint g_signal_connect_closure (IntPtr obj, IntPtr name, IntPtr closure, bool is_after);

			[DllImport ("libgobject-2.0-0.dll", CallingConvention = CallingConvention.Cdecl)]
			static extern void g_signal_handler_disconnect (IntPtr instance, uint handler);

			[DllImport ("libgobject-2.0-0.dll", CallingConvention = CallingConvention.Cdecl)]
			static extern bool g_signal_handler_is_connected (IntPtr instance, uint handler);
		}
	}
}
