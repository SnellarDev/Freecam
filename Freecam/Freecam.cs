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
                if (mainFreeCam == null && FreeCamObject != null)
                {
                    mainFreeCam = FreeCamObject.GetComponent<Camera>();
                }
                return mainFreeCam;
            }
        }

        public override void OnUpdate()
        {
            try
            {
                if (Input.GetKeyDown(KeyCode.B) && PlayerExtensions.IsInWorld())
                {
                    EnableFreecam(!Toggled);
                }
                else if (Input.GetKeyDown(KeyCode.B) && !PlayerExtensions.IsInWorld())
                {
                    MelonLogger.Msg("You are not in a world can't activate freecam");
                    return;
                }
                if (Toggled)
                {
                    float number = Input.GetKey(KeyCode.LeftShift) ? (Speed * 4f) : Speed;
                    for (int i = 0; i < keys.Length; i++) switch (Input.GetKey(keys[i]))
                        {
                            case true when (i == 0):
                                MoveFreecamCamera(FreeCamObject.transform.forward * number * Time.deltaTime);
                                break;

                            case true when (i == 1):
                                MoveFreecamCamera(FreeCamObject.transform.right * -number * Time.deltaTime);
                                break;

                            case true when (i == 2):
                                MoveFreecamCamera(FreeCamObject.transform.forward * -number * Time.deltaTime);
                                break;

                            case true when (i == 3):
                                MoveFreecamCamera(FreeCamObject.transform.right * number * Time.deltaTime);
                                break;

                            case true when (i == 3):
                                MoveFreecamCamera(Vector3.up * number * Time.deltaTime);
                                break;

                            case true when (i == 5):
                                MoveFreecamCamera(Vector3.up * -number * Time.deltaTime);
                                break;
                        }

                    RotateX += Input.GetAxis("Mouse X") * MouseSensX;
                    RotateY += Input.GetAxis("Mouse Y") * MouseSensY;
                    RotateY = Mathf.Clamp(RotateY, MinYRotation, MaxYRotation);
                    RotateFreecamcamera(Quaternion.Euler(-RotateY, RotateX, 0f));
                    TouchObjects(FreeCam);
                }
            }
            catch { }
        }

        public static void EnableFreecam(bool toggled)
        {
            Toggled = toggled;
            if (PlayerExtensions.LocalVRCPlayer == null)
            {
                return;
            }
            PlayerExtensions.FreezeLocalPlayer(toggled);
            if (toggled)
            {
                if (FreeCamObject == null)
                {
                    FreeCamObject = ObjectExtensions.CreateCamera(65f, true, 2f, false);
                }
                ToggleCamera(FreeCamObject, true);
                FreeCamObject.transform.position = PlayerExtensions.LocalPlayer.transform.position + new Vector3(0f, 1f, 0f);

                ToggleCamera(ObjectExtensions.GetPlayerCamera, false);
                PlayerRotation = PlayerExtensions.LocalVRCPlayer.transform.rotation;
                return;
            }
            ToggleCamera(ObjectExtensions.GetPlayerCamera, true);
            ToggleCamera(FreeCamObject, false);
        }

        private void RotateFreecamcamera(Quaternion q)
        {
            PlayerExtensions.LocalVRCPlayer.transform.rotation = PlayerRotation;
            FreeCamObject.transform.rotation = q;
        }

        private void MoveFreecamCamera(Vector3 v3)
        {
            FreeCamObject.transform.position += v3;
        }

        public static void TouchObjects(Camera camera)
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

        public static void ToggleCamera(GameObject camera, bool Enabled)
        {
            camera.GetComponent<Camera>().enabled = Enabled;
        }

        private KeyCode[] keys = new KeyCode[] { KeyCode.W, KeyCode.A, KeyCode.S, KeyCode.D, KeyCode.E, KeyCode.Q };

        private static float RotateX;

        private static float RotateY;

        public static bool Toggled;

        private static readonly float MouseSensX = 2f;

        private static readonly float MouseSensY = 2f;

        private static readonly float MinYRotation = -90f;

        private static readonly float MaxYRotation = 90f;

        private static GameObject FreeCamObject;

        private static Camera mainFreeCam;

        public static bool hasrotated;

        private static readonly float Speed = 0.75f;

        private static Quaternion PlayerRotation;
    }
}