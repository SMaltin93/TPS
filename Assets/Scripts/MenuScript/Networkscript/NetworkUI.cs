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

    public string ClientIpAdress = "127.0.0.1";

    private string hostAdress = "0.0.0.0";
    private ushort port = 3000;


   // private MenuManager menuManager;

   // start the player att random position by overiding the spawn position
   

    void Start()
    {
        spawnWeapon = SpawnPointWeapon.GetComponent<SpawnObject>();
        spawnBodies = SpawnPointBodies.GetComponent<SpawnObject>();

         NetworkManager.Singleton.OnServerStarted +=  () =>
        {
            Debug.Log("Server started event fired");
            Invoke("SpawnItems", 0.5f);  // Delay of 0.5 seconds
        };

       // menuManager = GetComponent<MenuManager>();
       StartTheGameAs();

    }

    // update the player count
    private void Update()
    { 
        playerCountText.text = "Player: " + playersNum.Value.ToString();

        if (!IsServer) return;
        playersNum.Value = NetworkManager.Singleton.ConnectedClients.Count;

        // quit the game by pressing the esc 3 seconds
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Invoke("QuitGame", 3f);
        }
    }


    public void StartTheGameAs() {

        string playrtState = MenuManager.gameState;

        // wait for NetworkManager to be initialized

        if (playrtState == "host")
        {
            StartTheGameAsHost();   
        }
        else if (playrtState == "client")
        {
            if (MenuManager.ipAdress == "") {
                Debug.Log(" NetworkUI  client ip adress NULL : " + ClientIpAdress);
                StartTheGameAsClient();
            } else {
                ClientIpAdress = MenuManager.ipAdress;
                Debug.Log(" NetworkUI  client ip adress: " + ClientIpAdress);
                StartTheGameAsClient();
            }
            
        } 
    }

    private  void SpawnItems()
    {
        Debug.Log("spawn items");
        spawnWeapon.SpawnedObjects(); 
        spawnBodies.SpawnedObjects();
    }


    // // start as host 

    private void StartTheGameAsHost() {
        Debug.Log("start as host");

       NetworkManager.Singleton.GetComponent<UnityTransport>().SetConnectionData(hostAdress, port);
         NetworkManager.Singleton.StartHost();

    }

    //start as client

    private void StartTheGameAsClient() {
        Debug.Log("start as client");

        // set address and port
        NetworkManager.Singleton.GetComponent<UnityTransport>().SetConnectionData(ClientIpAdress, port);
        NetworkManager.Singleton.StartClient();
    }


    // restart the game by pressing the j key 3 seconds
    public void QuitGame() {
        Debug.Log("quit game");
        Application.Quit();
        UnityEditor.EditorApplication.isPlaying = false;
    }

    // dont crash the game when the player press escape

   private void OnApplicationQuit()
    {
        if (NetworkManager.Singleton != null)
        {
            if (NetworkManager.Singleton.IsHost || NetworkManager.Singleton.IsClient)
            {
                NetworkManager.Shutdown();
            }
        }
    }


}