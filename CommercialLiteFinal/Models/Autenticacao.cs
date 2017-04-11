using System;
using Newtonsoft.Json;

namespace CommercialLiteFinal
{
	public class Autenticacao
	{
		[JsonProperty("device_authentication_user_id")]
		public string UserId { get; set; }

		[JsonProperty("device_authentication_user_name")]
		public string UserName { get; set; }

		[JsonProperty("device_authentication_date")]
		public DateTime Date { get; set; }
	}
}
