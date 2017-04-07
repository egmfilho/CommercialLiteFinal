using System;
using Android.Views;
using Android.Widget;

namespace CommercialLite.Droid
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
			View view = convertView;

			if (view == null)
				view = context.LayoutInflater.Inflate(Android.Resource.Layout.SimpleListItem1, null);

			view.FindViewById<TextView>(Android.Resource.Id.Text1).Text = array[position].CdProduto + " - " + array[position].NmProduto;

			return view;
		}
	}
}
