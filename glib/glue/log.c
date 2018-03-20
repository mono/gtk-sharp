/* log.c : Glue to invoke vararg logging functions.
 *
 * Author: Fabian Pietsch <fabian@canvon.de>
 *
 * Copyright (c) 2018 Fabian Pietsch
 *
 * This program is free software; you can redistribute it and/or
 * modify it under the terms of version 2 of the Lesser GNU General 
 * Public License as published by the Free Software Foundation.
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
 * Lesser General Public License for more details.
 *
 * You should have received a copy of the GNU Lesser General Public
 * License along with this program; if not, write to the
 * Free Software Foundation, Inc., 59 Temple Place - Suite 330,
 * Boston, MA 02111-1307, USA.
 */

#include <glib.h>

/* Forward declarations */
void glibsharp_log_msg (const gchar *log_domain, GLogLevelFlags log_level, const gchar *message);
/* */

void
glibsharp_log_msg (
		const gchar    *log_domain,
		GLogLevelFlags  log_level,
		/* Provide message as pre-formatted string, *instead of* a format string! */
		const gchar    *message)
{
	/* Log message without processing it for format specifiers. */
	g_log (log_domain, log_level, "%s", message);
}
