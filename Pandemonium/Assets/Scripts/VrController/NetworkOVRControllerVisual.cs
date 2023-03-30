using UnityEngine;
using UnityEngine.Assertions;
using Unity.Netcode;

namespace Oculus.Interaction.Input.Visuals
{
    public class NetworkOVRControllerVisual : NetworkBehaviour
    {
        [SerializeField, Interface(typeof(IController))]
        private MonoBehaviour _controller;

        public IController Controller;

        [SerializeField]
        private OVRControllerHelper _ovrControllerHelper;

        public bool ForceOffVisibility { get; set; }

        private bool _started = false;

        public string tagParent;

        protected virtual void Awake()
        {
            Controller = _controller as IController;
        }

        protected virtual void Start()
        {
            Debug.Log("owner : " + IsOwner);
            if (!IsOwner) return;

            _controller = GameObject.FindGameObjectWithTag(tagParent).GetComponent<MonoBehaviour>();
            Controller = _controller as IController;
            this.BeginStart(ref _started);
            this.AssertField(Controller, nameof(Controller));
            this.AssertField(_ovrControllerHelper, nameof(_ovrControllerHelper));
            switch (Controller.Handedness)
            {
                case Handedness.Left:
                    _ovrControllerHelper.m_controller = OVRInput.Controller.LTouch;
                    break;
                case Handedness.Right:
                    _ovrControllerHelper.m_controller = OVRInput.Controller.RTouch;
                    break;
            }
            this.EndStart(ref _started);
        }
        private void Update() {
            Debug.Log(Controller);
        }

        protected virtual void OnEnable()
        {
            if (_started)
            {
                Controller.WhenUpdated += HandleUpdated;
            }
        }

        protected virtual void OnDisable()
        {
            if (_started && _controller != null)
            {
                Controller.WhenUpdated -= HandleUpdated;
            }
        }

        private void HandleUpdated()
        {
            if (!Controller.IsConnected ||
                ForceOffVisibility ||
                !Controller.TryGetPose(out Pose rootPose))
            {
                _ovrControllerHelper.gameObject.SetActive(false);
                return;
            }

            _ovrControllerHelper.gameObject.SetActive(true);
            transform.position = rootPose.position;
            transform.rotation = rootPose.rotation;
            float parentScale = transform.parent != null ? transform.parent.lossyScale.x : 1f;
            transform.localScale = Controller.Scale / parentScale * Vector3.one;
        }

        #region Inject

        public void InjectAllOVRControllerVisual(IController controller, OVRControllerHelper ovrControllerHelper)
        {
            InjectController(controller);
            InjectAllOVRControllerHelper(ovrControllerHelper);
        }

        public void InjectController(IController controller)
        {
            _controller = controller as MonoBehaviour;
            Controller = controller;
        }

        public void InjectAllOVRControllerHelper(OVRControllerHelper ovrControllerHelper)
        {
            _ovrControllerHelper = ovrControllerHelper;
        }

        #endregion
    }
}
