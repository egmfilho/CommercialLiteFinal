using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace CommercialLiteFinal
{	
	public class Produto
	{
		[JsonProperty("product_id")]
		public string IdProduto { get; set; }

		[JsonProperty("product_code")]
		public string CdProduto { get; set; }

		[JsonProperty("product_name")]
		public string NmProduto { get; set; }

		//public string NmMarca { get; set; }

		[JsonProperty("product_classification")]
		public string Ref { get; set; }

		[JsonProperty("product_ean")]
		public string EAN { get; set; }

		[JsonProperty("stock")]
		public EstoqueProduto Estoque { get; set; }

		[JsonProperty("product_prices")]
		public List<Preco> Precos { get; set; }

		[JsonProperty("unit")]
		public Unidade Unidade { get; set; }

		[JsonProperty("product_active")]
		public char Ativo { get; set; } = 'Y';
	}
}
