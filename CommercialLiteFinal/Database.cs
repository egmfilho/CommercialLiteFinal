using System;
namespace CommercialLiteFinal
{
	public static class Database
	{
		public static string Teste 
		{
			get 
			{
				//return "http://172.16.0.82/commercial.dafel/php/mobile/";
				return "http://172.16.0.176/commercial2.api/lite/";
			}
		}

		public static string Producao
		{
			get
			{
				//return "http://138.204.201.14:8181/commercial/php/mobile/";
				//return "http://commercial.com.br/php/mobile/";
				return "http://172.16.0.176/commercial2.api/lite/";
			}
		}
	}
}
