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
    public TMPro.TMP_Text joinCodeInputField;
    [SerializeField] private GameObject toDelete;

    [SerializeField] private GameObject btnHost;
    [SerializeField] private GameObject btnClient;
    [SerializeField] private GameObject btnCancel;

    [SerializeField] private GameObject playerPrefab;


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
        Destroy(toDelete);
        btnCancel.SetActive(true);
        btnHost.SetActive(false);
        btnClient.SetActive(false);
    }

    public async void JoinSession()
    {
        try
        {
            JoinAllocation a = await RelayService.Instance.JoinAllocationAsync(joinCodeInputField.text);

            transport.SetClientRelayData(a.RelayServer.IpV4, (ushort)a.RelayServer.Port, a.AllocationIdBytes, a.Key, a.ConnectionData, a.HostConnectionData);

            NetworkManager.Singleton.StartClient();
            Destroy(toDelete);
            btnCancel.SetActive(true);
            btnHost.SetActive(false);
            btnClient.SetActive(false);
        }
        catch (Exception e)
        {
            Debug.LogError("Error when trying to join multiplayer lobby " + e.Message);
        }
    }

    public void CancelSession()
    {
        NetworkManager.Singleton.DisconnectClient(NetworkManager.Singleton.LocalClientId);
        
        GameManager.Instance.SetSolo(true);
        
        toDelete = Instantiate(playerPrefab);
        btnCancel.SetActive(false);
        btnHost.SetActive(true);
        btnClient.SetActive(true);
    }

}
