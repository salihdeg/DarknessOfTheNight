using Player;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class MultiplayerManager : NetworkBehaviour
{
    public static MultiplayerManager Instance { get; private set; }

    public const string PLAYER_PREFS_PLAYER_NAME_MULTIPLAYER = "PlayerNameMultiplayer";

    // EVENTS
    public event System.EventHandler OnPlayerDataNetworkListChanged;

    [SerializeField] private List<Color> _playerColorList;

    private NetworkList<PlayerData> _playerDataNetworkList;
    private string _playerName;

    private void Awake()
    {
        Instance = this;
        DontDestroyOnLoad(this);

        // Oyuncu ismini bir kere deðiþtirdiyse PlayerPrefs'ten al, deðiþmediyse Random bir numara ver 
        _playerName = PlayerPrefs.GetString(PLAYER_PREFS_PLAYER_NAME_MULTIPLAYER, "Player" + Random.Range(100, 10000));

        _playerDataNetworkList = new NetworkList<PlayerData>();
        _playerDataNetworkList.OnListChanged += PlayerDataNetworkList_OnListChanged;
    }

    private void PlayerDataNetworkList_OnListChanged(NetworkListEvent<PlayerData> changeEvent)
    {
        OnPlayerDataNetworkListChanged?.Invoke(this, System.EventArgs.Empty);
    }

    public void StartHost()
    {
        //NetworkManager.Singleton.ConnectionApprovalCallback += NetworkManager_ConnectionApprovalCallback;
        //NetworkManager.Singleton.OnClientConnectedCallback += NetworkManager_OnClientConnectedCallback;
        //NetworkManager.Singleton.OnClientDisconnectCallback += NetworkManager_Server_OnClientDisconnectCallback;
        NetworkManager.Singleton.StartHost();
    }
}
