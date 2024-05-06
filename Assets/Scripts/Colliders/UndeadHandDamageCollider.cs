using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EldenRing.NT
{
    public class UndeadHandDamageCollider : DamageCollider
    {
        [SerializeField] AICharacterManager undeadCharacter;

        protected override void Awake()
        {
            base.Awake();

            damageCollider = GetComponent<Collider>();
            undeadCharacter = GetComponentInParent<AICharacterManager>();
        }

        protected override void GetBlockingDotValues(CharacterManager damageTarget)
        {
            directionFromAttackToDamageTarget = undeadCharacter.transform.position - damageTarget.transform.position;
            dotValueFromAttackToDamageTarget = Vector3.Dot(directionFromAttackToDamageTarget, damageTarget.transform.forward);
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
            damageEffect.poiseDamage = poiseDamage;
            damageEffect.contactPoint = contactPoint;
            damageEffect.angleHitFrom = Vector3.SignedAngle(undeadCharacter.transform.forward, damageTarget.transform.forward, Vector3.up);

            //  OPTION 01:
            //  THIS WILL APPLY DAMAGE IF THE A.I HITS IT'S TARGET ON THE HOSTS SIDE REGARDLESS OF HOW IT LOOKS ON ANY OTHER CLIENTS SIDE
            //if (undeadCharacter.IsOwner)
            //{
            //    damageTarget.characterNetworkManager.NotifyTheServerOfCharacterDamageServerRpc(
            //        damageTarget.NetworkObjectId,
            //        undeadCharacter.NetworkObjectId,
            //        damageEffect.physicalDamage,
            //        damageEffect.magicDamage,
            //        damageEffect.fireDamage,
            //        damageEffect.lightningDamage,
            //        damageEffect.holyDamage,
            //        damageEffect.poiseDamage,
            //        damageEffect.angleHitFrom,
            //        damageEffect.contactPoint.x,
            //        damageEffect.contactPoint.y,
            //        damageEffect.contactPoint.z);
            //}

            //  OPTION 02:
            //  THIS WILL APPLY DAMAGE IF THE A.I HITS IT'S TARGET ON THE CONNECTED CHARACTERS SIDE REGARDLESS OF HOW IT LOOKS ON ANY OTHER CLIENTS SIDE
            if (undeadCharacter.IsOwner)
            {
                damageTarget.characterNetworkManager.NotifyTheServerOfCharacterDamageServerRpc(
                    damageTarget.NetworkObjectId,
                    undeadCharacter.NetworkObjectId,
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
    }
}