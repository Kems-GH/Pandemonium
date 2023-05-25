using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using UnityEngine;
using Unity.Services.Core;
using Unity.Services.Authentication;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using System;

public class StartSession : NetworkBehaviour
{
    private const int MAX_PLAYERS = 4;
    private UnityTransport transport;
    private Allocation allocation;
    public TMPro.TMP_Text joinCode;
    public TMPro.TMP_InputField joinCodeUi;

    [SerializeField] private GameObject btnHost;
    [SerializeField] private GameObject btnClient;
    [SerializeField] private GameObject btnCancel;

    private TouchScreenKeyboard overlayKeyboard;

    public static StartSession instance;

    private async void Awake() {
        if(instance == null) instance = this;
        else 
        {
            Debug.LogError("There is more than one StartSession in the scene");
            Destroy(this.gameObject);
            return;
        }

        transport = FindAnyObjectByType<UnityTransport>();
        if (transport == null)
        {
            Debug.LogError("No transport found in the scene");
            return;
        }
        await Authenticate();
        this.createMultiplayerRelay();
    }

    private async System.Threading.Tasks.Task Authenticate() {
        await UnityServices.InitializeAsync();
        await AuthenticationService.Instance.SignInAnonymouslyAsync();
    }

    public async void createMultiplayerRelay(){
        allocation = await RelayService.Instance.CreateAllocationAsync(MAX_PLAYERS);

        transport.SetRelayServerData(allocation.RelayServer.IpV4, (ushort) allocation.RelayServer.Port, allocation.AllocationIdBytes, allocation.Key, allocation.ConnectionData);
        
        NetworkManager.Singleton.StartHost();

        if(!NetworkManager.Singleton.IsHost) CancelSession();

    }

    public async void displayJoinCode(){
        OnStartSession();
        joinCode.text = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);
        if(joinCodeUi != null) joinCodeUi.text = joinCode.text;
    }

    public async void JoinSession()
    {
        if(joinCode.text == "") return;
        OnStartSession();
        NetworkManager.Singleton.Shutdown();
        try
        {
            JoinAllocation joinAllocation = await RelayService.Instance.JoinAllocationAsync(joinCode.text);

            transport.SetClientRelayData(joinAllocation.RelayServer.IpV4, (ushort)joinAllocation.RelayServer.Port, joinAllocation.AllocationIdBytes, joinAllocation.Key, joinAllocation.ConnectionData, joinAllocation.HostConnectionData);
            NetworkManager.Singleton.StartClient();
        }
        catch (Exception e)
        {
            Debug.LogError("Error when trying to join multiplayer lobby " + e.Message);
            CancelSession();
        }
    }

    public async void JoinSessionUi()
    {
        if(joinCodeUi.text == "") return;
        OnStartSession();
        NetworkManager.Singleton.Shutdown();
        try
        {
            JoinAllocation joinAllocation = await RelayService.Instance.JoinAllocationAsync(joinCodeUi.text);

            transport.SetClientRelayData(joinAllocation.RelayServer.IpV4, (ushort)joinAllocation.RelayServer.Port, joinAllocation.AllocationIdBytes, joinAllocation.Key, joinAllocation.ConnectionData, joinAllocation.HostConnectionData);
            NetworkManager.Singleton.StartClient();
        }
        catch (Exception e)
        {
            Debug.LogError("Error when trying to join multiplayer lobby " + e.Message);
            CancelSession();
        }
    }
    public void EnterCode()
    {
        overlayKeyboard = TouchScreenKeyboard.Open(joinCode.text, TouchScreenKeyboardType.Default);
    }
    private void Update()
    {
        if(overlayKeyboard != null)
        {
            joinCode.text = overlayKeyboard.text;
            if (overlayKeyboard.status == TouchScreenKeyboard.Status.Done)
            {
                overlayKeyboard = null;
                JoinSession();
            }
        }
    }
    private void OnStartSession()
    {
        btnCancel.SetActive(true);
        btnHost.SetActive(false);
        btnClient.SetActive(false);
    }

    public void CancelSession()
    {
        joinCode.text = "";
        if(joinCodeUi != null) joinCodeUi.text = "";

        NetworkManager.Singleton.Shutdown();
        this.createMultiplayerRelay();

        btnCancel.SetActive(false);
        btnHost.SetActive(true);
        btnClient.SetActive(true);
    }

    private void OnDisconnectedFromServer()
    {
        CancelSession();
    }

}
