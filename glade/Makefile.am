SUBDIRS = . glue

if ENABLE_GLADE
pkg = glade
pkgconfigdir = $(libdir)/pkgconfig
pkgconfig_DATA = glade-sharp-2.0.pc
else
pkg = 
endif

INCLUDE_API = $(top_builddir)/pango/pango-api.xml $(top_builddir)/atk/atk-api.xml $(top_builddir)/gdk/gdk-api.xml $(top_builddir)/gtk/gtk-api.xml
METADATA = Glade.metadata
SYMBOLS =
references = $(top_builddir)/glib/glib-sharp.dll $(top_builddir)/pango/pango-sharp.dll $(top_builddir)/atk/atk-sharp.dll $(top_builddir)/gdk/gdk-sharp.dll $(top_builddir)/gtk/gtk-sharp.dll
glue_includes = glade/glade.h,glade/glade-parser.h

sources =				\
	HandlerNotFoundExeception.cs	\
	WidgetAttribute.cs

customs =				\
	Global.custom			\
	Interface.custom		\
	XML.custom

add_dist = glade-sharp-2.0.pc.in

include $(top_srcdir)/Makefile.include

