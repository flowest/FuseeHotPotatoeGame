using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Json;
using Fusee.Base.Common;
using Fusee.Base.Core;
using Fusee.Engine.Common;
using Fusee.Engine.Core;
using Fusee.Math.Core;
using Fusee.Serialization;
using Fusee.Xene;
using static System.Math;
using static Fusee.Engine.Core.Input;
using static Fusee.Engine.Core.Time;
using NetworkHandler;


namespace Fusee.Tutorial.Core
{

    class Renderer : SceneVisitor
    {
        public ShaderEffect ShaderEffect;

        public RenderContext RC;
        private ITexture _leafTexture;
        public float4x4 View;
        private Dictionary<MeshComponent, Mesh> _meshes = new Dictionary<MeshComponent, Mesh>();
        private CollapsingStateStack<float4x4> _model = new CollapsingStateStack<float4x4>();
        private Mesh LookupMesh(MeshComponent mc)
        {
            Mesh mesh;
            if (!_meshes.TryGetValue(mc, out mesh))
            {
                mesh = new Mesh
                {
                    Vertices = mc.Vertices,
                    Normals = mc.Normals,
                    UVs = mc.UVs,
                    Triangles = mc.Triangles,
                };
                _meshes[mc] = mesh;
            }
            return mesh;
        }

        public Renderer(RenderContext rc)
        {
            RC = rc;
            // Read the Leaves.jpg image and upload it to the GPU
            ImageData leaves = AssetStorage.Get<ImageData>("Leaves.jpg");
            _leafTexture = RC.CreateTexture(leaves);

            // Initialize the shader(s)
            ShaderEffect = new ShaderEffect(

                new[]
                {
                    new EffectPassDeclaration
                    {
                        VS = AssetStorage.Get<string>("VertexShader.vert"),
                        PS = AssetStorage.Get<string>("PixelShader.frag"),
                        StateSet = new RenderStateSet
                        {
                            ZEnable = true,
                            CullMode = Cull.Counterclockwise,
                        }
                    }
                },
                new[]
                {
                    new EffectParameterDeclaration {Name = "albedo", Value = float3.One},
                    new EffectParameterDeclaration {Name = "shininess", Value = 1.0f},
                    new EffectParameterDeclaration {Name = "specfactor", Value= 1.0f},
                    new EffectParameterDeclaration {Name = "speccolor", Value = float3.Zero},
                    new EffectParameterDeclaration {Name = "ambientcolor", Value = float3.Zero},
                    new EffectParameterDeclaration {Name = "texture", Value = _leafTexture},
                    new EffectParameterDeclaration {Name = "texmix", Value = 0.0f},
                });
            ShaderEffect.AttachToContext(RC);
        }

        protected override void InitState()
        {
            _model.Clear();
            _model.Tos = float4x4.Identity;
        }
        protected override void PushState()
        {
            _model.Push();
        }
        protected override void PopState()
        {
            _model.Pop();
            RC.ModelView = View * _model.Tos;
        }
        [VisitMethod]
        void OnMesh(MeshComponent mesh)
        {
            ShaderEffect.RenderMesh(LookupMesh(mesh));
            // RC.Render(LookupMesh(mesh));
        }
        [VisitMethod]
        void OnMaterial(Fusee.Serialization.MaterialComponent material)
        {
            if (material.HasDiffuse)
            {
                ShaderEffect.SetEffectParam("albedo", material.Diffuse.Color);
                if (material.Diffuse.Texture == "Leaves.jpg")
                {
                    ShaderEffect.SetEffectParam("texture", _leafTexture);
                    ShaderEffect.SetEffectParam("texmix", 1.0f);
                }
                else
                {
                    ShaderEffect.SetEffectParam("texmix", 0.0f);
                }
            }
            else
            {
                ShaderEffect.SetEffectParam("albedo", float3.Zero);
            }
            if (material.HasSpecular)
            {
                ShaderEffect.SetEffectParam("shininess", material.Specular.Shininess);
                ShaderEffect.SetEffectParam("specfactor", material.Specular.Intensity);
                ShaderEffect.SetEffectParam("speccolor", material.Specular.Color);
            }
            else
            {
                ShaderEffect.SetEffectParam("shininess", 0);
                ShaderEffect.SetEffectParam("specfactor", 0);
                ShaderEffect.SetEffectParam("speccolor", float3.Zero);
            }
            if (material.HasEmissive)
            {
                ShaderEffect.SetEffectParam("ambientcolor", material.Emissive.Color);
            }
            else
            {
                ShaderEffect.SetEffectParam("ambientcolor", float3.Zero);
            }
        }
        [VisitMethod]
        void OnTransform(TransformComponent xform)
        {
            _model.Tos *= xform.Matrix();
            RC.ModelView = View * _model.Tos;
        }
    }


    [FuseeApplication(Name = "Tutorial Example", Description = "The official FUSEE Tutorial.")]
    public class Tutorial : RenderCanvas
    {
        // angle variables
        private static float _angleHorz = M.PiOver6 * 2.0f, _angleVert = -M.PiOver6 * 0.5f,
                             _angleVelHorz, _angleVelVert, _angleRoll, _angleRollInit, _zoomVel, _zoom;
        private static float2 _offset;
        private static float2 _offsetInit;

        private const float RotationSpeed = 7;
        private const float Damping = 0.8f;

        //private Fusee.Serialization.SceneContainer _scene;
        private float4x4 _sceneCenter;
        private float4x4 _sceneScale;
        private float4x4 _projection;
        private bool _twoTouchRepeated;

        private bool _keys;

        //private TransformComponent _wuggyTransform;
        //private TransformComponent _wgyWheelBigR;
        //private TransformComponent _wgyWheelBigL;
        //private TransformComponent _wgyWheelSmallR;
        //private TransformComponent _wgyWheelSmallL;
        //private TransformComponent _wgyNeckHi;
        //private List<Fusee.Serialization.SceneNodeContainer> _trees;

        private Renderer _renderer;

        MemoryStream memoryStream = new MemoryStream();
        NetworkHandlerSerializer serializer = new NetworkHandlerSerializer();

        private byte[] synchonizeDataByteArray;

        private List<SynchronizationData> connectedClients = new List<SynchronizationData>();




        // Init is called on startup. 
        public override void Init()
        {

            _sceneScale = float4x4.CreateScale(0.04f);


            // Instantiate our self-written renderer
            _renderer = new Renderer(RC);



            // Set the clear color for the backbuffer
            RC.ClearColor = new float4(1, 1, 1, 1);

            Network netCon = Network.Instance;
            netCon.Config.SysType = SysType.Server;
            netCon.StartPeer();


        }

        private void sendSynchronizeDataToClients(SynchronizationData connectedClient)
        {
            memoryStream = new MemoryStream();

            serializer.Serialize(memoryStream, connectedClient);
            synchonizeDataByteArray = memoryStream.ToArray();

            Network.Instance.SendMessage(synchonizeDataByteArray, MessageDelivery.ReliableOrdered, 1);
            memoryStream.Dispose();
        }

        private void getInputDataFromClients()
        {
            INetworkMsg msg;

            while ((msg = Network.Instance.IncomingMsg) != null)
            {
                if (msg.Type == MessageType.Data)
                {
                    memoryStream = new MemoryStream(msg.Message.ReadBytes);
                    ControlInputData controls = (ControlInputData)serializer.Deserialize(memoryStream, null, typeof(ControlInputData));

                    connectedClients.First(client => client._RemoteIPAdress == msg.Sender.RemoteEndPoint.Address)._ControlInput = controls;
                }


            }
        }

        private void calculateClientWuggyPositions()
        {
            foreach (var connectedClient in connectedClients)
            {

                SynchronizationData updatedClient = CalculateClientTransform.calculatePosition(connectedClient, DeltaTime);
                connectedClient._Rotation = updatedClient._Rotation;
                connectedClient._Translation = updatedClient._Translation;
                sendSynchronizeDataToClients(connectedClient);
            }
        }

        private void handleConnections(ConnectionStatus estatus, INetworkConnection connection)
        {
            if (estatus == ConnectionStatus.Connected && connectedClients.All(connectedClient => connectedClient._RemoteIPAdress != connection.RemoteEndPoint.Address))
            {
                connectedClients.Add(new SynchronizationData
                {
                    _ControlInput = new ControlInputData(),
                    _RemoteIPAdress = connection.RemoteEndPoint.Address,
                    _Rotation = new float3(),
                    _Translation = new float3(),
                    _Scale = new float3(1,1,1)
                });
            }

            else if (estatus == ConnectionStatus.Disconnected)
            {
                connectedClients.RemoveAll(client => client._RemoteIPAdress == connection.RemoteEndPoint.Address);
            }
        }

        // RenderAFrame is called once a frame
        public override void RenderAFrame()
        {
            Network.Instance.OnConnectionUpdate += handleConnections;
            getInputDataFromClients();
            calculateClientWuggyPositions();

            // Clear the backbuffer
            RC.Clear(ClearFlags.Color | ClearFlags.Depth);



            // UpDown / LeftRight rotation
            if (Mouse.LeftButton)
            {
                _keys = false;
                _angleVelHorz = -RotationSpeed * Mouse.XVel * 0.000002f;
                _angleVelVert = -RotationSpeed * Mouse.YVel * 0.000002f;
            }
            else if (Touch.GetTouchActive(TouchPoints.Touchpoint_0) && !Touch.TwoPoint)
            {
                _keys = false;
                float2 touchVel;
                touchVel = Touch.GetVelocity(TouchPoints.Touchpoint_0);
                _angleVelHorz = -RotationSpeed * touchVel.x * 0.000002f;
                _angleVelVert = -RotationSpeed * touchVel.y * 0.000002f;
            }
            else
            {
                if (_keys)
                {
                    _angleVelHorz = -RotationSpeed * Keyboard.LeftRightAxis * 0.002f;
                    _angleVelVert = -RotationSpeed * Keyboard.UpDownAxis * 0.002f;
                }
            }

            //float wuggyYawSpeed = controls._WSValue * controls._ADValue * 0.03f * DeltaTime * 50;
            //float wuggySpeed = controls._WSValue * -10 * DeltaTime * 50;


            _zoom += _zoomVel;
            // Limit zoom
            if (_zoom < 80)
                _zoom = 80;
            if (_zoom > 2000)
                _zoom = 2000;

            _angleHorz += _angleVelHorz;
            // Wrap-around to keep _angleHorz between -PI and + PI
            _angleHorz = M.MinAngle(_angleHorz);

            _angleVert += _angleVelVert;
            // Limit pitch to the range between [-PI/2, + PI/2]
            _angleVert = M.Clamp(_angleVert, -M.PiOver2, M.PiOver2);

            // Wrap-around to keep _angleRoll between -PI and + PI
            _angleRoll = M.MinAngle(_angleRoll);


            // Create the camera matrix and set it as the current ModelView transformation
            var mtxRot = float4x4.CreateRotationZ(_angleRoll) * float4x4.CreateRotationX(_angleVert) * float4x4.CreateRotationY(_angleHorz);
            var mtxCam = float4x4.LookAt(0, 20, -_zoom, 0, 0, 0, 0, 1, 0);
            _renderer.View = mtxCam * mtxRot * _sceneScale;
            var mtxOffset = float4x4.CreateTranslation(2 * _offset.x / Width, -2 * _offset.y / Height, 0);
            RC.Projection = mtxOffset * _projection;

            // Swap buffers: Show the contents of the backbuffer (containing the currently rerndered farame) on the front buffer.
            Present();


        }

        public static float NormRot(float rot)
        {
            while (rot > M.Pi)
                rot -= M.TwoPi;
            while (rot < -M.Pi)
                rot += M.TwoPi;
            return rot;
        }

        // Is called when the window was resized
        public override void Resize()
        {
            // Set the new rendering area to the entire new windows size
            RC.Viewport(0, 0, Width, Height);

            // Create a new projection matrix generating undistorted images on the new aspect ratio.
            var aspectRatio = Width / (float)Height;

            // 0.25*PI Rad -> 45° Opening angle along the vertical direction. Horizontal opening angle is calculated based on the aspect ratio
            // Front clipping happens at 1 (Objects nearer than 1 world unit get clipped)
            // Back clipping happens at 2000 (Anything further away from the camera than 2000 world units gets clipped, polygons will be cut)
            _projection = float4x4.CreatePerspectiveFieldOfView(M.PiOver4, aspectRatio, 1, 20000);
        }

    }
}