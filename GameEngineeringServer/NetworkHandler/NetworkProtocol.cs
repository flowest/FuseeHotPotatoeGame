using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
    }

}
