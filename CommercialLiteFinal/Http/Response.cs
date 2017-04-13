using System;

namespace CommercialLiteFinal
{
	public class Response<T> : HttpResponse
	{
		public T data
		{
			get;
			set;
		}
	}
}
