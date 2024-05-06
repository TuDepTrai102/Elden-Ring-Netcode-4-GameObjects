using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SG
{
    public class ManualAnimationPreview : MonoBehaviour
    {
        [SerializeField] AnimationDemoController controller;

        [Header("Manual Animation")]
        [SerializeField] bool playManualAnimation;
        [SerializeField] string animationName;
        [SerializeField] bool playManualMainHandWeaponFX;
        [SerializeField] bool playManualOffHandWeaponFX;

        private void Update()
        {
            if (playManualAnimation)
            {
                playManualAnimation = false;
                controller.HandleManualAnimation(animationName, playManualOffHandWeaponFX, playManualMainHandWeaponFX);
            }
        }
    }
}
