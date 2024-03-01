using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace EldenRing.NT
{
    [CreateAssetMenu(menuName = "A.I/States/Combat Stance")]
    public class CombatStanceState : AIState
    {
        [Header("ATTACKS")]
        public List<AICharacterAttackAction> aiCharacterAttacks;    //  A list of all possible attacks this character can do
        protected List<AICharacterAttackAction> potentialAttacks;   //  All attacks possible in this situation (based on angle, distance etc...)
        private AICharacterAttackAction choosenAttack;
        private AICharacterAttackAction previousAttack;
        protected bool hasAttack = false;

        [Header("COMBO")]
        [SerializeField] protected bool canPerformCombo = false;    //  If the character can perform a combo attack, after the initial attack
        [SerializeField] protected int chanceToPerformCombo = 25;   //  The chance (in percent) of the character to perform a combo on the next attack
        protected bool hasRolledForComboChance = false;             //  If we have already rolled for the chance during this state

        [Header("ENGAGEMENT DISTANCE")]
        public float maximumEngagementDistance = 5;                 //  The distance we have to be away from the target before we enter the pursue target state

        public override AIState Tick(AICharacterManager aiCharacter)
        {
            if (aiCharacter.isPerformingAction)
                return this;

            if (!aiCharacter.navMeshAgent.enabled)
                aiCharacter.navMeshAgent.enabled = true;

            //  IF YOU WANT THE A.I CHARACTER TO FACE AND TURN TOWARDS IT'S TARGET WHEN IT'S OUTSIDE IT'S FOV INCLUDE THIS
            if (!aiCharacter.aiCharacterNetworkManager.isMoving.Value)
            {
                if (aiCharacter.aiCharacterCombatManager.viewableAngle < -30 ||
                    aiCharacter.aiCharacterCombatManager.viewableAngle > 30)
                    aiCharacter.aiCharacterCombatManager.PivotTowardsTarget(aiCharacter);
            }

            aiCharacter.aiCharacterCombatManager.RotateTowardsAgent(aiCharacter);

            //  IF OUR TARGET IS NO LONGER PRESENT, SWITCH BACK TO IDLE
            if (aiCharacter.aiCharacterCombatManager.currentTarget == null)
                return SwitchState(aiCharacter, aiCharacter.idle);

            //  IF WE DO NOT HAVE AN ATTACK, GET ONE
            if (!hasAttack)
            {
                GetNewAttack(aiCharacter);
            }
            else
            {
                aiCharacter.attack.currentAttack = choosenAttack;
                //  ROLL FOR COMBO CHANCE
                return SwitchState(aiCharacter, aiCharacter.attack);
            }

            //  IF WE ARE OUTSIDE OF THE COMBAT ENGAGEMENT DISTANCE, SWITCH TO PURSUE TARGET STATE
            if (aiCharacter.aiCharacterCombatManager.distanceFromTarget > maximumEngagementDistance)
                return SwitchState(aiCharacter, aiCharacter.pursueTarget);

            NavMeshPath path = new NavMeshPath();
            aiCharacter.navMeshAgent.CalculatePath(aiCharacter.aiCharacterCombatManager.currentTarget.transform.position, path);
            aiCharacter.navMeshAgent.SetPath(path);

            return this;
        }

        protected virtual void GetNewAttack(AICharacterManager aiCharacter)
        {
            potentialAttacks = new List<AICharacterAttackAction>();

            foreach (var potentialAttack in aiCharacterAttacks)
            {
                //  IF WE ARE TOO CLOSE FOR THIS ATTACK, CHECK THE NEXT
                if (potentialAttack.minimumAttackDistance > aiCharacter.aiCharacterCombatManager.distanceFromTarget)
                    continue;

                //  IF WE ARE TOO FAR FOR THIS ATTACK, CHECK THE NEXT
                if (potentialAttack.maximumAttackDistance < aiCharacter.aiCharacterCombatManager.distanceFromTarget)
                    continue;

                //  IF THE TARGET IS OUTSIDE MINIMUM FIELD OF VIEW FOR THIS ATTACK, CHECK THE NEXT
                if (potentialAttack.minimumAttackAngle > aiCharacter.aiCharacterCombatManager.viewableAngle)
                    continue;

                //  IF THE TARGET IS OUTSIDE MAXIMUM FIELD OF VIEW FOR THIS ATTACK, CHECK THE NEXT
                if (potentialAttack.maximumAttackAngle < aiCharacter.aiCharacterCombatManager.viewableAngle)
                    continue;

                potentialAttacks.Add(potentialAttack);
            }

            if (potentialAttacks.Count <= 0)
                return;

            var totalWeight = 0;

            foreach (var attack in potentialAttacks)
            {
                totalWeight += attack.attackWeight;
            }

            var randomWeightValue = Random.Range(1, totalWeight + 1);
            var processWeight = 0;

            foreach (var attack in potentialAttacks)
            {
                processWeight += attack.attackWeight;

                if (randomWeightValue <= processWeight)
                {
                    choosenAttack = attack;
                    previousAttack = choosenAttack;
                    hasAttack = true;
                }
            }
        }

        protected virtual bool RollForOutcomeChance(int outcomeChance)
        {
            bool outcomeWillBePerform = false;

            int randomPercentage = Random.Range(0, 100);

            if (randomPercentage < outcomeChance)
                outcomeWillBePerform = true;

            return outcomeWillBePerform;
        }

        protected override void ResetStateFlags(AICharacterManager aiCharacter)
        {
            base.ResetStateFlags(aiCharacter);

            hasAttack = false;
            hasRolledForComboChance = false;
        }
    }
}