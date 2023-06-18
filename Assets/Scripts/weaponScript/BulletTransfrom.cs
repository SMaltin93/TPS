using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class BulletTransfrom : NetworkBehaviour
{

    private readonly NetworkVariable<BulletTransformData> bulletPosition = new NetworkVariable<BulletTransformData>(writePerm: NetworkVariableWritePermission.Owner);

    void Update()
    {
        // uppdate the position and rotation of the bullet for all clients
        if (IsOwner)
        {
                
            bulletPosition.Value = new BulletTransformData()
            {
                Position = transform.position,
                Rotation = transform.rotation
            };
        }
        else
        {
            transform.position = Vector3.Lerp(transform.position, bulletPosition.Value.Position, Time.deltaTime * 10f);
            transform.rotation = Quaternion.Lerp(transform.rotation, bulletPosition.Value.Rotation, Time.deltaTime * 10f);
        }
    }

    // serliaze the data
    struct BulletTransformData : INetworkSerializable
    {
        private float x, y, z;
        private short ry;

        internal Vector3 Position
        {

            get
            {
                return new Vector3(x, y, z);
            }
            set
            {
                x = value.x;
                y = value.y;
                z = value.z;
            }
        }

        internal Quaternion Rotation
        {
            get
            {
                return Quaternion.Euler(0, ry, 0);
            }
            set
            {
                ry = (short)value.eulerAngles.y;
            }
        }

        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        {
            serializer.SerializeValue(ref x);
            serializer.SerializeValue(ref y);
            serializer.SerializeValue(ref z);
            serializer.SerializeValue(ref ry);
        }
    }
    
}
