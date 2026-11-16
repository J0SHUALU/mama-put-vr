using UnityEngine;

namespace MamaPut
{
    public abstract class FoodPot : MonoBehaviour, IFoodSource
    {
        [SerializeField] DishData dish;
        [SerializeField] int capacity;
        [Tooltip("Mesh that sinks as the pot empties. Optional.")]
        [SerializeField] Transform fillVisual;

        int portionsLeft;
        float fullHeight;

        public DishData Dish => dish;
        public int PortionsLeft => portionsLeft;
        protected abstract int DefaultCapacity { get; }

        protected virtual void Reset() => capacity = DefaultCapacity;

        protected virtual void Awake()
        {
            if (capacity <= 0) capacity = DefaultCapacity;
            if (fillVisual != null) fullHeight = fillVisual.localPosition.y;
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
            p.y = Mathf.Lerp(0f, fullHeight, (float)portionsLeft / capacity);
            fillVisual.localPosition = p;
            fillVisual.gameObject.SetActive(portionsLeft > 0);
        }
    }
}
