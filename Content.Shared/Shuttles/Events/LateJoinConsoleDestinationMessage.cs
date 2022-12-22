using Robust.Shared.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Content.Shared.Shuttles.Events
{
    [Serializable, NetSerializable]
    public sealed class LateJoinConsoleDestinationMessage : BoundUserInterfaceMessage
    {
        public EntityUid Destination;
    }
}
