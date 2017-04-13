using System;
using Android.Views;
using Android.Widget;

namespace CommercialLiteFinal.Droid
{
	public class ProductListAdapter : BaseAdapter<Produto>
	{
		System.Collections.Generic.List<Produto> array;
		Android.App.Activity context;

		public ProductListAdapter(Android.App.Activity context, System.Collections.Generic.List<Produto> array) 
			: base()
		{
			this.context = context;
			this.array = array;
		}

		public override Produto this[int position]
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

			var view = context.LayoutInflater.Inflate(Resource.Layout.product_list_row_item, null);

			var codigo = view.FindViewById<TextView>(Resource.Id.lblCodigo);
			codigo.Text = string.Format("Código: {0:000000}", item.CdProduto);

			var nome = view.FindViewById<TextView>(Resource.Id.lblNome);
			nome.Text = item.NmProduto;

			var preco = view.FindViewById<TextView>(Resource.Id.lblPreco);
			preco.Text = item.VlPreco.ToString("C");

			if (!array[position].Ativo)
			{
				codigo.SetTextColor(Android.Graphics.Color.ParseColor("#bbbbbb"));
				nome.SetTextColor(Android.Graphics.Color.ParseColor("#bbbbbb"));
				preco.SetTextColor(Android.Graphics.Color.ParseColor("#bbbbbb"));
			}

			return view;
		}
	}
}
