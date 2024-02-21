using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace EldenRing.NT
{
    public class UI_StatBar : MonoBehaviour
    {
        private Slider slider;
        private RectTransform rectTransform;

        [Header("BAR OPTIONS")]
        [SerializeField] protected bool scaleBarLengthWithStats = true;
        [SerializeField] protected float widthScaleMultiplier = 1;
        //  SECONDARY BAR BEHIND MAY BAR FOR POLISH EFFECT (YELLOW BAR THAT SHOWS HOW MUCH AN ACTION/DAMAGE TAKES AWAY FROM CURRENT STAT)
        
        protected virtual void Awake()
        {
            slider = GetComponent<Slider>();
            rectTransform = GetComponent<RectTransform>();
        }

        public virtual void SetStat(int newValue)
        {
            slider.value = newValue;
        }

        public virtual void SetMaxStat(int maxValue)
        {
            slider.maxValue = maxValue;
            slider.value = maxValue;

            if (scaleBarLengthWithStats )
            {
                rectTransform.sizeDelta = new Vector2(maxValue * widthScaleMultiplier, rectTransform.sizeDelta.y);

                //  RESETS THE POSITION OF THE BARS BASED ON THEIR LAYOUT GROUP'S SETTINGS
                PlayerUIManager.instance.playerUIHudManager.RefreshHUD();
            }
        }
    }
}