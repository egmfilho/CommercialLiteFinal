using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace CommercialLiteFinal
{
	public class Usuario
	{
		[JsonProperty("user_id")]
		public int Id { get; set; }

		[JsonProperty("user_name")]
		public string Nome { get; set; }

		public string Token { get; set; }

		[JsonProperty("user_current_session")]
		public Sessao Sessao { get; set; }

		[JsonProperty("user_user")]
		public string Username { get; set; }

		[JsonProperty("user_max_discount")]
		public float MaxDiscount { get; set; }

		[JsonProperty("user_company")]
		public List<Loja> Lojas { get; set; }

		[JsonProperty("user_price")]
		public List<Preco> Precos { get; set; }

		[JsonProperty("user_seller")]
		public Vendedor Vendedor { get; set; }
	}
}