using TMPro;
using UnityEngine;

public class GameInformationUI : MonoBehaviour
{
    public static GameInformationUI Instance { get; private set; }

    [SerializeField] private TextMeshProUGUI _roleText;

    private void Awake()
    {
        Instance = this;
    }

    public void SetRoleText(Roles role)
    {
        _roleText.text = role.ToString();
        Debug.Log("Role setted to " + role.ToString());
    }
}
