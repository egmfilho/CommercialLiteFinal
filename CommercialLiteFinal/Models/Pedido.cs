using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace CommercialLiteFinal
{
	public class Pedido
	{
		[JsonProperty("order_id")]
		public String Id { get; set; }

		[JsonProperty("order_code")]
		public String Codigo { get; set; }

		[JsonProperty("order_code_erp")]
		public String Erp { get; set; }

		[JsonProperty("order_user_id")]
		public int IdUsuario { get; set; }

		[JsonProperty("order_status_id")]
		public int IdStatus { get; set; }

		[JsonProperty("order_client_id")]
		public string IdCliente { get; set; }

		[JsonProperty("order_seller_id")]
		public string IdVendedor { get; set; }

		[JsonProperty("order_company_id")]
		public string IdLoja { get; set; }

		[JsonProperty("order_price_id")]
		public string IdPrecos { get; set; }

		[JsonProperty("order_note")]
		public string Observacoes { get; set; }

		[JsonProperty("order_items")]
		public List<ItemPedido> Items { get; set; }

		private Pessoa cliente;
		[JsonProperty("order_client")]
		public Pessoa Cliente 
		{ 
			get 
			{
				return cliente;
			}

			set
			{
				this.cliente = value;
				this.IdCliente = value.Id;
			} 
		}

		public Pessoa vendedor;
		[JsonProperty("order_seller")]
		public Pessoa Vendedor 
		{ 
			get
			{
				return vendedor;
			} 

			set
			{
				this.vendedor = value;
				this.IdVendedor = value.Id;
			}
		}

		Decimal _valorTotal = 0;
		[JsonProperty("order_value")]
		public Decimal ValorTotalSalvo 
		{ 
			get
			{
				return _valorTotal == 0 ? ValorTotal : _valorTotal;
			}
			set 
			{
				_valorTotal = value;
			}
		}

		public Decimal ValorTotal
		{
			get
			{
				Decimal total = 0;
				foreach (ItemPedido i in this.Items)
				{
					total += i.ValorTotal;
				}
				return total;
			}
		}

		Decimal _valorTotalComDesconto = 0;
		[JsonProperty("order_value_total")]
		public Decimal ValorTotalComDescontoSalvo
		{
			get
			{
				return _valorTotalComDesconto == 0 ? ValorTotalComDesconto : _valorTotalComDesconto;
			}
			set
			{
				_valorTotalComDesconto = value;
			}
		}

		public Decimal ValorTotalComDesconto
		{
			get
			{
				Decimal total = 0;
				foreach (ItemPedido i in this.Items)
				{
					total += i.ValorTotalComDesconto;
				}
				return total;
			}
		}

		[JsonProperty("order_vl_discount")]
		public Decimal DescontoDinheiroTotal
		{
			get
			{
				Decimal total = 0;
				foreach (var i in this.Items)
				{
					total += i.DescontoDinheiro;
				}
				return total;
			}
		}

		[JsonProperty("order_al_discount")]
		public Decimal DescontoPercentTotal
		{
			get
			{				
				return ValorTotal == 0 ? 0 : (DescontoDinheiroTotal * 100) / ValorTotal;
			}
		}

		[JsonProperty("order_date")]
		public DateTime Data { get; set; }

		[JsonProperty("order_address_delivery_code")]
		public string CdEntrega { get; set; }

		Endereco entrega;
		[JsonProperty("address_delivery")]
		public Endereco Entrega
		{
			get { return entrega; }
			set
			{
				this.entrega = value;
				this.CdEntrega = value.Codigo;
			}
		}

		[JsonProperty("order_value_st")]
		public Decimal ST
		{
			get;
			set;
		} = 0;

		[JsonProperty("order_value_icms")]
		public Decimal ICMS
		{
			get;
			set;
		} = 0;

		public Pedido(int idUsuario, string idVendedor, string idLoja)
		{
			this.IdUsuario = idUsuario;
			this.IdVendedor = idVendedor;
			this.IdLoja = idLoja;
			this.Items = new List<ItemPedido>();
		}
	}
}
