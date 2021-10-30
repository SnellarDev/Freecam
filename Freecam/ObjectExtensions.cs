using System.Runtime.CompilerServices;
using UnityEngine;

namespace FreeCamMain
{
    internal static class ObjectExtensions
    {
        public static GameObject CreateCamera(float fov, bool forwards, float DistanceMultiplier = 2f, bool AttachToPlayer = true)
        {
            GameObject gameObject;

            if (AttachToPlayer)
            {
                gameObject = new GameObject
                {
                    transform =
                    {
                        localScale = Camera.transform.localScale,
                        parent = Camera.transform
                    }
                };
            }
            else
            {
                gameObject = new GameObject
                {
                    transform =
                    {
                        localScale = Camera.transform.localScale
                    }
                };
            }
            
            Rigidbody rigidbody = gameObject.AddComponent<Rigidbody>();
            rigidbody.isKinematic = true;
            rigidbody.useGravity = false;
            Camera camera = gameObject.AddComponent<Camera>();
            camera.nearClipPlane = 0.01f;
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
            camera.fieldOfView = fov;
            camera.enabled = false;
            return gameObject;
        }

        public static Camera Camera => Camera.main;
    }
}