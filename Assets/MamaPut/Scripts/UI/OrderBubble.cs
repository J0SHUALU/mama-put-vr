using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace MamaPut
{
    public class OrderBubble : MonoBehaviour
    {
        [SerializeField] GameObject root;
        [SerializeField] Image panel;
        [SerializeField] Image border;

        [Header("Order")]
        [SerializeField] GameObject orderGroup;
        [SerializeField] Image[] chips;
        [SerializeField] TMP_Text[] chipLabels;
        [SerializeField] TMP_Text[] chipNames;
        [SerializeField] Image patienceFill;

        [Header("Message")]
        [SerializeField] GameObject messageGroup;
        [SerializeField] TMP_Text message;

        [Header("Change")]
        [SerializeField] GameObject changeGroup;
        [SerializeField] TMP_Text paidLabel;
        [SerializeField] TMP_Text changeLabel;
        [SerializeField] TMP_Text[] noteLabels;

        [Header("Look")]
        [SerializeField] Color panelColor = new Color32(0x2C, 0x2C, 0x2A, 0xFF);
        [SerializeField] Color complaintColor = new Color32(0xD8, 0x5A, 0x30, 0xFF);
        [SerializeField] Color trimColor = new Color32(0xEF, 0x9F, 0x27, 0xFF);
        [SerializeField] Color lowPatienceColor = new Color32(0xE2, 0x4B, 0x4A, 0xFF);
        [SerializeField] float complaintSeconds = 4f;

        static readonly int[] Notes = { 1000, 500, 200, 100 };

        Transform cam;
        float complaintUntil;
        GameObject activeGroup;
        Vector3 restPosition;

        void Awake()
        {
            if (Camera.main != null) cam = Camera.main.transform;
            restPosition = root.transform.localPosition;
            Hide();
        }

        void LateUpdate()
        {
            if (cam == null && Camera.main != null) cam = Camera.main.transform;
            if (cam != null)
            {
                var dir = transform.position - cam.position;
                dir.y = 0f;
                if (dir.sqrMagnitude > 0.001f) transform.rotation = Quaternion.LookRotation(dir);
            }

            if (complaintUntil <= 0f) return;
            float left = complaintUntil - Time.time;
            float shake = left > complaintSeconds - 0.3f ? Mathf.Sin(Time.time * 60f) * 0.01f : 0f;
            root.transform.localPosition = restPosition + new Vector3(shake, 0f, 0f);
            if (left <= 0f) EndComplaint();
        }

        public void ShowOrder(Order order)
        {
            int i = 0;
            foreach (var item in order.Items)
            {
                if (i >= chips.Length) break;
                chips[i].transform.parent.gameObject.SetActive(true);
                chips[i].color = item.Key.chipColor;
                chipLabels[i].color = item.Key.chipTextColor;
                chipLabels[i].text = item.Key.shortLabel;
                chipNames[i].text = item.Value > 1 ? $"{item.Value} {item.Key.displayName}" : item.Key.displayName;
                i++;
            }
            for (; i < chips.Length; i++) chips[i].transform.parent.gameObject.SetActive(false);
            Switch(orderGroup);
        }

        public void ShowMessage(string text)
        {
            message.text = text;
            Switch(messageGroup);
        }

        public void ShowChange(int paid, int stillOwed)
        {
            paidLabel.text = "Paid " + Naira.Format(paid);
            changeLabel.text = "Change " + Naira.Format(stillOwed);
            var notes = new List<int>();
            int left = stillOwed;
            foreach (var n in Notes) while (left >= n) { notes.Add(n); left -= n; }
            for (int i = 0; i < noteLabels.Length; i++)
            {
                bool on = i < notes.Count;
                noteLabels[i].transform.parent.gameObject.SetActive(on);
                if (on) noteLabels[i].text = Naira.Format(notes[i]);
            }
            Switch(changeGroup);
        }

        public void Complain(string text)
        {
            root.SetActive(true);
            message.text = text;
            orderGroup.SetActive(false);
            changeGroup.SetActive(false);
            messageGroup.SetActive(true);
            panel.color = complaintColor;
            border.color = panelColor;
            complaintUntil = Time.time + complaintSeconds;
        }

        public void SetPatience(float t)
        {
            if (patienceFill == null) return;
            patienceFill.fillAmount = t;
            patienceFill.color = t < 0.3f ? lowPatienceColor : trimColor;
        }

        public void Hide()
        {
            complaintUntil = 0f;
            activeGroup = null;
            ResetLook();
            if (root != null) root.SetActive(false);
        }

        void Switch(GameObject group)
        {
            activeGroup = group;
            if (complaintUntil > 0f) return;
            root.SetActive(true);
            orderGroup.SetActive(group == orderGroup);
            messageGroup.SetActive(group == messageGroup);
            changeGroup.SetActive(group == changeGroup);
        }

        void EndComplaint()
        {
            complaintUntil = 0f;
            ResetLook();
            if (activeGroup != null) Switch(activeGroup);
            else root.SetActive(false);
        }

        void ResetLook()
        {
            if (panel != null) panel.color = panelColor;
            if (border != null) border.color = trimColor;
            if (root != null) root.transform.localPosition = restPosition;
        }
    }
}
