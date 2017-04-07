using System;
using Newtonsoft.Json;

namespace CommercialLite
{	
	public class ItemPedido
	{
		[JsonProperty("order_item_id")]
		public int Id { get; set; }

		public int IdPedido { get; set; }

		[JsonProperty("product_id")]
		public string IdProduto { get; set; }

		//public Decimal PrecoProduto { get; set; }

		[JsonProperty("order_item_al_discount")]
		public Decimal DescontoPercent { get; set; }

		public void SetDescontoPercent(Decimal value)
		{
			this.DescontoPercent = value < 0 ? 0 : value;
			DescontoDinheiro = ValorTotal * (DescontoPercent / 100);
		}

		[JsonProperty("order_item_vl_discount")]
		public Decimal DescontoDinheiro { get; set; }

		public void SetDescontoDinheiro(Decimal value)
		{
			this.DescontoDinheiro = value < 0 ? 0 : value;
			DescontoPercent = (value * 100) / ValorTotal;
		}

		private Decimal quantidade;
		[JsonProperty("order_item_amount")]
		public Decimal Quantidade 
		{ 
			get
			{
				return quantidade;
			} 
			set
			{
				quantidade = value;
				if (ValorTotal != 0)
					UpdateDescontos();
			}
		}

		public Produto Produto { get; set; }

		//public Auditoria Auditoria { get; set; }

		[JsonProperty("order_item_value")]
		public Decimal ValorTotal
		{
			get { return this.Produto != null ? this.Produto.VlPreco * this.Quantidade : 0; }
		}

		[JsonProperty("order_item_value_total")]
		public Decimal ValorTotalComDesconto
		{
			get { return ValorTotal - DescontoDinheiro; }
		}

		public ItemPedido()
		{
			this.Quantidade = 1;
			this.DescontoPercent = 0;
			this.DescontoDinheiro = 0;
		}

		public ItemPedido(Produto produto)
		{
			this.Produto = produto;
			this.Quantidade = 1;
			this.DescontoPercent = 0;
			this.DescontoDinheiro = 0;
		}

		public void SetProduto(Produto produto)
		{
			this.Produto = produto;
			this.IdProduto = produto.IdProduto;
		}

		public void UpdateDescontos()
		{
			this.SetDescontoPercent(DescontoPercent);
		}
	}
}
