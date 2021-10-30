using System.Runtime.CompilerServices;
using MelonLoader;
using UnityEngine;
using VRC.SDKBase;

namespace FreeCamMain
{
    public class Freecam : MelonMod
    {
        private static Camera FreeCam
        {
            get
            {
                if (_mainFreeCam == null && _freeCamObject != null) _mainFreeCam = _freeCamObject.GetComponent<Camera>();
                return _mainFreeCam;
            }
        }

        public override void OnUpdate()
        {
            try
            {
                if (_toggled)
                {
                    _rotateX += Input.GetAxis("Mouse X") * MouseSensX;
                    _rotateY += Input.GetAxis("Mouse Y") * MouseSensY;
                    _rotateY = Mathf.Clamp(_rotateY, MinYRotation, MaxYRotation);
                    RotateFreecamcamera(Quaternion.Euler(-_rotateY, _rotateX, 0f));
                    
                    if(!Input.anyKey) return;
                    float number = Event.current.shift ? SpeedFast : Speed;

                    if(Input.GetKey(KeyCode.W)) MoveFreecamCamera(_freeCamObject.transform.forward * number * Time.deltaTime);
                    if(Input.GetKey(KeyCode.A)) MoveFreecamCamera(_freeCamObject.transform.right * -number * Time.deltaTime);
                    if(Input.GetKey(KeyCode.S)) MoveFreecamCamera(_freeCamObject.transform.forward * -number * Time.deltaTime);
                    if(Input.GetKey(KeyCode.D)) MoveFreecamCamera(_freeCamObject.transform.right * number * Time.deltaTime);
                    if(Input.GetKey(KeyCode.E)) MoveFreecamCamera(Vector3.up * number * Time.deltaTime);
                    if(Input.GetKey(KeyCode.Q)) MoveFreecamCamera(Vector3.up * -number * Time.deltaTime);
                    TouchObjects(FreeCam);
                }

                if (!Input.GetKeyDown(KeyCode.B)) return;
                if (PlayerExtensions.IsInWorld())
                {
                    EnableFreecam(!_toggled);   
                }
                else
                {
                    MelonLogger.Msg("You are not in a world can't activate freecam");
                }
            }
            catch
            {
                // ignored
            }
        }

        public override void OnSceneWasLoaded(int buildIndex, string sceneName)
        {
            EnableFreecam(false);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void EnableFreecam(bool toggled)
        {
            _toggled = toggled;
            PlayerExtensions.FreezeLocalPlayer(toggled);
            if (toggled)
            {
                if (_freeCamObject == null) _freeCamObject = ObjectExtensions.CreateCamera(65f, true, 2f, false);
                ToggleCamera(_freeCamObject, true);
                _freeCamObject.transform.position = PlayerExtensions.LocalPlayer.transform.position + new Vector3(0f, 1f, 0f);

                ToggleCamera(ObjectExtensions.Camera, false);
                return;
            }
            ToggleCamera(ObjectExtensions.Camera, true);
            ToggleCamera(_freeCamObject, false);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void RotateFreecamcamera(Quaternion q)
        {
            _freeCamObject.transform.rotation = q;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void MoveFreecamCamera(Vector3 v3)
        {
            _freeCamObject.transform.position += v3;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void TouchObjects(Camera camera)
        {
            if (Input.GetMouseButtonDown(0))
            {
                var ray = new Ray(Camera.main.transform.position, Camera.main.transform.forward);

                if (Physics.Raycast(ray, out RaycastHit raycastHit) && raycastHit.transform != null && Vector3.Distance(raycastHit.transform.position, camera.gameObject.transform.position) < 5f)
                {
                    VRC_Trigger trigger = raycastHit.transform.gameObject.GetComponent<VRC_Trigger>();
                    if (trigger != null)
                    {
                        trigger.Interact();
                    }
                }
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void ToggleCamera(GameObject camera, bool enabled) => camera.GetComponent<Camera>().enabled = enabled;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void ToggleCamera(Camera camera, bool enabled) => camera.enabled = enabled;

        private static float _rotateX;

        private static float _rotateY;

        private static bool _toggled;

        private static readonly float MouseSensX = 2f;

        private static readonly float MouseSensY = 2f;

        private static readonly float MinYRotation = -90f;

        private static readonly float MaxYRotation = 90f;

        private static GameObject _freeCamObject;

        private static Camera _mainFreeCam;

        private const float Speed = 0.75f;
        private const float SpeedFast = 0.75f * 4f;
    }
}