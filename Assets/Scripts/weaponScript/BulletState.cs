using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;



public class BulletState : NetworkBehaviour
{

    [SerializeField] private float bulletSpeed ;

    private Rigidbody rb ;

    public NetworkVariable<Vector3> BulletDirection = new NetworkVariable<Vector3>(default,
    NetworkVariableReadPermission.Owner, NetworkVariableWritePermission.Server);


    private void Start()
    {
        rb = GetComponent<Rigidbody>();


    }


    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
       GetComponent<Rigidbody>().velocity = this.transform.forward * bulletSpeed;
    }


    void FixedUpdate()
    {
        if(!IsServer) return;
        StartCoroutine(PredictBullet());
    }


    IEnumerator PredictBullet()
    {
       Vector3 predictedPos = transform.position + rb.velocity * Time.fixedDeltaTime;

       RaycastHit hit;
       int layerMask =~ LayerMask.GetMask("Bullet") | ~LayerMask.GetMask("Player");
   


       Debug.DrawRay(transform.position, predictedPos, Color.red);

         if (Physics.Linecast(transform.position, predictedPos, out hit, layerMask))
         {
            transform.position = hit.point;
            rb.collisionDetectionMode = CollisionDetectionMode.ContinuousSpeculative;
            rb.isKinematic = true;
            yield return 0;
            Debug.Log("hit" + hit.collider.gameObject.name);
            OnTriggerEnterFixed(hit.collider);
         }
    }

   void OnTriggerEnterFixed(Collider other)
    {
        // get the main parent of the object
        GameObject mainParent = other.gameObject.transform.root.gameObject;
        PlayerHealth playerHealth = mainParent.GetComponent<PlayerHealth>();
      

        // get the networkID if it a player

        if (other.gameObject.CompareTag("PlayerHead") || other.gameObject.CompareTag("PlayerBody")
        || other.gameObject.CompareTag("PlayerLeg") || other.gameObject.CompareTag("PlayerArm")) 

        {

            NetworkObject networkObject = mainParent.GetComponent<NetworkObject>();
            ulong playerNetworkID = networkObject.NetworkObjectId;

            string playerTag = other.gameObject.tag;

            if (IsServer) {
                TakeDamageServerRpc(playerNetworkID, playerTag);
            }
            // player hited on tag 
            Debug.Log("player get hited on " + playerTag);
            Debug.Log("player id is " + playerNetworkID);
        }

        // destroy bullet if it collides whatever it collid with
        if (IsServer)
        {
            DestroyObjectServerRpc();
        }
    }

    [ServerRpc]
    void DestroyObjectServerRpc()
    {
        Destroy(gameObject);
    }

    [ServerRpc]
    public void TakeDamageServerRpc(ulong playerNetworkID, string playerTag)
    {
        // Retrieve the network object by its network id
        GameObject player = NetworkManager.Singleton.SpawnManager.SpawnedObjects[playerNetworkID].gameObject;
        PlayerHealth playerHealth = player.GetComponent<PlayerHealth>();
        playerHealth.TakeDamage(playerTag);
        
    
    }
}
