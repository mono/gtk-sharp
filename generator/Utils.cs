// GtkSharp.Generation.Utils.cs
//
// Author: Antonius Riha <antoniusriha@gmail.com>
//
// Copyright (c) 2014 Antonius Riha
//
// This program is free software; you can redistribute it and/or
// modify it under the terms of version 2 of the GNU General Public
// License as published by the Free Software Foundation.
//
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
// General Public License for more details.
//
// You should have received a copy of the GNU General Public
// License along with this program; if not, write to the
// Free Software Foundation, Inc., 59 Temple Place - Suite 330,
// Boston, MA 02111-1307, USA.
using System;
using System.IO;

namespace GtkSharp.Generation
{
	public static class Utils
	{
		public static void GenerateVersionIf (StreamWriter sw, string version)
		{
			sw.WriteLine ("#if V_" + Sanitize (version));
		}

		public static void GenerateVersionEndIf (StreamWriter sw)
		{
			sw.WriteLine ("#endif");
		}

		public static void GenerateDeprecated (StreamWriter sw, string deprecatedVersion,
			int indentation)
		{
			if (deprecatedVersion != null) {
				sw.WriteLine ("#if V_" + Sanitize (deprecatedVersion));
			}

			var addIndent = new string ('\t', indentation);
			sw.WriteLine (addIndent + "[Obsolete]");
			if (deprecatedVersion != null) {
				sw.WriteLine ("#endif");
			}
		}

		public static void GenerateObsoleteWarningDisablePragma (StreamWriter sw)
		{
			sw.WriteLine ("#pragma warning disable 612, 618");
		}

		public static void GenerateObsoleteWarningRestorePragma (StreamWriter sw)
		{
			sw.WriteLine ("#pragma warning restore 612, 618");
		}

		private static string Sanitize (string version)
		{
			return version.Replace ('.', '_').Replace ('-', '_').Replace (':', '_').Replace ('~', '_');
		}
	}
}
