SUBDIRS = sources generator parser glib cairo pango atk gdk gtk glade gtkdotnet sample doc

EXTRA_DIST = 			\
	mono.snk		\
	gtk-sharp.snk		\
	gapi-cdecl-insert	\
	license.rtf		\
	makefile.win32		\
	policy.config.in	\
	AssemblyInfo.cs.in	\
	ChangeLog		\
	HACKING			\
	README			\
	README.generator

configure.in: bootstrap.status configure.in.in 
	$(SHELL) $<

assembly_dirs = glib pango atk gdk gtk glade gtkdotnet

installer-bundle: all
	mkdir -p gtk-sharp-binaries-$(VERSION)
	rm -rf gtk-sharp-binaries-$(VERSION)/*
	cp $(srcdir)/license.rtf gtk-sharp-binaries-$(VERSION)
	cp *.wxs gtk-sharp-binaries-$(VERSION)
	cp generator/gapi_codegen.exe gtk-sharp-binaries-$(VERSION)
	cp */glue/.libs/*.dll gtk-sharp-binaries-$(VERSION)
	@for a in $(assembly_dirs); do \
		mkdir -p gtk-sharp-binaries-$(VERSION)/$$a; \
		cp $$a/*.dll gtk-sharp-binaries-$(VERSION)/$$a; \
		cp $$a/policy.*.config gtk-sharp-binaries-$(VERSION)/$$a; \
	done
	cp sample/GtkDemo/GtkDemo.exe gtk-sharp-binaries-$(VERSION)
	echo "candle -ext WixUIExtension gtk-sharp-2.0.wxs" > gtk-sharp-binaries-$(VERSION)/build-installer
	echo "light -cultures:en-us -ext WixUIExtension gtk-sharp-2.0.wixobj" >> gtk-sharp-binaries-$(VERSION)/build-installer
	chmod +x gtk-sharp-binaries-$(VERSION)/build-installer

