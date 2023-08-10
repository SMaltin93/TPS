using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using TMPro;
using System.Text.RegularExpressions;

public class SaveServer : MonoBehaviour
{
    [SerializeField] private string jsonUrl = "http://139.59.129.223:3000/hosts";
    [SerializeField] private Button saveButton;
    [SerializeField] private TMP_InputField nameInput;

    private string serverName = null;

    void Awake()
    {
        // deactivate the button initially
        saveButton.gameObject.SetActive(false);

        // check the name input while typing
        nameInput.onValueChanged.AddListener(delegate { CheckName(nameInput.text); });
    }

    public void SaveData()
    {
        if (serverName != null)
        {
            // Here, we use the serverName as the key and the IP as its value
            StartCoroutine(PostData($"{{\"name\": \"{serverName}\"}}"));
            DeactivateButtons();
        }
    }

    private IEnumerator PostData(string json)
    {
        // debug line 36 
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
            // deactivate the save button and the input field after sending data
         
   

    // Accept if up to 12 digits and in the format name@number
    private void CheckName(string name)
    {
        Regex pattern = new Regex(@"^(?=.{1,12}$)[A-Za-z]+@[0-9]+$");
        if (pattern.IsMatch(name))
        {
            saveButton.gameObject.SetActive(true);
            serverName = name;
        }
        else
        {
            saveButton.gameObject.SetActive(false);
            serverName = null;
        }
    }

    private void DeactivateButtons()
    {
        saveButton.gameObject.SetActive(false);
        nameInput.gameObject.SetActive(false);
    }
}
