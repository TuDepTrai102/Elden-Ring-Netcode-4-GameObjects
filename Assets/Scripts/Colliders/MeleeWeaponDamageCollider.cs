using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EldenRing.NT
{
    public class MeleeWeaponDamageCollider : DamageCollider
    {
        [Header("ATTACKING CHARACTER")]
        public CharacterManager characterCausingDamage; //  (WHEN CALCULATE DAMAGE THIS USED TO CHECK FOR ATTACKERS DAMAGE MODIFIERS, EFFECT ETC, ETC...)

        [Header("WEAPON ATTACK MODIFIERS")]
        public float light_Attack_01_Modifier;

        protected override void Awake()
        {
            base.Awake();

            if (damageCollider == null)
            {
                damageCollider = GetComponent<Collider>();
            }

            damageCollider.enabled = false; //  MELEE WEAPON COLLIDERS SHOULD BE DISABLED AT START, ONLY ENABLED WHEN ANIMATIONS ALLOW
        }

        protected override void OnTriggerEnter(Collider other)
        {
            CharacterManager damageTarget = other.GetComponentInParent<CharacterManager>();

            if (damageTarget != null)
            {
                //  WE DO NOT WANT TO DAMAGE OURSELVES
                if (damageTarget == characterCausingDamage)
                    return;

                contactPoint = other.gameObject.GetComponent<Collider>().ClosestPointOnBounds(transform.position);

                //  CHECK IF WE CAN DAMAGE TARGET BASED ON FRIENDLY FIRE

                //  CHECK IF TARGET IS BLOCKING

                //  CHECK IF TARGET IS INVULNERABLE

                DamageTarget(damageTarget);
            }
        }

        protected override void DamageTarget(CharacterManager damageTarget)
        {
            //  WE DON'T WANT TO DAMAGE THE SAME TARGET MORE THAN ONCE IN A SINGLE ATTACK
            //  SE WE ADD THEM TO A LIST THAT CHECKS BEFORE APPLYING DAMAGE
            if (charactersDamaged.Contains(damageTarget))
                return;

            charactersDamaged.Add(damageTarget);

            TakeDamageEffect damageEffect = Instantiate(WorldCharacterEffectsManager.instance.takeDamageEffect);
            damageEffect.physicalDamage = physicalDamage;
            damageEffect.magicDamage = magicDamage;
            damageEffect.fireDamage = fireDamage;
            damageEffect.lightningDamage = lightningDamage;
            damageEffect.holyDamage = holyDamage;
            damageEffect.contactPoint = contactPoint;
            damageEffect.angleHitFrom = Vector3.SignedAngle(characterCausingDamage.transform.forward, damageTarget.transform.forward, Vector3.up);

            switch (characterCausingDamage.characterCombatManager.currentAttackType)
            {
                case AttackType.LightAttack01:
                    ApplyAttackDamageMofiers(light_Attack_01_Modifier, damageEffect);
                    break;
                default: 
                    break;
            }

            //damageTarget.characterEffectsManager.ProcessInstantEffect(damageEffect);

            if (characterCausingDamage.IsOwner)
            {
                damageTarget.characterNetworkManager.NotifyTheServerOfCharacterDamageServerRpc(
                    damageTarget.NetworkObjectId,
                    characterCausingDamage.NetworkObjectId,
                    damageEffect.physicalDamage,
                    damageEffect.magicDamage,
                    damageEffect.fireDamage,
                    damageEffect.lightningDamage,
                    damageEffect.holyDamage,
                    damageEffect.poiseDamage,
                    damageEffect.angleHitFrom,
                    damageEffect.contactPoint.x,
                    damageEffect.contactPoint.y,
                    damageEffect.contactPoint.z);
            }
        }

        private void ApplyAttackDamageMofiers(float modifer, TakeDamageEffect damage)
        {
            damage.physicalDamage *= modifer;
            damage.magicDamage *= modifer;
            damage.fireDamage *= modifer;
            damage.lightningDamage *= modifer;
            damage.holyDamage *= modifer;
            damage.poiseDamage *= modifer;

            //  IF TRACK IS A FULLY CHARGED HEAVY, MULTIPY BY FULL CHARGED MODIFIER AFTER NORMAL MODIFIER HAVE BEEN CALCULATED
        }
    }
}