using System.Collections.Generic;
using System.Threading;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Preferences;
using Android.Widget;

namespace CommercialLiteFinal.Droid
{
	[Activity(Label = "CustomerSearchActivity")]
	public class CustomerSearchActivity : Activity
	{
		string token;

		List<Pessoa> arrayPessoas = new List<Pessoa>();
		ListView listView;
		SearchView searchView;

		protected override void OnCreate(Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);

			SetContentView(Resource.Layout.CustomerSearch);

			token = PreferenceManager.GetDefaultSharedPreferences(this).GetString("token", "");

			listView = FindViewById<ListView>(Resource.Id.listaClientes);
			listView.ItemClick += (object sender, AdapterView.ItemClickEventArgs e) => 
			{
				var pessoa = arrayPessoas[e.Position];

				if (!pessoa.Ativo)
				{
					AlertDialog.Builder alert = new AlertDialog.Builder(this);
					alert.SetTitle("Aviso");
					alert.SetMessage("Este cliente encontra-se inativo!");
					alert.SetPositiveButton("Fechar", (s, v) => { });
					alert.Show();
					return;
				}

				SendPerson(pessoa.Codigo);
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
			var progressDialog = ProgressDialog.Show(this, "Pesquisando", "Checando pessoas...", true);

			var t = new Thread(new ThreadStart(delegate
			{
				arrayPessoas = new List<Pessoa>();
				Status status;

				int codigo;
				if (int.TryParse(query, out codigo))
				{
					var res = Request.GetInstance().Post<Pessoa>("person", "get", token, new HttpParam("CdPessoa", query), new HttpParam("TpPessoa", "Cliente"));
					status = res.status;
					if (status.code == 401)
						StartActivity(new Intent(this, typeof(LogoutActivity)));
					else if (status.code == 200)
						arrayPessoas.Add(res.data);
				}
				else
				{
					var res = Request.GetInstance().Post<List<Pessoa>>("person", "getList", token, new HttpParam("NmPessoa", query), new HttpParam("TpPessoa", "Cliente"));
					status = res.status;
					if (status.code == 401)
						StartActivity(new Intent(this, typeof(LogoutActivity)));
					else if (status.code == 200)
						arrayPessoas = res.data;
				}

				RunOnUiThread(() =>
				{
					progressDialog.Hide();
					searchView.ClearFocus();

					listView.Adapter = new CustomerListAdapter(this, arrayPessoas);
					if (status.code != 200) 
						Toast.MakeText(this, status.description, ToastLength.Short).Show();
					else
						Toast.MakeText(this, arrayPessoas.Count + " pessoas encontradas!", ToastLength.Short).Show();
						
				});
			}));
			t.Start();
		}

		private void SendPerson(string code)
		{
			var progressDialog = ProgressDialog.Show(this, "Carregando", "Buscando informações da pessoa.", true);

			var t = new Thread(new ThreadStart(delegate
			{				
				var res = Request.GetInstance().Post<Pessoa>("person", "get", token, new HttpParam("CdPessoa", code), new HttpParam("TpPessoa", "Cliente"), new HttpParam("get_person_cep", "1"));
					var pessoa = res.data;

				RunOnUiThread(() =>
				{					
					progressDialog.Hide();

					if (res.status.code == 401)
						StartActivity(new Intent(this, typeof(LogoutActivity)));

					var intent = new Intent(this, typeof(OrderActivity));
					intent.AddFlags(ActivityFlags.ReorderToFront);
					intent.PutExtra("person", Serializador.ToXML(pessoa));
					StartActivity(intent);
				});
			}));
			t.Start();
		}
	}
}
