using UnityEngine;

namespace MamaPut
{
    public class CustomerPlateHold : MonoBehaviour
    {
        [SerializeField] float blendTime = 0.35f;
        [SerializeField] float forward = 0.28f;
        [SerializeField] float aboveHips = 0.1f;

        Animator animator;
        Transform hips;
        Transform root;
        Transform carry;
        Transform plate;
        float rimOffset = 0.12f;
        float weight;

        void Awake()
        {
            animator = GetComponent<Animator>();
            if (animator != null && animator.isHuman) hips = animator.GetBoneTransform(HumanBodyBones.Hips);
        }

        public void Setup(Transform customerRoot, Transform carryPoint)
        {
            root = customerRoot;
            carry = carryPoint;
            PlaceCarry();
        }

        public void Hold(Transform target)
        {
            plate = target;
            if (plate == null) return;
            var r = plate.GetComponentInChildren<Renderer>();
            if (r != null) rimOffset = Mathf.Max(0.08f, r.bounds.extents.x * 0.85f);
        }

        void Update()
        {
            float goal = plate != null && plate.gameObject.activeInHierarchy ? 1f : 0f;
            weight = Mathf.MoveTowards(weight, goal, Time.deltaTime / blendTime);
        }

        void LateUpdate()
        {
            PlaceCarry();
            if (plate != null && carry != null) plate.SetPositionAndRotation(carry.position, carry.rotation);
        }

        void PlaceCarry()
        {
            if (carry == null || root == null) return;
            float y = hips != null ? hips.position.y + aboveHips : root.position.y + 1f;
            carry.SetPositionAndRotation(
                new Vector3(root.position.x, y, root.position.z) + root.forward * forward,
                Quaternion.LookRotation(root.forward, Vector3.up));
        }

        void OnAnimatorIK(int layer)
        {
            if (animator == null || root == null) return;
            animator.SetIKPositionWeight(AvatarIKGoal.LeftHand, weight);
            animator.SetIKPositionWeight(AvatarIKGoal.RightHand, weight);
            animator.SetIKHintPositionWeight(AvatarIKHint.LeftElbow, weight * 0.6f);
            animator.SetIKHintPositionWeight(AvatarIKHint.RightElbow, weight * 0.6f);
            if (weight <= 0f || plate == null) return;

            Vector3 side = root.right * rimOffset;
            Vector3 p = plate.position + Vector3.down * 0.01f;
            animator.SetIKPosition(AvatarIKGoal.LeftHand, p - side);
            animator.SetIKPosition(AvatarIKGoal.RightHand, p + side);

            Vector3 elbowUp = Vector3.up * 0.05f - root.forward * 0.05f;
            animator.SetIKHintPosition(AvatarIKHint.LeftElbow, p - root.right * (rimOffset + 0.15f) + elbowUp);
            animator.SetIKHintPosition(AvatarIKHint.RightElbow, p + root.right * (rimOffset + 0.15f) + elbowUp);
        }
    }
}
