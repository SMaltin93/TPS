using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
public class BulletTransform : NetworkBehaviour
{
 
    private readonly NetworkVariable <BulletTransformData> bulletstate = new NetworkVariable<BulletTransformData>(writePerm: NetworkVariableWritePermission.Owner);  

    void Update()
    {

        if (IsOwner) {
            bulletstate.Value = new BulletTransformData () {

                Position = transform.position
            };
        } else {
            transform.position = Vector3.Lerp(transform.position, bulletstate.Value.Position, Time.deltaTime * 10f);
        }
        
        
    }

    struct  BulletTransformData : INetworkSerializable
    {
        private float x, y, z;
    
        internal Vector3 Position {
            get { 
                return new Vector3(x, y, z); 
            }
            set { 
                x = value.x;
                y = value.y;
                z = value.z;
            }
        }

        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        {
            serializer.SerializeValue(ref x);
            serializer.SerializeValue(ref y);
            serializer.SerializeValue(ref z); 
        }
        
    }

}
