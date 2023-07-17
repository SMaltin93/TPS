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

    /// to spawn the sniper
    [SerializeField] private GameObject SpawnPoint;
    private SpawnWeapon spawnWeapon;
    // 
    [SerializeField] private TextMeshProUGUI  playerCountText;

    private NetworkVariable<int> playersNum = new NetworkVariable<int>(0, NetworkVariableReadPermission.Everyone);
    // Update is called once per frame

    void Awake()
    {
        spawnWeapon = SpawnPoint.GetComponent<SpawnWeapon>();
        
        hostButton.onClick.AddListener(
        () =>{
            NetworkManager.Singleton.StartHost();            
        });
        clientButton.onClick.AddListener(
        () =>{
            NetworkManager.Singleton.StartClient();
        });

        NetworkManager.Singleton.OnServerStarted +=  () =>
        {
            spawnWeapon.SpawnedObjects();  
        };
    }

    // update the player count
    private void Update()
    { 
        playerCountText.text = "Player: " + playersNum.Value.ToString();

        if (!IsServer) return;
        playersNum.Value = NetworkManager.Singleton.ConnectedClients.Count;
    }
}
