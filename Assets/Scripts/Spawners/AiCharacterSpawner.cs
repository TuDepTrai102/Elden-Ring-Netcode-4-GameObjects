using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

namespace EldenRing.NT
{
    public class AiCharacterSpawner : MonoBehaviour
    {
        [Header("CHARACTER")]
        [SerializeField] GameObject characterGameObject;
        [SerializeField] GameObject instantiatedGameObject;

        private void Awake()
        {
            
        }

        private void Start()
        {
            WorldAIManager.instance.SpawnCharacter(this);
            gameObject.SetActive(false);
        }

        public void AttemptToSpawnCharacter()
        {
            if (characterGameObject != null)
            {
                instantiatedGameObject = Instantiate(characterGameObject);
                instantiatedGameObject.transform.position = transform.position;
                instantiatedGameObject.transform.rotation = transform.rotation;
                instantiatedGameObject.GetComponent<NetworkObject>().Spawn();
                WorldAIManager.instance.AddCharacterToSpawnedCharactersList(instantiatedGameObject.GetComponent<AICharacterManager>());
            }
        }
    }
}