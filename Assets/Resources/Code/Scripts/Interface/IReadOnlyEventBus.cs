using Player;
using UnityEngine.Events;

namespace EventBusInterface
{
    public interface IReadOnlyEventBus
    {
        public UnityEvent<MoveController.CordinateSide> Event { get; }
    }
}
