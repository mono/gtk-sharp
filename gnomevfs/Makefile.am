ASSEMBLY_NAME = gnome-vfs-sharp
references = /r:../glib/glib-sharp.dll

TARGET = $(ASSEMBLY)
ASSEMBLY = $(ASSEMBLY_NAME).dll
noinst_DATA = $(ASSEMBLY) $(ASSEMBLY).config
CLEANFILES = $(ASSEMBLY) gtk-sharp.pub
DISTCLEANFILES = $(ASSEMBLY).config AssemblyInfo.cs

sources = 				\
	AsyncCallback.cs		\
	AsyncCallbackNative.cs		\
	AsyncReadCallback.cs		\
	AsyncReadCallbackNative.cs	\
	AsyncWriteCallback.cs		\
	AsyncWriteCallbackNative.cs	\
	Handle.cs			\
	MimeActionType.cs		\
	Mime.cs				\
	OpenMode.cs			\
	Result.cs			\
	SeekPosition.cs			\
	Vfs.cs

build_sources = $(addprefix $(srcdir)/, $(sources)) AssemblyInfo.cs

EXTRA_DIST =			\
	$(ASSEMBLY).config.in	\
	AssemblyInfo.cs.in	\
	$(sources)

gtk-sharp.pub: $(top_srcdir)/gtk-sharp.pub
	cp $(top_srcdir)/gtk-sharp.pub .

$(ASSEMBLY): $(build_sources) gtk-sharp.pub
	$(CSC) --unsafe /out:$(ASSEMBLY) /target:library $(references) $(build_sources)

install-data-local:
	@if test -n '$(TARGET)'; then						\
	  echo "$(GACUTIL) /i $(ASSEMBLY) /f /package $(PACKAGE) /root $(DESTDIR)$(libdir)";	\
	  $(GACUTIL) /i $(ASSEMBLY) /f /package $(PACKAGE) /root $(DESTDIR)$(libdir) || exit 1;	\
	fi

uninstall-local:
	@if test -n '$(TARGET)'; then						\
	  echo "$(GACUTIL) /u $(ASSEMBLY_NAME) /package $(PACKAGE) /root $(DESTDIR)$(libdir)";	\
	  $(GACUTIL) /u $(ASSEMBLY_NAME) /package $(PACKAGE) /root $(DESTDIR)$(libdir) || exit 1;	\
	fi
