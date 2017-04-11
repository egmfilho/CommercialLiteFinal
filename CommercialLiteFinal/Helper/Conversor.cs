using System;
namespace CommercialLiteFinal
{
	public static class Conversor
	{
		public static decimal StringToDecimal(string val, decimal fallback)
		{
			decimal result;

			try
			{
				result = string.IsNullOrEmpty(val) ? fallback : System.Decimal.Parse(val.Replace(',', '.'), System.Globalization.CultureInfo.InvariantCulture);
			}
			catch
			{
				result = fallback;
			}

			return result;
		}
	}
}
