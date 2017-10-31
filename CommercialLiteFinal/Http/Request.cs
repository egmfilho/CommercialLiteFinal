using System;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace CommercialLiteFinal
{
	public class Request
	{
		private static Request _instance;

		public static Request GetInstance()
		{
			if (_instance == null)
			{
				_instance = new Request(Database.Producao);
			}

			return _instance;
		}

		public string Uri
		{
			get;
			set;
		}

		private Request(string uri)
		{
			Uri = uri;
		}

		public Response<T> Login<T>(string username, string password, string guid)
		{
			return Post<T>("login", "", "user_user=" + username + "&user_pass=" + password + "&device_guid=" + guid, "application/x-www-form-urlencoded", "");
		}

		public void Logout()
		{
			Post<Object>("logout", "", "", "application/x-www-form-urlencoded", "");
		}

		public Response<T> Post<T>(string module, string action, string token, params HttpParam[] arguments)
		{
			string data = "";

			HttpParam lastItem = arguments[arguments.Length - 1];
			foreach (HttpParam p in arguments)
			{
				data += p.ToString();
				if (p != lastItem)
				{
					data += "&";
				}
			}

			return Post<T>(module, action, data, "application/x-www-form-urlencoded", token);
		}

		public Response<T> Post<T>(string module, string action, Object obj, string token)
		{
			return Post<T>(module, action, JsonConvert.SerializeObject(obj), "application/json", token);
		}

		public Response<T> Post<T>(string module, string action, string data, string contentType, string token)
		{
			string uri = Uri;

			if (!string.IsNullOrEmpty(module))
				uri += module + ".php";

			if (!string.IsNullOrEmpty(action))
				uri += "?action=" + action;

			System.Diagnostics.Debug.WriteLine("[#] " + uri);

			HttpClient client = new HttpClient();
			client.DefaultRequestHeaders.Add("x-session-token", token);

			HttpRequestMessage req = new HttpRequestMessage(HttpMethod.Post, new Uri(uri));
			req.Content = new StringContent(data, System.Text.Encoding.UTF8, contentType);

			var res = client.SendAsync(req).Result;

			var j = res.Content.ReadAsStringAsync().Result;
			Response<T> obj = new Response<T>();

			try
			{
				obj = JsonConvert.DeserializeObject<Response<T>>(j);
			}
			catch (Exception e)
			{
#if DEBUG
				string error = e.Message;
				obj.error = error;
#endif
			}
#if DEBUG
			obj.debug = j;
#endif
			return obj;
		}
	}
}
