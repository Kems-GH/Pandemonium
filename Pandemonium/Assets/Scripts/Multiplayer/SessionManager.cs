using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using UnityEngine;
using Unity.Services.Core;
using Unity.Services.Authentication;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using System;

public class SessionManager : NetworkBehaviour
{
    [SerializeField] private SessionUI sessionUI;
    [SerializeField] private GameObject btnHost;
    [SerializeField] private GameObject btnJoin;
    [SerializeField] private GameObject btnCancel;

    private const int MAX_PLAYERS = 4;

    private UnityTransport transport;
    private Allocation allocation;

    public static SessionManager Instance { get; private set; }

    private async void Awake() {
        if(Instance == null) Instance = this;

        else 
        {
            Debug.LogError("There is more than one SessionManager in the scene");
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

    public async void createMultiplayerRelay(){
        allocation = await RelayService.Instance.CreateAllocationAsync(MAX_PLAYERS);

        transport.SetRelayServerData(allocation.RelayServer.IpV4, (ushort) allocation.RelayServer.Port, allocation.AllocationIdBytes, allocation.Key, allocation.ConnectionData);
        
        NetworkManager.Singleton.StartHost();

        if(!NetworkManager.Singleton.IsHost) CancelSession();

    }

    private async System.Threading.Tasks.Task Authenticate() {
        await UnityServices.InitializeAsync();
        await AuthenticationService.Instance.SignInAnonymouslyAsync();
    }

    public async void JoinSession()
    {
        if(sessionUI.IsJoinCodeEmpty()) return;
        NetworkManager.Singleton.Shutdown();
        try
        {
            JoinAllocation joinAllocation = await RelayService.Instance.JoinAllocationAsync(sessionUI.GetJoinCode());

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
        if(sessionUI.IsJoinCodeUIEmpty()) return;
        NetworkManager.Singleton.Shutdown();
        try
        {
            JoinAllocation joinAllocation = await RelayService.Instance.JoinAllocationAsync(sessionUI.GetJoinCodeUI());

            transport.SetClientRelayData(joinAllocation.RelayServer.IpV4, (ushort)joinAllocation.RelayServer.Port, joinAllocation.AllocationIdBytes, joinAllocation.Key, joinAllocation.ConnectionData, joinAllocation.HostConnectionData);
            NetworkManager.Singleton.StartClient();
        }
        catch (Exception e)
        {
            Debug.LogError("Error when trying to join multiplayer lobby " + e.Message);
            CancelSession();
        }
    }

    public void CancelSession()
    {
        sessionUI.SetJoinCode("");
        if(!sessionUI.IsJoinCodeUINull()) sessionUI.SetJoinCodeUI("");

        NetworkManager.Singleton.Shutdown();
        this.createMultiplayerRelay();
        this.ShowButtons();
    }

    private void OnDisconnectedFromServer()
    {
        CancelSession();
    }

    public Allocation GetAllocation()
    {
        return allocation;
    }

    public void HideButtons() {
        btnHost.SetActive(false);
        btnJoin.SetActive(false);
        btnCancel.SetActive(true);
    }

    public void ShowButtons() {
        btnHost.SetActive(true);
        btnJoin.SetActive(true);
        btnCancel.SetActive(false);
    }
}
