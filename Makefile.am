SUBDIRS = sources generator parser glib cairo pango atk gdk gtk glade gtkdotnet sample doc msi

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
