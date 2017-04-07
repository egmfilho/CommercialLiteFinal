using System;
namespace CommercialLiteFinal
{
	public class Preco
	{
		public string price_id { get; set; }
		public string price_code { get; set; }
		public string price_name { get; set; }
	}

	public class Vendedor
	{
		public string Id { get; set; }
		public string IdLoja { get; set; }
		public string IdCep { get; set; }
		public string Codigo { get; set; }
		public string Nome { get; set; }
	}
}
