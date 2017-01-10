using Fusee.Engine.Common;
using Fusee.Math.Core;
using ProtoBuf;

namespace NetworkHandler
{
    [ProtoContract]
    public class SynchronizationData
    {
        [ProtoMember(1)]
        public TransformationData _TransformationData { get; set; }
        [ProtoMember(2)]
        public long _RemoteIPAdress { get; set; }
        [ProtoMember(3)]
        public ControlInputData _InputData { get; set; }
    }
}
