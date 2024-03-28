using Player;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : NetworkBehaviour
{
    private enum State
    {
        Waiting,
        Voting,
        Day,
        Night
    }

    public static GameManager Instance { get; private set; }

    public event System.EventHandler OnStateChanged;
    public NetworkVariable<ulong> killerClientId = new NetworkVariable<ulong>(20);

    [SerializeField] private Transform _playerPrefab;

    private NetworkVariable<State> _state = new NetworkVariable<State>(State.Waiting);
    private List<Player.Player> _playerList = new List<Player.Player>();

    private void Awake()
    {
        Instance = this;
        _state.OnValueChanged += State_OnValueChanged;
        killerClientId.OnValueChanged += KillerClientId_OnValueChanged;
    }

    public override void OnNetworkSpawn()
    {
        if (IsServer)
        {
            NetworkManager.Singleton.SceneManager.OnLoadEventCompleted += SceneManager_OnLoadEventCompleted;
        }
    }

    private void KillerClientId_OnValueChanged(ulong previousValue, ulong newValue)
    {
        Debug.Log("Value Changed");
        FindAndDealRolesServerRpc();
    }

    private void SceneManager_OnLoadEventCompleted(string sceneName, LoadSceneMode loadSceneMode, List<ulong> clientsCompleted, List<ulong> clientsTimedOut)
    {
        int randomIndex = Random.Range(0, MultiplayerManager.Instance.GetPlayerDataNetworkList().Count);
        killerClientId.Value = MultiplayerManager.Instance.GetPlayerDataNetworkList()[randomIndex].clientId;
        Debug.Log("Server-> Killer Name On Server: " + MultiplayerManager.Instance.GetPlayerDataFromClientId(killerClientId.Value).playerName);

        foreach (ulong clientId in NetworkManager.Singleton.ConnectedClientsIds)
        {
            Transform playerTransform = Instantiate(_playerPrefab);
            playerTransform.GetComponent<NetworkObject>().SpawnAsPlayerObject(clientId, true);
        }
    }

    [Rpc(SendTo.Server)]
    private void FindAndDealRolesServerRpc()
    {
        FindAndDealRolesClientRpc();
    }

    [Rpc(SendTo.ClientsAndHost)]
    private void FindAndDealRolesClientRpc()
    {
        FindAllPlayersAndDealRoles();
    }

    private void FindAllPlayersAndDealRoles()
    {
        //if (NetworkManager.Singleton.LocalClientId != Player.Player.LocalPlayer.OwnerClientId) return;

        // Find all players
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");

        // Add all players to the player list
        foreach (GameObject player in players)
        {
            _playerList.Add(player.GetComponent<Player.Player>());
        }

        // Deal roles
        // Set the killer
        Player.Player killer = _playerList.Find(p => p.GetPlayerClientId() == killerClientId.Value);
        if (killer != null)
        {
            killer?.SetPlayerRole(Roles.Killer);
            Debug.Log("Killer: " + killer.name);
        }
        else
            Debug.Log("There is no killer!");

        // Set all other players to villagers expect the killer
        foreach (Player.Player player in _playerList)
        {
            if (player != killer)
            {
                player.SetPlayerRole(Roles.Villager);
            }
        }
    }

    private void State_OnValueChanged(State previousValue, State newValue)
    {
        OnStateChanged?.Invoke(this, System.EventArgs.Empty);
    }
}
