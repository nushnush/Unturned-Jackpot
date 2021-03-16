using Rocket.API;

namespace Jackpot
{
    public class Configuration : IRocketPluginConfiguration
    {
        public string MessageColor { get; set; }

        public void LoadDefaults()
        {
            MessageColor = "yellow";
        }
    }
}