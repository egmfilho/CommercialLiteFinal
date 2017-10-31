using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace CommercialLiteFinal
{
	public class Login
	{
		public int user_id { get; set; }

		[JsonProperty("user_current_session.user_session_value")]	
		public string user_current_session_id { get; set; }

		public string user_user { get; set;}

		public string user_name { get; set; }

		public Decimal user_max_discount { get; set; }

		[JsonProperty("user_company")]
		public List<Loja> user_shop { get; set; }

		public List<Preco> user_price { get; set; }

		public Vendedor user_seller { get; set; }
	}
}
