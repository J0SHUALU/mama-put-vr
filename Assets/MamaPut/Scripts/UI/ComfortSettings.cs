using UnityEngine;
using Unity.XR.CoreUtils;

namespace MamaPut
{
    // Seated mode lifts the player instead of lowering the whole stall.
    public class ComfortSettings : MonoBehaviour
    {
        [SerializeField] XROrigin origin;
        [SerializeField] float seatedLift = 0.45f;
        [SerializeField] MonoBehaviour snapTurnProvider;
        [SerializeField] TMPro.TMP_Text seatedLabel;

        const string SeatedKey = "mamaput.seated";
        bool seated;

        void Start()
        {
            seated = PlayerPrefs.GetInt(SeatedKey, 0) == 1;
            Apply();
        }

        public void ToggleSeated()
        {
            seated = !seated;
            PlayerPrefs.SetInt(SeatedKey, seated ? 1 : 0);
            Apply();
        }

        public void SetSnapTurn(bool on)
        {
            if (snapTurnProvider != null) snapTurnProvider.enabled = on;
        }

        void Apply()
        {
            // CameraYOffset is ignored in floor tracking mode, so move the offset object directly.
            var offset = origin.CameraFloorOffsetObject.transform;
            var p = offset.localPosition;
            p.y = seated ? seatedLift : 0f;
            offset.localPosition = p;
            if (seatedLabel != null) seatedLabel.text = seated ? "Seated: on" : "Seated: off";
        }
    }
}
