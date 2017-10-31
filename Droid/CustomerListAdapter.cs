using System;
using System.Collections.Generic;
using System.Globalization;
using Android.Views;
using Android.Widget;

namespace CommercialLiteFinal.Droid
{
	public class CustomerListAdapter : BaseAdapter<Pessoa>
	{
		List<Pessoa> array;
		Android.App.Activity context;

		public CustomerListAdapter(Android.App.Activity context, System.Collections.Generic.List<Pessoa> array) 
			: base()
		{
			this.context = context;
			this.array = array;
		}

		public override Pessoa this[int position]
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

			var view = context.LayoutInflater.Inflate(Resource.Layout.customer_list_row_item, null);

			var cod = view.FindViewById<TextView>(Resource.Id.lblCodigo);
			cod.Text = string.Format("{0}", item.Codigo);

			var nome = view.FindViewById<TextView>(Resource.Id.lblNome);
			nome.Text = item.Nome;

			var doc = view.FindViewById<TextView>(Resource.Id.lblDoc);
			doc.Text = string.Format("Documento: {0}", item.CPF != null ? item.CPF : item.CNPJ);

			if (array[position].Ativo != 'Y')
			{
				cod.SetTextColor(Android.Graphics.Color.ParseColor("#bbbbbb"));
				nome.SetTextColor(Android.Graphics.Color.ParseColor("#bbbbbb"));
				doc.SetTextColor(Android.Graphics.Color.ParseColor("#bbbbbb"));
			}

			return view;
		}
	}
}
