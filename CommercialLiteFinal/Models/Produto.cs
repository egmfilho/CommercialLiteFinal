using System;
using Newtonsoft.Json;

namespace CommercialLiteFinal
{	
	public class Produto
	{
		public string IdProduto { get; set; }
		public string CdProduto { get; set; }
		public string NmProduto { get; set; }
		public string NmMarca { get; set; }
		public string Ref { get; set; }
		public string EAN { get; set; }
		public Decimal QtEstoque { get; set; }
		public Decimal VlPreco { get; set; }
		public string Unidade { get; set; }
		public bool Ativo { get; set; }

		public DateTime? DtPreco { get; set; }
	}
}
