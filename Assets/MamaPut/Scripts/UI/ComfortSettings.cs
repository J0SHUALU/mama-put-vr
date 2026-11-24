using UnityEngine;
using Unity.XR.CoreUtils;

namespace MamaPut
{
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
            SetSnapTurn(PlayerPrefs.GetInt("mamaput.snap", 0) == 1);
            AudioListener.volume = PlayerPrefs.GetFloat("mamaput.volume", 1f);
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
            var offset = origin.CameraFloorOffsetObject.transform;
            var p = offset.localPosition;
            p.y = seated ? seatedLift : 0f;
            offset.localPosition = p;
            if (seatedLabel != null) seatedLabel.text = seated ? "Seated: on" : "Seated: off";
        }
    }
}
