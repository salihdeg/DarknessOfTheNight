using Network;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class MainMenuUI : MonoBehaviour
    {
        public static MainMenuUI Instance { get; private set; }
        public Button quickJoin, createLobby, joinLobby, options, exit;
        private void Awake()
        {
            Instance = this;
            quickJoin.onClick.AddListener(() =>
            {
                GameLobby.Instance.QuickJoin();
            });
            createLobby.onClick.AddListener(() =>
            {
                Hide();
                CreateLobbyUI.Instance.Show();
            });
            joinLobby.onClick.AddListener(() =>
            {
                Hide();
                LobbyListUI.Instance.Show();
            });
            options.onClick.AddListener(() =>
            {

            });
            exit.onClick.AddListener(() =>
            {
                Application.Quit();
            });


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

}

