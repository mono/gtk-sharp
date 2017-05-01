// Opaque .cs - Opaque struct wrapper implementation
//
// Authors: Bob Smith <bob@thestuff.net>
//	    Mike Kestner <mkestner@speakeasy.net>
//	    Rachel Hestilow <hestilow@ximian.com>
//
// Copyright (c) 2001 Bob Smith 
// Copyright (c) 2001 Mike Kestner
// Copyright (c) 2002 Rachel Hestilow
// Copyright (c) 2004 Novell, Inc.
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
	using System.Runtime.InteropServices;

	public class Opaque : IWrapper, IDisposable {

		IntPtr _obj;
		bool owned;

		[Obsolete ("Use more explicit overload.  This method always returns null")]
		public static Opaque GetOpaque (IntPtr o)
		{
			return null;
		}

		public static Opaque GetOpaque (IntPtr o, Type type, bool owned)
		{
			Opaque opaque = FastActivator.CreateOpaque (o, type);
			if (owned) {
				if (opaque.Owned) {
					// The constructor took a Ref it shouldn't have, so undo it
					opaque.Unref (o);
				}
				opaque.Owned = true;
			} else 
				opaque = opaque.Copy (o);

			return opaque;
  		}
  
		public Opaque ()
		{
			owned = true;
		}

		public Opaque (IntPtr raw)
		{
			GC.SuppressFinalize (this);
			owned = false;
			Raw = raw;
		}

		protected IntPtr Raw {
			get {
				return _obj;
			}
			set {
				if (_obj != IntPtr.Zero) {
					Unref (_obj);
					if (owned)
						Free (_obj);
				}
				_obj = value;
				if (_obj != IntPtr.Zero) {
					Ref (_obj);
				}
			}
		}

		static object lockObject = new object ();
		static List<Opaque> PendingFrees = new List<Opaque> ();
		static bool idleQueued;

		bool PerformQueuedFrees ()
		{
			List<Opaque> references;
			lock (lockObject) {
				references = PendingFrees;
				PendingFrees = new List<Opaque> ();
				idleQueued = false;
			}

			foreach (var opaque in references)
				opaque.Raw = IntPtr.Zero;
			
			return false;
		}

		~Opaque ()
		{
			lock (lockObject) {
				PendingFrees.Add (this);
				if (!idleQueued) {
					idleQueued = true;
					Timeout.Add (50, new TimeoutHandler (PerformQueuedFrees));
				}
			}
		}

		public virtual void Dispose ()
		{
			Raw = IntPtr.Zero;
			GC.SuppressFinalize (this);
		}

		// These take an IntPtr arg so we don't get conflicts if we need
		// to have an "[Obsolete] public void Ref ()"

		protected virtual void Ref (IntPtr raw) {}
		protected virtual void Unref (IntPtr raw) {}
		protected virtual void Free (IntPtr raw) {}
		protected virtual Opaque Copy (IntPtr raw) 
		{
			return this;
		}

		public IntPtr Handle {
			get {
				return _obj;
			}
		}

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public IntPtr OwnedCopy {
			get {
				Opaque result = Copy (Handle);
				result.Owned = false;
				return result.Handle;
			}
		}

		public bool Owned {
			get {
				return owned;
			}
			set {
				if (owned == value)
					return;

				if (value)
					GC.ReRegisterForFinalize (this);
				else
					GC.SuppressFinalize (this);
				owned = value;
			}
		}

		public override bool Equals (object o)
		{
			if (!(o is Opaque))
				return false;

			return (Handle == ((Opaque) o).Handle);
		}

		public override int GetHashCode ()
		{
			return Handle.GetHashCode ();
		}
	}
}
