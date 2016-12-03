using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Fusee.Tutorial.Core
{
    [DataContract]
    class ControlsForNetwork
    {
        [DataMember]
        public float _WSAxis { get; set; }
        [DataMember]
        public float _ADAxis { get; set; }
    }

}
