using Cardamom.Graphics;
using Cardamom.Graphics.Camera;
using Cardamom.Ui;
using Expeditionary.Controller;
using Expeditionary.Model.Mapping;
using OpenTK.Mathematics;

namespace Expeditionary.View
{
    public static class SceneFactory
    {
        public static IScene Create(Vector3 size, Map map)
        {
            var camera = new SubjectiveCamera3d(10);
            camera.SetPitch(MathF.PI);
            return new BasicScene(size, new Camera2dController(camera), camera, Array.Empty<IRenderable>());
        }
    }
}
