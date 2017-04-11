
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace CommercialLiteFinal.Droid
{
	[Activity(Label = "OrderItemsActivity", ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait)]
	public class OrderItemsActivity : Activity
	{
		protected override void OnCreate(Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);

			// Create your application here
			SetContentView(Resource.Layout.OrderItems);

			var s = this.Intent.GetStringExtra("itemList");
			if (!string.IsNullOrEmpty(s))
			{
				List<ItemPedido> items = Serializador.LoadFromXMLString<List<ItemPedido>>(s);

				var listView = FindViewById<ListView>(Resource.Id.listaItens);
				listView.Adapter = new ItemListAdapter(this, items);
				listView.ItemClick += (object sender, AdapterView.ItemClickEventArgs e) =>
				{
					var intent = new Intent(this, typeof(ItemActivity));
					intent.PutExtra("item", Serializador.ToXML(items[e.Position]));
					intent.PutExtra("action", "update");
					StartActivity(intent);
				};
			}

		}
	}
}
