using Gtk;
using System;
using UI = Gtk.Builder.ObjectAttribute;

namespace CustomWidgetBuilder
{
	[GLib.TypeName("MyWidget")]
	public class MyWidget : Entry
	{
		public MyWidget()
		{
			this.Init();
		}


		public MyWidget(IntPtr ptr) : base(ptr)
		{
			this.Init();
		}


		static public void Register()
		{
			GLib.Object.RegisterGType(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
		}


		private void Init()
		{
			this.Changed += (sender, e) => {
				if (this.MySignal != null)
					this.MySignal(sender, e);

				GLib.Signal.Emit(this, "my-signal");
			};
		}


		[GLib.Signal("my-signal")]
		public event EventHandler MySignal = null;


		[GLib.Property("my-text")]
		public string MyText
		{
			get { return base.Text; }
			set { base.Text = value; }
		}
	}


	public class MainWindow : Window
	{
		#pragma warning disable 169
		[UI] MyWidget m_mywidget;
		#pragma warning restore 169


		static void Main(string[] args)
		{
			Application.Init();
			MyWidget.Register();
			Builder builder = new Builder(new System.IO.FileStream("CustomWidgetBuilder.ui", System.IO.FileMode.Open));
			MainWindow wnd = new MainWindow(builder, builder.GetObject("m_window").Handle);
			wnd.Show();
			Application.Run();
		}


		public MainWindow(Builder builder, IntPtr handle) : base(handle)
		{
			builder.Autoconnect(this);
			this.DeleteEvent +=  (o, args) => {
				Gtk.Application.Quit();
				args.RetVal = true;
			};
		}


		private void MySignalHandler(object sender, EventArgs e)
		{
			this.m_mywidget.MyText = this.m_mywidget.MyText.ToUpper();
		}
	}
}
