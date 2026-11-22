using UnityEngine;

namespace MamaPut
{
    public abstract class FoodPot : MonoBehaviour, IFoodSource
    {
        [SerializeField] DishData dish;
        [SerializeField] int capacity;
        [Tooltip("Mesh that sinks as the pot empties. Optional.")]
        [SerializeField] Transform fillVisual;
        [Tooltip("How far down the food sinks and how much it narrows when the pot is nearly empty, so it stays inside sloped walls.")]
        [SerializeField] float emptyHeight = 0.55f;
        [SerializeField] float emptyWidth = 0.6f;

        int portionsLeft;
        float fullHeight;
        Vector3 fullScale;

        public DishData Dish => dish;
        public int PortionsLeft => portionsLeft;
        protected abstract int DefaultCapacity { get; }

        protected virtual void Reset() => capacity = DefaultCapacity;

        protected virtual void Awake()
        {
            if (capacity <= 0) capacity = DefaultCapacity;
            if (fillVisual != null)
            {
                fullHeight = fillVisual.localPosition.y;
                fullScale = fillVisual.localScale;
            }
            Refill(capacity);
        }

        public bool TakePortion()
        {
            if (portionsLeft <= 0) return false;
            portionsLeft--;
            UpdateVisual();
            if (portionsLeft == 0) GameEvents.RaisePotEmpty(this);
            return true;
        }

        public void Refill(int amount)
        {
            portionsLeft = Mathf.Min(capacity, portionsLeft + amount);
            UpdateVisual();
        }

        public void RefillToFull() => Refill(capacity);

        void UpdateVisual()
        {
            if (fillVisual == null) return;
            var p = fillVisual.localPosition;
            float t = (float)portionsLeft / capacity;
            p.y = Mathf.Lerp(fullHeight * emptyHeight, fullHeight, t);
            fillVisual.localPosition = p;
            float w = Mathf.Lerp(emptyWidth, 1f, t);
            fillVisual.localScale = new Vector3(fullScale.x * w, fullScale.y, fullScale.z * w);
            fillVisual.gameObject.SetActive(portionsLeft > 0);
        }
    }
}
