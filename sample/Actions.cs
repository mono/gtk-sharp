// Actions.cs - Gtk.Action class Test implementation
//
// Author: Jeroen Zwartepoorte <jeroen@xs4all.nl
//
// (c) 2003 Jeroen Zwartepoorte

namespace GtkSamples {

	using Gtk;
	using GtkSharp;
	using System;
	using System.Drawing;

	public class Actions {
		static VBox box = null;
		static Statusbar statusbar = null;

/* XML description of the menus for the test app.  The parser understands
 * a subset of the Bonobo UI XML format, and uses GMarkup for parsing */
static string ui_info = "  <menubar>\n" +
"    <menu name=\"Menu _1\" action=\"Menu1Action\">\n" +
"      <menuitem name=\"quit\" action=\"quit\" />\n" +
"    </menu>\n" +
"  </menubar>\n" +
"  <toolbar name=\"toolbar\">\n" +
"    <toolitem name=\"quit\" action=\"quit\" />\n" +
"  </toolbar>\n";

		public static int Main (string[] args)
		{
			Application.Init ();
			Window win = new Window ("Action Tester");
			win.DefaultSize = new Size (200, 150);
			win.DeleteEvent += new DeleteEventHandler (OnWindowDelete);
			
			box = new VBox (false, 0);
			win.Add (box);
			
			ActionGroup group = new ActionGroup ("TestGroup");
			ActionEntry entry = new ActionEntry ();
			entry.name = "quit";
			entry.stock_id = Stock.Quit;
			entry.accelerator = "<control>Q";
			entry.tooltip = "Quit the program";
			entry.ActionCallback = new GLib.Callback (OnQuit);
			group.AddActions (entry, 1, IntPtr.Zero);
			entry = new ActionEntry ();
			entry.name = "Menu1Action";
			entry.label = "_File";
			entry.tooltip = "";
			group.AddActions (entry, 1, IntPtr.Zero);

			UIManager uim = new UIManager ();
			uim.AddWidget += new AddWidgetHandler (OnWidgetAdd);
			uim.InsertActionGroup (group, 0);
			uim.AddUiFromString (ui_info);
			
			statusbar = new Statusbar ();
			box.PackEnd (statusbar, false, true, 0);

			Button button = new Button ("Blah");
			box.PackEnd (button, true, true, 0);

			GLib.List list = group.ListActions ();
			foreach (Action action in list) {
				action.ProxyConnected += new AddWidgetHandler (OnProxyConnected);
			}

			win.ShowAll ();
			Application.Run ();
			return 0;
		}

		static void OnWindowDelete (object obj, DeleteEventArgs args)
		{
			Application.Quit ();
			args.RetVal = true;
		}
		
		static void OnWidgetAdd (object obj, AddWidgetArgs args)
		{
			Console.WriteLine ("OnWidgetAdd {0}", args.Widget.Name);
			args.Widget.Show ();
			box.PackStart (args.Widget, false, true, 0);
		}

		static void OnSelect (object obj, EventArgs args)
		{
			Action action = ((GLib.Object)obj).Data["action"] as Action;
			statusbar.Push (0, action.Tooltip);
		}

		static void OnDeselect (object obj, EventArgs args)
		{
			statusbar.Pop (0);
		}

		static void OnProxyConnected (object obj, AddWidgetArgs args)
		{
			Console.WriteLine ("ProxyConnected {0}, {1}", obj, args.Widget.Name);
			if (args.Widget is MenuItem) {
				((GLib.Object)args.Widget).Data ["action"] = obj;
				((Item)args.Widget).Selected += new EventHandler (OnSelect);
				((Item)args.Widget).Deselected += new EventHandler (OnDeselect);
			}
		}

		static void OnQuit (GLib.Object obj)
		{
			Console.WriteLine ("quit");
		}
	}
}
