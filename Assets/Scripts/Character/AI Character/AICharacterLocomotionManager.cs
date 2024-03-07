using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace EldenRing.NT
{
    public class AICharacterLocomotionManager : CharacterLocomotionManager
    {
        protected override void Start()
        {
            base.Start();

            //  SET Y VELOCITY MAKE SURE A.I NOT WALKING IN THE AIR
            yVelocity.y = groundedYVelocity;
        }

        public void RotateTowardsAgent(AICharacterManager aiCharacter)
        {
            if (aiCharacter.aiCharacterNetworkManager.isMoving.Value)
            {
                aiCharacter.transform.rotation = aiCharacter.navMeshAgent.transform.rotation;
            }
        }
    }
}