// This file was auto-generated at one time, but is hardcoded here as part of the fix
// for the AtkTable interface;  see https://bugzilla.novell.com/show_bug.cgi?id=512477
// The generated code may have been modified as part of this fix; see atk-table.patch

namespace Atk {

	using System;

	public delegate void ColumnDeletedHandler(object o, ColumnDeletedArgs args);

	public class ColumnDeletedArgs : GLib.SignalArgs {
		public int Column{
			get {
				return (int) Args[0];
			}
		}

		public int NumDeleted{
			get {
				return (int) Args[1];
			}
		}

	}
}
