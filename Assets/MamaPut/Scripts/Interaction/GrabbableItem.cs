using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Inputs.Haptics;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

namespace MamaPut
{
    [RequireComponent(typeof(XRGrabInteractable))]
    public abstract class GrabbableItem : MonoBehaviour, IGrabbable
    {
        protected XRGrabInteractable grab;
        HapticImpulsePlayer haptics;

        public bool IsHeld => grab != null && grab.isSelected;

        protected virtual void Awake()
        {
            grab = GetComponent<XRGrabInteractable>();
            grab.selectEntered.AddListener(HandleGrab);
            grab.selectExited.AddListener(HandleRelease);
        }

        protected virtual void OnDestroy()
        {
            if (grab == null) return;
            grab.selectEntered.RemoveListener(HandleGrab);
            grab.selectExited.RemoveListener(HandleRelease);
        }

        void HandleGrab(SelectEnterEventArgs args)
        {
            haptics = args.interactorObject.transform.GetComponentInParent<HapticImpulsePlayer>();
            OnGrabbed();
        }

        void HandleRelease(SelectExitEventArgs args)
        {
            OnReleased();
            haptics = null;
        }

        protected virtual void OnGrabbed() => Pulse(0.2f, 0.05f);
        protected virtual void OnReleased() { }

        protected void Pulse(float amplitude, float duration)
        {
            if (haptics != null) haptics.SendHapticImpulse(amplitude, duration);
        }

        // Force-drop, e.g. when a customer takes the plate out of your hand.
        public void LetGo() => ForceRelease();

        protected void ForceRelease()
        {
            if (grab.isSelected && grab.interactionManager != null)
                grab.interactionManager.CancelInteractableSelection((IXRSelectInteractable)grab);
        }
    }
}
