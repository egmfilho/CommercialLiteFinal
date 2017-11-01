using System;
using System.Collections.Generic;
using System.Globalization;
using Android.Views;
using Android.Widget;

namespace CommercialLiteFinal.Droid
{
	public class ExportListAdapter : BaseAdapter<Pedido>
	{
		List<Pedido> array;
		Android.App.Activity context;

		public ExportListAdapter(Android.App.Activity context, System.Collections.Generic.List<Pedido> array)
			: base()
		{
			this.array = new List<Pedido>(array);
			this.context = context;
		}

		public override Pedido this[int position]
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

			var view = context.LayoutInflater.Inflate(Resource.Layout.export_list_row_item, null);

			view.FindViewById<TextView>(Resource.Id.lblCodigo).Text = string.Format("Código: {0:000000}", item.Codigo);
			view.FindViewById<TextView>(Resource.Id.lblCliente).Text = string.Format(item.Cliente.Nome);
			string data = string.Format(CultureInfo.GetCultureInfo("pt-BR"), "{0:f}", item.Data);
			view.FindViewById<TextView>(Resource.Id.lblData).Text = data;
			string total = string.Format(CultureInfo.GetCultureInfo("pt-BR"), "Valor total: {0:C}", item.ValorTotalComDescontoSalvo);
			view.FindViewById<TextView>(Resource.Id.lblTotal).Text = total;
			string info = string.Format(CultureInfo.GetCultureInfo("pt-BR"), "ICMS: {0:C} | Subist. Tributária: {1:C}", item.ICMS, item.ST);
			view.FindViewById<TextView>(Resource.Id.lblInfo).Text = info;

			return view;
		}
	}
}
