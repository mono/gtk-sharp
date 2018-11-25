//
// Mono.Cairo.FontFace.cs
//
// Author:
//   Alp Toker (alp@atoker.com)
//   Miguel de Icaza (miguel@novell.com)
//
// (C) Ximian Inc, 2003.
//
// This is an OO wrapper API for the Cairo API.
//
// Copyright (C) 2004, 2007 Novell, Inc (http://www.novell.com)
//
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
// 
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
//
using System;

namespace Cairo
{
	public class FontFace : IDisposable
	{
		IntPtr handle;

		internal static FontFace Lookup (IntPtr handle)
		{
			if (handle == IntPtr.Zero)
				return null;

			NativeMethods.cairo_font_face_reference (handle);

			return new FontFace (handle);
		}

		~FontFace ()
		{
			// Since Cairo is not thread safe, we can not unref the
			// font_face here, the programmer must do this with IDisposable.Dispose

			Console.Error.WriteLine ("Programmer forgot to call Dispose on the FontFace");
			Dispose (false);
		}

		void IDisposable.Dispose ()
		{
			Dispose (true);
		}

		protected virtual void Dispose (bool disposing)
		{
			if (disposing)
				NativeMethods.cairo_font_face_destroy (handle);
			handle = IntPtr.Zero;
			GC.SuppressFinalize (this);
		}
		
		// TODO: make non-public when all entry points are complete in binding
		public FontFace (IntPtr handle)
		{
			this.handle = handle;
		}

		public IntPtr Handle {
			get {
				return handle;
			}
		}

		public Status Status {
			get {
				return NativeMethods.cairo_font_face_status (handle);
			}
		}
		
		public FontType FontType {
			get {
				return NativeMethods.cairo_font_face_get_type (handle);
			}
		}

		public uint ReferenceCount {
			get { return NativeMethods.cairo_font_face_get_reference_count (handle); }
		}
	}
}

