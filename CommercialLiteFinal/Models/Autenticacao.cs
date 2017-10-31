using System;
using Newtonsoft.Json;

namespace CommercialLiteFinal
{
	public class Autenticacao
	{
		[JsonProperty("user_id")]
		public string UserId { get; set; }

		[JsonProperty("user_name")]
		public string UserName { get; set; }

		[JsonProperty("date")]
		public DateTime Date { get; set; }
	}
}
