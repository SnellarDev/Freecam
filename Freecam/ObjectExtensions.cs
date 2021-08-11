using UnityEngine;

namespace FreeCamMain
{
    internal static class ObjectExtensions
    {
        public static GameObject GetPlayerCamera
        {
            get
            {
                if (CachedPlayerCamera == null)
                {
                    CachedPlayerCamera = GameObject.Find("Camera (eye)");
                }
                return CachedPlayerCamera;
            }
        }

        public static GameObject CreateCamera(float fov, bool forwards, float DistanceMultiplier = 2f, bool AttachToPlayer = true)
        {
            GameObject gameObject = GameObject.CreatePrimitive(PrimitiveType.Cube);
            Object.Destroy(gameObject.GetComponent<MeshRenderer>());
            gameObject.transform.localScale = Camera.transform.localScale;
            Rigidbody rigidbody = gameObject.AddComponent<Rigidbody>();
            rigidbody.isKinematic = true;
            rigidbody.useGravity = false;
            gameObject.GetComponent<Collider>().enabled = false;
            gameObject.GetComponent<Renderer>().enabled = false;
            Camera camera = gameObject.AddComponent<Camera>();
            camera.nearClipPlane = 0.01f;
            if (AttachToPlayer)
            {
                gameObject.transform.parent = Camera.transform;
            }
            gameObject.transform.SetPositionAndRotation(Camera.transform.position, Camera.transform.rotation);
            if (forwards)
            {
                gameObject.transform.Rotate(0f, 180f, 0f);
                gameObject.transform.position += -gameObject.transform.forward * DistanceMultiplier;
            }
            else
            {
                gameObject.transform.position += -gameObject.transform.forward * DistanceMultiplier;
            }
            Camera.enabled = false;
            Camera component = gameObject.GetComponent<Camera>();
            component.fieldOfView = fov;
            component.enabled = false;
            return gameObject;
        }

        private static Camera Camera
        {
            get
            {
                return CameraObject.GetComponent<Camera>();
            }
        }

        private static GameObject CameraObject
        {
            get
            {
                return GetPlayerCamera;
            }
        }

        private static GameObject CachedPlayerCamera;
    }
}