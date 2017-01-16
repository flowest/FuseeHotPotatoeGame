using System;
using System.Collections.Generic;
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
    class ForeignWuggy
    {

        public SceneContainer _sceneContainer;
        public TransformComponent _wuggyTransform;
        public SynchronizationData _connectedPlayerSyncData;
        private MaterialComponent _wuggyMaterialComponent;

        private TransformComponent _wgyWheelBigR;
        private TransformComponent _wgyWheelBigL;
        private TransformComponent _wgyWheelSmallR;
        private TransformComponent _wgyWheelSmallL;

        public ForeignWuggy(SynchronizationData syncData)
        {
            this._connectedPlayerSyncData = syncData;
            _sceneContainer = AssetStorage.Get<Fusee.Serialization.SceneContainer>("Wuggy.fus");
            _wuggyTransform = _sceneContainer.Children.FindNodes(c => c.Name == "Wuggy").First()?.GetTransform();
            _wuggyMaterialComponent = _sceneContainer.Children.FindNodes(c => c.Name == "Wuggy").First().Children[1].GetMaterial();
            _wuggyTransform.Scale = new float3(1, 1, 1);

            _wgyWheelBigR = _sceneContainer.Children.FindNodes(c => c.Name == "WheelBigR").First()?.GetTransform();
            _wgyWheelBigL = _sceneContainer.Children.FindNodes(c => c.Name == "WheelBigL").First()?.GetTransform();
            _wgyWheelSmallR = _sceneContainer.Children.FindNodes(c => c.Name == "WheelSmallR").First()?.GetTransform();
            _wgyWheelSmallL = _sceneContainer.Children.FindNodes(c => c.Name == "WheelSmallL").First()?.GetTransform();
        }

        public void updateTransform()
        {
            if (_connectedPlayerSyncData._CanMove)
            {
                animateWuggy(); 
            }
            this._wuggyTransform.Translation = _connectedPlayerSyncData._Translation;
            this._wuggyTransform.Rotation = _connectedPlayerSyncData._Rotation;
        }

        private void animateWuggy()
        {
            float wuggySpeed = _connectedPlayerSyncData._ControlInput._WSValue * -10 * Time.DeltaTime * 50;

            // Wuggy Wheels
            _wgyWheelBigR.Rotation += new float3(wuggySpeed * 0.008f, 0, 0);
            _wgyWheelBigL.Rotation += new float3(wuggySpeed * 0.008f, 0, 0);
            _wgyWheelSmallR.Rotation = new float3(_wgyWheelSmallR.Rotation.x + wuggySpeed * 0.016f, -_connectedPlayerSyncData._ControlInput._ADValue * 0.3f, 0);
            _wgyWheelSmallL.Rotation = new float3(_wgyWheelSmallR.Rotation.x + wuggySpeed * 0.016f, -_connectedPlayerSyncData._ControlInput._ADValue * 0.3f, 0);
        }

        public void changeColor(float3 color)
        {
            _wuggyMaterialComponent.Diffuse.Color = color;
        }
    }
}
