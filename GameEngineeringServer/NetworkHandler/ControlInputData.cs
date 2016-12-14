using Fusee.Math.Core;
using ProtoBuf;

namespace NetworkHandler
{
    [ProtoContract]
    public class ControlInputData
    {
        [ProtoMember(1)]
        public float _WSValue { get; set; }
        [ProtoMember(2)]
        public float _ADValue { get; set; }
    }
}
