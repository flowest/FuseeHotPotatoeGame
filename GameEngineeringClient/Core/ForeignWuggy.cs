using System;
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

        public SceneContainer _sceneContainer;
        public TransformComponent _wuggyTransform;
        public SynchronizationData _connectedPlayerSyncData;
        private MaterialComponent _wuggyMaterialComponent;


        public ForeignWuggy(SynchronizationData syncData)
        {
            this._connectedPlayerSyncData = syncData;
            _sceneContainer = AssetStorage.Get<Fusee.Serialization.SceneContainer>("Wuggy.fus");
            _wuggyTransform = _sceneContainer.Children.FindNodes(c => c.Name == "Wuggy").First()?.GetTransform();
            _wuggyMaterialComponent = _sceneContainer.Children.FindNodes(c => c.Name == "Wuggy").First().Children[1].GetMaterial();
            _wuggyTransform.Scale = new float3(1, 1, 1);
        }

        public void updateTransform()
        {
            this._wuggyTransform.Translation = _connectedPlayerSyncData._Translation;
            this._wuggyTransform.Rotation = _connectedPlayerSyncData._Rotation;
        }

        public void changeColor(float3 color)
        {
            _wuggyMaterialComponent.Diffuse.Color = color;
        }
    }
}
