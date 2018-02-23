SUBDIRS = sources generator parser glib cairo pango atk gdk gtk glade gtkdotnet sample doc msi

test-gen-changes/glib.gir:
	mkdir -p test-gen-changes
	curl -z test-gen-changes/glib.gir -L https://raw.githubusercontent.com/gtk-rs/gir-files/master/GLib-2.0.gir > test-gen-changes/glib.gir

playground: test-gen-changes/glib.gir $(top_builddir)/generator/gapi_codegen.exe
	$(RUNTIME) $(top_builddir)/generator/gapi_codegen.exe --generate test-gen-changes/glib.gir --outdir=test-gen-changes/generated --assembly-name="glib-sharp-test"


EXTRA_DIST = 			\
	mono.snk		\
	gtk-sharp.snk		\
	policy.config.in	\
	AssemblyInfo.cs.in	\
	ChangeLog		\
	HACKING			\
	README			\
	README.generator

configure.in: bootstrap.status configure.in.in
	$(SHELL) $<
