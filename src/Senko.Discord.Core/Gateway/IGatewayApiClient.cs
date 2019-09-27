using System.Threading.Tasks;

namespace Senko.Discord.Gateway
{
	public interface IGatewayApiClient
	{
        ValueTask<GatewayConnectionPacket> GetGatewayAsync();

		ValueTask<GatewayConnectionPacket> GetGatewayBotAsync();
	}
}