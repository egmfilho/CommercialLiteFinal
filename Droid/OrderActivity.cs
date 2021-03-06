﻿using System;
using System.Globalization;
using System.Linq;
using System.Threading;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Preferences;
using Android.Widget;

namespace CommercialLiteFinal.Droid
{
	[Activity(Label = "OrderActivity", ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait)]
	public class OrderActivity : Activity
	{
		Usuario user;
		Loja shop;

		Pedido pedido;
		TextView lblItens;
		TextView lblTotal;

		protected override void OnCreate(Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);

			user = Serializador.LoadFromXMLString<Usuario>(PreferenceManager.GetDefaultSharedPreferences(this).GetString("user", ""));
			shop = Serializador.LoadFromXMLString<Loja>(PreferenceManager.GetDefaultSharedPreferences(this).GetString("shop", ""));

			pedido = new Pedido(user.Id, user.Vendedor.Id, shop.ERP.Codigo);

			// Create your application here
			SetContentView(Resource.Layout.Order);

			lblItens = FindViewById<TextView>(Resource.Id.lblItens);
			lblTotal = FindViewById<TextView>(Resource.Id.lblTotal);
			UpdateScreen();

			Toolbar toolbar = FindViewById<Toolbar>(Resource.Id.toolbar);
			toolbar.InflateMenu(Resource.Menu.order_options_menu);
			toolbar.FindViewById<TextView>(Resource.Id.home).SetTextColor(Android.Graphics.Color.White);
			toolbar.FindViewById<TextView>(Resource.Id.novo_pedido).SetTextColor(Android.Graphics.Color.White);

			toolbar.MenuItemClick += (sender, e) =>
			{
				switch (e.Item.ItemId)
				{
					case Resource.Id.home:
						AlertDialog.Builder alert = new AlertDialog.Builder(this);
						alert.SetTitle("Início");
						alert.SetMessage("Todas as alterações serão perdidas. Deseja continuar?");
						alert.SetPositiveButton("Sim", (s, v) =>
						{
							var i = new Intent(this, typeof(HomeActivity));
							i.AddFlags(ActivityFlags.ClearTop | ActivityFlags.NewTask);
							StartActivity(i);
						});
						alert.SetNegativeButton("Não", (s, v) => { });
						alert.Create().Show();
						return;

					case Resource.Id.novo_pedido:
						New();
						return;

					case Resource.Id.salvar_pedido:
						Export();
						return;

					case Resource.Id.add_produto:
						SearchProduct();
						return;

					case Resource.Id.logout:
						StartActivity(new Intent(this, typeof(LogoutActivity)));
						return;

					default:
						return;
				}
			};

			FindViewById<TextView>(Resource.Id.lblVendedor).Text = user.Vendedor.Nome;
			FindViewById<TextView>(Resource.Id.lblLoja).Text = string.Format("{0} - {1}", shop.ERP.Codigo, shop.ERP.Nome);

			FindViewById<Button>(Resource.Id.btnVerItens).Click += (sender, e) =>
			{
				ViewItems();
			};

			FindViewById<Button>(Resource.Id.btnAddItem).Click += (sender, e) => { SearchProduct(); };

			SetAddCustomerBtn();

			FindViewById<EditText>(Resource.Id.txtObs).TextChanged += (sender, e) =>
			{
				pedido.Observacoes = e.Text.ToString();
			};

			FindViewById<Button>(Resource.Id.btnFinalizar).Click += (sender, e) =>
			{
				Export();
			};
		}

		protected override void OnNewIntent(Intent intent)
		{
			base.OnNewIntent(intent);

			var s = intent.GetStringExtra("orderItem");
			var action = intent.GetStringExtra("action");

			var p = intent.GetStringExtra("person");

			// Adiciona, edita e remove Itens
			if (!string.IsNullOrEmpty(s) && !string.IsNullOrEmpty(action))
			{
				var item = Serializador.LoadFromXMLString<ItemPedido>(s);

				switch (action)
				{
					case "new":
						if (pedido.Items.FindIndex((x) => x.Produto.IdProduto == item.Produto.IdProduto) == -1)
						{
							if (item.IdPreco == null)
								item.IdPreco = item.Produto.Precos.ElementAt(0).Id;
							
							pedido.Items.Add(item);
							Toast.MakeText(this, "Item adicionado!", ToastLength.Short).Show();
						}
						else
						{
							AlertDialog.Builder alerta = new AlertDialog.Builder(this);
							alerta.SetTitle("Aviso");
							alerta.SetMessage("Não foi possível adicionar o produto pois o mesmo já se encontra neste orçamento.");
							alerta.SetNegativeButton("Ver Items", (sender, e) => { ViewItems(); });
							alerta.SetPositiveButton("Abrir busca", (sender, e) => { SearchProduct(); });
							alerta.SetNeutralButton("Fechar", (sender, e) => { });
							alerta.Show();
						}
						break;
					case "update":
						pedido.Items[pedido.Items.FindIndex(x => x.Produto.IdProduto == item.Produto.IdProduto)] = item;
						Toast.MakeText(this, "Item atualizado!", ToastLength.Short).Show();
						break;
					case "remove":
						pedido.Items.RemoveAt(pedido.Items.FindIndex(x => x.Produto.IdProduto == item.Produto.IdProduto));
						Toast.MakeText(this, "Item removido!", ToastLength.Short).Show();
						break;
				}

				UpdateScreen();
			}

			// Adiciona o Cliente
			if (!string.IsNullOrEmpty(p))
			{
				pedido.Cliente = new Pessoa();
				pedido.Cliente = Serializador.LoadFromXMLString<Pessoa>(p);
				Toast.MakeText(this, "Cliente adicionado!", ToastLength.Short).Show();
				FindViewById<TextView>(Resource.Id.lblCliente).Text = string.Format("{0} - {1}", pedido.Cliente.Codigo, pedido.Cliente.Nome);
				if (pedido.Cliente.Enderecos.Count == 0)
				{
					FindViewById<TextView>(Resource.Id.lblEndereco).Text = "Nenhum endereço cadastrado";
				}
				else
				{
					var display = pedido.Cliente.Enderecos.Select<Endereco, String>(e => e.ToString()).ToList();
					var adapter = new ArrayAdapter<String>(this, Android.Resource.Layout.SimpleListItemSingleChoice, display);
					int index = 0;
					AlertDialog.Builder alerta = new AlertDialog.Builder(this);
					alerta.SetTitle("Endereço de entrega");
					alerta.SetSingleChoiceItems(adapter, index, (sender, e) => {
						index = e.Which;
					});
					alerta.SetPositiveButton("Confirmar", (sender, e) =>
					{
						pedido.Entrega = pedido.Cliente.Enderecos.ElementAt(index);
						FindViewById<TextView>(Resource.Id.lblEndereco).Text = pedido.Entrega.ToString();
					});
					alerta.Show();
				}
				FindViewById<Button>(Resource.Id.btnAddCliente).Text = Resources.GetString(Resource.String.remove_customer);
			}
		}

		public void Alert(string title, string message, string button)
		{
			if (!Validate())
				return;
			
			AlertDialog.Builder alert = new AlertDialog.Builder(this);
			alert.SetTitle(title);
			alert.SetMessage(message);
			alert.SetPositiveButton(button, (sender, e) =>
			{

			});
			alert.Create().Show();
		}

		private void New()
		{
			AlertDialog.Builder alert = new AlertDialog.Builder(this);
			alert.SetTitle("Aviso!");
			alert.SetMessage("Descartar as alterações e começar um novo orçamento?");
			alert.SetPositiveButton("Sim", (sender, e) =>
			{
				pedido = new Pedido(user.Id, user.Vendedor.Id, shop.Id);
				Toast.MakeText(this, "Novo Orçamento!", ToastLength.Short).Show();
			});
			alert.SetNegativeButton("Não", (sender, e) => { });
			alert.Create().Show();
		}

		private void ViewItems()
		{
			if (pedido.Items.Count == 0)
			{
				Alert("Aviso", "Nenhum produto adicionado!", "Ok");
				return;
			}

			Intent intent = new Intent(this, typeof(OrderItemsActivity));
			intent.PutExtra("itemList", Serializador.ToXML(pedido.Items));
			StartActivity(intent);
		}

		private void SearchProduct()
		{
			var intent = new Intent(this, typeof(ProductSearchActivity));
			intent.AddFlags(ActivityFlags.ReorderToFront);
			StartActivity(intent);
		}

		private void UpdateScreen()
		{
			lblItens.Text = string.Format("{0} produto{1}", pedido.Items.Count, pedido.Items.Count == 1 ? "" : "s");
			lblTotal.Text = string.Format(CultureInfo.GetCultureInfo("pt-BR"), "{0:C}", pedido.ValorTotalComDesconto);
		}

		private void ClearScreen()
		{
			lblItens.Text = string.Format("{0} produto{1}", pedido.Items.Count, pedido.Items.Count == 1 ? "" : "s");
			lblTotal.Text = string.Format(CultureInfo.GetCultureInfo("pt-BR"), "{0:C}", pedido.ValorTotalComDesconto);
			FindViewById<EditText>(Resource.Id.txtObs).Text = "";
			FindViewById<Button>(Resource.Id.btnAddCliente).Text = Resources.GetString(Resource.String.add_customer);
			FindViewById<TextView>(Resource.Id.lblCliente).Text = "";
			FindViewById<TextView>(Resource.Id.lblEndereco).Text = "";
		}

		private void SetAddCustomerBtn()
		{
			var btn = FindViewById<Button>(Resource.Id.btnAddCliente);
			btn.Click += (sender, e) =>
			{
				if (pedido.Cliente == null || pedido.Cliente.Id == null)
					StartActivity(new Intent(this, typeof(CustomerSearchActivity)));
				else
				{
					pedido.Cliente = new Pessoa();
					FindViewById<TextView>(Resource.Id.lblCliente).Text = "Informe um cliente";
					FindViewById<TextView>(Resource.Id.lblEndereco).Text = "Nenhum cliente informado";
					btn.Text = Resources.GetString(Resource.String.add_customer);
					Toast.MakeText(this, "Cliente removido!", ToastLength.Short).Show();
				}
			};
			btn.Text = Resources.GetString(Resource.String.add_customer);
		}

		private bool Validate()
		{
			if (pedido.Items.Count == 0)
			{
				Toast.MakeText(this, "Nenhum item informado!", ToastLength.Short).Show();
				return false;
			}

			if (string.IsNullOrEmpty(pedido.IdCliente))
			{
				Toast.MakeText(this, "Informe um cliente!", ToastLength.Short).Show();
				return false;
			}

			return true;
		}

		private void Export()
		{
			AlertDialog.Builder alerta = new AlertDialog.Builder(this);
			alerta.SetTitle("Finalizar");
			alerta.SetMessage("Deseja salvar o orçamento na base de dados do Commercial?");
			alerta.SetPositiveButton("Salvar", (s, v) => { DoExport(); });
			alerta.SetNegativeButton("Cancelar", (s, v) => { });
			alerta.Show();
		}

		private void DoExport()
		{
			if (!Validate())
				return;
			
			var progressDialog = ProgressDialog.Show(this, "Exportando", "Enviando informações...", true);

			var t = new Thread(new ThreadStart(delegate
			{				
				var res = Request.GetInstance().Post<Pedido>("order", "insert", pedido, user.Token);
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

					if (res.status.code == 401)
					{
						Toast.MakeText(this, res.status.description, ToastLength.Short).Show();
						StartActivity(new Intent(this, typeof(LogoutActivity)));
					}
					else if (res.status.code == 200)
					{
						pedido = new Pedido(user.Id, user.Vendedor.Id, shop.Id);
						ClearScreen();
						AlertDialog.Builder alerta = new AlertDialog.Builder(this);
						alerta.SetTitle("Exportado");
						var msg = string.Format("O orçamento foi exportado com sucesso! \nCódigo: {0:000000}", res.data.Codigo);
						alerta.SetMessage(msg);
						alerta.SetPositiveButton("Novo", (sender, e) => { });
						alerta.SetNegativeButton("Início", (sender, e) => 
						{
							StartActivity(new Intent(this, typeof(HomeActivity)));
						});
						alerta.Show();
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
