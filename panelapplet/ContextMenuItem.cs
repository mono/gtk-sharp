using System;
using System.Runtime.InteropServices;

namespace PanelApplet
{

	public delegate void ContextMenuItemCallback ();

	[StructLayout (LayoutKind.Sequential)]
	public struct ContextMenuItem
	{
		string verb;
		ContextMenuItemCallback callback;

		public ContextMenuItem (string name, ContextMenuItemCallback cb)
		{
			verb = name;
			callback = cb;
		}
	}
}
