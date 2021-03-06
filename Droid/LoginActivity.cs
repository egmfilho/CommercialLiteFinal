﻿
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
	[Activity(Label = "LoginActivity", ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait)]
	public class LoginActivity : Activity
	{
		int magicCounter;
		System.Timers.Timer timer;

		protected override void OnCreate(Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);

			System.Diagnostics.Debug.WriteLine("[#] Login activity");

			// Set our view from the "main" layout resource
			SetContentView(Resource.Layout.Login);

			var xml = PreferenceManager.GetDefaultSharedPreferences(this).GetString("user", "");
			var storedUser = string.IsNullOrEmpty(xml) ? "" : Serializador.LoadFromXMLString<Usuario>(xml).Username;
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
			button.Click += delegate
			{
#if DEBUG
				Login("alessandro", "02091988");
#else
				Login(txtUsername.Text, txtPassword.Text);
#endif
			};
#if DEBUG
			button.Text = Request.GetInstance().Uri.Equals(Database.Teste) ? "Base de Teste" : "Base de Produção";
#endif
			magicCounter = 0;
			timer = new System.Timers.Timer();
			timer.Interval = 1000;
			timer.Elapsed += (sender, e) => { magicCounter = 0; timer.Enabled = false; };
			FindViewById<ImageView>(Resource.Id.logo).Click += (sender, e) =>
			{
				timer.Stop();
				magicCounter++;
				timer.Start();

				if (magicCounter == 20)
				{
					magicCounter = 0;
					string msg = "Device: " + Android.OS.Build.Device + "\n";
					msg += "Model: " + Android.OS.Build.Model + "\n";
					msg += "Brand: " + Android.OS.Build.Brand + "\n";
					msg += "GUID: " + PreferenceManager.GetDefaultSharedPreferences(this).GetString("guid", "") + "\n";
					var xmlAuth = PreferenceManager.GetDefaultSharedPreferences(this).GetString("authentication", "");
					var authorization = string.IsNullOrEmpty(xmlAuth) ? new Autenticacao() : Serializador.LoadFromXMLString<Autenticacao>(xmlAuth);
					msg += "Autorizado por: " + authorization.UserName + "\n";
					msg += "Autorizado em: " + authorization.Date.ToString("G") + "\n";
					msg += "Base: " + PreferenceManager.GetDefaultSharedPreferences(this).GetString("base", Database.Producao);

					AlertDialog.Builder alert = new AlertDialog.Builder(this);
					alert.SetTitle("Painel de controle");
					alert.SetCancelable(false);
					alert.SetMessage(msg);
					alert.SetPositiveButton("Resetar", (s, v) =>
					{
						var prompt = new AlertDialog.Builder(this);
						prompt.SetTitle("Confirmação");
						var input = new EditText(this);
						input.InputType = Android.Text.InputTypes.ClassText | Android.Text.InputTypes.TextVariationPassword;
						prompt.SetView(input);
						prompt.SetNegativeButton("Cancelar", (s2, e2) => { });
						prompt.SetPositiveButton("Ok", (s2, e2) =>
						{
							if (input.Text == "02091988")
							{
								var editor = PreferenceManager.GetDefaultSharedPreferences(this).Edit();
								editor.Clear();
								editor.Commit();
								txtUsername.Text = "";
								Toast.MakeText(this, "Master reset!", ToastLength.Short).Show();
							}
							else
							{
								Toast.MakeText(this, "Falha!", ToastLength.Short).Show();
							}
						});
						prompt.Show();
					});
					alert.SetNegativeButton("Fechar", (s, v) => { });
					alert.SetNeutralButton("Trocar base",(s, v) => 
					{
						Request.GetInstance().Uri = Request.GetInstance().Uri.Equals(Database.Producao) ? Database.Teste : Database.Producao;
						PreferenceManager.GetDefaultSharedPreferences(this).Edit().PutString("base", Request.GetInstance().Uri).Commit();

						FindViewById<Button>(Resource.Id.btnLogin).Text = Request.GetInstance().Uri.Equals(Database.Teste) ? "Base de Teste" : "Base de Produção"; ;
					});
					alert.Show();
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
				var guid = PreferenceManager.GetDefaultSharedPreferences(this).GetString("guid", "");
				Response<Usuario> res;

				try
				{					
					res = Request.GetInstance().Login<Usuario>(username, password, guid);
				}
				catch (Exception e)
				{
					res = new Response<Usuario>();
					res.status.code = -1;
					res.status.message = e.Message;
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
						if (string.IsNullOrEmpty(res.data.Vendedor.Id))
						{
							AlertDialog.Builder alert = new AlertDialog.Builder(this);
							alert.SetTitle("Erro");
							alert.SetMessage("Esta conta de usuário não possui nenhum vendedor vinculado.");
							alert.SetPositiveButton("Fechar", (sender, e) => { });
							alert.Create().Show();
							return;
						}

						Request.GetInstance().Uri = PreferenceManager.GetDefaultSharedPreferences(this).GetString("base", Database.Producao);
						var user = res.data;
						user.Token = res.data.Id + ":" + res.data.Sessao.Valor + ":" + guid;

						PreferenceManager.GetDefaultSharedPreferences(this).Edit().PutString("user", Serializador.ToXML(user)).Apply();
						PreferenceManager.GetDefaultSharedPreferences(this).Edit().PutString("shop", Serializador.ToXML(user.Lojas[0])).Apply();

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
