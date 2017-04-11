
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
	[Activity(Label = "HomeActivity", ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait)]
	public class HomeActivity : Activity
	{
		protected override void OnCreate(Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);

			// Create your application here
			SetContentView(Resource.Layout.Home);

			FindViewById<ImageButton>(Resource.Id.imgBtnNovo).Click += (sender, e) => 
			{
				StartActivity(new Intent(this, typeof(OrderActivity)));	
			};

			FindViewById<ImageButton>(Resource.Id.imgBtnSalvos).Click += (sender, e) =>
			{
				Toast.MakeText(this, "Em breve", ToastLength.Short).Show();
			};

			FindViewById<ImageButton>(Resource.Id.imgBtnExportados).Click += (sender, e) =>
			{
				StartActivity(new Intent(this, typeof(ExportListActivity)));
			};

			FindViewById<ImageButton>(Resource.Id.imgBtnSair).Click += (sender, e) =>
			{
				AlertDialog.Builder alert = new AlertDialog.Builder(this);
				alert.SetTitle("Sair");
				alert.SetMessage("Deseja efetuar o logout?");
				alert.SetPositiveButton("Sim", (s, v) =>
				{
					StartActivity(new Intent(this, typeof(LogoutActivity)));
				});
				alert.SetNegativeButton("Não", (s, v) => { });
				alert.Create().Show();
			};

			FindViewById<TextView>(Resource.Id.lblVendedor).Text = PreferenceManager.GetDefaultSharedPreferences(this).GetString("employeeName", "");
			FindViewById<TextView>(Resource.Id.lblVersao).Text = "versão 0.9";
			if (Request.GetInstance().Uri.Equals(Database.Teste))
				FindViewById<TextView>(Resource.Id.lblVersao).Text += " BASE TESTE";
		}
	}
}
