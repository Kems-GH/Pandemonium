using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using UnityEngine;

public class SessionUI : NetworkBehaviour
{
    private const string JOIN_CODE_TAG = "JoinCode";
    private const string JOIN_CODE_UI_TAG = "JoinCodeUI";

    private TMPro.TMP_Text joinCode;
    private TMPro.TMP_InputField joinCodeUi;
    
    private TouchScreenKeyboard overlayKeyboard;

    private void Start() {
        joinCode = GameObject.FindGameObjectWithTag(JOIN_CODE_TAG).GetComponent<TMPro.TMP_Text>();
        joinCodeUi = GameObject.FindGameObjectWithTag(JOIN_CODE_UI_TAG).GetComponent<TMPro.TMP_InputField>();
    }

    private void Update()
    {
        if(overlayKeyboard != null)
        {
            joinCode.text = overlayKeyboard.text;
            joinCodeUi.text = overlayKeyboard.text;
            if (overlayKeyboard.status == TouchScreenKeyboard.Status.Done)
            {
                overlayKeyboard = null;
                SessionManager.Instance.JoinSession();
            }
        }
    }

    public async void displayJoinCode(){
        SessionManager.Instance.HideButtons();
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

    public bool IsJoinCodeEmpty()
    {
        return joinCode.text == "";
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
        joinCode.text = code;
    }

    public void SetJoinCodeUI(string code)
    {
        joinCodeUi.text = code;
    }
}
