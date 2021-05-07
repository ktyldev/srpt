#if UNITY_EDITOR

using System;
using Ktyl.Util;
using Unity.Mathematics;
using UnityEditor;
using UnityEngine;

public class AberrationVisualizer : MonoBehaviour
{
    [Serializable]
    private enum Mode
    {
        Grid,
        Radial3D,
        Radial2D
    }

    private const float RAY_LENGTH = 1.0f;
    private const int RAYS_RADIAL = 8;
    private const int RAY_COUNT = 12;

    // image plane
    private const int IMG_PLANE_WIDTH = 31;
    private const int IMG_PLANE_HEIGHT = 17;
    private const float IMG_PLANE_CELL = 0.2f;
    private const float IMG_PLANE_DISTANCE = 10.0f;

    private readonly Color _progradeColor = Color.blue;
    private readonly Color _retrogradeColor = Color.red;

    // [SerializeField] private SerialDouble _B;

    [Range(0, 1)] [SerializeField] private float _v;
    
    [SerializeField] private bool _observerFrame;
    [SerializeField] private Mode _mode;

    private void Update()
    {
        // _B = Mathf.Sin(Time.time*0.5f) * 0.5f + 0.5f;
        // _B.Value = 1.0 - math.pow(math.E, -Time.time * 0.5);
    }

    private void OnDrawGizmos()
    {
        // draw rays in a circle around a point

        switch (_mode)
        {
            case Mode.Grid:
                DrawImagePlane();
                DrawRaysGrid();
                break;

            case Mode.Radial2D:
                DrawRays2D();
                DrawArrow();
                break;

            case Mode.Radial3D:
                DrawRays3D();
                DrawArrow();
                break;
        }

        DrawLabel();
    }

    private void DrawLabel()
    {
        Handles.color = Color.white;
        var frame = _observerFrame ? "observer" : "observed object";
        var labelPos = new Vector3(0, -2f, 0);
        Handles.Label(labelPos, $"{frame} v/c = {_v}");
    }

    private static void DrawArrow()
    {
        var y = -1.1f;
        
        // arrow indicating direction of observer movement
        Gizmos.color = Color.white;
        var tip = new Vector3(1.1f, y);
        Gizmos.DrawLine(new Vector3(-1.1f, y), tip);
        Gizmos.DrawLine(tip, tip - new Vector3(0.1f, 0.1f));
        Gizmos.DrawLine(tip, tip - new Vector3(0.1f, -0.1f));
    }

    // draw a grid centred on the x-axis in the yz plane
    private void DrawImagePlane()
    {
        Gizmos.color = Color.white;

        var width = IMG_PLANE_WIDTH * IMG_PLANE_CELL;
        var height = IMG_PLANE_HEIGHT * IMG_PLANE_CELL;

        var minW = -width / 2;
        var maxW = minW + width;
        var minH = -height / 2;
        var maxH = minH + height;

        // draw horizontal lines
        for (int i = 0; i <= IMG_PLANE_HEIGHT; i++)
        {
            var h = minH + i * IMG_PLANE_CELL;
            var a = new Vector3(IMG_PLANE_DISTANCE, h, minW);
            var b = new Vector3(IMG_PLANE_DISTANCE, h, maxW);
            Gizmos.DrawLine(a, b);
        }

        // draw vertical lines
        for (int i = 0; i <= IMG_PLANE_WIDTH; i++)
        {
            var w = minW + i * IMG_PLANE_CELL;
            var a = new Vector3(IMG_PLANE_DISTANCE, minH, w);
            var b = new Vector3(IMG_PLANE_DISTANCE, maxH, w);
            Gizmos.DrawLine(a, b);
        }
    }

    private void DrawRaysGrid()
    {
        var width = (IMG_PLANE_WIDTH - 1) * IMG_PLANE_CELL;
        var height = (IMG_PLANE_HEIGHT - 1) * IMG_PLANE_CELL;

        var minW = -width / 2;
        var minH = -height / 2;

        var fwd = new Vector3(1, 0, 0);
        
        for (int row = 0; row < IMG_PLANE_HEIGHT; row++)
        {
            // determine height for row
            var y = minH + row * IMG_PLANE_CELL;
            
            for (int col = 0; col < IMG_PLANE_WIDTH; col++)
            {
                var x = minW + col * IMG_PLANE_CELL;

                // aim a ray at the centre of each grid square
                var ray = new Vector3(IMG_PLANE_DISTANCE, y, x);
                
                // axis of rotation
                var axis = Vector3.Cross(ray.normalized, fwd);
                axis = Vector3.Normalize(axis);
                Gizmos.color = Color.green;
                Gizmos.DrawLine(Vector3.zero, axis*0.1f);

                var phi = math.acos(math.dot(ray.normalized, fwd));

                var phi_a_rad = AberrateAngle(phi);
                var phi_a_deg = phi_a_rad * Mathf.Rad2Deg;
                
                // rotate forward direction by axis to get original ray
                var ray_a = Quaternion.AngleAxis(-(float)phi_a_deg, axis) * fwd.normalized;
                ray_a *= IMG_PLANE_DISTANCE;
                
                Gizmos.color = Color.Lerp(
                    _progradeColor, 
                    _retrogradeColor, 
                    (float) row / (IMG_PLANE_HEIGHT - 1));
                // Gizmos.DrawLine(Vector3.zero, ray);
                
                // draw mark indicating pre-aberration ray
                Gizmos.color = Color.Lerp(Gizmos.color, Color.white, 0.1f);
                Gizmos.DrawLine(Vector3.zero, ray_a);
            }
        }
    }

    private void DrawRays3D()
    {
        // angle from front to back
        var angleIncrement = Mathf.PI / RAY_COUNT;

        // rays radially around prograde axis
        var radialIncrement = 2 * Mathf.PI / RAYS_RADIAL;

        // front to back
        for (int i = 0; i <= RAY_COUNT; i++)
        {
            double phi = i * angleIncrement;

            var t = Mathf.InverseLerp(0, Mathf.PI, (float)phi);
            Gizmos.color = GetColor(t);

            for (int j = 0; j < RAYS_RADIAL; j++)
            {
                var v = GetVector3D(phi, j * radialIncrement);
                var pos = (v * RAY_LENGTH).v3();
                Gizmos.DrawLine(pos * 0.95f, pos);
            }

            // aberrate angle
            phi = AberrateAngle(phi);

            // radially
            for (int j = 0; j < RAYS_RADIAL; j++)
            {
                // 2D 
                var v = GetVector3D(phi, j * radialIncrement);

                var pos = v * RAY_LENGTH;
                Gizmos.DrawLine(Vector3.zero, pos.v3());
            }
        }
    }

    private void DrawRays2D()
    {
        var angleIncrement = Mathf.PI / RAY_COUNT;

        for (int i = 0; i <= RAY_COUNT; i++)
        {
            // non-aberrated angle
            double phi = i * angleIncrement;

            var t = Mathf.InverseLerp(0, Mathf.PI, (float)phi);
            Gizmos.color = GetColor(t);

            // draw a lil mark to indicate the original origin of the ray
            var v = GetVector2D(phi);

            var pos2d = v * RAY_LENGTH;
            var pos = new Vector3((float) pos2d.x, (float) pos2d.y, 0);
            Gizmos.DrawLine(pos * 0.95f, pos);

            // perform aberration
            phi = AberrateAngle(phi);
            v = GetVector2D(phi);

            // draw apparent ray of light from observer frame
            pos2d = v * RAY_LENGTH;
            pos = new Vector3((float) pos2d.x, (float) pos2d.y, 0);
            Gizmos.DrawLine(Vector3.zero, pos);
        }
    }

    private Color GetColor(float t)
    {
        if (_observerFrame)
        {
            t = 1 - t;
        }

        return Color.Lerp(_progradeColor, _retrogradeColor, t);
    }

    private double3 GetVector3D(double phi, double r)
    {
        var v2 = GetVector2D(phi);

        var z = v2.y * math.cos(r);
        var y = v2.y * math.sin(r);

        return new double3(v2.x, y, z);
    }

    private double2 GetVector2D(double phi)
    {
        var x = math.cos(phi);
        var y = math.sin(phi);

        if (_observerFrame)
        {
            x *= -1;
        }

        return new double2(x, y);
    }
    
    private double AberrateAngle(double phi)
    {
        // cos(a') = (cos(a)-v/c)/(1-cos(a)*v/c)

        // aberrated phi
        var phi_a = math.acos((math.cos(phi) - _v) / (1.0f - math.cos(phi) * _v));

        return phi_a;
    }
}

#endif