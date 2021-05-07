using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ktyl.T2.Rendering
{
    [ExecuteAlways]
    public class FillScreen : MonoBehaviour
    {
        [SerializeField] private Camera _camera;

        private void LateUpdate()
        {
            var pos = (_camera.nearClipPlane + 0.01f);
            var camTrans = _camera.transform;
            var trans = transform;
            var h = Mathf.Tan(_camera.fieldOfView * Mathf.Deg2Rad * 0.5f) * pos * 2f;
            
            trans.position = camTrans.position + camTrans.forward * pos;
            transform.localScale = new Vector3(h*_camera.aspect,h,1f);
            transform.LookAt(trans.position + camTrans.forward);
        }
    }
}