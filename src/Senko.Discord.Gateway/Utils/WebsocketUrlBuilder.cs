using System.Collections.Generic;
using System.Linq;

namespace Senko.Discord.Gateway.Utils
{
	public static class DictionaryUtils
	{
		public static void AddOrUpdate<TKey, TValue>(this Dictionary<TKey, TValue> dict, TKey key, TValue value)
		{
			if (dict.ContainsKey(key))
			{
				dict[key] = value;
			}
			else
			{
				dict.Add(key, value);
			}
		}

		public static bool TryRemove<TKey, TValue>(this Dictionary<TKey, TValue> dict, TKey key)
		{
			if (dict.ContainsKey(key))
			{
				dict.Remove(key);
				return true;
			}
			return false;
		}
	}

	public class WebSocketUrlBuilder
	{
		private readonly string _url;
		private readonly Dictionary<string, object> _arguments = new Dictionary<string, object>
        {
            ["v"] = 6,
            ["encoding"] = "json"
        };

		public WebSocketUrlBuilder(string baseUrl)
		{
			_url = baseUrl;
		}

		public WebSocketUrlBuilder SetVersion(int version)
		{
			_arguments.AddOrUpdate("v", version);
			return this;
		}

		public WebSocketUrlBuilder SetCompression(bool compressed)
		{
			if (compressed)
			{
				_arguments.AddOrUpdate("compress", "zlib-stream");
			}
			else
			{
				_arguments.TryRemove("compress");
			}
			return this;
		}

		public string Build()
		{
			if (_arguments.Count == 0)
			{
				return _url;
			}
			return _url + "?" + string.Join("&", _arguments.Select(x => $"{x.Key}={x.Value}"));
		}
	}
}