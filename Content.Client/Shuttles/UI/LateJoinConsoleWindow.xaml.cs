using Content.Client.Computer;
using Content.Client.UserInterface.Controls;
using Content.Shared.Shuttles.BUIStates;
using Content.Shared.Shuttles.Systems;
using Robust.Client.AutoGenerated;
using Robust.Client.UserInterface.Controls;
using Robust.Client.UserInterface.XAML;
using Robust.Shared.Timing;

namespace Content.Client.Shuttles.UI;

[GenerateTypedNameReferences]
public sealed partial class LateJoinConsoleWindow : FancyWindow, IComputerWindow<LateJoinConsoleBoundUserInterfaceState>
{
    private readonly IGameTiming _timing;
    private readonly Dictionary<BaseButton, EntityUid> _destinations = new();
    public TimeSpan FTLTime;
    public Action<EntityUid>? DestinationPressed;
    public LateJoinConsoleWindow()
    {
        RobustXamlLoader.Load(this);
        _timing = IoCManager.Resolve<IGameTiming>();
    }
    public void UpdateState(LateJoinConsoleBoundUserInterfaceState ljcc)
    {
        UpdateFTL(ljcc.Destinations, ljcc.FTLState);
    }
    private void UpdateFTL(List<(EntityUid Entity, string Destination, bool Enabled)> destinations, FTLState state)
    {
        HyperspaceDestinations.DisposeAllChildren();
        _destinations.Clear();
        if (destinations.Count == 0)
        {
            HyperspaceDestinations.AddChild(new Label()
            {
                Text = Loc.GetString("shuttle-console-hyperspace-none"),
                HorizontalAlignment = HAlignment.Center,
            });
        }
        else
        {
            destinations.Sort((x, y) => string.Compare(x.Destination, y.Destination, StringComparison.Ordinal));
            foreach (var destination in destinations)
            {
                var button = new Button()
                {
                    Disabled = !destination.Enabled,
                    Text = destination.Destination,
                };

                _destinations[button] = destination.Entity;
                button.OnPressed += OnHyperspacePressed;
                HyperspaceDestinations.AddChild(button);
            }
        }
        string stateText;
        switch (state)
        {
            case Shared.Shuttles.Systems.FTLState.Available:
                stateText = Loc.GetString("shuttle-console-ftl-available");
                break;
            case Shared.Shuttles.Systems.FTLState.Starting:
                stateText = Loc.GetString("shuttle-console-ftl-starting");
                break;
            case Shared.Shuttles.Systems.FTLState.Travelling:
                stateText = Loc.GetString("shuttle-console-ftl-travelling");
                break;
            case Shared.Shuttles.Systems.FTLState.Cooldown:
                stateText = Loc.GetString("shuttle-console-ftl-cooldown");
                break;
            case Shared.Shuttles.Systems.FTLState.Arriving:
                stateText = Loc.GetString("shuttle-console-ftl-arriving");
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(state), state, null);
        }
        FTLState.Text = stateText;
    }
    private void OnHyperspacePressed(BaseButton.ButtonEventArgs obj)
    {
        var ent = _destinations[obj.Button];
        DestinationPressed?.Invoke(ent);
    }
}


