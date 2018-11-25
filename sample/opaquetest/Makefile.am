noinst_SCRIPTS = opaquetest.exe
lib_LTLIBRARIES = libopaque.la

assemblies=$(top_builddir)/glib/glib-sharp.dll $(top_builddir)/pango/pango-sharp.dll $(top_builddir)/atk/atk-sharp.dll $(top_builddir)/gdk/gdk-sharp.dll $(top_builddir)/gtk/gtk-sharp.dll
references=$(addprefix -r:, $(assemblies))

opaquetest.exe: OpaqueTest.cs generated/*.cs $(assemblies)
	$(CSC) -out:opaquetest.exe $(references) $(srcdir)/OpaqueTest.cs $(GENERATED_SOURCES)

libopaque_la_SOURCES =	\
	opaques.c	\
	opaques.h

libopaque_la_LDFLAGS = -module -avoid-version -no-undefined

libopaque_la_LIBADD = $(GTK_LIBS)

INCLUDES = $(GTK_CFLAGS)

generated/*.cs: opaque-api.xml
	$(RUNTIME) $(top_builddir)/generator/gapi_codegen.exe --generate $(srcdir)/opaque-api.xml --include $(top_builddir)/gtk/gtk-api.xml $(top_builddir)/gdk/gdk-api.xml --outdir=generated --assembly-name=opaque-sharp

api:
	PATH=$(top_builddir)/parser:$PATH $(RUNTIME) $(top_builddir)/parser/gapi-parser.exe opaque-sources.xml
	$(RUNTIME) $(top_builddir)/parser/gapi-fixup.exe --metadata=Opaque.metadata --api=opaque-api.xml

install:

CLEANFILES =			\
	opaquetest.exe		\
	opaquetest.exe.mdb	\
	generated/*.cs

EXTRA_DIST =		\
	OpaqueTest.cs	\
	opaque-api.xml	\
	opaque-sources.xml
