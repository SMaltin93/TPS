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
        StartCoroutine(PredictBullet());
    }


    IEnumerator PredictBullet()
    {
       Vector3 predictedPos = transform.position + rb.velocity * Time.fixedDeltaTime;

       RaycastHit hit;
       int layerMask =~ LayerMask.GetMask("Bullet");
       Debug.DrawRay(transform.position, predictedPos, Color.red);

         if (Physics.Linecast(transform.position, predictedPos, out hit, layerMask))
         {
            transform.position = hit.point;
            rb.collisionDetectionMode = CollisionDetectionMode.ContinuousSpeculative;
            rb.isKinematic = true;
            yield return 0;
            OnTriggerEnterFixed(hit.collider);
         }
    }

   void OnTriggerEnterFixed(Collider other)
    {
        // Log the name of the object the bullet collided with
        Debug.Log("Bullet collided with: " + other.gameObject.name);

        // get the networkID if it a player

        if (other.gameObject.CompareTag("Player"))
        { 

            NetworkObject networkObject = other.gameObject.GetComponent<NetworkObject>();
            ulong playerNetworkID = networkObject.NetworkObjectId;
         
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
}
