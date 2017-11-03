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
	[Activity(Label = "ProductSearchActivity", ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait)]
	public class ProductSearchActivity : Activity
	{
		Usuario user;
		Loja shop;

		List<Produto> arrayProdutos = new List<Produto>();
		ListView listView;
		SearchView searchView;

		protected override void OnCreate(Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);

			user = Serializador.LoadFromXMLString<Usuario>(PreferenceManager.GetDefaultSharedPreferences(this).GetString("user", ""));
			shop = Serializador.LoadFromXMLString<Loja>(PreferenceManager.GetDefaultSharedPreferences(this).GetString("shop", ""));

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

				if (produto.Ativo != 'Y')
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
				intent.PutExtra("shopCode", shop.ERP.Codigo);
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
				HttpResponse res;
				List<Produto> array;
				arrayProdutos.Clear();

				long codigo;
				if (long.TryParse(query, out codigo))
				{
					var response = Request.GetInstance().Post<Produto>("product", "get", user.Token, new HttpParam("product_code", query), new HttpParam("company_id", shop.ERP.Codigo), new HttpParam("get_product_unit", "1"), new HttpParam("get_product_stock", "1"), new HttpParam("get_product_price", "1"));
					array = response.data != null ? new List<Produto>(new Produto[] { response.data }) : new List<Produto>();
					res = (HttpResponse)response;
				}
				else
				{
					var response = Request.GetInstance().Post<List<Produto>>("product", "getList", user.Token, new HttpParam("product_name", query), new HttpParam("company_id", shop.ERP.Codigo), new HttpParam("get_product_unit", "1"), new HttpParam("get_product_stock", "1"), new HttpParam("get_product_price", "1"));
					array = response.data != null ? new List<Produto>(response.data) : new List<Produto>();
					res = (HttpResponse)response;
				}

				RunOnUiThread(() =>
				{
					progressDialog.Hide();

					if (res.status == null)
					{
#if DEBUG
						AlertDialog.Builder alerta = new AlertDialog.Builder(this);
						alerta.SetTitle("Debug");
						alerta.SetMessage(res.debug);
						alerta.SetPositiveButton("Fechar", (sender, e) => { });
						alerta.Show();
						return;
#else
						Toast.MakeText(this, "Erro no servidor!", ToastLength.Long).Show();
							return;
#endif
					}
											
					if (res.status.code == 200)
					{
						arrayProdutos = new List<Produto>(array);
						Toast.MakeText(this, arrayProdutos.Count + " produtos encontrados!", ToastLength.Short).Show();
					}
					else
					{
						if (res.status.code == 401)
							StartActivity(new Intent(this, typeof(LogoutActivity)));

						Toast.MakeText(this, res.status.description, ToastLength.Short).Show();
					}

					searchView.ClearFocus();
					listView.Adapter = new ProductListAdapter(this, arrayProdutos);
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