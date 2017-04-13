using System;

namespace CommercialLiteFinal
{
	public class Response<T>
	{
		public Status status
		{
			get;
			set;
		}

		public T data
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
#endif
	}
}
