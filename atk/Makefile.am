SUBDIRS = . glue

pkg = atk
METADATA = Atk.metadata
SYMBOLS =
references = $(top_builddir)/glib/glib-sharp.dll

glue_includes = atk/atk.h

sources = 			\
	ColumnDeletedHandler.cs \
	ColumnInsertedHandler.cs \
	RowDeletedHandler.cs \
	RowInsertedHandler.cs \
	TableAdapter.cs \
	Table.cs \
	TextChangedDetail.cs

customs =			\
	Global.custom		\
	Hyperlink.custom \
	Misc.custom		\
	NoOpObject.custom		\
	Object.custom		\
	ObjectFactory.custom		\
	SelectionAdapter.custom	\
	TextAdapter.custom	\
	Util.custom

add_dist =

include $(top_srcdir)/Makefile.include
