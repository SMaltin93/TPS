using UnityEngine;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;

public class NetworkManagerInitializer : MonoBehaviour
{
    private NetworkManager networkManager;

    private void Start()
    {
        networkManager = GetComponent<NetworkManager>();

        if (!GameManager.Instance.isHost)
        {
            // Create a new NetworkEndPoint with the server's IP address and port
            string ipAddress = GameManager.Instance.ipAdress;

            NetworkManager.Singleton.GetComponent<UnityTransport>().SetConnectionData(ipAddress, (ushort) 3000);
            // Add the server's NetworkEndPoint to the NetworkManager's NetworkConfig
            //networkManager.NetworkConfig.NetworkTransport = new UnityTransport.UnityTransport { ServerListenEndPoints = { serverEndPoint } };
        }
    }

    public void StartHost()
    {
        // Start the game as a host
        if (GameManager.Instance.isHost)
        {
            NetworkManager.Singleton.StartHost();
        }
    }

    public void StartClient()
    {
        // Start the game as a client
        if (!GameManager.Instance.isHost)
        {
            NetworkManager.Singleton.StartClient();
        }
    }
}
