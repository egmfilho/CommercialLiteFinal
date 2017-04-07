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

			view.FindViewById<TextView>(Resource.Id.lblCodigo).Text = string.Format("{0}", item.Codigo);
			view.FindViewById<TextView>(Resource.Id.lblNome).Text = item.Nome;
			view.FindViewById<TextView>(Resource.Id.lblDoc).Text = string.Format("Documento: {0}", item.Doc);

			return view;
		}
	}
}
