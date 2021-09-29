using System;
using OpenTK.Mathematics;

namespace GraphTool
{
    public class Camera
    {

        public Vector2 Size { get; private set; }

        // Those vectors are directions pointing outwards from the camera to define how it rotated
        private Vector3 _front = -Vector3.UnitZ;

        private Vector3 _up = Vector3.UnitY;

        // Rotation around the X axis (radians)
        private float _pitch;

        // Rotation around the Y axis (radians)
        private float _yaw = -90.0f; // Without this you would be started rotated 90 degrees right

        // The field of view of the camera (radians)
        private float _fov = 45.0f;

        public Camera(Vector3 position, Vector2i size)
        {
            Position = position;
            UpdateSize(size);
        }

        // The position of the camera
        public Vector3 Position { get; set; }

        // This is simply the aspect ratio of the viewport, used for the projection matrix
        public float AspectRatio;

        public Vector3 Front => _front;

        public Vector3 Up => _up;

        // We convert from degrees to radians as soon as the property is set to improve performance
        public float Pitch
        {
            get => _pitch;
            set
            {
                var angle = MathHelper.Clamp(value, -89f, 89f);
                _pitch = angle;
                UpdateVectors();
            }
        }

        // We convert from degrees to radians as soon as the property is set to improve performance
        public float Yaw
        {
            get => _yaw;
            set
            {
                _yaw = value;
                UpdateVectors();
            }
        }

        // The field of view (FOV) is the vertical angle of the camera view, this has been discussed more in depth in a
        // previous tutorial, but in this tutorial you have also learned how we can use this to simulate a zoom feature.
        // We convert from degrees to radians as soon as the property is set to improve performance
        public float Fov
        {
            get => _fov;
            set
            {
                var angle = MathHelper.Clamp(value, 1f, 45f);
                _fov = angle;
            }
        }

        // Get the view matrix using the amazing LookAt function described more in depth on the web tutorials
        public Matrix4 GetViewMatrix()
        {
            return Matrix4.LookAt(Position, Position + _front, _up);
        }

        // Get the projection matrix using the same method we have used up until this point
        public Matrix4 GetProjectionMatrix()
        {

            return Matrix4.CreatePerspectiveFieldOfView(MathHelper.DegreesToRadians(_fov), AspectRatio, 0.1f, 100f);
        }

        // This function is going to update the direction vertices using some of the math learned in the web tutorials
        private void UpdateVectors()
        {
            // First the front matrix is calculated using some basic trigonometry
            _front.X = MathF.Cos(MathHelper.DegreesToRadians(_pitch)) * MathF.Cos(MathHelper.DegreesToRadians(_yaw));
            _front.Y = MathF.Sin(MathHelper.DegreesToRadians(_pitch));
            _front.Z = MathF.Cos(MathHelper.DegreesToRadians(_pitch)) * MathF.Sin(MathHelper.DegreesToRadians(_yaw));

            // We need to make sure the vectors are all normalized, as otherwise we would get some funky results
            _front = Vector3.Normalize(_front);
        }

        public void UpdateSize(Vector2 size)
        {
            Size = size;
            AspectRatio = (float)size.X / size.Y;
        }
    }
}
