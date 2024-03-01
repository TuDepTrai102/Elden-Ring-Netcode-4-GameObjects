using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EldenRing.NT
{
    public class WorldGameSessionManager : MonoBehaviour
    {
        public static WorldGameSessionManager instance;

        [Header("ACTIVE PLAYERS IN SESSION")]
        public List<PlayerManager> players = new List<PlayerManager>();

        private void Awake()
        {
            if (instance == null)
            {
                instance = this;
            }
            else
            {
                Destroy(gameObject);
            }
        }

        public void AddPlayerToActivePlayersList(PlayerManager player)
        {
            //  CHECK THE LIST, IF IT DOES NOT ALREADY CONTAIN THE PLAYER, ADD THEM
            if (!players.Contains(player))
            {
                players.Add(player);
            }

            //  CHECK THE LIST FOR NULL SLOTS, AND REMOVE THE NULL SLOTS
            for (int i = players.Count - 1; i > -1; i--)
            {
                if (players[i] == null)
                {
                    players.RemoveAt(i);
                }
            }
        }

        public void RemovePlayerFromActivePlayersList(PlayerManager player)
        {
            //  CHECK THE LIST, IF IT DOES CONTAIN THE PLAYER, REMOVE THEM
            if (players.Contains(player))
            {
                players.Remove(player);
            }

            //  CHECK THE LIST FOR NULL SLOTS, AND REMOVE THE NULL SLOTS
            for (int i = players.Count - 1; i > -1; i--)
            {
                if (players[i] == null)
                {
                    players.RemoveAt(i);
                }
            }
        }
    }
}