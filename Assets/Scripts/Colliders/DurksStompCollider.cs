using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EldenRing.NT
{
    public class DurksStompCollider : DamageCollider
    {
        [SerializeField] AIDurkCharacterManager aiDurkCharacter;

        protected override void Awake()
        {
            base.Awake();

            aiDurkCharacter = GetComponentInParent<AIDurkCharacterManager>();
        }

        public void StompAttack()
        {
            GameObject stompVFX = Instantiate(aiDurkCharacter.durkCombatManager.durkImpactVFX, transform);

            Collider[] colliders = Physics.OverlapSphere(transform.position, aiDurkCharacter.durkCombatManager.stompAttackAOERadius, WorldUtilityManager.instance.GetCharacterLayers());
            
            List<CharacterManager> charactersDamaged = new List<CharacterManager>();

            foreach (var collider in colliders)
            {
                CharacterManager character = collider.GetComponentInParent<CharacterManager>();

                if (character != null)
                {
                    if (charactersDamaged.Contains(character))
                        continue;

                    //  WE DON'T WANT DURK TO HURT HIMSELF WHEN HE STOMPS
                    if (character == aiDurkCharacter)
                        continue;

                    charactersDamaged.Add(character);

                    //  WE ONLY PROCESS DAMAGE IF THE CHARACTER "ISOWNER" SO THAT THEY ONLY GET DAMAGED IF THE COLLIDER CONNECTS ON THEIR CLIENT
                    //  MEANING IF YOU ARE HIT ON THE HOSTS SCREEN BUT NOT ON YOUR OWN, YOU WILL NOT BE HIT
                    if (character.IsOwner)
                    {
                        //  CHECK FOR BLOCK

                        TakeDamageEffect damageEffect = Instantiate(WorldCharacterEffectsManager.instance.takeDamageEffect);
                        damageEffect.physicalDamage = aiDurkCharacter.durkCombatManager.stompDamage;
                        damageEffect.poiseDamage = aiDurkCharacter.durkCombatManager.stompDamage;

                        character.characterEffectsManager.ProcessInstantEffect(damageEffect);
                    }
                }
            }
        }
    }
}