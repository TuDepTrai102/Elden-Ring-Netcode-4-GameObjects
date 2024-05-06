using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EldenRing.NT
{
    public class WeaponItem : Item
    {
        [Header("ANIMATIONS")]
        public AnimatorOverrideController weaponAnimator;

        [Header("MODEL INSTANTIATION")]
        public WeaponModelType weaponModelType;

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
        public float running_Attack_01_Modifier = 1.1f;
        public float rolling_Attack_01_Modifier = 1.1f;
        public float backstep_attack_01_Modifier = 1.1f;

        [Header("STAMINA COST MODIFIERS")]
        public int baseStaminaCost = 20;
        public float lightAttackStaminaCostMultiplier = 1.0f;
        public float heavyAttackStaminaCostMultiplier = 1.3f;
        public float chargedAttackStaminaCostMultiplier = 1.5f;
        public float runningAttackStaminaCostMultiplier = 1.1f;
        public float rollingAttackStaminaCostMultiplier = 1.1f;
        public float backstepAttackStaminaCostMultiplier = 1.1f;

        [Header("WEAPON BLOCKING ABSORPTIONS")]
        public float physicalBaseDamageAbsorption = 50;
        public float magicBaseDamageAbsorption = 50;
        public float fireBaseDamageAbsorption = 50;
        public float holyBaseDamageAbsorption = 50;
        public float lightningBaseDamageAbsorption = 50;
        public float stability = 50;    // REDUCES STAMINA LOST FROM BLOCK 

        [Header("ACTIONS")]
        public WeaponItemAction oh_RB_Action;   //  ONE HANDED RIGHT BUMPER ACTION
        public WeaponItemAction oh_RT_Action;   //  ONE HANDED RIGHT TRIGGER ACTION
        public WeaponItemAction oh_LB_Action;   //  ONE HANDED LEFT BUMPER ACTION

        //  ASH OF WAR

        //  BLOCKING SOUNDS
        [Header("SFX")]
        public AudioClip[] whooshes;
        public AudioClip[] blocking;
    }
}