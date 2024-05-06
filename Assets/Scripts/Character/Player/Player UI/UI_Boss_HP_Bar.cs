using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace EldenRing.NT
{
    public class UI_Boss_HP_Bar : UI_StatBar
    {
        [SerializeField] AIBossCharacterManager aiBossCharacter;

        public void EnableBossHPBar(AIBossCharacterManager boss)
        {
            aiBossCharacter = boss;
            aiBossCharacter.aiCharacterNetworkManager.currentHealth.OnValueChanged += OnBossHPChanged;
            SetMaxStat(aiBossCharacter.aiCharacterNetworkManager.maxHealth.Value);
            SetStat(aiBossCharacter.aiCharacterNetworkManager.currentHealth.Value);
            GetComponentInChildren<TextMeshProUGUI>().text = aiBossCharacter.characterName;
        }

        private void OnDestroy()
        {
            aiBossCharacter.aiCharacterNetworkManager.currentHealth.OnValueChanged -= OnBossHPChanged;
        }

        private void OnBossHPChanged(int oldValue, int newValue)
        {
            SetStat(newValue);

            if (newValue < 0)
            {
                RemoveHPBar(2.5f);
            }
        }

        public void RemoveHPBar(float time)
        {
            Destroy(gameObject, time);
        }
    }
}