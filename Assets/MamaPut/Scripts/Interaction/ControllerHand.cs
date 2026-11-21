using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

namespace MamaPut
{
    // Shows a hand where the controller model would be and curls the fingers
    // with grip and trigger, so the player sees their hands instead of plastic.
    public class ControllerHand : MonoBehaviour
    {
        [SerializeField] bool leftHand = true;
        [SerializeField] Transform model;
        [SerializeField, Range(0f, 1f)] float previewGrip;
        [SerializeField, Range(0f, 1f)] float previewTrigger;
        [SerializeField] float curlSpeed = 14f;

        struct Joint
        {
            public Transform bone;
            public Quaternion rest;
            public Vector3 axis;   // in the parent's space
            public float maxAngle;
            public bool index;
            public bool thumb;
        }

        readonly List<Joint> joints = new List<Joint>();
        float grip, trigger;
        bool ready;

        void Awake()
        {
            var bones = FindBones();
            if (bones == null) return;
            Vector3 palm = Align(bones);
            BuildJoints(bones, palm);
        }

        // Also called from the editor so the hand sits right in the scene before you press Play.
        public void AlignInEditor()
        {
            var bones = FindBones();
            if (bones != null) Align(bones);
        }

        Dictionary<string, Transform> FindBones()
        {
            if (model == null) return null;
            var bones = new Dictionary<string, Transform>();
            foreach (var t in model.GetComponentsInChildren<Transform>(true))
            {
                int cut = t.name.IndexOf('_');
                if (cut > 0) bones[t.name.Substring(cut + 1)] = t;
            }
            return bones.ContainsKey("Wrist") && bones.ContainsKey("MiddleProximal") && bones.ContainsKey("MiddleTip") ? bones : null;
        }

        Vector3 Align(Dictionary<string, Transform> bones)
        {
            var wrist = bones["Wrist"];
            var midProx = bones["MiddleProximal"];

            // Work out which way the fingers point and which way the palm faces from the rest pose,
            // then turn the model so it sits round the controller grip.
            Vector3 along = (midProx.position - wrist.position).normalized;
            Vector3 palm = Vector3.ProjectOnPlane(bones["MiddleTip"].position - midProx.position, along).normalized;
            float side = leftHand ? 1f : -1f;
            Vector3 targetAlong = transform.forward;
            Vector3 targetPalm = (transform.right * side - transform.up * 0.35f).normalized;
            Quaternion turn = Quaternion.LookRotation(targetAlong, targetPalm) * Quaternion.Inverse(Quaternion.LookRotation(along, palm));
            model.rotation = turn * model.rotation;
            var palmBone = bones.TryGetValue("Palm", out var p) ? p : midProx;
            model.position += transform.position - targetPalm * 0.035f - transform.forward * 0.03f - palmBone.position;
            return targetPalm;
        }

        void BuildJoints(Dictionary<string, Transform> bones, Vector3 palm)
        {
            foreach (var finger in new[] { "Index", "Middle", "Ring", "Little", "Thumb" })
            {
                string[] parts = finger == "Thumb" ? new[] { "Proximal", "Distal" } : new[] { "Proximal", "Intermediate", "Distal" };
                float[] angles = finger == "Thumb" ? new[] { 25f, 35f } : new[] { 65f, 85f, 55f };
                for (int i = 0; i < parts.Length; i++)
                {
                    if (!bones.TryGetValue(finger + parts[i], out var bone) || bone.childCount == 0) continue;
                    Vector3 dir = (bone.GetChild(0).position - bone.position).normalized;
                    Vector3 axis = Vector3.Cross(dir, palm).normalized;
                    joints.Add(new Joint
                    {
                        bone = bone,
                        rest = bone.localRotation,
                        axis = bone.parent.InverseTransformDirection(axis),
                        maxAngle = angles[i],
                        index = finger == "Index",
                        thumb = finger == "Thumb"
                    });
                }
            }
            ready = true;
        }

        void Update()
        {
            if (!ready) return;
            float g = previewGrip, t = previewTrigger;
            var device = InputDevices.GetDeviceAtXRNode(leftHand ? XRNode.LeftHand : XRNode.RightHand);
            if (device.isValid)
            {
                if (device.TryGetFeatureValue(CommonUsages.grip, out float dg)) g = Mathf.Max(g, dg);
                if (device.TryGetFeatureValue(CommonUsages.trigger, out float dt)) t = Mathf.Max(t, dt);
            }
            float k = 1f - Mathf.Exp(-curlSpeed * Time.deltaTime);
            grip = Mathf.Lerp(grip, g, k);
            trigger = Mathf.Lerp(trigger, t, k);
            Apply(grip, trigger);
        }

        void Apply(float g, float t)
        {
            foreach (var j in joints)
            {
                // Resting fingers sit a little curled round the grip.
                float amount = j.index ? Mathf.Lerp(0.15f, 1f, t) : j.thumb ? Mathf.Lerp(0.2f, 0.8f, g) : Mathf.Lerp(0.25f, 1f, g);
                j.bone.localRotation = Quaternion.AngleAxis(j.maxAngle * amount, j.axis) * j.rest;
            }
        }
    }
}
