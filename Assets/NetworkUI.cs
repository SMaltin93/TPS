using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Unity.Netcode;
using TMPro;

public class NetworkUI : NetworkBehaviour
{
    
    [SerializeField] private Button hostButton;
    [SerializeField] private Button clientButton;

    // 
    [SerializeField] private TextMeshProUGUI  playerCountText;

    private NetworkVariable<int> playersNum = new NetworkVariable<int>(0, NetworkVariableReadPermission.Everyone);
    // Update is called once per frame
    void Awake()
    {
        
        hostButton.onClick.AddListener(
        () =>{
            NetworkManager.Singleton.StartHost();
        });
        
        clientButton.onClick.AddListener(
        () =>{
            NetworkManager.Singleton.StartClient();
        });
    }

    // update the player count
    private void Update()
    { 
        playerCountText.text = "Player: " + playersNum.Value.ToString();

        if (!IsServer) return;
        playersNum.Value = NetworkManager.Singleton.ConnectedClients.Count;
    }
}
