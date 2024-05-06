using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EldenRing.NT
{
    [CreateAssetMenu(menuName = "Character Actions/Weapon Actions/Off Hand Melee Action")]
    public class OffHandMeleeAction : WeaponItemAction
    {
        //  Q. Why call it "Off Hand Melee Action", and not "Block Action"?

        //  A. In the future, if a character is weilding a main hand and off hand weapon of the same weapon class,
        //     the off hand action will not be a block. The off hand's action becomes a dual attack.

        public override void AttempToPerformAction(PlayerManager playerPerformingAction, WeaponItem weaponPerformingAction)
        {
            base.AttempToPerformAction(playerPerformingAction, weaponPerformingAction);

            //  CHECK FOR POWER STANCE ACTION (DUAL ATTACK)

            //  CHECK FOR CAN BLOCK
            if (!playerPerformingAction.playerCombatManager.canBlock)
                return;

            //  CHECK FOR ATTACK STATUS
            if (playerPerformingAction.playerNetworkManager.isAttacking.Value)
            {
                //  DISABLE BLOCKING (When using a short/medium spear block attacking is allowed with light attacks. Handled on another class)
                if (playerPerformingAction.IsOwner)
                    playerPerformingAction.playerNetworkManager.isBlocking.Value = false;

                return;
            }

            if (playerPerformingAction.playerNetworkManager.isBlocking.Value)
                return;

            if (playerPerformingAction.IsOwner)
                playerPerformingAction.playerNetworkManager.isBlocking.Value = true;
        }
    }
}