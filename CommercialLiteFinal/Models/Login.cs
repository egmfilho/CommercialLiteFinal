using System;
namespace CommercialLiteFinal
{
	public class Login
	{
		public string user_current_session_id { get; set; }
		public int user_id { get; set; }
		public string user_shop_id { get; set; }
		public string user_price_id { get; set; }
		public string user_user { get; set;}
		public string user_name { get; set; }
		public Decimal user_max_discount { get; set; }
		public Loja user_shop { get; set; }
		public Preco user_price { get; set; }
		public Vendedor user_seller { get; set; }
	}
}
