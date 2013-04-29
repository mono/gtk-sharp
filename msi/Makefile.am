SUBDIRS = unmanaged .

# Don't change this GUID, it's used in the Updater Service
GTKSHARP_GUID=600D9C0E-3B74-4C51-B7E6-29A47E9325E4
VERSION_X=$(word 1, $(subst ., ,$(VERSION)))
VERSION_Y=$(word 2, $(subst ., ,$(VERSION)))
VERSION_Z=$(word 3, $(subst ., ,$(VERSION)))
UPDATEINFO=$(shell printf "%03d%03d%03d" $(VERSION_X) $(VERSION_Y) $(VERSION_Z))
assembly_dirs = glib pango atk gdk gtk glade gtkdotnet cairo

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
		cp $(top_builddir)/$$a/*.pdb binaries/$$a; \
		cp $(top_builddir)/$$a/policy.*.config binaries/$$a; \
	done
	mv binaries/libatksharpglue-2.dll   binaries/atksharpglue-2.dll
	mv binaries/libgdksharpglue-2.dll   binaries/gdksharpglue-2.dll
	mv binaries/libgladesharpglue-2.dll binaries/gladesharpglue-2.dll
	mv binaries/libglibsharpglue-2.dll  binaries/glibsharpglue-2.dll
	mv binaries/libgtksharpglue-2.dll   binaries/gtksharpglue-2.dll
	mv binaries/libpangosharpglue-2.dll binaries/pangosharpglue-2.dll

	echo $(GTKSHARP_GUID) $(UPDATEINFO) > updateinfo
	cp $(top_builddir)/sample/GtkDemo/GtkDemo.exe binaries
	candle -ext WixUIExtension gtk-sharp-2.0.wxs
	light -cultures:en-us -ext WixUIExtension -ext WixNetFxExtension -out gtk-sharp-$(VERSION).msi gtk-sharp-2.0.wixobj

EXTRA_DIST = license.rtf
