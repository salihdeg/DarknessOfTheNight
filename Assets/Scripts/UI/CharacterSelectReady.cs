using Network;
using System.Collections.Generic;
using Unity.Netcode;

public class CharacterSelectReady : NetworkBehaviour
{
    public static CharacterSelectReady Instance { get; private set; }

    public event System.EventHandler OnReadyChanged;

    private Dictionary<ulong, bool> _playerReadyDictionary;

    private void Awake()
    {
        Instance = this;

        _playerReadyDictionary = new Dictionary<ulong, bool>();
    }

    public void SetPlayerReady()
    {
        SetPlayerReadyServerRPC();
    }

    [ServerRpc(RequireOwnership = false)]
    private void SetPlayerReadyServerRPC(ServerRpcParams serverRpcParams = default)
    {
        SetPlayerReadyClientRPC(serverRpcParams.Receive.SenderClientId);
        _playerReadyDictionary[serverRpcParams.Receive.SenderClientId] = true;

        bool allClientsReady = true;
        foreach (ulong clientId in NetworkManager.Singleton.ConnectedClientsIds)
        {
            if (!_playerReadyDictionary.ContainsKey(clientId) || !_playerReadyDictionary[clientId])
            {
                // This player is NOT ready!
                allClientsReady = false;
                break;
            }
        }

        if (allClientsReady)
        {
            GameLobby.Instance.DeleteLobby();
            Loader.LoadNetwork(Loader.Scene.GameScene);
        }
    }

    [ClientRpc]
    private void SetPlayerReadyClientRPC(ulong clientId)
    {
        _playerReadyDictionary[clientId] = true;

        OnReadyChanged?.Invoke(this, System.EventArgs.Empty);
    }

    public bool IsPlayerReady(ulong clientId)
    {
        return _playerReadyDictionary.ContainsKey(clientId) && _playerReadyDictionary[clientId];
    }
}
