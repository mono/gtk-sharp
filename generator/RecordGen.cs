using System;
using System.Xml;

namespace GtkSharp.Generation
{
	public class RecordGen : GenBase
	{
		// Logic around Record is that it's a struct generation
		// If it has no fields it is an Opaque gen
		// Otherwise, it's a Struct gen

		protected override void ParseElement(XmlElement ns, XmlElement elem)
		{
			base.ParseElement(ns, elem);
		}

		protected override void ParseChildElement(XmlElement ns, XmlElement childElement)
		{
			base.ParseChildElement(ns, childElement);
		}
		public override string MarshalType {
			get {
				throw new NotImplementedException ();
			}
		}

		public override string DefaultValue {
			get {
				throw new NotImplementedException ();
			}
		}

		public override string CallByName (string var)
		{
			throw new NotImplementedException ();
		}

		public override string FromNative (string var)
		{
			throw new NotImplementedException ();
		}

		public override bool Validate ()
		{
			throw new NotImplementedException ();
		}

		public override void Generate (GenerationInfo geninfo)
		{
			throw new NotImplementedException ();
		}
	}
}
