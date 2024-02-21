using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EldenRing.NT
{
    [CreateAssetMenu(menuName = "Character Effects/Instant Effects/Take Damage")]
    public class TakeDamageEffect : InstantCharacterEffect
    {
        [Header("CHARACTER CAUSING DAMAGE")]
        public CharacterManager characterCausingDamage;         //  IF THE DMG IS CAUSED BY ANOTHER CHARACTERS ATTACK IT WILL BE STORED HERE

        [Header("DAMAGE TYPES")]
        public float physicalDamage = 0;                        //  (IN THE FUTURE WILL BE SPLIT INTO [STANDARD], [STRIKE], [SLASH] AND [PIERCE])
        public float magicDamage = 0;
        public float fireDamage = 0;
        public float lightningDamage = 0;
        public float holyDamage = 0;

        [Header("FINAL DAMAGE")]
        [SerializeField] private int finalDamageDealt = 0;    //  THE DAMAGE THE CHARACTER TAKES AFTER [ALL] CALCULATIONS HAVE BEEN MADE

        [Header("POISE")]
        public float poiseDamage = 0;
        public bool poiseIsBroken = false;                      //  IF A CHARACTER'S POISE IS BROKEN, THEY WILL BE [STUNNED] AND PLAY DAMAGE ANIMATION

        //  (TO DO) BUILD UPS
        //  BUILD UP EFFECT AMOUNTS

        [Header("ANIMATIONS")]
        public bool playDamageAnimation = true;
        public bool manuallySelectDamageAnimation = false;
        public string damageAnimation;

        [Header("SOUND FX")]
        public bool willPlayDamageSFX = true;
        public AudioClip elementalDamageSoundFX;                //  USED ON TOP OF REGULAR SFX IF THERE IS ELEMENTAL DAMAGE PRESENT (Magic/Fire/Lightning/Holy)

        [Header("DIRECTION DAMAGE TAKEN FROM")]
        public float angleHitFrom;                              //  USED TO DETERMINE WHAT DAMAGE ANIMATION TO PLAY (Move backwards, to the left, to the right etc, etc...)
        public Vector3 contactPoint;                            //  USED TO DETERMINE WHERE THE BLOOD FX INSTANTIATE

        public override void ProcessEffect(CharacterManager character)
        {
            base.ProcessEffect(character);

            //  IF THE CHARACTER IS DEAD, NO ADDITIONAL DAMAGE EFFECTS SHOULD BE PROCESSED
            if (character.isDead.Value)
                return;

            //  CHECK FOR "INVULNERABILITY"

            CalculateDamage(character);
            PlayDirectionalBasedDamageAnimation(character);
            //  CHECK FOR BUILD UPS (POISON, BLEED ETC...)
            PlayDamageSFX(character);
            PlayDamageVFX(character);

            //  IF CHARACTER IS A.I, CHECK FOR NEW TARGET IF CHARACTER CAUSING DAMAGE IS PRESENT
        }

        private void CalculateDamage(CharacterManager character)
        {
            if (!character.IsOwner)
                return;

            if (characterCausingDamage != null)
            {
                //  CHECK FOR DAMAGE MODIFIERS AND MODIFY BASE DAMAGE (PHYSICAL/ELEMENTAL DAMAGE BUFF)
            }

            //  CHECK CHARACTER FOR FLAT DEFENSES AND SUBTRACT THEM FROM DAMAGE

            //  CHECK CHARACTER FOR ARMOR ABSORPTIONS, AND SUBTRACT THE PERCENTAGE FROM THE DAMAGE

            //  ADD ALL DAMAGE TYPES TOGHETHER, AND APPLY FINAL DAMAGE
            finalDamageDealt = Mathf.RoundToInt(physicalDamage + magicDamage + fireDamage + lightningDamage + holyDamage);

            if (finalDamageDealt <= 0)
            {
                finalDamageDealt = 1;
            }

            character.characterNetworkManager.currentHealth.Value -= finalDamageDealt;

            //  CALCULATE POISE DAMAGE TO  DETERMINE IF THE CHARACTER WILL BE STUNNED
        }

        private void PlayDamageVFX(CharacterManager character)
        {
            //  IF WE HAVE FIRE DAMAGE, PLAY FIRE PARTICLES
            //  LIGHTNING DAMAGE, LIGHTNING PARTICLES ETC

            character.characterEffectsManager.PlayBloodSplatterVFX(contactPoint);
        }

        private void PlayDamageSFX(CharacterManager character)
        {
            AudioClip physicalDamageSFX = WorldSoundFXManager.instance.ChooseRandomSFXFromArray(WorldSoundFXManager.instance.physicalDamageSFX);

            character.characterSoundFXManager.PlaySoundFX(physicalDamageSFX);
            //  IF FIRE DAMAGE IS GREATER THAN 0, PLAY BURN SFX
            //  IF LIGHTNING DAMAGE IS GREATER THAN 0, PLAY ZAP SFX
        }

        private void PlayDirectionalBasedDamageAnimation(CharacterManager character)
        {
            if (!character.IsOwner)
                return;

            //  TODO CALCULATE IF POISE IS BROKEN
            poiseIsBroken = true;

            if (angleHitFrom >= 145 &&  angleHitFrom <= 180)
            {
                damageAnimation = character.characterAnimatorManager.GetRandomAnimationFromList(character.characterAnimatorManager.forward_Medium_Damage);
            }
            else if (angleHitFrom <= -145 && angleHitFrom >= -180)
            {
                damageAnimation = character.characterAnimatorManager.GetRandomAnimationFromList(character.characterAnimatorManager.forward_Medium_Damage);
            }
            else if (angleHitFrom >= -45 && angleHitFrom <= 45)
            {
                damageAnimation = character.characterAnimatorManager.GetRandomAnimationFromList(character.characterAnimatorManager.backward_Medium_Damage);
            }
            else if (angleHitFrom >= -144 && angleHitFrom <= -45)
            {
                damageAnimation = character.characterAnimatorManager.GetRandomAnimationFromList(character.characterAnimatorManager.left_Medium_Damage);
            }
            else if (angleHitFrom >= 45 && angleHitFrom <= 144)
            {
                damageAnimation = character.characterAnimatorManager.GetRandomAnimationFromList(character.characterAnimatorManager.right_Medium_Damage);
            }

            //  IF POISE IS BROKEN, PLAY A STRAGGERING DAMAGE ANIMATION
            if (poiseIsBroken)
            {
                character.characterAnimatorManager.lastDamageAnimationPlayed = damageAnimation;
                character.characterAnimatorManager.PlayTargetActionAnimation(damageAnimation, true);
            }
        }
    }
}