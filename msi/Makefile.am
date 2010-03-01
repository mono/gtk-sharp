SUBDIRS = unmanaged .

assembly_dirs = glib pango atk gdk gtk glade gtkdotnet

if ENABLE_MSI
TARGET=gtk-sharp-2.0.msi
else
TARGET=
endif

noinst_DATA = $(TARGET)

gtk-sharp-2.0.msi: gtk-sharp-2.0.wxs
	mkdir -p binaries
	rm -rf binaries/*
	cp $(top_builddir)/generator/gapi_codegen.exe binaries
	cp $(top_builddir)/*/glue/.libs/*.dll binaries
	for a in $(assembly_dirs); do \
		mkdir -p binaries/$$a; \
		cp $(top_builddir)/$$a/*.dll binaries/$$a; \
		cp $(top_builddir)/$$a/policy.*.config binaries/$$a; \
	done
	cp $(top_builddir)/sample/GtkDemo/GtkDemo.exe binaries
	candle -ext WixUIExtension gtk-sharp-2.0.wxs
	light -cultures:en-us -ext WixUIExtension -ext WixNetFxExtension gtk-sharp-2.0.wixobj

EXTRA_DIST = license.rtf
