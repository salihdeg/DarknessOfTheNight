using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterColorSelectSingleUI : MonoBehaviour
{
    [SerializeField] private int _colorId;
    [SerializeField] private Image _image;
    [SerializeField] private GameObject _selectedGameObject;

    private void Awake()
    {
        GetComponent<Button>().onClick.AddListener(() =>
        {
            MultiplayerManager.Instance.ChangePlayerColor(_colorId);
        });
    }

    private void Start()
    {
        MultiplayerManager.Instance.OnPlayerDataNetworkListChanged += MultiplayerManager_OnPlayerDataNetworkListChanged;
        _image.color = MultiplayerManager.Instance.GetPlayerColor(_colorId);
        UpdateIsSelected();
    }

    private void MultiplayerManager_OnPlayerDataNetworkListChanged(object sender, System.EventArgs e)
    {
        UpdateIsSelected();
    }

    private void UpdateIsSelected()
    {
        if (MultiplayerManager.Instance.GetPlayerData().colorId == _colorId)
        {
            _selectedGameObject.SetActive(true);
        }
        else
        {
            _selectedGameObject.SetActive(false);
        }
    }

    private void OnDestroy()
    {
        MultiplayerManager.Instance.OnPlayerDataNetworkListChanged -= MultiplayerManager_OnPlayerDataNetworkListChanged;
    }
}
