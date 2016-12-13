using System;
using Fusee.Math.Core;
using ProtoBuf;

namespace NetworkHandler
{
    [ProtoContract]
    public class ControlsForNetwork
    {
        [ProtoMember(1)]
        public float _WSAxis { get; set; }
        [ProtoMember(2)]
        public float _ADAxis { get; set; }
        [ProtoMember(3)]
        public float3 _Position { get; set; }
    }

}
