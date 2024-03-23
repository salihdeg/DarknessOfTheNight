using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using Unity.Services.Relay.Models;
using Unity.Services.Relay;
using UnityEngine;
using Unity.Netcode.Transports.UTP;
using Unity.Netcode;
using Unity.Networking.Transport.Relay;
using UnityEngine.SceneManagement;

namespace Network
{
    public class GameLobby : MonoBehaviour
    {
        public static GameLobby Instance { get; private set; }

        private const string KEY_RELAY_JOIN_CODE = "RelayJoinCode";
        private const int GAME_MAX_PLAYER_COUNT_TEMP = 10;

        // EVENTS
        public event System.EventHandler OnCreateLobbyStarted;
        public event System.EventHandler OnCreateLobbyFailed;
        public event System.EventHandler OnJoinStarted;
        public event System.EventHandler OnJoinFailed;
        public event System.EventHandler OnQuickJoinFailed;
        public event System.EventHandler<OnLobbyListChangedEventArgs> OnLobbyListChanged;
        public class OnLobbyListChangedEventArgs : System.EventArgs
        {
            public List<Lobby> lobbyList;
        }

        private Lobby _joinedLobby;

        // TIMERS
        private float _heartbeatTimer;
        private float _listLobbiesTimer;

        private void Awake()
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            InitializeUnityAuthentication();
        }

        // Unity servislerine anonim þekilde giriþ yap
        // (steam ile giriþ yap buraya eklenebilir)
        private async void InitializeUnityAuthentication()
        {
            // Eðer giriþ yapýlmamýþsa
            if (UnityServices.State != ServicesInitializationState.Initialized)
            {
                InitializationOptions options = new InitializationOptions();
                options.SetProfile(Random.Range(0, 10000).ToString()); // Rastgele bir profil oluþtur (kalýcý profil ilerde eklenecek)
                await UnityServices.InitializeAsync(options);
                await AuthenticationService.Instance.SignInAnonymouslyAsync(); // Anonim giriþ yap
            }
        }

        private void Update()
        {
            HandleHeartbeat();
            HandlePeriodicListLobbies();
        }

        // 20 saniye'de bir Unity Servislerine lobi'nin aktif olup olmadýðýný bildirmek için kalp atýþý
        // (SADECE HOST GERÇEKLEÞTÝREBÝLÝR)
        private async void HandleHeartbeat()
        {
            if (!IsLobbyHost()) return;

            _heartbeatTimer -= Time.deltaTime;
            if (_heartbeatTimer <= 0f)
            {
                Debug.Log("HEARTBEAT!");
                await LobbyService.Instance.SendHeartbeatPingAsync(_joinedLobby.Id);
                float _heartbeatTimerMax = 20f;
                _heartbeatTimer = _heartbeatTimerMax;
            }
        }

        // 3 saniyede bir düzenli olarak müsait lobileri güncellemek için
        private void HandlePeriodicListLobbies()
        {
            if (!AuthenticationService.Instance.IsSignedIn) return;
            // Sadece Lobi sahnesindeyken lobileri güncellemesi için!
            // if (SceneManager.GetActiveScene().name != Loader.Scene.LobbyScene.ToString()) return;
            if (_joinedLobby != null) return;

            _listLobbiesTimer -= Time.deltaTime;
            if (_listLobbiesTimer <= 0f)
            {
                float listLobbiesTimerMax = 3f;
                _listLobbiesTimer = listLobbiesTimerMax;
                ListLobbies();
            }
        }

        // RELAY
        // Host'un ip'sini lobi için public hale getiriyor gibi düþünebiliriz

        // Bu fonksiyon Relay'i baþlatmak için kullanýlýyor,
        // MAXIMUM Oyuncu sayýsý burada belirleniyor!!!
        private async Task<Allocation> AllocateRelay()
        {
            try
            {
                // TODO: Change max player count on UI
                Allocation allocation = await RelayService.Instance.CreateAllocationAsync(GAME_MAX_PLAYER_COUNT_TEMP - 1);
                return allocation;
            }
            catch (RelayServiceException e)
            {
                Debug.Log(e);
                return default;
            }
        }

        // Oluþturulan relay'e baðlanmak için gereken kod,
        // lobi giriþ kodu deðil! lobiye girdikten sonra arkaplanda host'un ip'sine baðlandýðýmýz kod!
        private async Task<string> GetRelayJoinCode(Allocation allocation)
        {
            try
            {
                string relayJoinCode = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);
                return relayJoinCode;
            }
            catch (RelayServiceException e)
            {
                Debug.Log(e);
                return default;
            }
        }

        // Relay kod ile host'un ip'sine baðlanmamýzý saðlýyor
        private async Task<JoinAllocation> JoinRelay(string joinCode)
        {
            try
            {
                return await RelayService.Instance.JoinAllocationAsync(joinCode);
            }
            catch (RelayServiceException e)
            {
                Debug.Log(e);
                return default;
            }
        }

        // Lobiye girdikten sonra lobi sahibi olup olmadýðýný kontrol etmek için
        public bool IsLobbyHost()
        {
            return _joinedLobby != null && AuthenticationService.Instance.PlayerId == _joinedLobby.HostId;
        }

        // Lobileri listeleyip, listelenmiþ lobileri event'in bir deðeri olarak geri döndüren fonksyion
        public async void ListLobbies()
        {
            try
            {
                QueryLobbiesOptions queryLobbiesOptions = new QueryLobbiesOptions
                {
                    Filters = new List<QueryFilter>
                    {
                        new QueryFilter (QueryFilter.FieldOptions.AvailableSlots, "0", QueryFilter.OpOptions.GT)
                    }
                };
                QueryResponse queryResponse = await LobbyService.Instance.QueryLobbiesAsync(queryLobbiesOptions);
                OnLobbyListChanged?.Invoke(this, new OnLobbyListChangedEventArgs { lobbyList = queryResponse.Results });
            }
            catch (LobbyServiceException e)
            {
                Debug.Log(e);
            }
        }

        // Lobiy oluþturma fonksyionu!
        // Maximum oyuncu sayýsýný arayüz'de belirtilmesi lazým!
        public async void CreateLobby(string lobbyName, bool isPrivate)
        {
            OnCreateLobbyStarted?.Invoke(this, System.EventArgs.Empty);
            try
            {
                _joinedLobby = await LobbyService.Instance.CreateLobbyAsync(lobbyName, GAME_MAX_PLAYER_COUNT_TEMP,
                    new CreateLobbyOptions { IsPrivate = isPrivate });

                Allocation allocation = await AllocateRelay();

                string relayJoinCode = await GetRelayJoinCode(allocation);
                await LobbyService.Instance.UpdateLobbyAsync(_joinedLobby.Id, new UpdateLobbyOptions
                {
                    Data = new Dictionary<string, DataObject>
                {
                    {KEY_RELAY_JOIN_CODE, new DataObject(DataObject.VisibilityOptions.Member, relayJoinCode) }
                }
                });

                NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(new RelayServerData(allocation, "dtls"));

                // TODO:
                MultiplayerManager.Instance.StartHost();
                // TODO: Sahne sýrasý ve dizayný konuþulduktan sonra düzenlenecek!!!
                Loader.LoadNetwork(Loader.Scene.CharacterSelectScene);
            }
            catch (LobbyServiceException e)
            {
                OnCreateLobbyFailed?.Invoke(this, System.EventArgs.Empty);
                Debug.Log(e);
            }
        }

        // Otomatik olarak müsait olan ilk public oyuna girer
        public async void QuickJoin()
        {
            OnJoinStarted?.Invoke(this, System.EventArgs.Empty);
            try
            {
                _joinedLobby = await LobbyService.Instance.QuickJoinLobbyAsync();
                string relayJoinCode = _joinedLobby.Data[KEY_RELAY_JOIN_CODE].Value;

                JoinAllocation joinAllocation = await JoinRelay(relayJoinCode);
                NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(new RelayServerData(joinAllocation, "dtls"));

                //MultiplayerManager.Instance.StartClient();
            }
            catch (LobbyServiceException e)
            {
                OnQuickJoinFailed?.Invoke(this, System.EventArgs.Empty);
                Debug.Log(e);
            }
        }

        // private lobi'lere Katýlma kodu ile girmek için!
        public async void JoinWithCode(string lobbyCode)
        {
            OnJoinStarted?.Invoke(this, System.EventArgs.Empty);
            try
            {
                _joinedLobby = await LobbyService.Instance.JoinLobbyByCodeAsync(lobbyCode);
                string relayJoinCode = _joinedLobby.Data[KEY_RELAY_JOIN_CODE].Value;

                JoinAllocation joinAllocation = await JoinRelay(relayJoinCode);
                NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(new RelayServerData(joinAllocation, "dtls"));
                //MultiplayerManager.Instance.StartClient();
            }
            catch (LobbyServiceException e)
            {
                OnJoinFailed?.Invoke(this, System.EventArgs.Empty);
                Debug.Log(e);
            }
        }

        // Normal odaya girmek için
        public async void JoinWithId(string lobbyId)
        {
            OnJoinStarted?.Invoke(this, System.EventArgs.Empty);
            try
            {
                _joinedLobby = await LobbyService.Instance.JoinLobbyByIdAsync(lobbyId);
                string relayJoinCode = _joinedLobby.Data[KEY_RELAY_JOIN_CODE].Value;

                JoinAllocation joinAllocation = await JoinRelay(relayJoinCode);
                NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(new RelayServerData(joinAllocation, "dtls"));
                //MultiplayerManager.Instance.StartClient();
            }
            catch (LobbyServiceException e)
            {
                OnJoinFailed?.Invoke(this, System.EventArgs.Empty);
                Debug.Log(e);
            }
        }

        // OYUN EKRANINA GEÇERKEN LOBÝYÝ SÝLMEK ÝÇÝN
        // LOBÝ SÝLÝNDÝKTEN SONRA OYUN LOADER.LOADNETWORK ÝLE SAHNE YÜKLENECEK!
        public async void DeleteLobby()
        {
            if (_joinedLobby == null) return;

            try
            {
                await LobbyService.Instance.DeleteLobbyAsync(_joinedLobby.Id);
                _joinedLobby = null;
            }
            catch (LobbyServiceException e)
            {
                Debug.Log(e);
            }
        }

        // Manuel yolla lobiden çýkmak için!
        public async void LeaveLobby()
        {
            if (_joinedLobby == null) return;

            try
            {
                await LobbyService.Instance.RemovePlayerAsync(_joinedLobby.Id, AuthenticationService.Instance.PlayerId);
                _joinedLobby = null;
            }
            catch (LobbyServiceException e)
            {
                Debug.Log(e);
            }
        }
        
        // Oyuncuyu atmak için
        public async void KickPlayer(string playerId)
        {
            if (!IsLobbyHost()) return;

            try
            {
                await LobbyService.Instance.RemovePlayerAsync(_joinedLobby.Id, playerId);
            }
            catch (LobbyServiceException e)
            {
                Debug.Log(e);
            }
        }

        public Lobby GetLobby()
        {
            return _joinedLobby;
        }
    }
}
