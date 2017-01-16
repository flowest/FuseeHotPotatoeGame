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
        public float3 _Scale { get; set; }
        [ProtoMember(4)]
        public long _RemoteIPAdress { get; set; }
        [ProtoMember(5)]
        public ControlInputData _ControlInput { get; set; }
        [ProtoMember(6)]
        public bool _IsPotatoe { get; set; }
    }
}
