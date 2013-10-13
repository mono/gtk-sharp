assemblydir = $(prefix)/lib/gapi-3.0
pkgconfigdir = $(libdir)/pkgconfig
assembly_DATA = gapi-fixup.exe
bin_SCRIPTS = gapi3-fixup
CLEANFILES = gapi-fixup.exe
DISTCLEANFILES = gapi3-fixup

sources = 				\
	gapi-fixup.cs

EXTRA_DIST =					\
	$(sources)

gapi-fixup.exe: $(srcdir)/gapi-fixup.cs
	$(CSC) $(CSFLAGS) -out:gapi-fixup.exe $(srcdir)/gapi-fixup.cs

