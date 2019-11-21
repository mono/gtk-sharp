SUBDIRS = glue

TARGET = $(ASSEMBLY)
ASSEMBLY = $(ASSEMBLY_NAME).dll
ASSEMBLY_NAME = glib-sharp
noinst_DATA = $(ASSEMBLY) $(ASSEMBLY).config $(POLICY_ASSEMBLIES)
pkgconfigdir = $(libdir)/pkgconfig
pkgconfig_DATA = glib-sharp-2.0.pc
gapidir = $(datadir)/gapi-2.0
gapi_DATA = glib-api.xml

CLEANFILES = $(ASSEMBLY) $(ASSEMBLY).mdb gtk-sharp.snk AssemblyInfo.cs $(POLICY_ASSEMBLIES) $(POLICY_CONFIGS)
DISTCLEANFILES = $(ASSEMBLY).config

POLICY_ASSEMBLIES = $(addsuffix .$(ASSEMBLY), $(addprefix policy., $(POLICY_VERSIONS)))
POLICY_CONFIGS = $(addsuffix .config, $(addprefix policy., $(POLICY_VERSIONS)))

references =

sources =		 			\
	Argv.cs					\
	Boxed.cs				\
	CDeclCallbackAttribute.cs		\
	ClassInitializerAttribute.cs		\
	ConnectBeforeAttribute.cs		\
	DefaultSignalHandlerAttribute.cs	\
	DelegateWrapper.cs			\
	DestroyNotify.cs			\
	EnumWrapper.cs				\
	ExceptionManager.cs			\
	FastActivator.cs			\
	FileUtils.cs				\
	Format.cs				\
	GException.cs				\
	GInterfaceAdapter.cs			\
	GInterfaceAttribute.cs			\
	Global.cs				\
	GString.cs				\
	GType.cs				\
	GTypeAttribute.cs			\
	Idle.cs					\
	IgnoreClassInitializersAttribute.cs	\
	InitiallyUnowned.cs			\
	IntPtrEqualityComparer.cs	\
	IOChannel.cs				\
	IWrapper.cs				\
	ListBase.cs				\
	List.cs					\
	Log.cs					\
	MainContext.cs				\
	MainLoop.cs				\
	ManagedValue.cs				\
	Markup.cs				\
	Marshaller.cs				\
	MissingIntPtrCtorException.cs		\
	NotifyHandler.cs			\
	Object.cs				\
	ObjectManager.cs			\
	Opaque.cs				\
	PropertyAttribute.cs			\
	PointerWrapper.cs				\
	PtrArray.cs				\
	Signal.cs				\
	SignalArgs.cs				\
	SignalAttribute.cs			\
	SignalCallback.cs			\
	SignalClosure.cs			\
	SList.cs				\
	Source.cs				\
	Spawn.cs				\
	Thread.cs				\
	Timeout.cs				\
	ToggleRef.cs				\
	TypeConverter.cs			\
	TypeFundamentals.cs			\
	TypeInitializerAttribute.cs		\
	TypeEqualityComparer.cs			\
	UnwrappedObject.cs			\
	ValueArray.cs				\
	Value.cs

build_sources = $(addprefix $(srcdir)/, $(sources)) AssemblyInfo.cs
dist_sources = $(sources)

EXTRA_DIST =				\
	$(dist_sources)			\
	$(ASSEMBLY).config.in		\
	glib-sharp-2.0.pc.in		\
	glib-api.xml

gtk-sharp.snk: $(top_srcdir)/gtk-sharp.snk
	cp $(top_srcdir)/gtk-sharp.snk .

AssemblyInfo.cs: $(top_builddir)/AssemblyInfo.cs
	cp $(top_builddir)/AssemblyInfo.cs .
	echo "[assembly:InternalsVisibleTo(\"gtk-sharp, PublicKey=002400000480000094000000060200000024000052534131000400000100010071eb6c5575529cbf7244f7a6ea056284f9eae03bcff2cc132c9c490ab309eab0b56bce449df503d9c0a81e520585cdbe70e2fb90434bac04fa6222a80098b7a1a7b3af991a412324bb4325f6b865bb64ebf6d1c206d5732ddfbc70a7389ee53e0c246e3279741ad00503e49842e19bf37b198b402126cb3689c2ea6496a47cb4\")]" >> AssemblyInfo.cs

$(ASSEMBLY): $(build_sources) gtk-sharp.snk AssemblyInfo.cs
	@rm -f $(ASSEMBLY).mdb
	$(CSC) $(CSFLAGS) -nowarn:0169,0612,0618 -out:$(ASSEMBLY) -target:library $(references) $(build_sources)
	$(GAPI_CDECL_INSERT)

policy.%.config: $(top_builddir)/policy.config
	sed -e "s/@ASSEMBLY_NAME@/$(ASSEMBLY_NAME)/" -e "s/@POLICY@/$*/" $(top_builddir)/policy.config > $@

$(POLICY_ASSEMBLIES) : policy.%.$(ASSEMBLY): policy.%.config gtk-sharp.snk
	$(AL) -link:policy.$*.config -version:$* -out:$@ -keyfile:gtk-sharp.snk

install-data-local:
	@if test -n '$(TARGET)'; then							\
	  echo "$(GACUTIL) -i $(ASSEMBLY) -f $(GACUTIL_FLAGS)";				\
	  $(GACUTIL) -i $(ASSEMBLY) -f $(GACUTIL_FLAGS) || exit 1;			\
	  if test -n '$(POLICY_VERSIONS)'; then						\
	    for i in $(POLICY_VERSIONS); do						\
	      echo "$(GACUTIL) -i policy.$$i.$(ASSEMBLY) -f $(GACUTIL_FLAGS)";		\
	      $(GACUTIL) -i policy.$$i.$(ASSEMBLY) -f $(GACUTIL_FLAGS) || exit 1;	\
	    done									\
	  fi										\
	fi

uninstall-local:
	@if test -n '$(TARGET)'; then							\
	  echo "$(GACUTIL) -u $(ASSEMBLY_NAME) $(GACUTIL_FLAGS)";			\
	  $(GACUTIL) -u $(ASSEMBLY_NAME) $(GACUTIL_FLAGS) || exit 1;			\
	  if test -n '$(POLICY_VERSIONS)'; then						\
	    for i in $(POLICY_VERSIONS); do						\
	      echo "$(GACUTIL) -u policy.$$i.$(ASSEMBLY_NAME) $(GACUTIL_FLAGS)";	\
	      $(GACUTIL) -u policy.$$i.$(ASSEMBLY_NAME) $(GACUTIL_FLAGS) || exit 1;	\
	    done									\
	  fi										\
	fi
