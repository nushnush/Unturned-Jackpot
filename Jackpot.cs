using System.Linq;
using Rocket.Core.Plugins;
using Rocket.Unturned.Chat;
using Rocket.Unturned.Player;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Steamworks;
using Logger = Rocket.Core.Logging.Logger;

namespace Jackpot
{
    class Jackpot : RocketPlugin<Configuration>
    {
        public static Jackpot Instance { get; private set; }
        public Color MessageColor { get; set; }
        public List<GambleInfo> items { get; set; } = new List<GambleInfo>();
        public bool jackpotRunning { get; set; }
        public int time = 30;
        public Coroutine timer { get; set; }

        protected override void Load()
        {
            Instance = this;
            MessageColor = UnturnedChat.GetColorFromName(Configuration.Instance.MessageColor, Color.yellow);
            jackpotRunning = false;
            
            Logger.Log($"{Name} {Assembly.GetName().Version} has been loaded!", ConsoleColor.Yellow);
        }

        public void Start()
        {
            items.Clear();
            jackpotRunning = true;
            time = 30;

            timer = StartCoroutine(StartJackpot());
        }

        IEnumerator StartJackpot()
        {
            while (time != 0)
            {
                if (time == 30 || time == 20 || time == 10)
                {
                    UnturnedChat.Say(time.ToString() + " seconds until Jackpot ends!");
                    UnturnedChat.Say("Enter the jackpot with /gamble");
                }

                yield return new WaitForSeconds(1f);
                time--;
            }

            Stop();
        }

        public void Stop()
        {
            StopCoroutine(timer);
            jackpotRunning = false;
            int sum = items.Sum(x => x.gambleAmount);

            for (int i = 0; i < items.Count; i++)
            {
                items[i].chance = (int)(items[i].gambleAmount / (double)sum * 100);
            }

            items = items.OrderByDescending(x => x.chance).ToList();

            for (int i = 0; i < items.Count; i++)
            {
                if(UnityEngine.Random.Range(1, 101) <= items[i].chance)
                {
                    UnturnedPlayer p = UnturnedPlayer.FromCSteamID(items[i].steamid);
                    if (p == null)
                        continue;

                    p.Experience += Convert.ToUInt32(sum); // raise currency
                    break;
                }
            }

        }

        public int IsInJackpot(UnturnedPlayer player)
        {
            return items.Where(x => x.steamid.m_SteamID == player.CSteamID.m_SteamID).Select(x => x.gambleAmount).FirstOrDefault();
        }

        public void AddToJackpot(GambleInfo info)
        {
            items.Add(info);
        }
    }

    class GambleInfo
    {
        public CSteamID steamid { get; set; }
        public int gambleAmount { get; set; }
        public int chance { get; set; }
    }
}
