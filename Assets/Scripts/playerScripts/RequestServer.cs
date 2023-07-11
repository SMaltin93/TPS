using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

// netcode 
using Unity.Netcode;

public class RequestServer : NetworkBehaviour
{

     [ServerRpc(RequireOwnership = false)]
    public void RequestServerRpc(string className, string MethodName, ulong objectId, ulong playerId)
    {
      // reference to the class
        
    }

}
