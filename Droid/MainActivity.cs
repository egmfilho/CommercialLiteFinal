using Android.App;
using Android.Widget;
using Android.OS;
using Android.Content;
using Android.Preferences;
using System;
using System.Threading;
using Android.Net;

namespace CommercialLite.Droid
{
	[Activity(Label = "Commercial Lite", MainLauncher = true, Icon = "@mipmap/icon")]
	public class MainActivity : Activity
	{
		int magicCounter;
		protected override void OnCreate(Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);

			// Set our view from the "main" layout resource
			SetContentView(Resource.Layout.Main);

			var storedUser = PreferenceManager.GetDefaultSharedPreferences(this).GetString("username", "");
			if (!string.IsNullOrEmpty(storedUser))
			{
				FindViewById<EditText>(Resource.Id.txtUsername).Text = storedUser;
			}

			var txtUsername = FindViewById<EditText>(Resource.Id.txtUsername);
			var txtPassword = FindViewById<EditText>(Resource.Id.txtPassword);
			txtPassword.EditorAction += (sender, e) =>
			{
				e.Handled = false;
				if (e.ActionId == Android.Views.InputMethods.ImeAction.Done || 
				    e.ActionId == Android.Views.InputMethods.ImeAction.Go)
				{
					Login(txtUsername.Text, txtPassword.Text);
					e.Handled = true;
				}
			};

			Button button = FindViewById<Button>(Resource.Id.btnLogin);
			button.Click += delegate {				
				//Login("alessandro", "02091988");
				Login(txtUsername.Text, txtPassword.Text);
			};

			magicCounter = 0;
			FindViewById<ImageView>(Resource.Id.logo).Click += (sender, e) => 
			{
				magicCounter++;
				if (magicCounter == 20)
				{
					var editor = PreferenceManager.GetDefaultSharedPreferences(this).Edit();
					editor.Clear();
					editor.Commit();
					txtUsername.Text = "";
					Toast.MakeText(this, "Master reset!", ToastLength.Short).Show();
				}
			};
		}

		private void Login(string username, string password)
		{
			if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
			{
				Toast.MakeText(this, "Preencha todos os campos!", ToastLength.Short).Show();
				return;
			}

			if (!IsOnline())
			{
				Toast.MakeText(this, "Verifique sua conexão com a internet!", ToastLength.Short).Show();
				return;
			}

			var progressDialog = ProgressDialog.Show(this, "Conectando", "Efetuando login...", true);

			var t = new Thread(new ThreadStart(delegate
			{
				Response<Login> res;
				try
				{
					res = Request.GetInstance().Login<Login>(username, password);	
				}
				catch(Exception e)
				{
					res = new Response<Login>();
					res.status.code = -1;
					res.status.message = e.Message;
				}


				RunOnUiThread(() =>
				{
					progressDialog.Hide();

					if (res.status.code == 200)
					{

						if (string.IsNullOrEmpty(res.data.user_seller.Id))
						{
							AlertDialog.Builder alert = new AlertDialog.Builder(this);
							alert.SetTitle("Erro");
							alert.SetMessage("Esta conta de usuário não possui nenhum vendedor vinculado.");
							alert.SetPositiveButton("Fechar", (sender, e) => { });
							alert.Create().Show();
							return;
						}

						ISharedPreferences prefs = PreferenceManager.GetDefaultSharedPreferences(this);
						ISharedPreferencesEditor editor = prefs.Edit();
						editor.PutString("username", res.data.user_user);
						editor.PutInt("userId", res.data.user_id);
						editor.PutString("token", res.data.user_current_session_id + ":" + res.data.user_id);
						editor.PutString("employeeId", res.data.user_seller.Id);
						editor.PutString("employeeName", res.data.user_seller.Nome);
						editor.PutFloat("maxDiscount", (float)res.data.user_max_discount);
						editor.PutString("priceId", res.data.user_price_id);
						editor.PutString("priceName", res.data.user_price.price_name);
						editor.PutString("shopId", res.data.user_shop_id);
						editor.PutString("shopName", res.data.user_shop.shop_name);
						editor.Apply();
						Toast.MakeText(this, "Login efetuado!", ToastLength.Short).Show();
						StartActivity(new Intent(this, typeof(HomeActivity)));
					}
					else
					{
						Toast.MakeText(this, res.status.description, ToastLength.Short).Show();
					}
					
				});
			}));
			t.Start();
		}

		private bool IsOnline()
		{
			ConnectivityManager cm = (ConnectivityManager)GetSystemService(Context.ConnectivityService);
			NetworkInfo netInfo = cm.ActiveNetworkInfo;
			return netInfo != null && netInfo.IsConnectedOrConnecting;		
		}
	}
}

