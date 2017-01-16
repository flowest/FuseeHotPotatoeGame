using Fusee.Math.Core;
using NetworkHandler;

namespace Fusee.Tutorial.Core
{
    static class CalculateClientTransform
    {
        public static SynchronizationData calculatePosition(SynchronizationData clientSyncData, float deltaTime)
        {
            float wuggyYawSpeed = clientSyncData._ControlInput._WSValue * clientSyncData._ControlInput._ADValue * 0.03f * deltaTime * 50;
            float wuggySpeed = clientSyncData._ControlInput._WSValue * -10 * deltaTime * 50;

            float wuggyYaw = clientSyncData._Rotation.y;
            wuggyYaw += wuggyYawSpeed;
            wuggyYaw = NormRot(wuggyYaw);
            float3 wuggyPos = clientSyncData._Translation;
            wuggyPos += new float3((float)System.Math.Sin(wuggyYaw), 0, (float)System.Math.Cos(wuggyYaw)) * wuggySpeed;
            clientSyncData._Rotation = new float3(0, wuggyYaw, 0);
            clientSyncData._Translation = wuggyPos;

            return  new SynchronizationData
                {
                    _ControlInput = clientSyncData._ControlInput,
                    _RemoteIPAdress = clientSyncData._RemoteIPAdress,
                    _Rotation =  clientSyncData._Rotation,
                    _Translation = clientSyncData._Translation,
                    _Scale = clientSyncData._Scale
                };
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
