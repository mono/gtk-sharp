using System;
using System.Threading;

namespace Gnome.Vfs {
	public class VfsStreamAsyncResult : IAsyncResult {
		private object state;
		private bool completed = false;
		private bool done = false;
		private Exception exception = null;
		private int nbytes = -1;
		private ManualResetEvent wh;

		public VfsStreamAsyncResult (object state)
		{
			this.state = state;
		}

		public object AsyncState {
			get {
				return state;
			}
		}
		
		public WaitHandle AsyncWaitHandle {
			get {
				lock (this) {
					if (wh == null)
						wh = new ManualResetEvent (completed);
					return wh;
				}
			}
		}
		
		public bool CompletedSynchronously {
			get {
				return true;
			}
		}
		
		public bool Done {
			get {
				return done;
			}
			set {
				done = value;
			}
		}
		
		public Exception Exception {
			get {
				return exception;
			}
		}
		
		public bool IsCompleted {
			get {
				return completed;
			}
		}
		
		public int NBytes {
			get {
				return nbytes;
			}
		}
		
		public void SetComplete (Exception e)
		{
			exception = e;
			completed = true;
			lock (this) {
				if (wh != null)
					wh.Set ();
			}
		}
		
		public void SetComplete (Exception e, int nbytes)
		{
			this.nbytes = nbytes;
			SetComplete (e);
		}
	}
}
