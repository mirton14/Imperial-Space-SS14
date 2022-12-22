using Content.Server.UserInterface;
using Content.Shared.Shuttles.BUIStates;
using Content.Shared.Shuttles.Systems;
using Robust.Server.GameObjects;

namespace Content.Server.Shuttles.Components
{
    [RegisterComponent]
    public sealed class LateJoinShuttleConsoleComponent : Component
    {
        [ViewVariables] private BoundUserInterface? UserInterface => Owner.GetUIOrNull(LateJoinConsoleUiKey.Key);
        protected override void Initialize()
        {
            base.Initialize();
            Owner.EnsureComponentWarn<ServerUserInterfaceComponent>();
        }
        public void UpdateUserInterface(LateJoinConsoleBoundUserInterfaceState state)
        {
            if (!Initialized || UserInterface == null)
                return;
            UserInterface.SetState(state);
        }
    }
}
