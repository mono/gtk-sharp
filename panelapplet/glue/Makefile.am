lib_LTLIBRARIES = $(TARGET)

if ENABLE_PANEL
TARGET = libpanelappletsharpglue.la
else
TARGET = 
endif

libpanelappletsharpglue_la_SOURCES =	\
	panelapplet.c

libpanelappletsharpglue_la_LDFLAGS = -module -avoid-version -no-undefined

libpanelappletsharpglue_la_LIBADD = $(LIBPANEL_LIBS)

INCLUDES = $(LIBPANEL_CFLAGS) -I$(top_srcdir)

CLEANFILES = lib*.a
