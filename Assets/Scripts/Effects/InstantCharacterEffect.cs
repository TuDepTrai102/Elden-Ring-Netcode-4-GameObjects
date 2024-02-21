using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EldenRing.NT
{
    public class InstantCharacterEffect : ScriptableObject
    {
        [Header("EFFECT ID")]
        public int instantEffectID;

        public virtual void ProcessEffect(CharacterManager character)
        {

        }
    }
}