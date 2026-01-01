using System;
using System.Threading.Tasks;
using JetBrains.Annotations;
using UnityEditor;

namespace DELTation.UnityPix.Editor
{
    internal sealed class SceneViewCaptureTarget : ICaptureTarget
    {
        private readonly SceneView _sceneView;

        public SceneViewCaptureTarget([NotNull] SceneView sceneView) => _sceneView = sceneView ?? throw new ArgumentNullException(nameof(sceneView));

        public async Task RenderAsync()
        {
            _sceneView.Show();
            _sceneView.Focus();
            _sceneView.Repaint();
            
            // Render the camera - for HDRP, this ensures all rendering passes are executed
            _sceneView.camera.Render();
            
            // Additional yield to ensure HDRP's async rendering has time to start
            await Task.Yield();
            await Task.Yield();
        }

        public int NumFrames =>
            // First present is typically a simple blit, we should wait for the next one to finish.
            // HDRP uses more complex rendering with multiple passes, so we capture 3 frames to ensure
            // all rendering work (including async compute) is captured.
            3;
    }
}