using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Fusee.Math.Core;
using ProtoBuf;

namespace NetworkHandler
{
    [ProtoContract]
    public class TransformationData
    {
        [ProtoMember(1)]
        public float3 _Translation { get; set; }
        [ProtoMember(2)]
        public float3 _Rotation { get; set; }
    }
}
