using GLib;
using Gtk;
using System;

class CustomWidgetTest {
	public static int Main (string[] args)
	{
		Application.Init ();
		Window win = new Window ("Custom Widget Test");
		win.DeleteEvent += new DeleteEventHandler (OnQuit);
		
		VBox box = new VBox (true, 5);
		CustomWidget cw = new CustomWidget ();
		cw.BorderWidth = 0;
		box.PackStart (cw, true, true, 0);
		Button button = new Button ("Ordinary button");
		button.BorderWidth = 0;
		box.PackEnd (button, true, true, 0);
		
		win.Add (box);
		win.ShowAll ();
		Application.Run ();
		return 0;
	}

	static void OnQuit (object sender, DeleteEventArgs args)
	{
		Application.Quit ();
	}
}

class CustomWidget : Container {
	internal static GType customWidgetGType;
	internal Gdk.Window eventWindow;
	protected Button button;
	protected Pango.Layout layout;

	internal static GType CustomWidgetGetType () {
		if (customWidgetGType.Val == IntPtr.Zero)
			customWidgetGType = GLib.Object.RegisterGType (typeof (CustomWidget));
		
		return customWidgetGType;
	}

	static CustomWidget () {
		OverrideForall (CustomWidgetGetType ());
	}

	public CustomWidget () : base (CustomWidgetGetType ())
	{
		eventWindow = null;
		layout = CreatePangoLayout ("PangoLayout");
		button = new Button ("Custom widget");
		button.Parent = this;
		button.Show ();
		
		Flags |= (int)WidgetFlags.NoWindow;
	}

	public void PrintRect (Gdk.Rectangle rect)
	{
		Console.WriteLine ("{0}, {1}, {2}, {3}", rect.X, rect.Y,
				   rect.Width, rect.Height);
	}

	public Gdk.Rectangle TitleArea {
		get {
			Gdk.Rectangle area;
			area.X = Allocation.X + (int)BorderWidth;
			area.Y = Allocation.Y + (int)BorderWidth;
			area.Width = (Allocation.Width - 2 * (int)BorderWidth);
			
			int w, h;
			if (layout != null) {
				layout.GetPixelSize (out w, out h);
				area.Height = h;
			} else {
				Console.WriteLine ("layout == null");
				area.Height = 0;
			}
			
			return area;
		}
	}

	protected override bool OnExposeEvent (Gdk.EventExpose args)
	{
		if (layout != null) {
			Gdk.Rectangle exposeArea = new Gdk.Rectangle ();
			Gdk.Rectangle titleArea = TitleArea;

			PrintRect (titleArea);
			PrintRect (args.Area);

			Style.PaintLayout (Style, eventWindow, State,
					   true, args.Area, this, null,
					   titleArea.X, titleArea.Y, layout);
		}
	
		return base.OnExposeEvent (args);
	}

	protected override void OnForall (bool include_internals, CallbackInvoker invoker)
	{
		invoker.Invoke (button);
	}

	protected override bool OnMapEvent (Gdk.Event evnt)
	{
		Console.WriteLine ("OnMapEvent");
		
		base.OnMapEvent (evnt);

		if (eventWindow != null)
			eventWindow.Show ();
		
		return false;
	}

	protected override void OnUnmapped ()
	{
		Console.WriteLine ("OnUnmapped");
	
		if (eventWindow != null)
			eventWindow.Hide ();
		
		base.OnUnmapped ();
	}

	protected override void OnRealized ()
	{
		Console.WriteLine ("OnRealized");

		base.OnRealized ();
	
		Gdk.Rectangle area = TitleArea;
	
		Gdk.WindowAttr attr = new Gdk.WindowAttr ();
		attr.WindowType = Gdk.WindowType.Child;
		attr.Wclass = Gdk.WindowClass.Only;
		attr.X = area.X;
		attr.Y = area.Y;
		attr.Width = area.Width;
		attr.Height = area.Height;
		attr.OverrideRedirect = true;
		attr.EventMask = (int)Events;
		attr.EventMask |= (int)(Gdk.EventMask.ButtonPressMask |
					Gdk.EventMask.ButtonReleaseMask |
					Gdk.EventMask.KeyPressMask);
		
		int attr_mask = (int)(Gdk.WindowAttributesType.X |
				      Gdk.WindowAttributesType.Y |
				      Gdk.WindowAttributesType.Noredir);
		
		eventWindow = new Gdk.Window (ParentWindow, attr, attr_mask);
		eventWindow.SetUserData (this.Handle);
		
		Style = Style.Attach (GdkWindow);
	}
	
	protected override void OnUnrealized ()
	{
		Console.WriteLine ("OnUnrealized");
	
		if (eventWindow != null) {
			eventWindow.SetUserData (IntPtr.Zero);
			eventWindow.Destroy ();
			eventWindow = null;
		}
		
		base.OnUnrealized ();
	}

	protected override void OnSizeAllocated (Gdk.Rectangle allocation)
	{
		Console.WriteLine ("OnSizeAllocated: {0}, {1}, {2}, {3}",
				   allocation.X, allocation.Y, allocation.Width,
				   allocation.Height);
	
		base.OnSizeAllocated (allocation);
	
		int bw = (int)BorderWidth;

		Gdk.Rectangle titleArea = TitleArea;
		Console.WriteLine ("titleArea.Height = {0}", titleArea.Height);

		Gdk.Rectangle childAllocation;
		childAllocation.X = allocation.X + bw;
		childAllocation.Y = allocation.Y + bw + titleArea.Height;
		childAllocation.Width = allocation.Width - 2 * bw;
		childAllocation.Height = allocation.Height - 2 * bw - titleArea.Height;
		button.SizeAllocate (childAllocation);

		/*Console.WriteLine ("{0}, {1}, {2}, {3}", childAllocation.X,
				   childAllocation.Y, childAllocation.Width,
				   childAllocation.Height);*/

		if (eventWindow != null) {
			eventWindow.MoveResize (titleArea.X, titleArea.Y,
						titleArea.Width, titleArea.Height);
		}
	}

	protected override void OnSizeRequested (ref Requisition requisition)
	{
		Console.WriteLine ("OnSizeRequested");
	
		requisition.Width = requisition.Height = (int)BorderWidth * 2;
	
		int w, h;
		layout.GetPixelSize (out w, out h);
		requisition.Height += h;
		
		button.GetSizeRequest (out w, out h);
		requisition.Height += h;
	}
}
