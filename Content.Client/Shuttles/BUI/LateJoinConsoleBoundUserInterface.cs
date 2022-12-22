using Content.Client.Shuttles.UI;
using Content.Shared.Shuttles.BUIStates;
using Robust.Client.GameObjects;
using Content.Shared.Shuttles.Events;

namespace Content.Client.Shuttles.BUI
{
    public sealed class LateJoinConsoleBoundUserInterface : BoundUserInterface
    {
        private LateJoinConsoleWindow? _window;

        public LateJoinConsoleBoundUserInterface(ClientUserInterfaceComponent owner, Enum uiKey) : base(owner, uiKey) { }

        protected override void Open()
        {
            base.Open();
            _window = new LateJoinConsoleWindow();
            _window.DestinationPressed += OnDestinationPressed;
            _window.OpenCentered();
            _window.OnClose += OnClose;
        }

        private void OnDestinationPressed(EntityUid obj)
        {
            SendMessage(new LateJoinConsoleDestinationMessage()
            {
                Destination = obj,
            });
        }

        private void OnClose()
        {
            Close();
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            if (disposing)
            {
                _window?.Dispose();
            }
        }
        protected override void UpdateState(BoundUserInterfaceState state)
        {
            base.UpdateState(state);
            if (state is not LateJoinConsoleBoundUserInterfaceState ljcState) return;
            _window?.UpdateState(ljcState);
        }
    }
}
