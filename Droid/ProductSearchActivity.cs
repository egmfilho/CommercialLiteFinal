using System.Collections.Generic;
using System.Threading;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Preferences;
using Android.Widget;
using ZXing.Mobile;

namespace CommercialLiteFinal.Droid
{
	[Activity(Label = "ProductSearchActivity")]
	public class ProductSearchActivity : Activity
	{
		string token;
		string idPreco;

		List<Produto> arrayProdutos = new List<Produto>();
		ListView listView;
		SearchView searchView;

		protected override void OnCreate(Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);

			token = PreferenceManager.GetDefaultSharedPreferences(this).GetString("token", "");
			idPreco = PreferenceManager.GetDefaultSharedPreferences(this).GetString("priceId", "");

			MobileBarcodeScanner.Initialize(Application);
			SetContentView(Resource.Layout.ProductSearch);

			Toolbar toolbar = FindViewById<Toolbar>(Resource.Id.toolbar);
			toolbar.InflateMenu(Resource.Menu.product_search_options_menu);
			toolbar.FindViewById<TextView>(Resource.Id.scan).SetTextColor(Android.Graphics.Color.White);

			toolbar.MenuItemClick += (sender, e) =>
			{
				switch (e.Item.ItemId)
				{
					case Resource.Id.scan:
						Scan();
						break;
				}
			};

			listView = FindViewById<ListView>(Resource.Id.listaProdutos);
			listView.ItemClick += (object sender, AdapterView.ItemClickEventArgs e) => 
			{
				var produto = arrayProdutos[e.Position];

				if (!produto.Ativo)
				{
					AlertDialog.Builder alert = new AlertDialog.Builder(this);
					alert.SetTitle("Aviso");
					alert.SetMessage("Este produto encontra-se inativo!");
					alert.SetPositiveButton("Fechar", (s, v) => { });
					alert.Show();
					return;
				}
					
				var intent = new Intent(this, typeof(ItemActivity));
				intent.PutExtra("productCode", produto.CdProduto);
				intent.PutExtra("action", "new");
				StartActivity(intent);
			};

			searchView = FindViewById<SearchView>(Resource.Id.searchView);
			searchView.SetIconifiedByDefault(false);

			searchView.QueryTextSubmit += (object sender, SearchView.QueryTextSubmitEventArgs e) =>
			{				
				string query = e.Query;

				if (string.IsNullOrEmpty(query))
				{
					return;
				}

				Search(query);
			};
		}

		private void Search(string query)
		{
			var progressDialog = ProgressDialog.Show(this, "Pesquisando", "Checando produtos...", true);

			var t = new Thread(new ThreadStart(delegate
			{				
				Status status;
				arrayProdutos.Clear();

				long codigo;
				if (long.TryParse(query, out codigo))
				{
					var res = Request.GetInstance().Post<Produto>("product", "get", token, new HttpParam("CdProduto", query), new HttpParam("price_id", idPreco));
					status = res.status;

					if (status.code == 401)
						StartActivity(new Intent(this, typeof(LogoutActivity)));					
					else if (status.code == 200)
						arrayProdutos = new List<Produto>(new Produto[] { res.data } );
				}
				else
				{
					var res = Request.GetInstance().Post<List<Produto>>("product", "getList", token, new HttpParam("NmProduto", query), new HttpParam("price_id", idPreco));
					status = res.status;

					if (status.code == 401)
						StartActivity(new Intent(this, typeof(LogoutActivity)));
					else if (status.code == 200)
						arrayProdutos = new List<Produto>(res.data);
				}

				RunOnUiThread(() =>
				{
					progressDialog.Hide();
					searchView.ClearFocus();
					listView.Adapter = new ProductListAdapter(this, arrayProdutos);

					if (status.code != 200)
						Toast.MakeText(this, status.description, ToastLength.Short).Show();
					else
						Toast.MakeText(this, arrayProdutos.Count + " produtos encontrados!", ToastLength.Short).Show();						
				});
			}));
			t.Start();
		}

		private async void Scan()
		{			
			MobileBarcodeScanningOptions options = new MobileBarcodeScanningOptions();
			options.AutoRotate = false;

			var scanner = new MobileBarcodeScanner();
			scanner.AutoFocus();
			var result = await scanner.Scan(options);

			if (result != null)
				searchView.SetQuery(result.Text, true);
		}
	}
}
