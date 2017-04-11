
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Android.App;
using Android.Content;
using Android.Net;
using Android.OS;
using Android.Preferences;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace CommercialLiteFinal.Droid
{
	[Activity(Label = "UnlockActivity", ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait)]
	public class UnlockActivity : Activity
	{
		string guid;

		protected override void OnCreate(Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);

			GetId();
			// Create your application here
			SetContentView(Resource.Layout.Unlock);

			FindViewById<Button>(Resource.Id.btnDesbloquear).Click += (sender, e) =>
			{
				AlertDialog.Builder alert = new AlertDialog.Builder(this);
				alert.SetTitle("Credencial");

				var layout = new LinearLayout(this);
				layout.Orientation = Orientation.Vertical;
				var layoutParams = new LinearLayout.LayoutParams(ViewGroup.LayoutParams.MatchParent, ViewGroup.LayoutParams.WrapContent);
				layoutParams.SetMargins(15, 20, 15, 0);

				var txtUser = new TextView(this);
				txtUser.Text = "Usuário";
				layout.AddView(txtUser);

				var user = new EditText(this);
				user.InputType = Android.Text.InputTypes.ClassText | Android.Text.InputTypes.TextVariationNormal;
				layout.AddView(user);

				var txtPass = new TextView(this);
				txtPass.Text = "Senha";
				layout.AddView(txtPass);

				var pass = new EditText(this);
				pass.InputType = Android.Text.InputTypes.ClassText | Android.Text.InputTypes.TextVariationPassword;
				layout.AddView(pass);

				var container = new LinearLayout(this);
				container.Orientation = Orientation.Vertical;
				container.AddView(layout, layoutParams);

				alert.SetView(container);
				alert.SetPositiveButton("Autorizar", (s, v) => { Authenticate(user.Text, pass.Text); });
				alert.SetNegativeButton("Fechar", (s, v) => { });

				alert.Show();
			};
		}

		private void GetId()
		{
			var id = Guid.NewGuid();

			while (id == Guid.Empty)
			{
				id = Guid.NewGuid();
			}

			guid = id.ToString();
		}

		private HttpParam[] GetData()
		{
			HttpParam[] vet =
			{
				new HttpParam("device_guid", guid),
				new HttpParam("device_device", Build.Device),
				new HttpParam("device_model", Build.Model),
				new HttpParam("device_brand", Build.Brand)
			};

			return vet;
		}

		private bool IsOnline()
		{
			ConnectivityManager cm = (ConnectivityManager)GetSystemService(Context.ConnectivityService);
			NetworkInfo netInfo = cm.ActiveNetworkInfo;
			return netInfo != null && netInfo.IsConnectedOrConnecting;
		}

		private void Authenticate(string user, string pass)
		{
			if (!IsOnline())
			{
				Toast.MakeText(this, "Verifique a conexão com a internet", ToastLength.Long).Show();
				return;
			}

			var login = new HttpParam[] { new HttpParam("user_user", user), new HttpParam("user_pass", pass) };
			var deviceData = GetData();
			var data = new HttpParam[login.Length + deviceData.Length];
			login.CopyTo(data, 0);
			deviceData.CopyTo(data, login.Length);

			var progressDialog = ProgressDialog.Show(this, "Aguarde", "Autenticando aparelho...", true);
			var t = new Thread(new ThreadStart(delegate
			{
				var res = Request.GetInstance().Post<Autenticacao>("authentication", "register", "", data);
				RunOnUiThread(() =>
				{
					progressDialog.Hide();
					if (res.status.code == 200)
					{
						var editor = PreferenceManager.GetDefaultSharedPreferences(this).Edit();
						editor.PutString("guid", guid);
						editor.PutString("authentication", Serializador.ToXML(res.data));
						editor.Apply();
						var intent = new Intent(this, typeof(LoginActivity));
						intent.AddFlags(ActivityFlags.ClearTop | ActivityFlags.NewTask);
						StartActivity(intent);
						Finish();
					}
					else //420 = ja cadastrado
					{
						AlertDialog.Builder alerta = new AlertDialog.Builder(this);
						alerta.SetTitle("Erro");
						alerta.SetMessage("Não foi possível autenticar o aparelho!");
						alerta.SetPositiveButton("Fechar", (sender, e) => { });
						alerta.Show();
						GetId();
					}
				});
			}));
			t.Start();
		}
	}
}
