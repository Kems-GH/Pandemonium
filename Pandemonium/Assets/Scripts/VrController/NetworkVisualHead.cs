using UnityEngine;
using Unity.Netcode;


public class NetworkVisualHead : NetworkBehaviour
{
    public Transform _head;

    private void Start()
    {
        if (!IsOwner) return;
        _head = GameObject.FindWithTag("MainCamera").transform;
    }

    private void Update()
    {
        if (!IsOwner) return;
        transform.position = _head.position;
        transform.rotation = _head.rotation;
    }
}
