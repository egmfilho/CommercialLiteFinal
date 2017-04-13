using Android.App;
using Android.OS;
using Android.Content;
using Android.Preferences;
using Android.Widget;
using System.Threading;

namespace CommercialLiteFinal.Droid
{
	[Activity(Label = "Commercial Lite", MainLauncher = true, Icon = "@mipmap/icon", ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait)]
	public class MainActivity : Activity
	{
		protected override void OnCreate(Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);

			// Set our view from the "main" layout resource
			SetContentView(Resource.Layout.Main);

			Request.GetInstance().Uri = PreferenceManager.GetDefaultSharedPreferences(this).GetString("base", Database.Producao);

			Redirect();
		}

		private async void Redirect()
		{
			var intent = new Intent();
			intent.AddFlags(ActivityFlags.ClearTop | ActivityFlags.NewTask);

			var guid = PreferenceManager.GetDefaultSharedPreferences(this).GetString("guid", "vazio");

			if (guid.Equals("vazio"))
			{
				await System.Threading.Tasks.Task.Delay(1500);

				intent.SetClass(this, typeof(UnlockActivity));
				StartActivity(intent);
				Finish();
			}
			else
			{
				var t = new Thread(new ThreadStart(delegate
				{
					var res = Request.GetInstance().Post<System.Object>("authentication", "validate", "", new HttpParam("device_guid", guid));
					RunOnUiThread(() =>
					{
						if (res.status == null)
						{
#if DEBUG
							AlertDialog.Builder alerta = new AlertDialog.Builder(this);
							alerta.SetTitle("Debug");
							alerta.SetMessage(res.debug);
							alerta.SetPositiveButton("Fechar", (sender, e) => { Finish(); });
							alerta.Show();
							return;
#else
							AlertDialog.Builder alerta = new AlertDialog.Builder(this);
							alerta.SetTitle("Aviso");
							alerta.SetMessage("Erro no servidor. O Aplicativo será fechado, tente novamente mais tarde.");
							alerta.SetPositiveButton("Fechar", (sender, e) => { Finish(); });
							alerta.Show();
							return;
#endif
						}

						if (res.status.code == 200)
						{
							intent.SetClass(this, typeof(LoginActivity));		
						}
						else
						{
							var editor = PreferenceManager.GetDefaultSharedPreferences(this).Edit();
							editor.Remove("guid");
							editor.Remove("authentication");
							editor.Commit();

							intent.SetClass(this, typeof(UnlockActivity));
						}

						StartActivity(intent);
						Finish();
					});
				}));
				t.Start();
			}
		}
	}
}

