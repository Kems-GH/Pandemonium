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
    public TMPro.TMP_Text joinCode;
    [SerializeField] private GameObject toDelete;

    [SerializeField] private GameObject btnHost;
    [SerializeField] private GameObject btnClient;
    [SerializeField] private GameObject btnCancel;

    [SerializeField] private GameObject playerPrefab;

    private TouchScreenKeyboard overlayKeyboard;
    void Start()
    {
        string test = "";
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
        OnStartSession();
        Allocation a = await RelayService.Instance.CreateAllocationAsync(2);
        joinCode.text = await RelayService.Instance.GetJoinCodeAsync(a.AllocationId);


        transport.SetRelayServerData(a.RelayServer.IpV4, (ushort) a.RelayServer.Port, a.AllocationIdBytes, a.Key, a.ConnectionData);
        
        NetworkManager.Singleton.StartHost();
        Destroy(toDelete);

        if(!NetworkManager.Singleton.IsHost) CancelSession();

    }

    public async void JoinSession()
    {
        OnStartSession();
        try
        {
            JoinAllocation a = await RelayService.Instance.JoinAllocationAsync(joinCode.text);

            transport.SetClientRelayData(a.RelayServer.IpV4, (ushort)a.RelayServer.Port, a.AllocationIdBytes, a.Key, a.ConnectionData, a.HostConnectionData);
            NetworkManager.Singleton.StartClient();
            Destroy(toDelete);
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

    private void OnStartSession()
    {
        btnCancel.SetActive(true);
        btnHost.SetActive(false);
        btnClient.SetActive(false);
        GameManager.Instance.SetSolo(false);
    }

    public void CancelSession()
    {
        NetworkManager.Singleton.Shutdown();
        
        GameManager.Instance.SetSolo(true);
        
        toDelete = Instantiate(playerPrefab);

        btnCancel.SetActive(false);
        btnHost.SetActive(true);
        btnClient.SetActive(true);
    }

}
