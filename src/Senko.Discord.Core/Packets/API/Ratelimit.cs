using System;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace Senko.Discord.Packets
{
	/// <summary>
    /// General ratelimit struct used to verify ratelimits and block potentially ratelimited requests.
    /// </summary>
	[DataContract]
	public struct Ratelimit
	{
        /// <summary>
        /// Remaining amount of entities that can be sent on this route.
        /// </summary>
        [JsonPropertyName("remaining")]
        [DataMember(Order = 1)]
        public int Remaining { get; set; }

        /// <summary>
        /// Total limit of entities that can be sent until <see cref="Reset"/> occurs.
        /// </summary>
        [JsonPropertyName("limit")]
        [DataMember(Order = 2)]
        public int Limit { get; set; }

        /// <summary>
        /// Epoch until ratelimit resets values.
        /// </summary>
        [JsonPropertyName("reset")]
        [DataMember(Order = 3)]
        public long Reset { get; set; }

        /// <summary>
        /// An optional global value for a shared ratelimit value.
        /// </summary>
        [JsonPropertyName("global")]
        [DataMember(Order = 4)]
        public int? Global { get; set; }

        /// <summary>
        /// Checks if the current ratelimit is valid and/or is expired.
        /// </summary>
        /// <returns>Whether the current instance is being ratelimited</returns>
        /// <param name="result">Information about the ratelimit</param>
		public bool IsRatelimited(out RatelimitResult result)
        {
	        return IsRatelimited(this, out result);
        }

        /// <summary>
        /// Checks if the ratelimit is valid and/or is expired.
        /// </summary>
        /// <param name="rl">The instance that is being checked.</param>
        /// <param name="result">Information about the ratelimit</param>
        /// <returns>Whether the instance is being ratelimited</returns>
        public static bool IsRatelimited(Ratelimit rl, out RatelimitResult result)
        {
	        var now = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
			
			if ((rl.Global <= 0 || rl.Remaining <= 0)
			    && now < rl.Reset)
			{
				result = new RatelimitResult((int)(rl.Reset - now));
				return true;
			}

			result = default;
			return false;
		}
	}
}