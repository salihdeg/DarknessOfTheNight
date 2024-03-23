using Network;
using TMPro;
using UI;
using UnityEngine;
using UnityEngine.UI;

public class CreateLobbyUI : MonoBehaviour
{
    public static CreateLobbyUI Instance { get; private set; }
    public TMP_InputField lobbyName;
    public Button createPrivate, createPublic,backButton;
   
    private void Awake()
    {
        Instance = this;
        createPrivate.onClick.AddListener(() =>
        {
            GameLobby.Instance.CreateLobby(lobbyName.text, true);
        });
        createPublic.onClick.AddListener(() =>
        {
            GameLobby.Instance.CreateLobby(lobbyName.text, false);
        });
        backButton.onClick.AddListener(() =>
        {
            Hide();
            MainMenuUI.Instance.Show();
        });
    }
    private void Start()
    {
        Hide();
    }
    public void Show()
    {
        gameObject.SetActive(true);
    }
    public void Hide()
    {
        gameObject.SetActive(false);
    }

}//class
