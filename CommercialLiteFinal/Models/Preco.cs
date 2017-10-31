using System;
using Newtonsoft.Json;

namespace CommercialLiteFinal
{
	public class Preco
	{
		[JsonProperty("price_id")]
		public String Id
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

		[JsonProperty("company_id")]
		public String IdEmpresa
		{
			get;
			set;
		}

		[JsonProperty("price_value")]
		public Decimal Valor
		{
			get;
			set;
		}

		[JsonProperty("price_date")]
		public DateTime? Data
		{
			get;
			set;
		}

		[JsonProperty("price_name")]
		public String Nome
		{
			get;
			set;
		}

		public string ToString(System.Globalization.CultureInfo info)
		{
			return string.Format(info, "{0:C} ({1})", Valor, Nome);
		}
	}
}
