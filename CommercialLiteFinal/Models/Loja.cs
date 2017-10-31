using System;
using Newtonsoft.Json;

namespace CommercialLiteFinal
{
	public class LojaERP
	{
		[JsonProperty("company_code")]
		public string Codigo { get; set; }

		[JsonProperty("company_name")]
		public string Nome { get; set; }

		[JsonProperty("company_cnpj")]
		public string CNPJ { get; set; }

		[JsonProperty("company_phone")]
		public string Telefone { get; set; }
	}

	public class Loja
	{
		[JsonProperty("user_company_id")]
		public string Id { get; set; }

		[JsonProperty("company_erp")]
		public LojaERP ERP { get; set; }
	}
}
