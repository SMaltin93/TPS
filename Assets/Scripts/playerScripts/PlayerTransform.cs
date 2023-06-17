using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class PlayerTransform :  NetworkBehaviour
{
    private readonly NetworkVariable<PlayerTransformData> playerState= new NetworkVariable<PlayerTransformData>(writePerm: NetworkVariableWritePermission.Owner);

    // animation

    void Update()
    {
        if (IsOwner) {

            playerState.Value = new PlayerTransformData () {
                Position = transform.position,
                Rotation = transform.rotation
            };
        } else {
            transform.position = Vector3.Lerp(transform.position, playerState.Value.Position, Time.deltaTime * 10f);
            transform.rotation = Quaternion.Lerp(transform.rotation, playerState.Value.Rotation, Time.deltaTime * 10f);
        }
    }

    // serliaze the data
    struct PlayerTransformData : INetworkSerializable
    {
        private float x, y, z;
        private short ry;

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

        internal Quaternion Rotation {
            get { 
                return Quaternion.Euler(0, ry, 0); 
            }
            set { 
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
