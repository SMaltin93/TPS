using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using Unity.Netcode;

public class StartServer : MonoBehaviour
{
    [SerializeField]
    private string jsonUrl = "http://139.59.129.223:3000/hosts";
    
    [SerializeField]
    private TMP_InputField inputField; // or just InputField if you are not using TextMeshPro

    void Start()
    {
        // Assuming this script is attached to your button
        // Add a click listener to the button to start the game as host and post data when clicked
        GetComponent<Button>().onClick.AddListener(StartGameAsHostAndPostData);
    }

    private void StartGameAsHostAndPostData()
    {
        string serverName = inputField.text; // get server name from input field
        StartCoroutine(PostData($"{{\"name\": \"{serverName}\"}}")); // post server name to server
        
        SceneManager.LoadScene("Game");

        //NetworkManager.Singleton.StartHost();

        Debug.Log("starting game as host...");


    }

    private IEnumerator PostData(string json)
    {
        UnityWebRequest www = new UnityWebRequest(jsonUrl, "POST");
        byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(json);
        www.uploadHandler = new UploadHandlerRaw(bodyRaw);
        www.downloadHandler = new DownloadHandlerBuffer();
        www.SetRequestHeader("Content-Type", "application/json");
        
        yield return www.SendWebRequest();
        
        if(www.result != UnityWebRequest.Result.Success)
        {
            Debug.Log(www.error);
        }
        else
        {
            Debug.Log("Post complete!");
        }
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Start the game as host
        if (scene.name == "Game")
        {
            NetworkManager.Singleton.StartHost();
            Debug.Log("Game started as host");
            // Remove the event listener when the host starts
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }
    }
}
