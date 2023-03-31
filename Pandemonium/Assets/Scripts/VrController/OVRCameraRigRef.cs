using System;
using UnityEngine;
using UnityEngine.Assertions;

namespace Oculus.Interaction.Input
{
    public interface IOVRCameraRigRef
    {
        OVRCameraRig CameraRig { get; }
        /// <summary>
        /// Returns a valid OVRHand object representing the left hand, if one exists on the
        /// OVRCameraRig. If none is available, returns null.
        /// </summary>
        OVRHand LeftHand { get; }
        /// <summary>
        /// Returns a valid OVRHand object representing the right hand, if one exists on the
        /// OVRCameraRig. If none is available, returns null.
        /// </summary>
        OVRHand RightHand { get; }
        Transform LeftController { get; }
        Transform RightController { get; }

        event Action<bool> WhenInputDataDirtied;
    }

    /// <summary>
    /// Points to an OVRCameraRig instance. This level of indirection provides a single
    /// configuration point on the root of a prefab.
    /// Must execute before all other OVR related classes so that the fields are
    /// initialized correctly and ready to use.
    /// </summary>
    [DefaultExecutionOrder(-90)]
    public class OVRCameraRigRef : MonoBehaviour, IOVRCameraRigRef
    {
        [Header("Configuration")]
        [SerializeField]
        private OVRCameraRig _ovrCameraRig;

        [SerializeField]
        private bool _requireOvrHands = true;

        public OVRCameraRig CameraRig;

        private OVRHand _leftHand;
        private OVRHand _rightHand;
        public OVRHand LeftHand;
        public OVRHand RightHand;

        public Transform LeftController;
        public Transform RightController;

        public event Action<bool> WhenInputDataDirtied = delegate { };

        protected bool _started = false;

        private bool _isLateUpdate;

        OVRCameraRig IOVRCameraRigRef.CameraRig => CameraRig;

        OVRHand IOVRCameraRigRef.LeftHand => LeftHand;

        OVRHand IOVRCameraRigRef.RightHand => RightHand;

        Transform IOVRCameraRigRef.LeftController => LeftController;

        Transform IOVRCameraRigRef.RightController => RightController;

        protected virtual void Start()
        {
            _ovrCameraRig = FindObjectOfType<OVRCameraRig>();

            CameraRig = _ovrCameraRig;

            LeftHand = GetHandCached(ref _leftHand, _ovrCameraRig.leftHandAnchor);
            RightHand = GetHandCached(ref _rightHand, _ovrCameraRig.rightHandAnchor);

            LeftController = _ovrCameraRig.leftControllerAnchor;
            RightController = _ovrCameraRig.rightControllerAnchor;

            this.BeginStart(ref _started);
            this.AssertField(_ovrCameraRig, nameof(_ovrCameraRig));
            this.EndStart(ref _started);
        }

        protected virtual void FixedUpdate()
        {
            _isLateUpdate = false;
        }

        protected virtual void Update()
        {
            _isLateUpdate = false;
        }

        protected virtual void LateUpdate()
        {
            _isLateUpdate = true;
        }

        protected virtual void OnEnable()
        {
            if (_started)
            {
                CameraRig.UpdatedAnchors += HandleInputDataDirtied;
            }
        }

        protected virtual void OnDisable()
        {
            if (_started)
            {
                CameraRig.UpdatedAnchors -= HandleInputDataDirtied;
            }
        }

        private OVRHand GetHandCached(ref OVRHand cachedValue, Transform handAnchor)
        {
            if (cachedValue != null)
            {
                return cachedValue;
            }

            cachedValue = handAnchor.GetComponentInChildren<OVRHand>(true);
            if (_requireOvrHands)
            {
                Assert.IsNotNull(cachedValue);
            }

            return cachedValue;
        }

        private void HandleInputDataDirtied(OVRCameraRig cameraRig)
        {
            WhenInputDataDirtied(_isLateUpdate);
        }

        #region Inject
        public void InjectAllOVRCameraRigRef(OVRCameraRig ovrCameraRig, bool requireHands)
        {
            InjectInteractionOVRCameraRig(ovrCameraRig);
            InjectRequireHands(requireHands);
        }

        public void InjectInteractionOVRCameraRig(OVRCameraRig ovrCameraRig)
        {
            _ovrCameraRig = ovrCameraRig;
            // Clear the cached values to force new values to be read on next access
            _leftHand = null;
            _rightHand = null;
        }

        public void InjectRequireHands(bool requireHands)
        {
            _requireOvrHands = requireHands;
        }
        #endregion
    }
}
