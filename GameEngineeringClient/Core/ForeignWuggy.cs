﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Fusee.Base.Core;
using Fusee.Engine.Common;
using Fusee.Math.Core;
using Fusee.Serialization;
using Fusee.Xene;
using NetworkHandler;

namespace Fusee.Tutorial.Core
{
    class ForeignWuggy
    {
        public long _remoteIpAdress { get; set; }
        public ControlInputData inputData;

        public SceneContainer _sceneContainer;
        public TransformComponent _wuggyTransform;

        public ForeignWuggy(long remoteIPAdress)
        {
            _sceneContainer = AssetStorage.Get<Fusee.Serialization.SceneContainer>("Wuggy.fus");
            _wuggyTransform = _sceneContainer.Children.FindNodes(c => c.Name == "Wuggy").First()?.GetTransform();
            _wuggyTransform.Scale = new float3(1, 1, 1);
            inputData = new ControlInputData();

            this._remoteIpAdress = remoteIPAdress;
        }

        public void updateTransform(SynchronizationData synchData)
        {
            this._wuggyTransform.Translation = synchData._TransformationData._Translation;
            this._wuggyTransform.Rotation = synchData._TransformationData._Rotation;
        }
    }
}
