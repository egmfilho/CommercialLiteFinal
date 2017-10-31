using System;
using Newtonsoft.Json;

namespace CommercialLiteFinal
{
	public class Sessao
	{
		[JsonProperty("user_session_value")]
		public string Valor
		{
			get;
			set;
		}

		[JsonProperty("user_session_date")]
		public DateTime Data
		{
			get;
			set;
		}
	}
}
