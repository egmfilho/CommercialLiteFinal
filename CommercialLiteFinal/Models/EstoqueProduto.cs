using System;
using Newtonsoft.Json;

namespace CommercialLiteFinal
{
	public class EstoqueProduto
	{
		[JsonProperty("company_id")]
		public String IdEmpresa
		{
			get;
			set;
		}

		[JsonProperty("product_id")]
		public String IdProduto
		{
			get;
			set;
		}

		[JsonProperty("product_stock")]
		public float Quantidade
		{
			get;
			set;
		}

		[JsonProperty("product_stock_date")]
		public DateTime? Data
		{
			get;
			set;
		}
	}
}
