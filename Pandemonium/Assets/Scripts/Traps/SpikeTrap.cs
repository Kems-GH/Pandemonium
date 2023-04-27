using Unity.Netcode;
using UnityEngine;

public class SpikeTrap : NetworkBehaviour
{
    [SerializeField] private Transform _spike;
    private Animator _animator;

    private float _cooldown = 5f;

    private float _timeLastActivated = 5f;

    void Start()
    {
        if (!IsServer && !GameManager.Instance.IsSolo()) return;
        this._animator = GetComponent<Animator>();
    }

    public void Activate()
    {
        if(_timeLastActivated < _cooldown) return;
        _timeLastActivated = 0f;

        this._animator.SetTrigger("StartAction");
    }

    void Update()
    {
        if (_timeLastActivated < _cooldown) _timeLastActivated  += Time.deltaTime;
    }

}
