using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

namespace EldenRing.NT {
    public class CharacterCombatManager : NetworkBehaviour
    {
        protected CharacterManager character;

        [Header("LAST ATTACK ANIMATION PERFORMED")]
        public string lastAttackAnimationPerformed;

        [Header("LOCK ON/ATTACK TARGET")]
        public CharacterManager currentTarget;

        [Header("ATTACK TYPE")]
        public AttackType currentAttackType;

        [Header("LOCK ON TRANSFORM")]
        public Transform lockOnTransform;

        protected virtual void Awake()
        {
            character = GetComponent<CharacterManager>();
        }

        public virtual void SetTarget(CharacterManager newTarget)
        {
            if (character.IsOwner)
            {
                if (newTarget != null)
                {
                    currentTarget = newTarget;
                    character.characterNetworkManager.currentTargetNetworkObjectID.Value = newTarget.GetComponent<NetworkObject>().NetworkObjectId;
                }
                else
                {
                    currentTarget = null;
                }
            }
        }
    }
}
