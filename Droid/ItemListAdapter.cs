using System;
using System.Collections.Generic;
using System.Globalization;
using Android.Views;
using Android.Widget;

namespace CommercialLiteFinal.Droid
{
	public class ItemListAdapter : BaseAdapter<ItemPedido>
	{
		List<ItemPedido> array;
		Android.App.Activity context;

		public ItemListAdapter(Android.App.Activity context, List<ItemPedido> array) 
			: base()
		{
			this.context = context;
			this.array = array;
		}

		public override ItemPedido this[int position]
		{
			get
			{
				return array[position];
			}
		}

		public override int Count
		{
			get
			{
				return array.Count;
			}
		}

		public override long GetItemId(int position)
		{
			return position;
		}

		public override View GetView(int position, View convertView, ViewGroup parent)
		{
			var item = array[position];

			var view = context.LayoutInflater.Inflate(Resource.Layout.item_list_row_item, null);

			view.FindViewById<TextView>(Resource.Id.lblCodigo).Text = string.Format("{0}", item.Produto.CdProduto);
			view.FindViewById<TextView>(Resource.Id.lblNome).Text = string.Format("{0}x {1}", item.Quantidade, item.Produto.NmProduto);
			view.FindViewById<TextView>(Resource.Id.lblQtd).Text = string.Format("Quantidade: {0} {1}", item.Quantidade, item.Produto.Unidade.Iniciais);
			//view.FindViewById<TextView>(Resource.Id.lblPreco).Text = string.Format(CultureInfo.GetCultureInfo("pt-BR"), "Valor unitário: {0:C}", item.GetTabelaDePreco().Valor);
			view.FindViewById<TextView>(Resource.Id.lblPreco).Text = item.GetTabelaDePreco().ToString(CultureInfo.GetCultureInfo("pt-BR"));
			view.FindViewById<TextView>(Resource.Id.lblDesconto).Text = string.Format(CultureInfo.GetCultureInfo("pt-BR"), "Desconto: {0:0.##}% {1:C}", item.DescontoPercent, item.DescontoDinheiro);
			view.FindViewById<TextView>(Resource.Id.lblTotal).Text = string.Format(CultureInfo.GetCultureInfo("pt-BR"), "Valor Total: {0:C}", item.ValorTotalComDesconto);

			return view;
		}
	}
}
