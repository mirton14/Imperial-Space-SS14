using Content.Shared.Shuttles.Systems;
using Robust.Shared.Serialization;

namespace Content.Shared.Shuttles.BUIStates
{
    [Serializable, NetSerializable]
    public sealed class LateJoinConsoleBoundUserInterfaceState : BoundUserInterfaceState
    {
        public readonly FTLState FTLState;
        public List<(EntityUid Entity, string Destination, bool Enabled)> Destinations;
        public LateJoinConsoleBoundUserInterfaceState(
            FTLState ftlState,
            List<(EntityUid Entity, string Destination, bool Enabled)> destinations)
        {
            FTLState = ftlState;
            Destinations = destinations;
        }
    }
}
