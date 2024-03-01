using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EldenRing.NT
{
    public class WeaponItem : Item
    {
        //  ANIMATOR CONTROLLER OVERRIDE (Change attack aniamations based on weapon you are currently using)

        [Header("WEAPON MODEL")]
        public GameObject weaponModel;

        [Header("WEAPON REQUIREMENTS")]
        public int strengthREQ = 0;
        public int dexREQ = 0;
        public int intREQ = 0;
        public int faithREQ = 0;

        [Header("WEAPON BASE DAMAGE")]
        public int physicalDamage = 0;
        public int magicDamage = 0;
        public int fireDamage = 0;
        public int holyDamage = 0;
        public int lightningDamage = 0;

        //  WEAPON GUARD ABSORPTIONS (BLOCKING POWER)

        [Header("WEAPON BASE POISE DAMAGE")]
        public float poiseDamage = 10;
        //  OFFENSIVE POISE BONUS WHEN ATTACKING

        [Header("ATTACK MODIFIERS")]
        public float light_Attack_01_Modifier = 1.0f;
        public float light_Attack_02_Modifier = 1.2f;
        public float heavy_Attack_01_Modifier = 1.4f;
        public float heavy_Attack_02_Modifier = 1.6f;
        public float charge_Attack_01_Modifier = 2.0f;
        public float charge_Attack_02_Modifier = 2.2f;

        [Header("STAMINA COST MODIFIERS")]
        public int baseStaminaCost = 20;
        public float lightAttackStaminaCostMultiplier = 0.9f;
        //  RUNNING ATTACK STAMINA COST MODIFIER
        //  HEAVY ATTACK STAMINA COST MODIFIER ETC, ETC...

        [Header("ACTIONS")]
        public WeaponItemAction oh_RB_Action;   //  ONE HANDED RIGHT BUMPER ACTION
        public WeaponItemAction oh_RT_Action;   //  ONE HANDED RIGHT TRIGGER ACTION

        //  ASH OF WAR

        //  BLOCKING SOUNDS
    }
}