using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EldenRing.NT
{
    [System.Serializable]
    //  SINCE WE WANT TO REFERENCE THIS DATA FOR EVERY SAVE FILE, THIS SCRIPT IS NOT A MONOBEHAVIOR AND IS INSTEAD SERIALIZABLE
    public class CharacterSaveData
    {
        [Header("SCENE INDEX")]
        public int sceneIndex = 1;

        [Header("CHARACTER NAME")]
        public string characterName = "Character";

        [Header("TIME PLAYED")]
        public float secondsPlayed;

        //  QUESTION: WHY NOT USE A VECTOR3?
        //  ANSWER: WE CAN ONLY SAVE DATA FROM "BASIC" VARIABLE TYPES (float, int, string, bool, ... ETC, ETC)
        [Header("WORLD COORDINATES")]
        public float xPosition;
        public float yPosition;
        public float zPosition;

        [Header("RESOURCES")]
        public int currentHealth;
        public float currentStamina;

        [Header("STATS")]
        public int vitality;
        public int endurance;

        [Header("BOSSES")]
        public SerializableDictionary<int, bool> boosesAwakened;    //  THE INT IS THE BOSS I.D, THE BOOL IS AWAKENED STATUS
        public SerializableDictionary<int, bool> boosesDefeated;    //  THE INT IS THE BOSS I.D, THE BOOL IS DEFEATED STATUS

        public CharacterSaveData()
        {
            boosesAwakened = new SerializableDictionary<int, bool>();
            boosesDefeated = new SerializableDictionary<int, bool>();
        }
    }
}