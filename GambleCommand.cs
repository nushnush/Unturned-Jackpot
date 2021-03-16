using Rocket.API;
using Rocket.Unturned.Chat;
using Rocket.Unturned.Player;
using System.Collections.Generic;
using UnityEngine;

namespace Jackpot
{
    class GambleCommand : IRocketCommand
    {
        public AllowedCaller AllowedCaller => AllowedCaller.Player;

        public string Name => "gamble";

        public string Help => "Gamble to win more currency.";

        public string Syntax => "<amount";

        public List<string> Aliases => new List<string>();

        public List<string> Permissions => new List<string>() { "jackpot" };

        public void Execute(IRocketPlayer caller, string[] command)
        {
            UnturnedPlayer client = (UnturnedPlayer)caller;

            int amount = Jackpot.Instance.IsInJackpot(client);
            if (amount > 0)
            {
                UnturnedChat.Say(caller, "You already gambled on " + amount.ToString() + " credits.", Jackpot.Instance.MessageColor);
                return;
            }

            if (command.Length < 1)
            {
                UnturnedChat.Say(caller, "Usage: /gamble <amount>", Jackpot.Instance.MessageColor);
                return;
            }

            if(int.TryParse(command[0], out amount) || amount <= 0)
            {
                UnturnedChat.Say(caller, "Invalid amount.", Jackpot.Instance.MessageColor);
                return;
            }

            if(Jackpot.Instance.jackpotRunning)
            {
                Jackpot.Instance.AddToJackpot(new GambleInfo { gambleAmount = amount, steamid = client.CSteamID , chance = 0});
            }
            else
            {
                Jackpot.Instance.Start();
            }
            
        }
    }
}
