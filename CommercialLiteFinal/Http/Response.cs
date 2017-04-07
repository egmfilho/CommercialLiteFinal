using System;

namespace CommercialLite
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
	}
}
