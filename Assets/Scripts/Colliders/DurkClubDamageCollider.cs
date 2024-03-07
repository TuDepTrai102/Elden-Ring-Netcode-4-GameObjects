using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.TextCore.Text;

namespace EldenRing.NT
{
    public class DurkClubDamageCollider : DamageCollider
    {
        public AIDurkCharacterManager aiDurkCharacter;

        protected override void Awake()
        {
            base.Awake();

            damageCollider = GetComponent<Collider>();
            aiDurkCharacter = GetComponentInParent<AIDurkCharacterManager>();
        }

        protected override void DamageTarget(CharacterManager damageTarget)
        {
            //  WE DON'T WANT TO DAMAGE THE SAME TARGET MORE THAN ONCE IN A SINGLE ATTACK
            //  SE WE ADD THEM TO A LIST THAT CHECKS BEFORE APPLYING DAMAGE
            if (charactersDamaged.Contains(damageTarget))
                return;

            //  WE DON'T WANT DURK TO HURT HIMSELF
            if (aiDurkCharacter == damageTarget)
                return;

            charactersDamaged.Add(damageTarget);

            TakeDamageEffect damageEffect = Instantiate(WorldCharacterEffectsManager.instance.takeDamageEffect);
            damageEffect.physicalDamage = physicalDamage;
            damageEffect.magicDamage = magicDamage;
            damageEffect.fireDamage = fireDamage;
            damageEffect.lightningDamage = lightningDamage;
            damageEffect.holyDamage = holyDamage;
            damageEffect.contactPoint = contactPoint;
            damageEffect.angleHitFrom = Vector3.SignedAngle(aiDurkCharacter.transform.forward, damageTarget.transform.forward, Vector3.up);

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
            if (aiDurkCharacter.IsOwner)
            {
                damageTarget.characterNetworkManager.NotifyTheServerOfCharacterDamageServerRpc(
                    damageTarget.NetworkObjectId,
                    aiDurkCharacter.NetworkObjectId,
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
