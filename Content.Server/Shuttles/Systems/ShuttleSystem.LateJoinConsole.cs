using Content.Server.Shuttles.Components;
using Content.Server.Shuttles.Events;
using Content.Server.UserInterface;
using Content.Shared.Shuttles.BUIStates;
using Content.Shared.Shuttles.Events;
using Content.Shared.Shuttles.Systems;
using Robust.Shared.Physics.Components;
using Robust.Shared.Player;

namespace Content.Server.Shuttles.Systems
{
    public sealed partial class ShuttleSystem
    {
        private void InitializeLateJoinConsole()
        {
            SubscribeLocalEvent<LateJoinShuttleConsoleComponent, ComponentStartup>(OnLateJoinStartup);
            SubscribeLocalEvent<LateJoinShuttleConsoleComponent, LateJoinConsoleDestinationMessage>(OnDestinationMessage);
            SubscribeLocalEvent<DockEvent>(OnDock);
            SubscribeLocalEvent<UndockEvent>(OnUndock);
        }
        private void OnLateJoinStartup(EntityUid uid, LateJoinShuttleConsoleComponent component, ComponentStartup args)
        {
            UpdateLateJoinConsoleState(component);
        }
        private void OnDock(DockEvent ev)
        {
            RefreshLateJoinConsoles();
        }

        private void OnUndock(UndockEvent ev)
        {
            RefreshLateJoinConsoles();
        }
        public void RefreshLateJoinConsoles()
        {
            foreach (var comp in EntityQuery<LateJoinShuttleConsoleComponent>(true))
            {
                UpdateLateJoinConsoleState(comp);
            }
        }
        private void UpdateLateJoinConsoleState(LateJoinShuttleConsoleComponent component)
        {
            // Some peace of shitcode, oh yeah
            EntityUid? entity = component.Owner;
            var getShuttleEv = new ConsoleShuttleEvent
            {
                Console = entity,
            };
            RaiseLocalEvent(entity.Value, ref getShuttleEv);
            entity = getShuttleEv.Console;
            TryComp<TransformComponent>(entity, out var consoleXform);
            TryComp<ShuttleComponent>(consoleXform?.GridUid, out var shuttle);
            var destinations = new List<(EntityUid, string, bool)>();
            var ftlState = FTLState.Available;
            if (TryComp<FTLComponent>(shuttle?.Owner, out var shuttleFtl))
            {
                ftlState = shuttleFtl.State;
            }
            if (entity != null && shuttle?.Owner != null && (!TryComp<PhysicsComponent>(shuttle?.Owner, out var shuttleBody) ||
                shuttleBody.Mass < 1000f))
            {
                var metaQuery = GetEntityQuery<MetaDataComponent>();
                var locked = shuttleFtl != null || Paused(shuttle!.Owner);
                foreach (var comp in EntityQuery<FTLDestinationComponent>(true))
                {
                    if (comp.Owner == shuttle?.Owner ||
                        comp.Whitelist?.IsValid(entity.Value) == false) continue;
                    var meta = metaQuery.GetComponent(comp.Owner);
                    var name = meta.EntityName;
                    if (string.IsNullOrEmpty(name))
                        name = Loc.GetString("shuttle-console-unknown");
                    var canTravel = !locked &&
                                    comp.Enabled &&
                                    !Paused(comp.Owner, meta) &&
                                    (!TryComp<FTLComponent>(comp.Owner, out var ftl) || ftl.State == FTLState.Cooldown);
                    if (canTravel && consoleXform?.MapUid == Transform(comp.Owner).MapUid)
                    {
                        canTravel = false;
                    }
                    if (!canTravel) continue;
                    destinations.Add((comp.Owner, name, canTravel));
                }
            }
            var newState = new LateJoinConsoleBoundUserInterfaceState(
                ftlState,
                destinations
                );
            component.UpdateUserInterface(newState);
        }
        private void OnDestinationMessage(EntityUid uid, LateJoinShuttleConsoleComponent component, LateJoinConsoleDestinationMessage args)
        {
            var player = args.Session.AttachedEntity;
            if (player == null) return;
            if (!TryComp<FTLDestinationComponent>(args.Destination, out var dest)) return;
            if (!dest.Enabled) return;
            EntityUid? entity = component.Owner;
            var getShuttleEv = new ConsoleShuttleEvent
            {
                Console = uid,
            };
            RaiseLocalEvent(entity.Value, ref getShuttleEv);
            entity = getShuttleEv.Console;
            if (entity == null || dest.Whitelist?.IsValid(entity.Value, EntityManager) == false) return;
            if (!TryComp<TransformComponent>(entity, out var xform) ||
                !TryComp<ShuttleComponent>(xform.GridUid, out var shuttle)) return;
            if (HasComp<FTLComponent>(xform.GridUid))
            {
                if (args.Session.AttachedEntity != null)
                    _popup.PopupCursor(Loc.GetString("shuttle-console-in-ftl"), Filter.Entities(args.Session.AttachedEntity.Value));

                return;
            }
            if (!CanFTL(shuttle.Owner, out var reason))
            {
                if (args.Session.AttachedEntity != null)
                    _popup.PopupCursor(reason, Filter.Entities(args.Session.AttachedEntity.Value));

                return;
            }
            FTLTravel(shuttle, args.Destination, hyperspaceTime: TransitTime, dock: true);
        }
    }
}
