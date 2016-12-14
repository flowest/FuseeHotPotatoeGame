﻿using Fusee.Engine.Common;
using Fusee.Math.Core;
using ProtoBuf;

namespace NetworkHandler
{
    [ProtoContract]
    public class SynchronizationData
    {
        [ProtoMember(1)]
        public float3 _Translation { get; set; }
        [ProtoMember(2)]
        public float3 _Rotation { get; set; }
        [ProtoMember(3)]
        public long _RemoteIPAdress { get; set; }
    }
}
