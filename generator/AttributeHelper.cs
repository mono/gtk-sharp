using System;
using System.IO;

namespace GtkSharp.Generation
{
	public static class AttributeHelper
	{
		public static void Gen (StreamWriter sw, string name, string libraryName, string pinvoke)
		{
			sw.WriteLine ();
			sw.WriteLine ("\tinternal class " + name + "Attribute : GLib.GTypeTypeAttribute {");
			sw.WriteLine ("\t\t[DllImport (\"" + libraryName + "\", CallingConvention = CallingConvention.Cdecl)]");
			sw.WriteLine ("\t\tstatic extern IntPtr {0} ();", pinvoke);
			sw.WriteLine ();
			sw.WriteLine ("\t\tprivate static GLib.GType _gtype = new GLib.GType ({0} ());", pinvoke);
			sw.WriteLine ("\t\tpublic static GLib.GType GType { get { return _gtype; } }");
			sw.WriteLine ("\t\tpublic override GLib.GType Type { get { return _gtype; } }");
			sw.WriteLine ();
			sw.WriteLine ("\t}");
		}
	}
}
