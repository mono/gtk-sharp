using GLib;
using System;

namespace TestGio
{
	public class TestVolume
	{
		static void Main (string[] args)
		{
			GLib.GType.Init ();
			VolumeMonitor monitor = VolumeMonitor.Default;
			Console.WriteLine ("Volumes:");
			foreach (IVolumeBase v in monitor.Volumes)
				Console.WriteLine ("\t{0}", v.Name);
			Console.WriteLine ("\nMounts:");
			foreach (IMountBase m in monitor.Mounts) {
				Console.WriteLine ("\tName:{0}, UUID:{1}, root:{2}, CanUnmount: {3}", m.Name, m.Uuid, m.Root, m.CanUnmount);
				IVolumeBase v = m.Volume;
				if (v != null)
					Console.WriteLine ("\t\tVolume:{0}", v.Name);
				IDriveBase d = m.Drive;
				if (d != null)
					Console.WriteLine ("\t\tDrive:{0}", d.Name);
			}
			Console.WriteLine ("\nConnectedDrives:");
			foreach (IDriveBase d in monitor.ConnectedDrives)
				Console.WriteLine ("\t{0}, HasVolumes:{1}", d.Name, d.HasVolumes);
		}
	}
}

