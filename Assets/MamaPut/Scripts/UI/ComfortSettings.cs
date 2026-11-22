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

        public bool Seated => seated;

        void Start()
        {
            seated = PlayerPrefs.GetInt(SeatedKey, 0) == 1;
            Apply();
            // Saved from the settings panel; smooth turning and full volume by default.
            SetSnapTurn(PlayerPrefs.GetInt("mamaput.snap", 0) == 1);
            AudioListener.volume = PlayerPrefs.GetFloat("mamaput.volume", 1f);
        }

        public void ToggleSeated()
        {
            seated = !seated;
            PlayerPrefs.SetInt(SeatedKey, seated ? 1 : 0);
            Apply();
        }

        // Snap turn on: right stick turns in steps. Off: it turns smoothly as you push.
        public void SetSnapTurn(bool on)
        {
            if (snapTurnProvider != null) snapTurnProvider.enabled = on;
            // The starter rig's ControllerInputActionManager picks snap or smooth; set it by name so this
            // script doesn't depend on the sample's assembly.
            foreach (var mb in GetComponentsInChildren<MonoBehaviour>(true))
            {
                var type = mb.GetType();
                if (type.Name == "ControllerInputActionManager")
                    type.GetProperty("smoothTurnEnabled")?.SetValue(mb, !on);
                else if (type.Name.Contains("ContinuousTurnProvider"))
                    mb.enabled = !on;
            }
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
