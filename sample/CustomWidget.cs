using GLib;
using Gtk;
using System;

class CustomWidgetTest {
	public static int Main (string[] args)
	{
		Application.Init ();
		Window win = new Window ("Custom Widget Test");
		win.DeleteEvent += new DeleteEventHandler (OnQuit);
		
		VBox box = new VBox (false, 5);
		CustomWidget cw = new CustomWidget ();
		cw.BorderWidth = 0;
		box.PackStart (cw, true, true, 0);
		Button button = new Button ("Ordinary button");
		button.BorderWidth = 0;
		box.PackEnd (button, false, false, 0);
		
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
	protected Button button;
	protected Gdk.Pixbuf icon;
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
		icon = null;
		layout = null;
	
		button = new Button ("Custom widget");
		button.Parent = this;
		button.Show ();
		
		Flags |= (int)WidgetFlags.NoWindow;
	}

	public Gdk.Rectangle TitleArea {
		get {
			EnsureLayout ();
		
			Gdk.Rectangle area;
			area.X = Allocation.X + (int)BorderWidth;
			area.Y = Allocation.Y + (int)BorderWidth;
			area.Width = (Allocation.Width - 2 * (int)BorderWidth);
			
			int layoutWidth, layoutHeight;
			layout.GetPixelSize (out layoutWidth, out layoutHeight);
			area.Height = Math.Max (layoutHeight, icon.Height);
			
			return area;
		}
	}

	private void EnsureLayout ()
	{
		if (layout == null) {
			layout = CreatePangoLayout ("PangoLayout");
			layout.SetText ("Pango drawn text...");
		}
		
		if (icon == null) {
			icon = RenderIcon (Stock.Execute, IconSize.Menu, "");
		}
	}

	protected override bool OnExposeEvent (Gdk.EventExpose args)
	{
		EnsureLayout ();
	
		Gdk.Rectangle exposeArea = new Gdk.Rectangle ();
		Gdk.Rectangle titleArea = TitleArea;

		if (args.Area.Intersect (titleArea, ref exposeArea))
			GdkWindow.DrawPixbuf (Style.BackgroundGC (State), icon, 0, 0,
					      titleArea.X, titleArea.Y, icon.Width,
					      icon.Height, Gdk.RgbDither.None, 0, 0);
		
		titleArea.X += icon.Width + 1;
		titleArea.Width -= icon.Width - 1;
		
		if (args.Area.Intersect (titleArea, ref exposeArea)) {
			int layoutWidth, layoutHeight;
			layout.GetPixelSize (out layoutWidth, out layoutHeight);
		
			titleArea.Y += (titleArea.Height - layoutHeight) / 2;

			Style.PaintLayout (Style, GdkWindow, State,
					   true, exposeArea, this, null,
					   titleArea.X, titleArea.Y, layout);
		}
	
		return base.OnExposeEvent (args);
	}

	protected override void OnForall (bool include_internals, CallbackInvoker invoker)
	{
		invoker.Invoke (button);
	}

	protected override void OnRealized ()
	{
		Flags |= (int)WidgetFlags.Realized;
		
		GdkWindow = ParentWindow;
		Style = Style.Attach (GdkWindow);
	}
	
	protected override void OnSizeAllocated (Gdk.Rectangle allocation)
	{
		base.OnSizeAllocated (allocation);
	
		int bw = (int)BorderWidth;

		Gdk.Rectangle titleArea = TitleArea;

		Gdk.Rectangle childAllocation;
		childAllocation.X = allocation.X + bw;
		childAllocation.Y = allocation.Y + bw + titleArea.Height;
		childAllocation.Width = allocation.Width - 2 * bw;
		childAllocation.Height = allocation.Height - 2 * bw - titleArea.Height;
		button.SizeAllocate (childAllocation);
	}

	protected override void OnSizeRequested (ref Requisition requisition)
	{
		EnsureLayout ();
	
		requisition.Width = requisition.Height = (int)BorderWidth * 2;
		requisition.Width += icon.Width + 1;
	
		int layoutWidth, layoutHeight;
		layout.GetPixelSize (out layoutWidth, out layoutHeight);
		requisition.Height += layoutHeight;
		
		if (button.Visible) {
			Requisition childReq = new Requisition ();
			button.SizeRequest (ref childReq);
			requisition.Height += childReq.Height;

			requisition.Width += Math.Max (layoutWidth, childReq.Width);
		} else {
			requisition.Width += layoutWidth;
		}
	}
}
