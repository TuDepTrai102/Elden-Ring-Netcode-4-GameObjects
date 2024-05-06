using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EldenRing.NT {
    public class DamageCollider : MonoBehaviour
    {
        [Header("COLLIDER")]
        [SerializeField] protected Collider damageCollider;

        [Header("DAMAGE TYPES")]
        public float physicalDamage = 0;    //  (IN THE FUTURE WILL BE SPLIT INTO [STANDARD], [STRIKE], [SLASH] AND [PIERCE])
        public float magicDamage = 0;
        public float fireDamage = 0;
        public float lightningDamage = 0;
        public float holyDamage = 0;

        [Header("POISE")]
        public float poiseDamage = 0;

        [Header("CONTACT POINT")]
        protected Vector3 contactPoint;

        [Header("CHARACTERS DAMAGED")]
        protected List<CharacterManager> charactersDamaged = new List<CharacterManager>();

        [Header("BLOCK")]
        protected Vector3 directionFromAttackToDamageTarget;
        protected float dotValueFromAttackToDamageTarget;

        protected virtual void Awake()
        {
            
        }

        protected virtual void OnTriggerEnter(Collider other)
        {
            CharacterManager damageTarget = other.GetComponentInParent<CharacterManager>();

            if (damageTarget != null)
            {
                contactPoint = other.gameObject.GetComponent<Collider>().ClosestPointOnBounds(transform.position);

                //  CHECK IF WE CAN DAMAGE TARGET BASED ON FRIENDLY FIRE

                //  CHECK IF TARGET IS BLOCKING
                CheckForBlock(damageTarget);

                DamageTarget(damageTarget);
            }
        }

        protected virtual void CheckForBlock(CharacterManager damageTarget)
        {
            //  IF THIS CHARACTER ALREADY HAS BEEN DAMAGED, DO NOT PROCEED
            if (charactersDamaged.Contains(damageTarget))
                return;

            GetBlockingDotValues(damageTarget);

            //  1. CHECK IF THE CHARACTER BEING DAMAGED IS BLOCKING 
            if (damageTarget.characterNetworkManager.isBlocking.Value && dotValueFromAttackToDamageTarget > 0.3f)
            {
                //  2. IF THE CHARACTER IS BLOCKING, CHECK IF THEY ARE FACING IN THE CORRECT DIRECTION TO BLOCK SUCCESFULLY

                charactersDamaged.Add(damageTarget);

                TakeBlockedDamageEffect damageEffect = Instantiate(WorldCharacterEffectsManager.instance.takeBlockedDamageEffect);
                damageEffect.physicalDamage = physicalDamage;
                damageEffect.magicDamage = magicDamage;
                damageEffect.fireDamage = fireDamage;
                damageEffect.lightningDamage = lightningDamage;
                damageEffect.holyDamage = holyDamage;
                damageEffect.poiseDamage = poiseDamage;
                damageEffect.staminaDamage = poiseDamage;   //  IF YOU WANT TO GIVE STAMINA ITS OWN VARIABLE, INSTEAD OF USING POISE GO FOR IT
                damageEffect.contactPoint = contactPoint;

                //  3. APPLY BLOCKED CHARACTER DAMAGE TO TARGET
                damageTarget.characterEffectsManager.ProcessInstantEffect(damageEffect);
            }
        }

        protected virtual void GetBlockingDotValues(CharacterManager damageTarget)
        {
            directionFromAttackToDamageTarget = transform.position - damageTarget.transform.position;
            dotValueFromAttackToDamageTarget = Vector3.Dot(directionFromAttackToDamageTarget, damageTarget.transform.forward);
        }

        protected virtual void DamageTarget(CharacterManager damageTarget)
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
            damageTarget.characterEffectsManager.ProcessInstantEffect(damageEffect);
        }

        public virtual void EnableDamageCollider()
        {
            damageCollider.enabled = true;
        }

        public virtual void DisableDamageCollider()
        {
            damageCollider.enabled = false;
            charactersDamaged.Clear();  //  WE RESET THE CHARACTERS THAT HAVE BEEN HIT WHEN WE RESET THE COLLIDER, SO THEY MAY BE HIT AGAIN
        }
    }
}