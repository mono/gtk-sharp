// This file was auto-generated at one time, but is hardcoded here as part of the fix
// for the AtkTable interface;  see https://bugzilla.novell.com/show_bug.cgi?id=512477
// The generated code may have been modified as part of this fix; see atk-table.patch

namespace Atk {

	using System;
	using System.Runtime.InteropServices;

#region Autogenerated code
	public class TableAdapter : GLib.GInterfaceAdapter, Atk.Table {

		static TableIface iface;

		struct TableIface {
			public IntPtr gtype;
			public IntPtr itype;

			public RefAtDelegate ref_at;
			public GetIndexAtDelegate get_index_at;
			public GetColumnAtIndexDelegate get_column_at_index;
			public GetRowAtIndexDelegate get_row_at_index;
			public GetNColumnsDelegate get_n_columns;
			public GetNRowsDelegate get_n_rows;
			public GetColumnExtentAtDelegate get_column_extent_at;
			public GetRowExtentAtDelegate get_row_extent_at;
			public GetCaptionDelegate get_caption;
			public GetColumnDescriptionDelegate get_column_description;
			public GetColumnHeaderDelegate get_column_header;
			public GetRowDescriptionDelegate get_row_description;
			public GetRowHeaderDelegate get_row_header;
			public GetSummaryDelegate get_summary;
			public SetCaptionDelegate set_caption;
			public SetColumnDescriptionDelegate set_column_description;
			public SetColumnHeaderDelegate set_column_header;
			public SetRowDescriptionDelegate set_row_description;
			public SetRowHeaderDelegate set_row_header;
			public SetSummaryDelegate set_summary;
			public GetSelectedColumnsDelegate get_selected_columns;
			public GetSelectedRowsDelegate get_selected_rows;
			public IsColumnSelectedDelegate is_column_selected;
			public IsRowSelectedDelegate is_row_selected;
			public IsSelectedDelegate is_selected;
			public AddRowSelectionDelegate add_row_selection;
			public RemoveRowSelectionDelegate remove_row_selection;
			public AddColumnSelectionDelegate add_column_selection;
			public RemoveColumnSelectionDelegate remove_column_selection;
			public IntPtr row_inserted;
			public IntPtr column_inserted;
			public IntPtr row_deleted;
			public IntPtr column_deleted;
			public IntPtr row_reordered;
			public IntPtr column_reordered;
			public IntPtr model_changed;
		}

		static TableAdapter ()
		{
			GLib.GType.Register (_gtype, typeof(TableAdapter));
			iface.ref_at = new RefAtDelegate (RefAtCallback);
			iface.get_index_at = new GetIndexAtDelegate (GetIndexAtCallback);
			iface.get_column_at_index = new GetColumnAtIndexDelegate (GetColumnAtIndexCallback);
			iface.get_row_at_index = new GetRowAtIndexDelegate (GetRowAtIndexCallback);
			iface.get_n_columns = new GetNColumnsDelegate (GetNColumnsCallback);
			iface.get_n_rows = new GetNRowsDelegate (GetNRowsCallback);
			iface.get_column_extent_at = new GetColumnExtentAtDelegate (GetColumnExtentAtCallback);
			iface.get_row_extent_at = new GetRowExtentAtDelegate (GetRowExtentAtCallback);
			iface.get_caption = new GetCaptionDelegate (GetCaptionCallback);
			iface.get_column_description = new GetColumnDescriptionDelegate (GetColumnDescriptionCallback);
			iface.get_column_header = new GetColumnHeaderDelegate (GetColumnHeaderCallback);
			iface.get_row_description = new GetRowDescriptionDelegate (GetRowDescriptionCallback);
			iface.get_row_header = new GetRowHeaderDelegate (GetRowHeaderCallback);
			iface.get_summary = new GetSummaryDelegate (GetSummaryCallback);
			iface.set_caption = new SetCaptionDelegate (SetCaptionCallback);
			iface.set_column_description = new SetColumnDescriptionDelegate (SetColumnDescriptionCallback);
			iface.set_column_header = new SetColumnHeaderDelegate (SetColumnHeaderCallback);
			iface.set_row_description = new SetRowDescriptionDelegate (SetRowDescriptionCallback);
			iface.set_row_header = new SetRowHeaderDelegate (SetRowHeaderCallback);
			iface.set_summary = new SetSummaryDelegate (SetSummaryCallback);
			iface.get_selected_columns = new GetSelectedColumnsDelegate (GetSelectedColumnsCallback);
			iface.get_selected_rows = new GetSelectedRowsDelegate (GetSelectedRowsCallback);
			iface.is_column_selected = new IsColumnSelectedDelegate (IsColumnSelectedCallback);
			iface.is_row_selected = new IsRowSelectedDelegate (IsRowSelectedCallback);
			iface.is_selected = new IsSelectedDelegate (IsSelectedCallback);
			iface.add_row_selection = new AddRowSelectionDelegate (AddRowSelectionCallback);
			iface.remove_row_selection = new RemoveRowSelectionDelegate (RemoveRowSelectionCallback);
			iface.add_column_selection = new AddColumnSelectionDelegate (AddColumnSelectionCallback);
			iface.remove_column_selection = new RemoveColumnSelectionDelegate (RemoveColumnSelectionCallback);
		}


		[UnmanagedFunctionPointer (CallingConvention.Cdecl)]
		delegate IntPtr RefAtDelegate (IntPtr table, int row, int column);

		static IntPtr RefAtCallback (IntPtr table, int row, int column)
		{
			try {
				Atk.TableImplementor __obj = GLib.Object.GetObject (table, false) as Atk.TableImplementor;
				Atk.Object __result = __obj.RefAt (row, column);
				return __result == null ? IntPtr.Zero : __result.OwnedHandle;
			} catch (Exception e) {
				GLib.ExceptionManager.RaiseUnhandledException (e, true);
				// NOTREACHED: above call does not return.
				throw e;
			}
		}

		[UnmanagedFunctionPointer (CallingConvention.Cdecl)]
		delegate int GetIndexAtDelegate (IntPtr table, int row, int column);

		static int GetIndexAtCallback (IntPtr table, int row, int column)
		{
			try {
				Atk.TableImplementor __obj = GLib.Object.GetObject (table, false) as Atk.TableImplementor;
				int __result = __obj.GetIndexAt (row, column);
				return __result;
			} catch (Exception e) {
				GLib.ExceptionManager.RaiseUnhandledException (e, true);
				// NOTREACHED: above call does not return.
				throw e;
			}
		}

		[UnmanagedFunctionPointer (CallingConvention.Cdecl)]
		delegate int GetColumnAtIndexDelegate (IntPtr table, int index_);

		static int GetColumnAtIndexCallback (IntPtr table, int index_)
		{
			try {
				Atk.TableImplementor __obj = GLib.Object.GetObject (table, false) as Atk.TableImplementor;
				int __result = __obj.GetColumnAtIndex (index_);
				return __result;
			} catch (Exception e) {
				GLib.ExceptionManager.RaiseUnhandledException (e, true);
				// NOTREACHED: above call does not return.
				throw e;
			}
		}

		[UnmanagedFunctionPointer (CallingConvention.Cdecl)]
		delegate int GetRowAtIndexDelegate (IntPtr table, int index_);

		static int GetRowAtIndexCallback (IntPtr table, int index_)
		{
			try {
				Atk.TableImplementor __obj = GLib.Object.GetObject (table, false) as Atk.TableImplementor;
				int __result = __obj.GetRowAtIndex (index_);
				return __result;
			} catch (Exception e) {
				GLib.ExceptionManager.RaiseUnhandledException (e, true);
				// NOTREACHED: above call does not return.
				throw e;
			}
		}

		[UnmanagedFunctionPointer (CallingConvention.Cdecl)]
		delegate int GetNColumnsDelegate (IntPtr table);

		static int GetNColumnsCallback (IntPtr table)
		{
			try {
				Atk.TableImplementor __obj = GLib.Object.GetObject (table, false) as Atk.TableImplementor;
				int __result = __obj.NColumns;
				return __result;
			} catch (Exception e) {
				GLib.ExceptionManager.RaiseUnhandledException (e, true);
				// NOTREACHED: above call does not return.
				throw e;
			}
		}

		[UnmanagedFunctionPointer (CallingConvention.Cdecl)]
		delegate int GetNRowsDelegate (IntPtr table);

		static int GetNRowsCallback (IntPtr table)
		{
			try {
				Atk.TableImplementor __obj = GLib.Object.GetObject (table, false) as Atk.TableImplementor;
				int __result = __obj.NRows;
				return __result;
			} catch (Exception e) {
				GLib.ExceptionManager.RaiseUnhandledException (e, true);
				// NOTREACHED: above call does not return.
				throw e;
			}
		}

		[UnmanagedFunctionPointer (CallingConvention.Cdecl)]
		delegate int GetColumnExtentAtDelegate (IntPtr table, int row, int column);

		static int GetColumnExtentAtCallback (IntPtr table, int row, int column)
		{
			try {
				Atk.TableImplementor __obj = GLib.Object.GetObject (table, false) as Atk.TableImplementor;
				int __result = __obj.GetColumnExtentAt (row, column);
				return __result;
			} catch (Exception e) {
				GLib.ExceptionManager.RaiseUnhandledException (e, true);
				// NOTREACHED: above call does not return.
				throw e;
			}
		}

		[UnmanagedFunctionPointer (CallingConvention.Cdecl)]
		delegate int GetRowExtentAtDelegate (IntPtr table, int row, int column);

		static int GetRowExtentAtCallback (IntPtr table, int row, int column)
		{
			try {
				Atk.TableImplementor __obj = GLib.Object.GetObject (table, false) as Atk.TableImplementor;
				int __result = __obj.GetRowExtentAt (row, column);
				return __result;
			} catch (Exception e) {
				GLib.ExceptionManager.RaiseUnhandledException (e, true);
				// NOTREACHED: above call does not return.
				throw e;
			}
		}

		[UnmanagedFunctionPointer (CallingConvention.Cdecl)]
		delegate IntPtr GetCaptionDelegate (IntPtr table);

		static IntPtr GetCaptionCallback (IntPtr table)
		{
			try {
				Atk.TableImplementor __obj = GLib.Object.GetObject (table, false) as Atk.TableImplementor;
				Atk.Object __result = __obj.Caption;
				return __result == null ? IntPtr.Zero : __result.Handle;
			} catch (Exception e) {
				GLib.ExceptionManager.RaiseUnhandledException (e, true);
				// NOTREACHED: above call does not return.
				throw e;
			}
		}

		[UnmanagedFunctionPointer (CallingConvention.Cdecl)]
		delegate IntPtr GetColumnDescriptionDelegate (IntPtr table, int column);

		static IntPtr GetColumnDescriptionCallback (IntPtr table, int column)
		{
			try {
				Atk.TableImplementor __obj = GLib.Object.GetObject (table, false) as Atk.TableImplementor;
				string __result = __obj.GetColumnDescription (column);
				return GLib.Marshaller.StringToPtrGStrdup (__result);
			} catch (Exception e) {
				GLib.ExceptionManager.RaiseUnhandledException (e, true);
				// NOTREACHED: above call does not return.
				throw e;
			}
		}

		[UnmanagedFunctionPointer (CallingConvention.Cdecl)]
		delegate IntPtr GetColumnHeaderDelegate (IntPtr table, int column);

		static IntPtr GetColumnHeaderCallback (IntPtr table, int column)
		{
			try {
				Atk.TableImplementor __obj = GLib.Object.GetObject (table, false) as Atk.TableImplementor;
				Atk.Object __result = __obj.GetColumnHeader (column);
				return __result == null ? IntPtr.Zero : __result.Handle;
			} catch (Exception e) {
				GLib.ExceptionManager.RaiseUnhandledException (e, true);
				// NOTREACHED: above call does not return.
				throw e;
			}
		}

		[UnmanagedFunctionPointer (CallingConvention.Cdecl)]
		delegate IntPtr GetRowDescriptionDelegate (IntPtr table, int row);

		static IntPtr GetRowDescriptionCallback (IntPtr table, int row)
		{
			try {
				Atk.TableImplementor __obj = GLib.Object.GetObject (table, false) as Atk.TableImplementor;
				string __result = __obj.GetRowDescription (row);
				return GLib.Marshaller.StringToPtrGStrdup (__result);
			} catch (Exception e) {
				GLib.ExceptionManager.RaiseUnhandledException (e, true);
				// NOTREACHED: above call does not return.
				throw e;
			}
		}

		[UnmanagedFunctionPointer (CallingConvention.Cdecl)]
		delegate IntPtr GetRowHeaderDelegate (IntPtr table, int row);

		static IntPtr GetRowHeaderCallback (IntPtr table, int row)
		{
			try {
				Atk.TableImplementor __obj = GLib.Object.GetObject (table, false) as Atk.TableImplementor;
				Atk.Object __result = __obj.GetRowHeader (row);
				return __result == null ? IntPtr.Zero : __result.Handle;
			} catch (Exception e) {
				GLib.ExceptionManager.RaiseUnhandledException (e, true);
				// NOTREACHED: above call does not return.
				throw e;
			}
		}

		[UnmanagedFunctionPointer (CallingConvention.Cdecl)]
		delegate IntPtr GetSummaryDelegate (IntPtr table);

		static IntPtr GetSummaryCallback (IntPtr table)
		{
			try {
				Atk.TableImplementor __obj = GLib.Object.GetObject (table, false) as Atk.TableImplementor;
				Atk.Object __result = __obj.Summary;
				return __result == null ? IntPtr.Zero : __result.Handle;
			} catch (Exception e) {
				GLib.ExceptionManager.RaiseUnhandledException (e, true);
				// NOTREACHED: above call does not return.
				throw e;
			}
		}

		[UnmanagedFunctionPointer (CallingConvention.Cdecl)]
		delegate void SetCaptionDelegate (IntPtr table, IntPtr caption);

		static void SetCaptionCallback (IntPtr table, IntPtr caption)
		{
			try {
				Atk.TableImplementor __obj = GLib.Object.GetObject (table, false) as Atk.TableImplementor;
				__obj.Caption = GLib.Object.GetObject(caption) as Atk.Object;
			} catch (Exception e) {
				GLib.ExceptionManager.RaiseUnhandledException (e, false);
			}
		}

		[UnmanagedFunctionPointer (CallingConvention.Cdecl)]
		delegate void SetColumnDescriptionDelegate (IntPtr table, int column, IntPtr description);

		static void SetColumnDescriptionCallback (IntPtr table, int column, IntPtr description)
		{
			try {
				Atk.TableImplementor __obj = GLib.Object.GetObject (table, false) as Atk.TableImplementor;
				__obj.SetColumnDescription (column, GLib.Marshaller.Utf8PtrToString (description));
			} catch (Exception e) {
				GLib.ExceptionManager.RaiseUnhandledException (e, false);
			}
		}

		[UnmanagedFunctionPointer (CallingConvention.Cdecl)]
		delegate void SetColumnHeaderDelegate (IntPtr table, int column, IntPtr header);

		static void SetColumnHeaderCallback (IntPtr table, int column, IntPtr header)
		{
			try {
				Atk.TableImplementor __obj = GLib.Object.GetObject (table, false) as Atk.TableImplementor;
				__obj.SetColumnHeader (column, GLib.Object.GetObject(header) as Atk.Object);
			} catch (Exception e) {
				GLib.ExceptionManager.RaiseUnhandledException (e, false);
			}
		}

		[UnmanagedFunctionPointer (CallingConvention.Cdecl)]
		delegate void SetRowDescriptionDelegate (IntPtr table, int row, IntPtr description);

		static void SetRowDescriptionCallback (IntPtr table, int row, IntPtr description)
		{
			try {
				Atk.TableImplementor __obj = GLib.Object.GetObject (table, false) as Atk.TableImplementor;
				__obj.SetRowDescription (row, GLib.Marshaller.Utf8PtrToString (description));
			} catch (Exception e) {
				GLib.ExceptionManager.RaiseUnhandledException (e, false);
			}
		}

		[UnmanagedFunctionPointer (CallingConvention.Cdecl)]
		delegate void SetRowHeaderDelegate (IntPtr table, int row, IntPtr header);

		static void SetRowHeaderCallback (IntPtr table, int row, IntPtr header)
		{
			try {
				Atk.TableImplementor __obj = GLib.Object.GetObject (table, false) as Atk.TableImplementor;
				__obj.SetRowHeader (row, GLib.Object.GetObject(header) as Atk.Object);
			} catch (Exception e) {
				GLib.ExceptionManager.RaiseUnhandledException (e, false);
			}
		}

		[UnmanagedFunctionPointer (CallingConvention.Cdecl)]
		delegate void SetSummaryDelegate (IntPtr table, IntPtr accessible);

		static void SetSummaryCallback (IntPtr table, IntPtr accessible)
		{
			try {
				Atk.TableImplementor __obj = GLib.Object.GetObject (table, false) as Atk.TableImplementor;
				__obj.Summary = GLib.Object.GetObject(accessible) as Atk.Object;
			} catch (Exception e) {
				GLib.ExceptionManager.RaiseUnhandledException (e, false);
			}
		}

		[UnmanagedFunctionPointer (CallingConvention.Cdecl)]
		delegate int GetSelectedColumnsDelegate (IntPtr table, out IntPtr selected_ptr);

		static int GetSelectedColumnsCallback (IntPtr table, out IntPtr selected_ptr)
		{
			try {
				Atk.TableImplementor __obj = GLib.Object.GetObject (table, false) as Atk.TableImplementor;
				int [] selected = __obj.SelectedColumns;
				if (selected.Length > 0) {
					selected_ptr = GLib.Marshaller.Malloc ((ulong)(sizeof(int) * selected.Length));
					Marshal.Copy (selected, 0, selected_ptr, selected.Length);
				} else {
					selected_ptr = IntPtr.Zero;
				}
				return selected.Length;
			} catch (Exception e) {
				GLib.ExceptionManager.RaiseUnhandledException (e, true);
				// NOTREACHED: above call does not return.
				throw e;
			}
		}

		[UnmanagedFunctionPointer (CallingConvention.Cdecl)]
		delegate int GetSelectedRowsDelegate (IntPtr table, out IntPtr selected);

		static int GetSelectedRowsCallback (IntPtr table, out IntPtr selected_ptr)
		{
			try {
				Atk.TableImplementor __obj = GLib.Object.GetObject (table, false) as Atk.TableImplementor;
				int [] selected = __obj.SelectedRows;
				if (selected.Length > 0) {
					selected_ptr = GLib.Marshaller.Malloc ((ulong)(sizeof (int) * selected.Length));
					Marshal.Copy (selected, 0, selected_ptr, selected.Length);
				} else {
					selected_ptr = IntPtr.Zero;
				}
				return selected.Length;
			} catch (Exception e) {
				GLib.ExceptionManager.RaiseUnhandledException (e, true);
				// NOTREACHED: above call does not return.
				throw e;
			}
		}

		[UnmanagedFunctionPointer (CallingConvention.Cdecl)]
		delegate bool IsColumnSelectedDelegate (IntPtr table, int column);

		static bool IsColumnSelectedCallback (IntPtr table, int column)
		{
			try {
				Atk.TableImplementor __obj = GLib.Object.GetObject (table, false) as Atk.TableImplementor;
				bool __result = __obj.IsColumnSelected (column);
				return __result;
			} catch (Exception e) {
				GLib.ExceptionManager.RaiseUnhandledException (e, true);
				// NOTREACHED: above call does not return.
				throw e;
			}
		}

		[UnmanagedFunctionPointer (CallingConvention.Cdecl)]
		delegate bool IsRowSelectedDelegate (IntPtr table, int row);

		static bool IsRowSelectedCallback (IntPtr table, int row)
		{
			try {
				Atk.TableImplementor __obj = GLib.Object.GetObject (table, false) as Atk.TableImplementor;
				bool __result = __obj.IsRowSelected (row);
				return __result;
			} catch (Exception e) {
				GLib.ExceptionManager.RaiseUnhandledException (e, true);
				// NOTREACHED: above call does not return.
				throw e;
			}
		}

		[UnmanagedFunctionPointer (CallingConvention.Cdecl)]
		delegate bool IsSelectedDelegate (IntPtr table, int row, int column);

		static bool IsSelectedCallback (IntPtr table, int row, int column)
		{
			try {
				Atk.TableImplementor __obj = GLib.Object.GetObject (table, false) as Atk.TableImplementor;
				bool __result = __obj.IsSelected (row, column);
				return __result;
			} catch (Exception e) {
				GLib.ExceptionManager.RaiseUnhandledException (e, true);
				// NOTREACHED: above call does not return.
				throw e;
			}
		}

		[UnmanagedFunctionPointer (CallingConvention.Cdecl)]
		delegate bool AddRowSelectionDelegate (IntPtr table, int row);

		static bool AddRowSelectionCallback (IntPtr table, int row)
		{
			try {
				Atk.TableImplementor __obj = GLib.Object.GetObject (table, false) as Atk.TableImplementor;
				bool __result = __obj.AddRowSelection (row);
				return __result;
			} catch (Exception e) {
				GLib.ExceptionManager.RaiseUnhandledException (e, true);
				// NOTREACHED: above call does not return.
				throw e;
			}
		}

		[UnmanagedFunctionPointer (CallingConvention.Cdecl)]
		delegate bool RemoveRowSelectionDelegate (IntPtr table, int row);

		static bool RemoveRowSelectionCallback (IntPtr table, int row)
		{
			try {
				Atk.TableImplementor __obj = GLib.Object.GetObject (table, false) as Atk.TableImplementor;
				bool __result = __obj.RemoveRowSelection (row);
				return __result;
			} catch (Exception e) {
				GLib.ExceptionManager.RaiseUnhandledException (e, true);
				// NOTREACHED: above call does not return.
				throw e;
			}
		}

		[UnmanagedFunctionPointer (CallingConvention.Cdecl)]
		delegate bool AddColumnSelectionDelegate (IntPtr table, int column);

		static bool AddColumnSelectionCallback (IntPtr table, int column)
		{
			try {
				Atk.TableImplementor __obj = GLib.Object.GetObject (table, false) as Atk.TableImplementor;
				bool __result = __obj.AddColumnSelection (column);
				return __result;
			} catch (Exception e) {
				GLib.ExceptionManager.RaiseUnhandledException (e, true);
				// NOTREACHED: above call does not return.
				throw e;
			}
		}

		[UnmanagedFunctionPointer (CallingConvention.Cdecl)]
		delegate bool RemoveColumnSelectionDelegate (IntPtr table, int column);

		static bool RemoveColumnSelectionCallback (IntPtr table, int column)
		{
			try {
				Atk.TableImplementor __obj = GLib.Object.GetObject (table, false) as Atk.TableImplementor;
				bool __result = __obj.RemoveColumnSelection (column);
				return __result;
			} catch (Exception e) {
				GLib.ExceptionManager.RaiseUnhandledException (e, true);
				// NOTREACHED: above call does not return.
				throw e;
			}
		}
		static void Initialize (IntPtr ifaceptr, IntPtr data)
		{
			TableIface native_iface = Marshal.PtrToStructure<TableIface> (ifaceptr);
			native_iface.ref_at = iface.ref_at;
			native_iface.get_index_at = iface.get_index_at;
			native_iface.get_column_at_index = iface.get_column_at_index;
			native_iface.get_row_at_index = iface.get_row_at_index;
			native_iface.get_n_columns = iface.get_n_columns;
			native_iface.get_n_rows = iface.get_n_rows;
			native_iface.get_column_extent_at = iface.get_column_extent_at;
			native_iface.get_row_extent_at = iface.get_row_extent_at;
			native_iface.get_caption = iface.get_caption;
			native_iface.get_column_description = iface.get_column_description;
			native_iface.get_column_header = iface.get_column_header;
			native_iface.get_row_description = iface.get_row_description;
			native_iface.get_row_header = iface.get_row_header;
			native_iface.get_summary = iface.get_summary;
			native_iface.set_caption = iface.set_caption;
			native_iface.set_column_description = iface.set_column_description;
			native_iface.set_column_header = iface.set_column_header;
			native_iface.set_row_description = iface.set_row_description;
			native_iface.set_row_header = iface.set_row_header;
			native_iface.set_summary = iface.set_summary;
			native_iface.get_selected_columns = iface.get_selected_columns;
			native_iface.get_selected_rows = iface.get_selected_rows;
			native_iface.is_column_selected = iface.is_column_selected;
			native_iface.is_row_selected = iface.is_row_selected;
			native_iface.is_selected = iface.is_selected;
			native_iface.add_row_selection = iface.add_row_selection;
			native_iface.remove_row_selection = iface.remove_row_selection;
			native_iface.add_column_selection = iface.add_column_selection;
			native_iface.remove_column_selection = iface.remove_column_selection;
			Marshal.StructureToPtr (native_iface, ifaceptr, false);
			GCHandle gch = (GCHandle) data;
			gch.Free ();
		}

		public TableAdapter ()
		{
			InitHandler = new GLib.GInterfaceInitHandler (Initialize);
		}

		TableImplementor implementor;

		public TableAdapter (TableImplementor implementor)
		{
			if (implementor == null)
				throw new ArgumentNullException ("implementor");
			this.implementor = implementor;
		}

		public TableAdapter (IntPtr handle)
		{
			this.handle = handle;
		}

		[DllImport("libatk-1.0-0.dll", CallingConvention=CallingConvention.Cdecl)]
		static extern IntPtr atk_table_get_type();

		private static GLib.GType _gtype = new GLib.GType (atk_table_get_type ());

		public override GLib.GType GType {
			get {
				return _gtype;
			}
		}

		IntPtr handle;
		public override IntPtr Handle {
			get {
				if (handle != IntPtr.Zero)
					return handle;
				return implementor == null ? IntPtr.Zero : implementor.Handle;
			}
		}

		public static Table GetObject (IntPtr handle, bool owned)
		{
			GLib.Object obj = GLib.Object.GetObject (handle, owned);
			return GetObject (obj);
		}

		public static Table GetObject (GLib.Object obj)
		{
			if (obj == null)
				return null;
			else if (obj is TableImplementor)
				return new TableAdapter (obj as TableImplementor);
			else if (obj as Table == null)
				return new TableAdapter (obj.Handle);
			else
				return obj as Table;
		}

		public TableImplementor Implementor {
			get {
				return implementor;
			}
		}

		[GLib.Signal("column_reordered")]
		public event System.EventHandler ColumnReordered {
			add {
				GLib.Signal sig = GLib.Signal.Lookup (GLib.Object.GetObject (Handle), "column_reordered");
				sig.AddDelegate (value);
			}
			remove {
				GLib.Signal sig = GLib.Signal.Lookup (GLib.Object.GetObject (Handle), "column_reordered");
				sig.RemoveDelegate (value);
			}
		}

		[GLib.Signal("column_deleted")]
		public event Atk.ColumnDeletedHandler ColumnDeleted {
			add {
				GLib.Signal sig = GLib.Signal.Lookup (GLib.Object.GetObject (Handle), "column_deleted", typeof (Atk.ColumnDeletedArgs));
				sig.AddDelegate (value);
			}
			remove {
				GLib.Signal sig = GLib.Signal.Lookup (GLib.Object.GetObject (Handle), "column_deleted", typeof (Atk.ColumnDeletedArgs));
				sig.RemoveDelegate (value);
			}
		}

		[GLib.Signal("row_reordered")]
		public event System.EventHandler RowReordered {
			add {
				GLib.Signal sig = GLib.Signal.Lookup (GLib.Object.GetObject (Handle), "row_reordered");
				sig.AddDelegate (value);
			}
			remove {
				GLib.Signal sig = GLib.Signal.Lookup (GLib.Object.GetObject (Handle), "row_reordered");
				sig.RemoveDelegate (value);
			}
		}

		[GLib.Signal("column_inserted")]
		public event Atk.ColumnInsertedHandler ColumnInserted {
			add {
				GLib.Signal sig = GLib.Signal.Lookup (GLib.Object.GetObject (Handle), "column_inserted", typeof (Atk.ColumnInsertedArgs));
				sig.AddDelegate (value);
			}
			remove {
				GLib.Signal sig = GLib.Signal.Lookup (GLib.Object.GetObject (Handle), "column_inserted", typeof (Atk.ColumnInsertedArgs));
				sig.RemoveDelegate (value);
			}
		}

		[GLib.Signal("model_changed")]
		public event System.EventHandler ModelChanged {
			add {
				GLib.Signal sig = GLib.Signal.Lookup (GLib.Object.GetObject (Handle), "model_changed");
				sig.AddDelegate (value);
			}
			remove {
				GLib.Signal sig = GLib.Signal.Lookup (GLib.Object.GetObject (Handle), "model_changed");
				sig.RemoveDelegate (value);
			}
		}

		[GLib.Signal("row_inserted")]
		public event Atk.RowInsertedHandler RowInserted {
			add {
				GLib.Signal sig = GLib.Signal.Lookup (GLib.Object.GetObject (Handle), "row_inserted", typeof (Atk.RowInsertedArgs));
				sig.AddDelegate (value);
			}
			remove {
				GLib.Signal sig = GLib.Signal.Lookup (GLib.Object.GetObject (Handle), "row_inserted", typeof (Atk.RowInsertedArgs));
				sig.RemoveDelegate (value);
			}
		}

		[GLib.Signal("row_deleted")]
		public event Atk.RowDeletedHandler RowDeleted {
			add {
				GLib.Signal sig = GLib.Signal.Lookup (GLib.Object.GetObject (Handle), "row_deleted", typeof (Atk.RowDeletedArgs));
				sig.AddDelegate (value);
			}
			remove {
				GLib.Signal sig = GLib.Signal.Lookup (GLib.Object.GetObject (Handle), "row_deleted", typeof (Atk.RowDeletedArgs));
				sig.RemoveDelegate (value);
			}
		}

		[DllImport("libatk-1.0-0.dll", CallingConvention=CallingConvention.Cdecl)]
		static extern int atk_table_get_n_rows(IntPtr raw);

		public int NRows {
			get {
				int raw_ret = atk_table_get_n_rows(Handle);
				int ret = raw_ret;
				return ret;
			}
		}

		[DllImport("libatk-1.0-0.dll", CallingConvention=CallingConvention.Cdecl)]
		static extern bool atk_table_add_row_selection(IntPtr raw, int row);

		public bool AddRowSelection(int row) {
			bool raw_ret = atk_table_add_row_selection(Handle, row);
			bool ret = raw_ret;
			return ret;
		}

		[DllImport("libatk-1.0-0.dll", CallingConvention=CallingConvention.Cdecl)]
		static extern int atk_table_get_column_extent_at(IntPtr raw, int row, int column);

		public int GetColumnExtentAt(int row, int column) {
			int raw_ret = atk_table_get_column_extent_at(Handle, row, column);
			int ret = raw_ret;
			return ret;
		}

		[DllImport("libatk-1.0-0.dll", CallingConvention=CallingConvention.Cdecl)]
		static extern IntPtr atk_table_get_column_header(IntPtr raw, int column);

		public Atk.Object GetColumnHeader(int column) {
			IntPtr raw_ret = atk_table_get_column_header(Handle, column);
			Atk.Object ret = GLib.Object.GetObject(raw_ret) as Atk.Object;
			return ret;
		}

		[DllImport("libatk-1.0-0.dll", CallingConvention=CallingConvention.Cdecl)]
		static extern bool atk_table_is_selected(IntPtr raw, int row, int column);

		public bool IsSelected(int row, int column) {
			bool raw_ret = atk_table_is_selected(Handle, row, column);
			bool ret = raw_ret;
			return ret;
		}

		[DllImport("libatk-1.0-0.dll", CallingConvention=CallingConvention.Cdecl)]
		static extern IntPtr atk_table_get_summary(IntPtr raw);

		[DllImport("libatk-1.0-0.dll", CallingConvention=CallingConvention.Cdecl)]
		static extern void atk_table_set_summary(IntPtr raw, IntPtr accessible);

		public Atk.Object Summary {
			get {
				IntPtr raw_ret = atk_table_get_summary(Handle);
				Atk.Object ret = GLib.Object.GetObject(raw_ret) as Atk.Object;
				return ret;
			}
			set {
				atk_table_set_summary(Handle, value == null ? IntPtr.Zero : value.Handle);
			}
		}

		[DllImport("libatk-1.0-0.dll", CallingConvention=CallingConvention.Cdecl)]
		static extern IntPtr atk_table_get_column_description(IntPtr raw, int column);

		public string GetColumnDescription(int column) {
			IntPtr raw_ret = atk_table_get_column_description(Handle, column);
			string ret = GLib.Marshaller.Utf8PtrToString (raw_ret);
			return ret;
		}

		[DllImport("libatk-1.0-0.dll", CallingConvention=CallingConvention.Cdecl)]
		static extern bool atk_table_add_column_selection(IntPtr raw, int column);

		public bool AddColumnSelection(int column) {
			bool raw_ret = atk_table_add_column_selection(Handle, column);
			bool ret = raw_ret;
			return ret;
		}

		[DllImport("libatk-1.0-0.dll", CallingConvention=CallingConvention.Cdecl)]
		static extern void atk_table_set_row_header(IntPtr raw, int row, IntPtr header);

		public void SetRowHeader(int row, Atk.Object header) {
			atk_table_set_row_header(Handle, row, header == null ? IntPtr.Zero : header.Handle);
		}

		[DllImport("libatk-1.0-0.dll", CallingConvention=CallingConvention.Cdecl)]
		static extern IntPtr atk_table_get_row_description(IntPtr raw, int row);

		public string GetRowDescription(int row) {
			IntPtr raw_ret = atk_table_get_row_description(Handle, row);
			string ret = GLib.Marshaller.Utf8PtrToString (raw_ret);
			return ret;
		}

		[DllImport("libatk-1.0-0.dll", CallingConvention=CallingConvention.Cdecl)]
		static extern IntPtr atk_table_ref_at(IntPtr raw, int row, int column);

		public Atk.Object RefAt(int row, int column) {
			IntPtr raw_ret = atk_table_ref_at(Handle, row, column);
			Atk.Object ret = GLib.Object.GetObject(raw_ret, true) as Atk.Object;
			return ret;
		}

		[DllImport("libatk-1.0-0.dll", CallingConvention=CallingConvention.Cdecl)]
		static extern void atk_table_set_column_description(IntPtr raw, int column, IntPtr description);

		public void SetColumnDescription(int column, string description) {
			IntPtr native_description = GLib.Marshaller.StringToPtrGStrdup (description);
			atk_table_set_column_description(Handle, column, native_description);
			GLib.Marshaller.Free (native_description);
		}

		[DllImport("libatk-1.0-0.dll", CallingConvention=CallingConvention.Cdecl)]
		static extern int atk_table_get_index_at(IntPtr raw, int row, int column);

		public int GetIndexAt(int row, int column) {
			int raw_ret = atk_table_get_index_at(Handle, row, column);
			int ret = raw_ret;
			return ret;
		}

		[DllImport("libatk-1.0-0.dll", CallingConvention=CallingConvention.Cdecl)]
		static extern IntPtr atk_table_get_row_header(IntPtr raw, int row);

		public Atk.Object GetRowHeader(int row) {
			IntPtr raw_ret = atk_table_get_row_header(Handle, row);
			Atk.Object ret = GLib.Object.GetObject(raw_ret) as Atk.Object;
			return ret;
		}

		[DllImport("libatk-1.0-0.dll", CallingConvention=CallingConvention.Cdecl)]
		static extern bool atk_table_is_column_selected(IntPtr raw, int column);

		public bool IsColumnSelected(int column) {
			bool raw_ret = atk_table_is_column_selected(Handle, column);
			bool ret = raw_ret;
			return ret;
		}

		[DllImport("libatk-1.0-0.dll", CallingConvention=CallingConvention.Cdecl)]
		static extern int atk_table_get_selected_rows(IntPtr raw, out IntPtr selected);

		private static readonly int [] empty_int_array = new int[0];

		public int [] SelectedRows {
			get {
				IntPtr selected_ptr;
				int len = atk_table_get_selected_rows(Handle, out selected_ptr);
				int [] selected = null;

				if (len > 0) {
					selected = new int [len];
					Marshal.Copy (selected_ptr, selected, 0, len);
				}

				if (selected_ptr != IntPtr.Zero) {
					GLib.Marshaller.Free (selected_ptr);
				}

				return selected == null ? empty_int_array : selected;
			}
		}

		[DllImport("libatk-1.0-0.dll", CallingConvention=CallingConvention.Cdecl)]
		static extern void atk_table_set_row_description(IntPtr raw, int row, IntPtr description);

		public void SetRowDescription(int row, string description) {
			IntPtr native_description = GLib.Marshaller.StringToPtrGStrdup (description);
			atk_table_set_row_description(Handle, row, native_description);
			GLib.Marshaller.Free (native_description);
		}

		[DllImport("libatk-1.0-0.dll", CallingConvention=CallingConvention.Cdecl)]
		static extern bool atk_table_is_row_selected(IntPtr raw, int row);

		public bool IsRowSelected(int row) {
			bool raw_ret = atk_table_is_row_selected(Handle, row);
			bool ret = raw_ret;
			return ret;
		}

		[DllImport("libatk-1.0-0.dll", CallingConvention=CallingConvention.Cdecl)]
		static extern int atk_table_get_row_extent_at(IntPtr raw, int row, int column);

		public int GetRowExtentAt(int row, int column) {
			int raw_ret = atk_table_get_row_extent_at(Handle, row, column);
			int ret = raw_ret;
			return ret;
		}

		[DllImport("libatk-1.0-0.dll", CallingConvention=CallingConvention.Cdecl)]
		static extern int atk_table_get_selected_columns(IntPtr raw, out IntPtr selected);

		public int [] SelectedColumns{
			get {
				IntPtr selected_ptr;
				int len = atk_table_get_selected_columns(Handle, out selected_ptr);
				int [] selected = null;

				if (len > 0) {
					selected = new int [len];
					Marshal.Copy (selected_ptr, selected, 0, len);
				}

				if (selected_ptr != IntPtr.Zero) {
					GLib.Marshaller.Free (selected_ptr);
				}

				return selected == null ? empty_int_array : selected;
			}
		}

		[DllImport("libatk-1.0-0.dll", CallingConvention=CallingConvention.Cdecl)]
		static extern int atk_table_get_column_at_index(IntPtr raw, int index_);

		public int GetColumnAtIndex(int index_) {
			int raw_ret = atk_table_get_column_at_index(Handle, index_);
			int ret = raw_ret;
			return ret;
		}

		[DllImport("libatk-1.0-0.dll", CallingConvention=CallingConvention.Cdecl)]
		static extern int atk_table_get_row_at_index(IntPtr raw, int index_);

		public int GetRowAtIndex(int index_) {
			int raw_ret = atk_table_get_row_at_index(Handle, index_);
			int ret = raw_ret;
			return ret;
		}

		[DllImport("libatk-1.0-0.dll", CallingConvention=CallingConvention.Cdecl)]
		static extern IntPtr atk_table_get_caption(IntPtr raw);

		[DllImport("libatk-1.0-0.dll", CallingConvention=CallingConvention.Cdecl)]
		static extern void atk_table_set_caption(IntPtr raw, IntPtr caption);

		public Atk.Object Caption {
			get {
				IntPtr raw_ret = atk_table_get_caption(Handle);
				Atk.Object ret = GLib.Object.GetObject(raw_ret) as Atk.Object;
				return ret;
			}
			set {
				atk_table_set_caption(Handle, value == null ? IntPtr.Zero : value.Handle);
			}
		}

		[DllImport("libatk-1.0-0.dll", CallingConvention=CallingConvention.Cdecl)]
		static extern int atk_table_get_n_columns(IntPtr raw);

		public int NColumns {
			get {
				int raw_ret = atk_table_get_n_columns(Handle);
				int ret = raw_ret;
				return ret;
			}
		}

		[DllImport("libatk-1.0-0.dll", CallingConvention=CallingConvention.Cdecl)]
		static extern bool atk_table_remove_row_selection(IntPtr raw, int row);

		public bool RemoveRowSelection(int row) {
			bool raw_ret = atk_table_remove_row_selection(Handle, row);
			bool ret = raw_ret;
			return ret;
		}

		[DllImport("libatk-1.0-0.dll", CallingConvention=CallingConvention.Cdecl)]
		static extern bool atk_table_remove_column_selection(IntPtr raw, int column);

		public bool RemoveColumnSelection(int column) {
			bool raw_ret = atk_table_remove_column_selection(Handle, column);
			bool ret = raw_ret;
			return ret;
		}

		[DllImport("libatk-1.0-0.dll", CallingConvention=CallingConvention.Cdecl)]
		static extern void atk_table_set_column_header(IntPtr raw, int column, IntPtr header);

		public void SetColumnHeader(int column, Atk.Object header) {
			atk_table_set_column_header(Handle, column, header == null ? IntPtr.Zero : header.Handle);
		}

#endregion
	}
}
