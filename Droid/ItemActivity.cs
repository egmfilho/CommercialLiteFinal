using System.Collections.Generic;
using System.Globalization;
using System.Threading;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Preferences;
using Android.Widget;

namespace CommercialLite.Droid
{
	[Activity(Label = "ItemActivity")]
	public class ItemActivity : Activity
	{
		string token;

		ItemPedido item = new ItemPedido();
		string action;
		decimal maxDiscount;

		protected override void OnCreate(Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);

			// Create your application here
			SetContentView(Resource.Layout.Item);

			token = PreferenceManager.GetDefaultSharedPreferences(this).GetString("token", "");

			action = this.Intent.GetStringExtra("action");
			maxDiscount = (decimal)PreferenceManager.GetDefaultSharedPreferences(this).GetFloat("maxDiscount", 0);

			Toolbar toolbar = FindViewById<Toolbar>(Resource.Id.toolbar);
			toolbar.InflateMenu(Resource.Menu.item_options_menu);

			var addItem = toolbar.FindViewById<TextView>(Resource.Id.adicionar_produto);
			addItem.SetTextColor(Android.Graphics.Color.White);
			addItem.Text = action.Equals("new") ? "Adicionar" : "Editar";

			var removeItem = toolbar.FindViewById<TextView>(Resource.Id.remover_produto);
			removeItem.SetTextColor(Android.Graphics.Color.White);
			removeItem.Visibility = action.Equals("update") ? Android.Views.ViewStates.Visible : Android.Views.ViewStates.Gone;

			toolbar.MenuItemClick += (sender, e) =>
			{
				switch (e.Item.ItemId)
				{
					case Resource.Id.adicionar_produto:
						Confirm();
						return;

					case Resource.Id.remover_produto:
						AlertDialog.Builder alert = new AlertDialog.Builder(this);
						alert.SetTitle("Aviso!");
						alert.SetMessage("Deseja remover o item do orçamento?");
						alert.SetPositiveButton("Sim", (s, v) => 
						{
							action = "remove";
							Confirm();	
						});
						alert.SetNegativeButton("Não",(s, v) => { });
						alert.Show();
						return;

					default:
						return;
				}
			};

			item = new ItemPedido();
			Load();

			var qtd = FindViewById<EditText>(Resource.Id.txtQuantidade);
			var total = FindViewById<TextView>(Resource.Id.lblValorTotal);

			qtd.ClearFocus();
			qtd.Text = item.Quantidade.ToString("0.00");
			qtd.TextChanged += (object sender, Android.Text.TextChangedEventArgs e) => 
			{
				int x = 1;
				int.TryParse(e.Text.ToString(), out x);
				item.Quantidade = x;
				total.Text = string.Format(CultureInfo.GetCultureInfo("pt-BR"), "{0:C}", item.ValorTotal);
				UpdateScreen();
			};

			FindViewById<Button>(Resource.Id.btnDecrement).Click += (object sender, System.EventArgs e) => 
			{
				item.Quantidade -= 1;
				item.Quantidade = item.Quantidade < 0 ? 0 : item.Quantidade;
				qtd.Text = item.Quantidade.ToString();
				qtd.ClearFocus();
				UpdateScreen();
			};

			FindViewById<Button>(Resource.Id.btnIncrement).Click += (object sender, System.EventArgs e) =>
			{
				item.Quantidade += 1;
				qtd.Text = item.Quantidade.ToString();
				qtd.ClearFocus();
				UpdateScreen();
			};

			var txtAl = FindViewById<Input>(Resource.Id.txtAliquota);
			var txtVl = FindViewById<Input>(Resource.Id.txtVlDesc);

			txtAl.EditorAction += (sender, e) => 
			{
				e.Handled = false;
				if (e.ActionId == Android.Views.InputMethods.ImeAction.Done)
				{
					decimal desconto;
					if (!decimal.TryParse(txtAl.Text, out desconto))
						desconto = 0;

					if (desconto > maxDiscount)
					{
						Toast.MakeText(this, string.Format("Desconto máximo permitido: {0:#.##}%", maxDiscount), ToastLength.Short).Show();
					}
					else
					{
						txtAl.ClearFocus();
						item.SetDescontoPercent(desconto);
						UpdateScreen();	
					}

					e.Handled = true;
				}
			};

			txtAl.FocusChange += (sender, e) => 
			{
				if (!e.HasFocus)
					txtAl.Text = item.DescontoPercent.ToString("F");				
			};

			txtVl.EditorAction += (sender, e) => 
			{
				e.Handled = false;
				if (e.ActionId == Android.Views.InputMethods.ImeAction.Done)
				{
					var maxValue = item.ValorTotal * (maxDiscount / 100);
					decimal desconto;
					if (!decimal.TryParse(txtVl.Text, out desconto))
						desconto = 0;

					if (desconto > maxValue)
					{
						var msg = string.Format(CultureInfo.GetCultureInfo("pt-BR"), "Desconto máximo para este item: {0:C}", maxValue);
						Toast.MakeText(this, msg, ToastLength.Short).Show();
					}
					else
					{
						txtVl.ClearFocus();
						item.SetDescontoDinheiro(desconto);
						UpdateScreen();	
					}

					e.Handled = true;
				}
			};



			txtVl.FocusChange += (sender, e) =>
			{
				if (!e.HasFocus)
					txtVl.Text = item.DescontoDinheiro.ToString("F");
			};

			var btnAdd = FindViewById<Button>(Resource.Id.btnAddItem);
			btnAdd.Click += (sender, e) => { Confirm(); };
			btnAdd.Text = action.Equals("new") ? "Adicionar" : "Editar";
		}

		private void Load()
		{			
			if (!string.IsNullOrEmpty(action))
			{
				switch (action)
				{
					case "new":
						var progressDialog = ProgressDialog.Show(this, "Carregando", "Buscando informações do produto.", true);

						var t = new Thread(new ThreadStart(delegate
						{
							var c = this.Intent.GetStringExtra("productCode");
							item.SetProduto(Request.GetInstance().Post<Produto>("product", "get", token, new HttpParam("CdProduto", c), new HttpParam("price_id", "00A0000001")).data);

							RunOnUiThread(() =>
							{
								progressDialog.Hide();

								if (item.Produto.VlPreco == 0)
								{									
									Toast.MakeText(this, "O produto não possui preço cadastrado", ToastLength.Long).Show();
									Finish();
									return;
								}

								UpdateScreen();
							});
						}));
						t.Start();
						break;

					case "update":						
						item = Serializador.LoadFromXMLString<ItemPedido>(this.Intent.GetStringExtra("item"));
						UpdateScreen();
						break;
				}
			}
			else
			{
				Finish();
			}
		}

		private void UpdateScreen()
		{
			FindViewById<TextView>(Resource.Id.lblCodigo).Text = "Código: " + item.Produto.CdProduto;
			FindViewById<TextView>(Resource.Id.lblNome).Text = item.Produto.NmProduto;
			FindViewById<TextView>(Resource.Id.lblEAN).Text = "EAN: " + item.Produto.EAN;
			FindViewById<TextView>(Resource.Id.lblUnidade).Text = "Unidade: " + item.Produto.Unidade;
			string dataPreco = string.Format(CultureInfo.GetCultureInfo("pt-BR"), "Data do preço: {0:d}", item.Produto.DtPreco);
			FindViewById<TextView>(Resource.Id.lblDataPreco).Text = dataPreco;
			FindViewById<TextView>(Resource.Id.lblValor).Text = string.Format(CultureInfo.GetCultureInfo("pt-BR"), "Valor unitário {0:C}", item.Produto.VlPreco);
			FindViewById<TextView>(Resource.Id.lblQtdEstoque).Text = "Quantidade em estoque: " + item.Produto.QtEstoque.ToString();
			FindViewById<TextView>(Resource.Id.lblValorTotal).Text = string.Format(CultureInfo.GetCultureInfo("pt-BR"), "{0:C}", item.ValorTotal);
			FindViewById<EditText>(Resource.Id.txtAliquota).Text = item.DescontoPercent == 0 ? "" : item.DescontoPercent.ToString("0.00");
			FindViewById<EditText>(Resource.Id.txtVlDesc).Text = item.DescontoDinheiro == 0 ? "" : item.DescontoDinheiro.ToString("0.00");
			FindViewById<TextView>(Resource.Id.lblDesconto).Text = string.Format(CultureInfo.GetCultureInfo("pt-BR"), "Valor total: {0:C}", item.ValorTotalComDesconto);
		}

		private void Confirm()
		{
			string s = Serializador.ToXML<ItemPedido>(item);

			var intent = new Intent(this, typeof(OrderActivity));
			intent.AddFlags(ActivityFlags.ReorderToFront);
			intent.PutExtra("orderItem", s);
			intent.PutExtra("action", action);

			StartActivity(intent);
		}
	}
}
