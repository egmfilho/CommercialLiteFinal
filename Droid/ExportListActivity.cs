
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Preferences;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace CommercialLiteFinal.Droid
{		
	[Activity(Label = "ExportActivity", ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait)]
	public class ExportListActivity : Activity
	{
		Usuario user;

		List<Pedido> array;
		ListView lista;

		protected override void OnCreate(Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);

			// Create your application here
			SetContentView(Resource.Layout.ExportList);

			user = Serializador.LoadFromXMLString<Usuario>(PreferenceManager.GetDefaultSharedPreferences(this).GetString("user", ""));

			lista = FindViewById<ListView>(Resource.Id.listaExportados);

			Load();
		}

		private void Load()
		{
			var progressDialog = ProgressDialog.Show(this, "Carregando", "Buscando orçamentos...", true);
			var t = new Thread(new ThreadStart(delegate
			{				
				var res = Request.GetInstance().Post<List<Pedido>>("order", "getList", user.Token, new HttpParam("order_user_id", user.Id.ToString()), new HttpParam("order_limit", "20"));

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
						array = new List<Pedido>(res.data);
						lista.Adapter = new ExportListAdapter(this, array);
					}
					else if (res.status.code == 401)
					{
						StartActivity(new Intent(this, typeof(LogoutActivity)));
					}
					else
					{
						Toast.MakeText(this, res.status.description, ToastLength.Short).Show();
					}
				});
			}));
			t.Start();
		}
	}
}
