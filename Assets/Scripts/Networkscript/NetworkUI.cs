using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Unity.Netcode;
using TMPro;
using Unity.Netcode.Transports.UTP;


public class NetworkUI : NetworkBehaviour
{
    
    [SerializeField] private Button hostButton;
    [SerializeField] private Button clientButton;

    /// to spawn the sniper
    [SerializeField] private GameObject SpawnPointWeapon;
    [SerializeField] private GameObject SpawnPointBodies;
    
    private SpawnObject spawnWeapon;
    private SpawnObject spawnBodies;
    // 
    [SerializeField] private TextMeshProUGUI  playerCountText;

    private NetworkVariable<int> playersNum = new NetworkVariable<int>(0, NetworkVariableReadPermission.Everyone);
    // Update is called once per frame

    public string ipAdress = "127.0.0.1";
   

    void Awake()
    {
        spawnWeapon = SpawnPointWeapon.GetComponent<SpawnObject>();
        spawnBodies = SpawnPointBodies.GetComponent<SpawnObject>();
        
        hostButton.onClick.AddListener(
        () =>{
     
            NetworkManager.Singleton.StartHost();            
        });



        clientButton.onClick.AddListener(
        () =>{
                   // set address and port
            NetworkManager.Singleton.GetComponent<UnityTransport>().SetConnectionData(ipAdress, 7777);
            NetworkManager.Singleton.StartClient();
        });



        NetworkManager.Singleton.OnServerStarted +=  () =>
        {
            spawnWeapon.SpawnedObjects(); 
            spawnBodies.SpawnedObjects();
        };


    }

    // update the player count
    private void Update()
    { 
        playerCountText.text = "Player: " + playersNum.Value.ToString();

        if (!IsServer) return;
        playersNum.Value = NetworkManager.Singleton.ConnectedClients.Count;
    }

    public void IpAdressInput(string ip)
    {
        this.ipAdress = ip;
    }
}