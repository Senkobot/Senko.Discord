namespace Senko.Discord.Packets
{
    public readonly struct RatelimitResult
    {
        public RatelimitResult(int timeOut)
        {
            TimeOut = timeOut;
        }

        public int TimeOut { get;}
    }
}