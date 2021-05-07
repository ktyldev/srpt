#if UNITY_EDITOR

using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEditor;
using UnityEngine;

public class LorentzVisualizer : MonoBehaviour
{
    // y = 1/sqrt(1-B^2)     nb: y is gamma
    // x' = y*(x-Bct)        nb: ct is the y axis - time measured in metres :)
    // ct' = y*(ct-Bx)
    
    private const float AXIS_LENGTH = 100f;
    private const float LABEL_DISTANCE = 10f;
    
    // ratio of observed velocity of S' reference frame to speed of light c
    [Range(0, 1)] [SerializeField] private float _B;
    [SerializeField] private Vector2 _event;
    
    private float LorentzFactor => 1.0f / math.sqrt(1.0f - _B * _B);
    private float y => LorentzFactor;
    private float a => math.atan(_B);
    
    private void OnDrawGizmos()
    {
        DrawOrigin();
        
        DrawSAxes();
        DrawSPrimeAxes();

        DrawEvent();
        DrawEventInfo();
    }

    private void DrawEventInfo()
    {
        var sp = GetSPrimePosition(_event);
        
        var labelPos = new Vector3(-3,4.5f,0);
        var labelText = $"S:  ({_event.x}, {_event.y})\n" +
                        $"S': ({sp.x}, {sp.y})\n" +
                        $"v:  {_B}c";
        
        Handles.Label(labelPos, labelText);
    }
    
    private void DrawEvent()
    {
        Gizmos.color = Color.green;
        var radius = 0.1f;
        // draw event
        var x = _event.x;
        var ct = _event.y;
        var pos = new Vector3(x, ct, 0);
        Gizmos.DrawSphere(pos, radius);

        // draw lines to S axes
        Gizmos.DrawLine(pos, new Vector3(x, 0, 0));
        Gizmos.DrawLine(pos, new Vector3(0, ct, 0));

        Gizmos.color = Color.magenta;
        // apply lorentz transformation to get positions on x' and ct' axes
        var pos_p = GetSPrimePosition(pos);

        // unit x' axis
        var x_p_axis = new Vector3(math.cos(a), math.sin(a), 0);
        // position on graph of x' axis intersect
        var x_p_pos = x_p_axis * pos_p.x;
        Gizmos.DrawLine(x_p_pos, pos);

        // unit ct' axis
        var ct_p_axis = new Vector3(math.sin(a), math.cos(a), 0);
        // position on graph of ct' axis intersect
        var ct_p_pos = ct_p_axis * pos_p.y;
        Gizmos.DrawLine(ct_p_pos, pos);
    }

    // origin never moves
    private void DrawOrigin()
    {
        Gizmos.color = Color.white;
        var radius = 0.1f;
        Gizmos.DrawSphere(Vector3.zero, radius);
    }
    
    private void DrawSAxes()
    {
        // super duper simple just do some regular axes
        Gizmos.color = Color.white;
        
        // x
        Gizmos.DrawLine(new Vector3(-AXIS_LENGTH,0,0), new Vector3(AXIS_LENGTH,0,0));
        Handles.Label(new Vector3(LABEL_DISTANCE,0,0), "x");
        
        // ct
        Gizmos.DrawLine(new Vector3(0,-AXIS_LENGTH,0), new Vector3(0,AXIS_LENGTH,0));
        Handles.Label(new Vector3(0,LABEL_DISTANCE,0), "ct");
    }
    
    private void DrawSPrimeAxes()
    {
        // squish these axes :bolb:
        Gizmos.color = Color.cyan;
        
        // angle between axes in S and S'
        var a = math.atan(_B);

        // x' axis
        var x_p = new Vector3(math.cos(a), math.sin(a), 0);
        var x_p_start = x_p * AXIS_LENGTH;
        var x_p_end = -x_p_start;
        Gizmos.DrawLine(x_p_start,x_p_end);
        Handles.Label(x_p * LABEL_DISTANCE, "x'");

        // ct' axis
        var ct_p = new Vector3(math.sin(a), math.cos(a),0);
        var ct_p_start = ct_p * AXIS_LENGTH;
        var ct_p_end = -ct_p_start;
        Gizmos.DrawLine(ct_p_start,ct_p_end);
        Handles.Label(ct_p * LABEL_DISTANCE, "ct'");
    }

    // get the coordinates in S' of a point in S
    private Vector2 GetSPrimePosition(Vector2 sPosition)
    {
        var x = sPosition.x;
        var ct = sPosition.y;
        
        var x_p = y * (x - _B * ct);
        var ct_p = y * (ct - _B * x);
        
        return new Vector2(x_p, ct_p);
    }
}

#endif