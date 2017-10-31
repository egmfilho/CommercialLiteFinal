using System;
namespace CommercialLiteFinal
{
	public abstract class HttpResponse
	{
		public Status status
		{
			get;
			set;
		}

		public Object info
		{
			get;
			set;
		}

#if DEBUG
		public string debug { get; set; }
		public string error { get; set; }
#endif
	}
}
