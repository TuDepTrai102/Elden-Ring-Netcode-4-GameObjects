using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EldenRing.NT
{
    public class CharacterStatsManager : MonoBehaviour
    {
        CharacterManager character;

        [Header("STAMINA REGENERATION")]
        [SerializeField] int staminaRegenerationAmount = 2;
        private float staminaRegenerationTimer = 0;
        private float staminaTickTimer = 0;
        [SerializeField] float staminaRegenerationDelay = 2;

        [Header("BLOCKING ABSORPTIONS")]
        public float blockingPhysicalAbsorption = 0;
        public float blockingFireAbsorption = 0;
        public float blockingMagicAbsorption = 0;
        public float blockingLightningAbsorption = 0;
        public float blockingHolyAbsorption = 0;
        public float blockingStability = 0;

        [Header("POISE")]
        public float totalPoiseDamage = 0;          //  HOW MUCH POISE DAMAGE WE HAVE TAKEN
        public float offensivePoiseBonus = 0;       //  THE POISE BONUS GAINED FROM USING WEAPONS (HEAVY WEAPONS HAVE A MUCH LARGER BONUS)
        public float basePoiseDefense = 0;          //  THE POISE BONUS GAINED FROM ARMOR/TALISMANS ETC, ETC...
        public float defaultPoiseResetTime = 8;     //  THE TIME IT TAKES FOR POISE DAMAGE TO RESET (MUST NOT BE HIT IN THE TIME OR IT WILL RESET)
        public float poiseResetTimer = 0;           //  THE CURRENT TIMER FOR POISE RESET

        protected virtual void Awake()
        {
            character = GetComponent<CharacterManager>();
        }

        protected virtual void Start()
        {

        }

        protected virtual void Update()
        {
            HandlePoiseResetTimer();
        }

        public int CalculateHealthBasedOnVitalityLevel(int vitality)
        {
            float health = 0;

            //  CREATE AN EQUATION FOR HOW YOU WANT YOUR STAMINA TO BE CALCULATED

            health = vitality * 15;

            return Mathf.RoundToInt(health);
        }

        public int CalculateStaminaBasedOnEnduranceLevel(int endurance)
        {
            float stamina = 0;

            //  CREATE AN EQUATION FOR HOW YOU WANT YOUR STAMINA TO BE CALCULATED

            stamina = endurance * 10;

            return Mathf.RoundToInt(stamina);
        }

        public virtual void RegenerateStamina()
        {
            if (!character.IsOwner)
                return;

            if (character.characterNetworkManager.isSprinting.Value)
                return;

            if (character.isPerformingAction)
                return;

            staminaRegenerationTimer += Time.deltaTime;

            if (staminaRegenerationTimer >= staminaRegenerationDelay)
            {
                if (character.characterNetworkManager.currentStamina.Value < character.characterNetworkManager.maxStamina.Value)
                {
                    staminaTickTimer += Time.deltaTime;

                    if (staminaTickTimer >= 0.1)
                    {
                        staminaTickTimer = 0;
                        character.characterNetworkManager.currentStamina.Value += staminaRegenerationAmount;
                    }
                }
            }
        }

        public virtual void ResetStaminaRegenTimer(float previousStaminaAmount, float currentStaminaAmount)
        {
            //  WE ONLY WANT TO RESET THE REGENERATION STAMINA IF THE ACTION USED STAMINA
            //  WE DON'T WANT TO RESET THE REGENERTAION STAMINA IF WE ARE ALREADY REGENERATING STAMINA
            if (currentStaminaAmount < previousStaminaAmount)
            {
                staminaRegenerationTimer = 0;
            }
        }

        protected virtual void HandlePoiseResetTimer()
        {
            if (poiseResetTimer > 0)
            {
                poiseResetTimer -= Time.deltaTime;
            }
            else
            {
                totalPoiseDamage = 0;
            }
        }
    }
}