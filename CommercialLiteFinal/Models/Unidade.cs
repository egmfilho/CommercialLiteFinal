using System;
using Newtonsoft.Json;

namespace CommercialLiteFinal
{
	public class Unidade
	{
		[JsonProperty("unit_id")]
		public String Id
		{
			get;
			set;
		}

		[JsonProperty("unit_code")]
		public String Codigo
		{
			get;
			set;
		}

		[JsonProperty("unit_initials")]
		public String Iniciais
		{
			get;
			set;
		}

		[JsonProperty("unit_name")]
		public String Nome
		{
			get;
			set;
		}

		[JsonProperty("unit_format")]
		public Char Formato
		{
			get;
			set;
		}
	}
}
