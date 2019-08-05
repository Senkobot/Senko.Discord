using System.Collections.Generic;
using System.Linq;

namespace Senko.Discord.Rest.Http
{
    public class QueryString
    {
		public string Query
			=> "?" + string.Join("&", queryArgs.Select(x => $"{x.Key}={x.Value.ToString()}"));

		private Dictionary<string, object> queryArgs = new Dictionary<string, object>();

		public void Add(string key, object value)
		{
			queryArgs.Add(key, value);
		}
    }
}
