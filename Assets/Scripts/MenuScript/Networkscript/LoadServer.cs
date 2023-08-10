using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using Newtonsoft.Json;
using TMPro;

public class LoadServer : MonoBehaviour
{
    [SerializeField] private string jsonUrl = "http://139.59.129.223:3000/hosts";

    
    private MenuManager menuManager;

    // Use this for initialization
    void Awake()
    {
       // StartCoroutine(FetchAndPopulate());
        menuManager = GetComponent<MenuManager>();
    }


    public void LoadServers(){
        StartCoroutine(FetchAndPopulate());
        
    }

    private IEnumerator FetchAndPopulate()
    {
        using (UnityWebRequest request = UnityWebRequest.Get(jsonUrl))
        {
            yield return request.SendWebRequest();
            if (request.result != UnityWebRequest.Result.Success)
            {
                Debug.Log(request.error);
            }
            else
            {
                // Parse JSON response into a dictionary
                Dictionary<string, string> ipNamePairs = JsonConvert.DeserializeObject<Dictionary<string, string>>(request.downloadHandler.text);
                Debug.Log($"server response: {request.downloadHandler.text}");
                Debug.Log($"number of servers = {ipNamePairs.Count}");
                foreach (KeyValuePair<string, string> pair in ipNamePairs)
                {
                    // add the server to the Dictionary
                    MenuManager.ServersHash[pair.Key] = pair.Value;
                    Debug.Log($"ip: {pair.Key}, name: {pair.Value}");
                }
                menuManager.UpdateServer();
            }
        }
    }

    // public Dictionary<string, string> GetServersHash()
    // {
    //     return ServersHash;
    // }
}