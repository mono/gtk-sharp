using System;
using System.Text;
using System.Xml;

namespace GtkSharp.Generation
{
	static class XmlElementExtensions
	{
		public static string GetCType (this XmlElement elem)
		{
			return elem.GetAttribute (Constants.CName);
		}

		public static string GetName (this XmlElement elem)
		{
			var name = elem.GetAttribute (Constants.Name);

			return CSharpify (name);
		}

		static string CSharpify (string cname)
		{
			// Capitalize the first letter, and parse for underscores, capitalizing the letters after them
			var sb = new StringBuilder (cname.Length);

			bool isUpper = true;
			foreach (var c in cname) {
				if (c == '_') {
					isUpper = true;
					continue;
				}

				sb.Append (isUpper ? char.ToUpper (c) : c);
				isUpper = false;
			}

			return sb.ToString ();
		}
	}
}
