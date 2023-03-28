using Unity.Collections;
using Unity.Netcode;
using UnityEngine;

public class PlayerNetwork : NetworkBehaviour {

    [SerializeField] private Transform spawnedObjectPrefab;

    private Transform spawnObjectTransform;
    private NetworkVariable<MyCustomData> randomNumber = new NetworkVariable<MyCustomData>(
        new MyCustomData {
            myInt = 55,
            myBool = false,
            message = "Hello World"
    }, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);

    public struct MyCustomData : INetworkSerializable {
        public int myInt;
        public bool myBool;

        public FixedString128Bytes message;

        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        {
            serializer.SerializeValue(ref myInt);
            serializer.SerializeValue(ref myBool);
            serializer.SerializeValue(ref message);
        }
    }

    public override void OnNetworkSpawn() {
        randomNumber.OnValueChanged += (MyCustomData oldValue, MyCustomData newValue) => {
            Debug.Log("OnValueChanged: " + newValue.myInt + " -- " + newValue.myBool + " -- " + newValue.message);
        };
    }

    private void  Update() {
        if (!IsOwner) return;

        if (Input.GetKeyDown(KeyCode.T)) {
            spawnObjectTransform = Instantiate(spawnedObjectPrefab);
            spawnObjectTransform.GetComponent<NetworkObject>().Spawn(true);
            TestServerRpc();
            // randomNumber.Value = new MyCustomData
            // {
            //     myInt = 10,
            //     myBool = true,
            // };
        }

        if (Input.GetKeyDown(KeyCode.Y)) {
            spawnObjectTransform.GetComponent<NetworkObject>().Despawn(true);
            Destroy(spawnObjectTransform.gameObject);
        }

        Vector3 movDir = new Vector3(0, 0, 0);

        if(Input.GetKey(KeyCode.Z)) movDir.z = +1f;
        if(Input.GetKey(KeyCode.S)) movDir.z = -1f;
        if(Input.GetKey(KeyCode.Q)) movDir.x = -1f;
        if(Input.GetKey(KeyCode.D)) movDir.x = +1f;

        float speed = 3f;
        transform.Translate(movDir * speed * Time.deltaTime);

    }

    // run on server only
    [ServerRpc]
    public void TestServerRpc() {
        Debug.Log("ServerRpcTest");
    }

    // run on client only called by server only
    [ClientRpc]
    public void TestClientRpc() {
        Debug.Log("ClientRpcTest");
    }
}
