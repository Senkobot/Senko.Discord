using System.Threading.Tasks;

namespace Senko.Discord.Gateway
{
	public interface IGatewayApiClient
	{
		Task<GatewayConnectionPacket> GetGatewayAsync();

		Task<GatewayConnectionPacket> GetGatewayBotAsync();
	}
}