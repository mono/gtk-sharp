noinst_SCRIPTS = valtest.exe
lib_LTLIBRARIES = libvalobj.la

assemblies=$(top_builddir)/glib/glib-sharp.dll $(top_builddir)/pango/pango-sharp.dll $(top_builddir)/atk/atk-sharp.dll $(top_builddir)/gdk/gdk-sharp.dll $(top_builddir)/gtk/gtk-sharp.dll
references=$(addprefix -r:, $(assemblies))

valtest.exe: Valtest.cs Valobj.cs $(assemblies)
	$(CSC) -out:valtest.exe $(references) $(srcdir)/Valtest.cs Valobj.cs

libvalobj_la_SOURCES =	\
	valobj.c	\
	valobj.h

libvalobj_la_LDFLAGS = -module -avoid-version -no-undefined

libvalobj_la_LIBADD = $(GTK_LIBS)

INCLUDES = $(GTK_CFLAGS)

Valobj.cs: valobj-api.xml
	$(RUNTIME) $(top_builddir)/generator/gapi_codegen.exe --generate $(srcdir)/valobj-api.xml --include $(top_builddir)/gtk/gtk-api.xml $(top_builddir)/gdk/gdk-api.xml --outdir=. --assembly-name=valobj-sharp

api:
	PATH=$(top_builddir)/parser:$PATH $(RUNTIME) $(top_builddir)/parser/gapi-parser.exe valobj-sources.xml

install:

CLEANFILES =		\
	valtest.exe	\
	valtest.exe.mdb	\
	Valobj.cs

EXTRA_DIST =		\
	Valtest.cs	\
	valobj-api.xml	\
	valobj-sources.xml
