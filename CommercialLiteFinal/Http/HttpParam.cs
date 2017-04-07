using System;
namespace CommercialLiteFinal
{
	public class HttpParam
	{
		public string Key
		{
			get;
			set;
		}

		public string Value
		{
			get;
			set;
		}

		public HttpParam(string key, string value)
		{
			this.Key = key;
			this.Value = value;
		}

		public override string ToString()
		{
			return string.Format("{0}={1}", Key, Value);
		}
	}
}
