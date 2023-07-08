using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using Newtonsoft.Json;
using TMPro;

public class HostListPopulator : MonoBehaviour
{
    [SerializeField]
    private string jsonUrl = "http://139.59.129.223:3000/hosts";
    
    [SerializeField]
    private Button buttonPrefab;
    
    [SerializeField]
    private Transform contentPanel;

    // Use this for initialization
    void Start()
    {
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
                    // Instantiate a new button
                    Debug.Log("creating button");
                    Button button = Instantiate(buttonPrefab, contentPanel);
                    Debug.Log("Parent is " + button.transform.parent.name);
                    
                    // Set the button's text
                    button.GetComponentInChildren<TextMeshProUGUI>().text = pair.Value;  // Here we use the 'name' (value of the pair) as the text
                    
                    // Add a click listener to the button that calls ConnectToServer with the IP (key of the pair)
                    button.onClick.AddListener(() => ConnectToServer(pair.Key));
                }
            }
        }
    }

    private void ConnectToServer(string ip) {
        Debug.Log(ip);
    }
}
