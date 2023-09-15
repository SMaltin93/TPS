using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// scene management
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;



public class MenuManager : MonoBehaviour
{

   //[SerializeField] private GameObject ServersObject;

   [SerializeField] private GameObject Dropdown;


   private TMP_Dropdown ServerDropdown;

   private LoadServer loadedServers;

   // hashTabel with server name as key , adress as value
   
   public static Dictionary<string, string> ServersHash;
   public static string gameState;
   public static string ipAdress;


   private NetworkUI networkUI;


   void Start() {

    // dont destroy the menu scene
    DontDestroyOnLoad(gameObject);

    loadedServers = GetComponent<LoadServer>();
    networkUI = GetComponent<NetworkUI>();

    ServersHash = new Dictionary<string, string>();

    ServerDropdown = Dropdown.GetComponent<TMP_Dropdown>();
    ServerDropdown.ClearOptions();

    ServerDropdown.onValueChanged.AddListener(delegate { getTheChosenServer(); }); 




   }

   public void UpdateServer() { 



        // Debug Se
        if(ServersHash.Count == 0) return;
        foreach (var key in ServersHash.Keys)
        {
            ServerDropdown.options.Add(new TMP_Dropdown.OptionData() { text = key });
        }
        
   }
    
    // play game
    public void PlayGame()
    {
        // load the game scene
        UnityEngine.SceneManagement.SceneManager.LoadScene("Game");
    }
    // quit game
    public void QuitGame()
    {
        // stop the game in the editor
        // close the game in the build
        Application.Quit();
    }


    public string getTheChosenServer()
    {
        if(ServersHash.Count == 0) return "";
        string serverName = ServerDropdown.options[ServerDropdown.value].text;
        // get the ip adress from the hash table
        
        return  serverName; //ServersHash[serverName];
    }


    // load Game Senec and set the ip adress

    public void LoadGame(string state)
    {
        gameState = state;
        if (state == "client") {

            ipAdress = getTheChosenServer();
            Debug.Log("MenuManager ipAdress hhhhhhhhhhhhhhhhhh: " + ipAdress);
        } 

        // destroy the menu scene

        UnityEngine.SceneManagement.SceneManager.LoadScene("Game");

    }

}
