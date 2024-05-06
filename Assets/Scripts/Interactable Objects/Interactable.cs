using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

namespace EldenRing.NT
{
    public class Interactable : NetworkBehaviour
    {
        public string interactableText; //  TEXT PROMT WHEN ENTERING THE INTERACTION COLLIDER (PICK UP ITEM, PULL LEVEL ETC...)
        [SerializeField] protected Collider interactableCollider;   //  COLLIDER THAT CHECKS FOR PLAYER INTERACTION
        [SerializeField] protected bool hostOnlyInteractable = true;    //  WHEN ENABLED, OBJECT CANNOT BE INTERACTED WITH BY CO-OP PLAYERS

        protected virtual void Awake()
        {
            //  CHECK IF IT'S NULL, IN SOME CASES YOU MAY WANT TO MANUALLY ASIGN A COLLIDER AS A CHILD OBJECT (DEPENDING ON INTERACTABLE)
            if (interactableCollider == null)
                interactableCollider = GetComponent<Collider>();
        }

        protected virtual void Start()
        {

        }

        public virtual void Interact(PlayerManager player)
        {
            Debug.Log("YOU HAVE INTERACTED !!!");

            if (!player.IsOwner)
                return;

            //  REMOVE THE INTERACTION FROM THE PLAYER
            interactableCollider.enabled = false;
            player.playerInteractionManager.RemoveInteractionFromList(this);
            PlayerUIManager.instance.playerUIPopUpManager.CloseAllPopUpWindows();
        }

        public virtual void OnTriggerEnter(Collider other)
        {
            PlayerManager player = other.GetComponent<PlayerManager>();

            if (player != null)
            {
                if (!player.playerNetworkManager.IsHost && hostOnlyInteractable)
                    return;

                if (!player.IsOwner)
                    return;

                //  PAST THE INTERACTION TO THE PLAYER
                player.playerInteractionManager.AddInteractionToList(this);
            }
        }

        public virtual void OnTriggerExit(Collider other)
        {
            PlayerManager player = other.GetComponent<PlayerManager>();

            if (player != null)
            {
                if (!player.playerNetworkManager.IsHost && hostOnlyInteractable)
                    return;

                if (!player.IsOwner)
                    return;

                //  REMOVE THE INTERACTION FROM THE PLAYER
                player.playerInteractionManager.RemoveInteractionFromList(this);
                PlayerUIManager.instance.playerUIPopUpManager.CloseAllPopUpWindows();
            }
        }
    }
}