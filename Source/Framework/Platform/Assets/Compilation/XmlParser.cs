using System;

namespace Pegasus.Framework.Platform.Assets.Compilation
{
	using System.Collections.Generic;
	using System.Linq;
	using System.Xml.Linq;

	/// <summary>
	///   Helper methods for XML file parsing.
	/// </summary>
	internal sealed class XmlParser
	{
		private readonly XDocument _document;

		public XmlParser(string fileName)
		{
			_document = XDocument.Load(fileName);
		}

		public XElement Root
		{
			get { return _document.Root; }
		}

		public bool HasElement(XElement element, string name)
		{
			return FindElements(element, name).Any();
		}

		public XElement FindElement(XElement element, string name)
		{
			var children = element.Descendants(name).ToArray();
			if (!children.Any())
				Log.Die("Unable to find element '{0}'.", name);
			if (children.Count() > 1)
				Log.Die("More than one element '{0}' found. Expected only one.", name);

			return children.Single();
		}

		public IEnumerable<XElement> FindElements(XElement element, string name)
		{
			return element.Descendants(name);
		}

		public XAttribute FindAttribute(XElement element, string name)
		{
			var attribute = element.Attribute(name);
			if (attribute == null)
				Log.Die("Unable to find attribute '{0}'.", name);
			return attribute;
		}

		public uint ReadAttributeUInt32(XElement element, string name)
		{
			var attribute = FindAttribute(element, name);

			uint result;
			if (!UInt32.TryParse(attribute.Value, out result))
				Log.Die("Unable to read value of attribute '{0}'. Expected an unsigned integer.", name);
			return result;
		}

		public int ReadAttributeInt32(XElement element, string name)
		{
			var attribute = FindAttribute(element, name);

			int result;
			if (!Int32.TryParse(attribute.Value, out result))
				Log.Die("Unable to read value of attribute '{0}'. Expected a signed integer.", name);
			return result;
		}

		public ushort ReadAttributeUInt16(XElement element, string name)
		{
			var attribute = FindAttribute(element, name);

			ushort result;
			if (!UInt16.TryParse(attribute.Value, out result))
				Log.Die("Unable to read value of attribute '{0}'. Expected an unsigned short.", name);
			return result;
		}

		public short ReadAttributeInt16(XElement element, string name)
		{
			var attribute = FindAttribute(element, name);

			short result;
			if (!Int16.TryParse(attribute.Value, out result))
				Log.Die("Unable to read value of attribute '{0}'. Expected a signed short.", name);
			return result;
		}

		public string ReadAttributeString(XElement element, string name)
		{
			var attribute = FindAttribute(element, name);
			return attribute.Value;
		}
	}
}