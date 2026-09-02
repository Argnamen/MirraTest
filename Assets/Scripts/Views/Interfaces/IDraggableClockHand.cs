using UnityEngine;

namespace ClockApp.Views.Interfaces
{
    public interface IDraggableClockHand
    {
        void OnDragStart();
        void OnDrag(Vector2 deltaPosition);
        void OnDragEnd();
    }
}
