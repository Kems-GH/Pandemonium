using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using UnityEngine;

public class SessionUI : NetworkBehaviour
{
    private TMPro.TMP_Text joinCode;
    private TMPro.TMP_InputField joinCodeUi;
    
    private TouchScreenKeyboard overlayKeyboard;

    private void Start() {
        joinCode = GameObject.Find("JoinCode").GetComponent<TMPro.TMP_Text>();
        joinCodeUi = GameObject.Find("JoinCodeInput").GetComponent<TMPro.TMP_InputField>();
    }

    public async void displayJoinCode(){
        Allocation allocation = SessionManager.Instance.GetAllocation();
        joinCode.text = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);
    }
    
    public void EnterCode()
    {
        overlayKeyboard = TouchScreenKeyboard.Open(joinCode.text, TouchScreenKeyboardType.Default);
    }

    public string GetJoinCode()
    {
        return joinCode.text;
    }

    public string GetJoinCodeUI()
    {
        return joinCodeUi.text;
    }

    public TouchScreenKeyboard GetOverlayKeyboard()
    {
        return overlayKeyboard;
    }

    public bool IsOverlayKeyboardNull()
    {
        return overlayKeyboard == null;
    }

    public bool IsJoinCodeEmpty()
    {
        return joinCodeUi.text == "";
    }

    public bool IsJoinCodeUIEmpty()
    {
        return joinCodeUi.text == "";
    }

    public bool IsJoinCodeUINull() {
        return joinCodeUi == null;
    }

    public void SetJoinCode(string code)
    {
        joinCodeUi.text = code;
    }

    public void SetJoinCodeUI(string code)
    {
        joinCodeUi.text = code;
    }
}
