
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
	[Activity(Label = "LogoutActivity")]
	public class LogoutActivity : Activity
	{
		protected override void OnCreate(Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);

			// Create your application here

			var progressDialog = ProgressDialog.Show(this, "Saindo", "Finalizando sessão...", true);
			var t = new Thread(new ThreadStart(delegate
			{
				Request.GetInstance().Logout();
				RunOnUiThread(() =>
				{
					progressDialog.Hide();
					//if (res.status.code == 200)
					{
						ISharedPreferences prefs = PreferenceManager.GetDefaultSharedPreferences(this);
						ISharedPreferencesEditor editor = prefs.Edit();
						var username = prefs.GetString("username", "");
						editor.Clear();
						editor.PutString("username", username);
						editor.Commit();
						var intent = new Intent(this, typeof(MainActivity));
						intent.AddFlags(ActivityFlags.ClearTop | ActivityFlags.NewTask);
						StartActivity(intent);
					}
					//else
					//{
					//	AlertDialog.Builder alerta = new AlertDialog.Builder(this);
					//	alerta.SetTitle("Erro");
					//	alerta.SetMessage("Não foi possível efetuar o logout!");
					//	alerta.SetPositiveButton("fechar", (sender, e) => { });
					//}
				});
			}));
			t.Start();
		}
	}
}
