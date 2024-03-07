using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

namespace EldenRing.NT
{
    public class NetworkObjectSpawner : MonoBehaviour
    {
        [Header("OBJECT")]
        [SerializeField] GameObject networkGameObject;
        [SerializeField] GameObject instantiatedGameObject;

        private void Awake()
        {

        }

        private void Start()
        {
            WorldObjectManager.instance.SpawnObject(this);
            gameObject.SetActive(false);
        }

        public void AttemptToSpawnObject()
        {
            if (networkGameObject != null)
            {
                instantiatedGameObject = Instantiate(networkGameObject);
                instantiatedGameObject.transform.position = transform.position;
                instantiatedGameObject.transform.rotation = transform.rotation;
                instantiatedGameObject.GetComponent<NetworkObject>().Spawn();
            }
        }
    }
}