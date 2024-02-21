using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EldenRing.NT
{
    public class PlayerStatsManager : CharacterStatsManager
    {
        PlayerManager player;

        protected override void Awake()
        {
            base.Awake();

            player = GetComponent<PlayerManager>();
        }

        protected override void Start()
        {
            base.Start();

            //  WHY CALCULATE THESE HERE?
            /*  WHEN WE MAKE A CHARACTER (IN A CREATION MENU), AND SET THE STATS DEPENDING ON THE CLASS, THIS WILL BE CALCULATED THERE.
                UNTIL THE HOWEVER, STATS ARE NEVER CALCULATED, SO WE DO IT HERE ON START, 
                IF A SAVE FILE EXISTS THEY WILL BE OVER WRITTEN WHEN LOADING IN TO A SCENE  */
            CalculateHealthBasedOnVitalityLevel(player.playerNetworkManager.vitality.Value);
            CalculateStaminaBasedOnEnduranceLevel(player.playerNetworkManager.endurance.Value);
        }
    }
}