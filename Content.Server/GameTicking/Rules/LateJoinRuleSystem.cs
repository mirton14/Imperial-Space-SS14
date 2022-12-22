using Content.Server.GameTicking.Rules.Configurations;
using Content.Server.Shuttles.Components;
using Content.Server.Shuttles.Systems;
using Robust.Server.GameObjects;
using Robust.Server.Maps;
using Robust.Shared.Map;
using System.Linq;

namespace Content.Server.GameTicking.Rules
{
    public sealed class LateJoinRuleSystem : GameRuleSystem
    {
        [Dependency] private readonly IMapManager _mapManager = default!;
        [Dependency] private readonly MapLoaderSystem _map = default!;
        [Dependency] private readonly ShuttleSystem _shuttleSystem = default!;

        private MapId? _lateJoinMapId;

        private EntityUid? _lateJoinOutpost;
        private EntityUid? _lateJoinShuttle;
        private EntityUid? _targetStation;

        public override string Prototype => "LateJoin";

        private LateJoinRuleConfiguration _lateJoinRuleConfig = new();


        public override void Initialize()
        {
            base.Initialize();
        }
        private void OnRunLevelChanged(GameRunLevelChangedEvent ev)
        {
            switch (ev.New)
            {
                case GameRunLevel.InRound:
                    OnRoundStart();
                    break;
                case GameRunLevel.PostRound:
                    OnRoundEnd();
                    break;
            }
        }

        private void OnRoundStart()
        {
        }
        private void OnRoundEnd()
        {
        }
        private bool SpawnMap()
        {
            if (_lateJoinMapId != null)
                return true; // Map is already loaded.

            var path = _lateJoinRuleConfig.LateJoinOutpostMap;
            var shuttlePath = _lateJoinRuleConfig.LateJoinShuttleMap;
            if (path == null)
            {
                Logger.ErrorS("latejoin", "No station map specified for latejoin!");
                return false;
            }
            if (shuttlePath == null)
            {
                Logger.ErrorS("latejoin", "No shuttle map specified for latejoin!");
                return false;
            }

            var mapId = _mapManager.CreateMap();
            var options = new MapLoadOptions()
            {
                LoadMap = true,
            };

            if (!_map.TryLoad(mapId, path.ToString(), out var outpostGrids, options) || outpostGrids.Count == 0)
            {
                Logger.ErrorS("latejoin", $"Error loading map {path} for latejoin!");
                return false;
            }

            _lateJoinOutpost = outpostGrids[0];

            if (!_map.TryLoad(mapId, shuttlePath.ToString(), out var grids, new MapLoadOptions { Offset = Vector2.One * 1000f }) || !grids.Any())
            {
                Logger.ErrorS("latejoin", $"Error loading grid {shuttlePath} for latejoin!");
                return false;
            }

            var shuttleId = grids.First();

            if (Deleted(shuttleId))
            {
                Logger.ErrorS("latejoin", $"Tried to load latejoin shuttle as a map, aborting.");
                _mapManager.DeleteMap(mapId);
                return false;
            }

            if (TryComp<ShuttleComponent>(shuttleId, out var shuttle))
            {
                _shuttleSystem.TryFTLDock(shuttle, _lateJoinOutpost.Value);
            }

            _lateJoinMapId = mapId;
            _lateJoinShuttle = shuttleId;

            return true;
        }
        public override void Started()
        {
            _lateJoinOutpost = null;
            _lateJoinMapId = null;
            if (!SpawnMap())
            {
                Logger.InfoS("latejoin", "Failed to load map for latejoin");
                return;
            }
        }
        public override void Ended() { }
    }
}
