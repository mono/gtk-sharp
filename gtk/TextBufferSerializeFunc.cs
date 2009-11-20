// This file was auto-generated at one time, but is hardcoded here as part of the fix
// for the TextBufferSerializeFunc;  see https://bugzilla.novell.com/show_bug.cgi?id=555495
// The generated code may have been modified as part of this fix; see textbuffer-serializefunc.patch

namespace Gtk {

	using System;

	public delegate byte [] TextBufferSerializeFunc(Gtk.TextBuffer register_buffer, Gtk.TextBuffer content_buffer, Gtk.TextIter start, Gtk.TextIter end, out ulong length);

}
