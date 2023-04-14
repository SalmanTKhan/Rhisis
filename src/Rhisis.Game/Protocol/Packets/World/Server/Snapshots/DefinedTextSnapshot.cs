﻿using Rhisis.Game.Common;
using Rhisis.Game.Entities;
using Rhisis.Protocol;
using System.Linq;

namespace Rhisis.Game.Protocol.Packets.World.Server.Snapshots;

public class DefinedTextSnapshot : FFSnapshot
{
    public DefinedTextSnapshot(Player player, DefineText textId, params object[] parameters)
        : base(parameters.Any() ? SnapshotType.DEFINEDTEXT : SnapshotType.DEFINEDTEXT1, player.ObjectId)
    {
        WriteInt32((int)textId);
        WriteString(string.Join(" ", parameters));
    }
}