using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using UnityEngine;
using Unity.Services.Core;
using Unity.Services.Authentication;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using System;

public class StartSession : MonoBehaviour
{
    private UnityTransport transport;
    public TMPro.TMP_InputField joinCodeInputField;

    [SerializeField] private Transform OVRControllerVisualLeftPrefab;
    [SerializeField] private Transform OVRControllerVisualRightPrefab;
    [SerializeField] private Transform HandVisualsLeftPrefab;
    [SerializeField] private Transform HandVisualsRightPrefab;


    // Start is called before the first frame update
    void Start()
    {
        transport = FindAnyObjectByType<UnityTransport>();

    }

    private async void Awake() {
        await Authenticate();
    }

    private async System.Threading.Tasks.Task Authenticate() {
        await UnityServices.InitializeAsync();
        await AuthenticationService.Instance.SignInAnonymouslyAsync();
    }

    public async void createMultiplayerRelay(){
        Allocation a = await RelayService.Instance.CreateAllocationAsync(2);
        joinCodeInputField.text = await RelayService.Instance.GetJoinCodeAsync(a.AllocationId);


        transport.SetRelayServerData(a.RelayServer.IpV4, (ushort) a.RelayServer.Port, a.AllocationIdBytes, a.Key, a.ConnectionData);
        NetworkManager.Singleton.StartHost();
        spawnPlayer();
    }

    public async void JoinSession()
    {
        try
        {
            JoinAllocation a = await RelayService.Instance.JoinAllocationAsync(joinCodeInputField.text);

            transport.SetClientRelayData(a.RelayServer.IpV4, (ushort)a.RelayServer.Port, a.AllocationIdBytes, a.Key, a.ConnectionData, a.HostConnectionData);

            NetworkManager.Singleton.StartClient();
            spawnPlayer();
        }
        catch (Exception e)
        {
            Debug.LogError("Error when trying to join multiplayer lobby " + e.Message);
        }
    }

    private void spawnPlayer()
    {
        Transform controllerLeft = Instantiate(OVRControllerVisualLeftPrefab);
        Transform controllerRight = Instantiate(OVRControllerVisualRightPrefab);
        // Transform handLeft = Instantiate(HandVisualsLeftPrefab);
        // Transform handRight = Instantiate(HandVisualsRightPrefab);

        controllerLeft.GetComponent<NetworkObject>().Spawn(true);
        controllerRight.GetComponent<NetworkObject>().Spawn(true);
        // handLeft.GetComponent<NetworkObject>().Spawn(true);
        // handRight.GetComponent<NetworkObject>().Spawn(true);


    }

}
