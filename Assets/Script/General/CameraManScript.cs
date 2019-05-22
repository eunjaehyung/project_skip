using UnityEngine;

namespace Script
{
    public class CameraManScript : MonoBehaviour
    {
        public Transform Target;

        private void Update()
        {
            Vector3 dis = Target.transform.localPosition - transform.localPosition;

            Vector3 vec = transform.localPosition;

            vec.x = dis.x / 16f;

            transform.localPosition = vec;
        }
    }
}