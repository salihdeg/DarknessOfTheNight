using Unity.Netcode;
using UnityEngine;

namespace Player
{
    public class Player : NetworkBehaviour
    {
        private PlayerData _playerData;
        [SerializeField] private string _playerName;
        public static Player LocalPlayer { get; private set; }

        public override void OnNetworkSpawn()
        {
            if (IsOwner)
            {
                LocalPlayer = this;
                _playerData = MultiplayerManager.Instance.GetPlayerData();
                _playerName = _playerData.playerName.ToString();
                gameObject.name = _playerName;
                Debug.Log("My client Id: " + OwnerClientId);
            }
            else
            {
                _playerData = MultiplayerManager.Instance.GetPlayerDataFromClientId(OwnerClientId);
                _playerName = _playerData.playerName.ToString();
                gameObject.name = _playerName;
            }
        }

        public ulong GetPlayerClientId()
        {
            return _playerData.clientId;
        }

        public string GetPlayerName()
        {
            return _playerName;
        }

        public void SetPlayerRole(Roles role)
        {
            switch (role)
            {
                case Roles.Killer:
                    gameObject.AddComponent<Killer>();
                    break;
                case Roles.Villager:
                    gameObject.AddComponent<Villager>();
                    break;
            }
        }

        // On network connect, set the player's role
        
    }
}
