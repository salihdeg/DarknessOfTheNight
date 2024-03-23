using UnityEngine;
using UnityEngine.UI;
using Network;
using System.Collections.Generic;
using Unity.Services.Lobbies.Models;
using TMPro;
using UI;

public class LobbyListUI : MonoBehaviour
{
    [SerializeField] private Button backButton, joinLobbyWithCodeButton;
    [SerializeField] Transform lobbyContainer, lobbyTemplate;
    [SerializeField] private TMP_InputField code;
    private void Awake()
    {
        backButton.onClick.AddListener(() =>
        {
            Hide();
            MainMenuUI.Instance.Show();
        });
        joinLobbyWithCodeButton.onClick.AddListener(() =>
        {
            GameLobby.Instance.JoinWithCode(code.text);
        });
    }
    private void Start()
    {
        GameLobby.Instance.OnLobbyListChanged += GameLobby_OnLobbyListChanged;
        Hide();
    }

    private void GameLobby_OnLobbyListChanged(object sender, GameLobby.OnLobbyListChangedEventArgs e)
    {
        UpdateLobbyList(e.lobbyList);
    }
    public void UpdateLobbyList(List<Lobby> lobbyList)
    {
        foreach (Transform child in lobbyContainer)
        {
            if (child == lobbyTemplate)
            {
                continue;
            }
            Destroy(child.gameObject);
        }
        foreach (Lobby lobby in lobbyList)
        {
            Transform lobbyTransform = Instantiate(lobbyTemplate,lobbyContainer);
            lobbyTransform.gameObject.SetActive(true);
            lobbyTransform.GetComponent<LobbyListSingleUI>().SetLobby(lobby);
        }
    }

    public void Show()
    {
        gameObject.SetActive(true);
    }
    public void Hide()
    {
        gameObject.SetActive(false);
    }
    private void OnDestroy()
    {
        GameLobby.Instance.OnLobbyListChanged -= GameLobby_OnLobbyListChanged;
    }

}//class
