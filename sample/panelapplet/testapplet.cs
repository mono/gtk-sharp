using System;
using PanelApplet;
using Gtk;

namespace AppletTest
{
	public class PanelAppletClass : PanelApplet
	{

		protected PanelAppletClass (IntPtr raw) : base (raw) {}

		static void Main (string[] argv)
		{
			Gnome.Program p = new Gnome.Program ("CSharpTestApplet", "0.1", Gnome.Modules.UI, argv);
			AppletFactory.Register (typeof (PanelAppletClass));
			//new PanelAppletClass ();
		}

		/*public PanelAppletClass ()
		{
			FactoryMain ("OAFIID:CSharpTestApplet_Factory", GLib.Object.LookupGType (typeof (PanelAppletClass)), new FactoryCallback (Creationcb));
		}

		protected bool Creationcb (PanelApplet obj, string iid)
		{
			if (iid != "OAFIID:CSharpTestApplet")
				return false;
			return true;
		}*/

		public override void Creation ()
		{
			this.Add (new Gtk.Label ("MonoTest"));
			this.ShowAll ();
		}

		public override string IID {
			get { return "OAFIID:CSharpTestApplet"; }
		}

		public override string FactoryIID {
			get { return "OAFIID:CSharpTestApplet_Factory"; }
		}
	}
}
