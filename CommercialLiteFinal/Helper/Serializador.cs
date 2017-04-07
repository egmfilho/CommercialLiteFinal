using System;
namespace CommercialLiteFinal
{
	public static class Serializador
	{
		public static string ToXML<T>(T obj)
		{
			var stringwriter = new System.IO.StringWriter();
			var serializer = new System.Xml.Serialization.XmlSerializer(obj.GetType());
			serializer.Serialize(stringwriter, obj);
			return stringwriter.ToString();
		}

		public static T LoadFromXMLString<T>(string xmlText)
		{
			var stringReader = new System.IO.StringReader(xmlText);
			var serializer = new System.Xml.Serialization.XmlSerializer(typeof(T));
			var obj = serializer.Deserialize(stringReader);
			return (T)Convert.ChangeType(obj, typeof(T));
		}
	}
}
