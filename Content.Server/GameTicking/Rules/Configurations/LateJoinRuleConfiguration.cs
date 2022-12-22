using Content.Shared.Dataset;
using Content.Shared.Humanoid.Prototypes;
using Content.Shared.Roles;
using Robust.Shared.Audio;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype;
using Robust.Shared.Serialization.TypeSerializers.Implementations;
using Robust.Shared.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Content.Server.GameTicking.Rules.Configurations
{
    public sealed class LateJoinRuleConfiguration : GameRuleConfiguration
    {
        public override string Id => "LateJoin";

        [DataField("spawnPointProto", customTypeSerializer: typeof(PrototypeIdSerializer<StartingGearPrototype>))]
        public string SpawnPointPrototype = "SpawnPointLateJoin";

        [DataField("outpostMap", customTypeSerializer: typeof(ResourcePathSerializer))]
        public ResourcePath? LateJoinOutpostMap = new("/Maps/latejoinoutpost.yml");

        [DataField("shuttleMap", customTypeSerializer: typeof(ResourcePathSerializer))]
        public ResourcePath? LateJoinShuttleMap = new("/Maps/Shuttles/latejoinshuttle.yml");
    }
}
