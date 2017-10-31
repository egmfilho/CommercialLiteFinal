using System;
using Newtonsoft.Json;

namespace CommercialLiteFinal
{
	public class Vendedor
	{
		[JsonProperty("person_id")]
		public string Id { get; set; }

		[JsonProperty("person_code")]
		public string Codigo { get; set; }

		[JsonProperty("person_name")]
		public string Nome { get; set; }
	}
}
