using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using Unity.Multiplayer.Samples.Utilities.ClientAuthority;


public class PlayerPickUp : NetworkBehaviour
{
    [SerializeField] private Transform findWeapon;
    [SerializeField] private LayerMask pickUpLayer;
    [SerializeField] private Transform weaponHolder;

    public static bool isGrabbed;

    private GameObject weapon;

    private void Awake()
    {
        isGrabbed = false;
        weapon = null;
    }

    private void FixedUpdate()
    {
        if (!IsOwner) return;

        if (Input.GetKeyDown(KeyCode.E))
        {
            if (!isGrabbed && weapon == null)
            {
                RequiresPickupServerRpc();
            }
        }

        if (Input.GetKeyDown(KeyCode.Q))
        {
            if (isGrabbed && weapon != null)
            {
                DropWeapon();
            }
        }
    }

    private void DropWeapon()
    {
        weapon.transform.SetParent(null);
        weapon.GetComponent<ObjectGrabb>().Drop(this.weaponHolder);
        isGrabbed = false;
        weapon = null;
    }

    private void PickUpWeapon()
    {
        RaycastHit hit;
        float pickUpRange = 4f;
        if (Physics.Raycast(findWeapon.position, findWeapon.forward, out hit, pickUpRange))
        {
            if (hit.transform.TryGetComponent(out ObjectGrabb objectGrabb))
            {
                weapon = hit.transform.gameObject;
                objectGrabb.Grab(this.weaponHolder);
                isGrabbed = true;
            }
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void RequiresPickupServerRpc(ServerRpcParams serverRpcParams = default)
    {
        ExecutePickupClientRpc();
        ExecuteReparentServerRpc(serverRpcParams);
    }

    [ClientRpc]
    private void ExecutePickupClientRpc()
    {
        PickUpWeapon();
    }

    [ServerRpc(RequireOwnership = false)]
    private void ExecuteReparentServerRpc(ServerRpcParams serverRpcParams = default)
    {
        ReparentWeapon(serverRpcParams.Receive.SenderClientId);
    }

    private void ReparentWeapon(ulong clientId)
    {
        if (weapon != null && weapon.TryGetComponent<NetworkObject>(out var networkObject))
        {
            networkObject.ChangeOwnership(clientId);
            weapon.transform.SetParent(this.transform);
        }
    }
}