using ProtoBuf;

namespace NetworkHandler
{
    [ProtoContract]
    public class DisconnectData
    {
        [ProtoMember(1)]
        public long disconnectedIP { get; set; }
    }
}
