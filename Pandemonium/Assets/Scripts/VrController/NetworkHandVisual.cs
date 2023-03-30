using Oculus.Interaction.Input;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using Unity.Netcode;

namespace Oculus.Interaction
{
    public class NetworkHandVisual : NetworkBehaviour, IHandVisual
    {
        [SerializeField, Interface(typeof(IHand))]
        private MonoBehaviour _hand;
        public IHand Hand { get; private set; }

        [SerializeField]
        private SkinnedMeshRenderer _skinnedMeshRenderer;

        [SerializeField]
        private bool _updateRootPose = true;

        [SerializeField]
        private bool _updateRootScale = true;

        [SerializeField, Optional]
        private Transform _root = null;

        [SerializeField, Optional]
        private MaterialPropertyBlockEditor _handMaterialPropertyBlockEditor;

        [HideInInspector]
        [SerializeField]
        private List<Transform> _jointTransforms = new List<Transform>();
        public event Action WhenHandVisualUpdated = delegate { };

        public bool IsVisible => _skinnedMeshRenderer != null && _skinnedMeshRenderer.enabled;

        private int _wristScalePropertyId;

        public IList<Transform> Joints => _jointTransforms;

        public bool ForceOffVisibility { get; set; }

        private bool _started = false;

        public string tagParent;

        protected virtual void Awake()
        {
            Hand = _hand as IHand;
            if (_root == null && _jointTransforms.Count > 0 && _jointTransforms[0] != null)
            {
                _root = _jointTransforms[0].parent;
            }
        }

        public override void OnNetworkSpawn()
        {
            if (!IsOwner ) return;


            Hand = GameObject.FindGameObjectWithTag(tagParent).GetComponent<IHand>();

            this.BeginStart(ref _started);
            this.AssertField(Hand, nameof(Hand));
            this.AssertField(_skinnedMeshRenderer, nameof(_skinnedMeshRenderer));
            if (_handMaterialPropertyBlockEditor != null)
            {
                _wristScalePropertyId = Shader.PropertyToID("_WristScale");
            }

            this.EndStart(ref _started);
        }

        protected virtual void OnEnable()
        {
            if (_started)
            {
                Hand.WhenHandUpdated += UpdateSkeleton;
            }
        }

        protected virtual void OnDisable()
        {
            if (_started && _hand != null)
            {
                Hand.WhenHandUpdated -= UpdateSkeleton;
            }
        }

        public void UpdateSkeleton()
        {
            if (!Hand.IsTrackedDataValid)
            {
                if (IsVisible || ForceOffVisibility)
                {
                    _skinnedMeshRenderer.enabled = false;
                }
                WhenHandVisualUpdated.Invoke();
                return;
            }

            if (!IsVisible && !ForceOffVisibility)
            {
                _skinnedMeshRenderer.enabled = true;
            }
            else if (IsVisible && ForceOffVisibility)
            {
                _skinnedMeshRenderer.enabled = false;
            }

            if (_updateRootPose)
            {
                if (_root != null && Hand.GetRootPose(out Pose handRootPose))
                {
                    _root.position = handRootPose.position;
                    _root.rotation = handRootPose.rotation;
                }
            }

            if (_updateRootScale)
            {
                if (_root != null)
                {
                    float parentScale = _root.parent != null ? _root.parent.lossyScale.x : 1f;
                    _root.localScale = Hand.Scale / parentScale * Vector3.one;
                }
            }

            if (!Hand.GetJointPosesLocal(out ReadOnlyHandJointPoses localJoints))
            {
                return;
            }
            for (var i = 0; i < Constants.NUM_HAND_JOINTS; ++i)
            {
                if (_jointTransforms[i] == null)
                {
                    continue;
                }
                _jointTransforms[i].SetPose(localJoints[i], Space.Self);
            }

            if (_handMaterialPropertyBlockEditor != null)
            {
                _handMaterialPropertyBlockEditor.MaterialPropertyBlock.SetFloat(_wristScalePropertyId, Hand.Scale);
                _handMaterialPropertyBlockEditor.UpdateMaterialPropertyBlock();
            }
            WhenHandVisualUpdated.Invoke();
        }

        public Transform GetTransformByHandJointId(HandJointId handJointId)
        {
            return _jointTransforms[(int)handJointId];
        }

        public Pose GetJointPose(HandJointId jointId, Space space)
        {
            return GetTransformByHandJointId(jointId).GetPose(space);
        }

        #region Inject

        public void InjectAllHandSkeletonVisual(IHand hand, SkinnedMeshRenderer skinnedMeshRenderer)
        {
            InjectHand(hand);
            InjectSkinnedMeshRenderer(skinnedMeshRenderer);
        }

        public void InjectHand(IHand hand)
        {
            _hand = hand as MonoBehaviour;
            Hand = hand;
        }

        public void InjectSkinnedMeshRenderer(SkinnedMeshRenderer skinnedMeshRenderer)
        {
            _skinnedMeshRenderer = skinnedMeshRenderer;
        }

        public void InjectOptionalUpdateRootPose(bool updateRootPose)
        {
            _updateRootPose = updateRootPose;
        }

        public void InjectOptionalUpdateRootScale(bool updateRootScale)
        {
            _updateRootScale = updateRootScale;
        }

        public void InjectOptionalRoot(Transform root)
        {
            _root = root;
        }

        public void InjectOptionalMaterialPropertyBlockEditor(MaterialPropertyBlockEditor editor)
        {
            _handMaterialPropertyBlockEditor = editor;
        }

        #endregion
    }
}
