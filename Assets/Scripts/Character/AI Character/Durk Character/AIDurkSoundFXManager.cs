using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EldenRing.NT
{
    public class AIDurkSoundFXManager : CharacterSoundFXManager
    {
        AIDurkCharacterManager aiDurkCharacter;

        [Header("CLUB WHOOSHES")]
        public AudioClip[] clubWhooshes;

        [Header("CLUB IMPACTS")]
        public AudioClip[] clubImpacts;

        [Header("STOMP IMPACTS")]
        public AudioClip[] stompImpacts;

        protected override void Awake()
        {
            base.Awake();

            aiDurkCharacter = GetComponent<AIDurkCharacterManager>();
        }

        public virtual void PlayClubImpactSoundFX()
        {
            if (clubImpacts.Length > 0)
                PlaySoundFX(WorldSoundFXManager.instance.ChooseRandomSFXFromArray(clubImpacts));
        }

        public virtual void PlayStompImpactSoundFX()
        {
            if (stompImpacts.Length > 0)
                PlaySoundFX(WorldSoundFXManager.instance.ChooseRandomSFXFromArray(stompImpacts));
        }

        public virtual void PlayClubWhooshSoundFX()
        {
            aiDurkCharacter.characterSoundFXManager.PlaySoundFX
                (WorldSoundFXManager.instance.ChooseRandomSFXFromArray(aiDurkCharacter.durkSoundFXManager.clubWhooshes));
        }
    }
}