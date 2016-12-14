using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Fusee.Base.Core;
using Fusee.Engine.Common;
using Fusee.Engine.Core;
using Fusee.Math.Core;
using Fusee.Serialization;
using Fusee.Xene;
using NetworkHandler;

namespace Fusee.Tutorial.Core
{
    class ClientWuggy
    {
        public IPEndpointData RemoteEndpoint { get; set; }

        public SceneContainer _sceneContainer;
        public TransformComponent _wuggyTransform;
        public ControlInputData _controlData = new ControlInputData();

        public SynchronizationData synchronizationData = new SynchronizationData();


        public ClientWuggy(IPEndpointData remoteEndPoint)
        {
            this.RemoteEndpoint = remoteEndPoint;

            _sceneContainer = AssetStorage.Get<Fusee.Serialization.SceneContainer>("Wuggy.fus");
            _wuggyTransform = _sceneContainer.Children.FindNodes(c => c.Name == "Wuggy").First()?.GetTransform();
            _wuggyTransform.Scale = new float3(1, 1, 1);

            synchronizationData._RemoteIPAdress = this.RemoteEndpoint.Address;
        }

        public void calculatePosition(float deltaTime)
        {
            float wuggyYawSpeed = _controlData._WSValue * _controlData._ADValue * 0.03f * deltaTime * 50;
            float wuggySpeed = _controlData._WSValue * -10 * deltaTime * 50;

            float wuggyYaw = _wuggyTransform.Rotation.y;
            wuggyYaw += wuggyYawSpeed;
            wuggyYaw = NormRot(wuggyYaw);
            float3 wuggyPos = _wuggyTransform.Translation;
            wuggyPos += new float3((float)System.Math.Sin(wuggyYaw), 0, (float)System.Math.Cos(wuggyYaw)) * wuggySpeed;
            _wuggyTransform.Rotation = new float3(0, wuggyYaw, 0);
            _wuggyTransform.Translation = wuggyPos;

            synchronizationData._Rotation = _wuggyTransform.Rotation;
            synchronizationData._Translation = _wuggyTransform.Translation;
        }


        private static float NormRot(float rot)
        {
            while (rot > M.Pi)
                rot -= M.TwoPi;
            while (rot < -M.Pi)
                rot += M.TwoPi;
            return rot;
        }
    }
}
