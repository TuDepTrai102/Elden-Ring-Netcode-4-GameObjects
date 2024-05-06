using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EldenRing.NT
{
    [CreateAssetMenu(menuName = "A.I/Actions/Attack")]
    public class AICharacterAttackAction : ScriptableObject
    {
        [Header("ATTACK")]
        [SerializeField] private string attackAnimation;

        [Header("COMBO ACTION")]
        public AICharacterAttackAction comboAction; //  THE COMBO ACTION OF THIS ATTACK ACTION

        [Header("ACTION VALUES")]
        [SerializeField] AttackType attackType;
        public int attackWeight = 50;
        //  ATTACK CAN BE REPEATED
        public float actionRecoveryTime = 1.5f; //  THE TIME BEFORE THE CHARACTER CAN MAKE ANOTHER ATTACK AFTER PERFORMING THIS ONE
        public float minimumAttackAngle = -35;
        public float maximumAttackAngle = 35;
        public float minimumAttackDistance = 0;
        public float maximumAttackDistance = 2;

        public void AttemptToPerformAction(AICharacterManager aiCharacter)
        {
            //  DOES YOUR A.I ACT LIKE A PLAYER CHARACTER (LIKE AN INVADER A.I?) IF SO USE THIS
            //aiCharacter.characterAnimatorManager.PlayTargetAttackActionAnimation(attackType, attackAnimation, true);

            //  DOES YOUR A.I USE SIMPLE ATTACKS THAT ARE PURELY ANIMATION BASED (NOT EQUIPMENT/ITEM BASED) IF SO USE THIS
            aiCharacter.characterAnimatorManager.PlayTargetActionAnimation(attackAnimation, true);
        }
    }
}