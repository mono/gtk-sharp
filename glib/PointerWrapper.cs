// SafeObjectHandle.cs - SafeHandle implementation for GObject
//
// Authors: Marius Ungureanu <maungu@microsoft.com>
//
// Copyright (c) 2017-2017 Microsoft, Inc.
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

using System;
using System.Collections.Generic;

namespace GLib
{
	class PointerWrapper : IDisposable
	{
		internal ToggleRef tref;
		internal IntPtr handle;

		static Action<IntPtr> ObjectCreated;
		static Action<IntPtr> ObjectDestroyed;

		static readonly Dictionary<IntPtr, ToggleRef> Objects = new Dictionary<IntPtr, ToggleRef> (IntPtrEqualityComparer.Instance);

		protected PointerWrapper (IntPtr handle)
		{
			this.handle = handle;
		}

		internal static PointerWrapper Create (Object obj, IntPtr handle)
		{
			if (handle == IntPtr.Zero)
				return null;

			PointerWrapper safeHandle;
			safeHandle = new PointerWrapper (handle);
			safeHandle.tref = new ToggleRef (obj, handle);

			lock (Objects)
				Objects [handle] = safeHandle.tref;
			
			if (ObjectCreated != null)
				ObjectCreated.Invoke (handle);

			return safeHandle;
		}

		internal static bool TryGetObject (IntPtr o, out Object obj)
		{
			bool ret;
			ToggleRef tr;

			lock (Objects)
				ret = Objects.TryGetValue (o, out tr);

			obj = ret ? tr.Target : null;
			return obj != null;
		}

		static List<ToggleRef> PendingDestroys = new List<ToggleRef> ();
		static readonly object lockObject = new object ();
		static bool idle_queued;

		static TimeoutHandler PerformQueuedUnrefsHandler = PerformQueuedUnrefs;
		static bool PerformQueuedUnrefs ()
		{
			List<ToggleRef> references;

			lock (lockObject) {
				references = PendingDestroys;
				PendingDestroys = new List<ToggleRef>();
				idle_queued = false;
			}

			foreach (ToggleRef r in references)
				r.Free();

			return false;
		}

		#region IDisposable
		~PointerWrapper ()
		{
			lock (Objects)
				Objects.Remove (handle);

			if (ObjectDestroyed != null)
				ObjectDestroyed.Invoke (handle);

			lock (lockObject) {
				if (tref != null) {
					PendingDestroys.Add (tref);
					if (!idle_queued) {
						Timeout.Add (50, PerformQueuedUnrefsHandler);
						idle_queued = true;
					}
				}
			}
		}

		public void Dispose ()
		{
			if (handle == IntPtr.Zero)
				return;

			GC.SuppressFinalize (this);
			lock (Objects)
				Objects.Remove (handle);

			if (ObjectDestroyed != null)
				ObjectDestroyed.Invoke (handle);
			
			handle = IntPtr.Zero;
			tref.Free ();
		}
		#endregion
	}
}
