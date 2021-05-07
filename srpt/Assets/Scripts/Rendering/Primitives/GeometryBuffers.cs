using System;
using Ktyl.Util;
using UnityEngine;
using UnityEngine.Rendering;

namespace Ktyl.Rendering
{
    [CreateAssetMenu(menuName = "ktyl/Scene Description")]
    public class GeometryBuffers : ScriptableObject, IDisposable
    {
        private Cylinder[] _cylinders;
        private ComputeBuffer _cylinderBuffer;

        private Sphere[] _spheres;
        private ComputeBuffer _sphereBuffer;
        
        [SerializeField] private LatticeSettings _lattice;

        private Vector3 GetGridOrigin()
        {
            var grid = _lattice.Value;
            var side = grid.size;
            var spacing = grid.spacing;
            var height = (side-1)*spacing;


            var o = -Vector3.one * (height + spacing) * 0.5f;
            if (side % 2 == 0)
            {
                o += Vector3.one * spacing * 0.5f;
            }

            return o;
        }

        private void InitialiseCylinders()
        {
            var grid = _lattice.Value;
            var spacing = grid.spacing;
            var radius = grid.cylinderRadius;
            
            var albedo = grid.cylinderColor.v4();
            var emission = _lattice.Value.cylinderEmission.v4();
            var specular = new Vector3(0, 0, 0);

            // how many cylinders to an edge
            var side = grid.size;
            // how many cylinders to a face
            var face = side * side;

            var height = spacing * (side-1);

            // a square in each dimension
            var count = face * 3;
            
            _cylinders = new Cylinder[count];
            
            var axis = Vector3.forward;
            for (int i = 0; i < face; i++)
            {
                // calculate our coord within the face from i
                var x = i % side;
                var y = i / side;

                var pos = new Vector3(x * spacing, y * spacing, 0);

                var c = new Cylinder();
                c.position = pos;
                c.axis = axis;
                c.radius = radius;
                c.height = height;

                c.albedo = albedo;
                c.emission = emission;
                c.specular = specular;
                _cylinders[i] = c;
            }
            
            axis = Vector3.right;
            for (int i = 0; i < face; i++)
            {
                // calculate our coord within the face from i
                var x = i % side;
                var y = i / side;

                var pos = new Vector3(0, y * spacing, x*spacing);
                
                var c = new Cylinder();
                c.position = pos;
                c.axis = axis;
                c.radius = radius;
                c.height = height;

                c.albedo = albedo;
                c.emission = emission;
                c.specular = specular;
                _cylinders[face + i] = c;
            }
            
            axis = Vector3.up;
            for (int i = 0; i < face; i++)
            {
                // calculate our coord within the face from i
                var x = i % side;
                var y = i / side;

                var pos = new Vector3(x*spacing, 0, y*spacing);
                
                var c = new Cylinder();
                c.position = pos;
                c.axis = axis;
                c.radius = radius;
                c.height = height;

                c.albedo = albedo;
                c.emission = emission;
                c.specular = specular;
                _cylinders[2 * face + i] = c;
            }

            var gridOrigin = GetGridOrigin();
            for (int i = 0; i < _cylinders.Length; i++)
            {
                var c = _cylinders[i];
                c.position += gridOrigin;
                _cylinders[i] = c;
            }

            _cylinderBuffer?.Dispose();
            _cylinderBuffer = new ComputeBuffer(_cylinders.Length, Strides.SIZE_CYLINDER);
        }
        
        private void InitialiseSpheres()
        {
            var lattice = _lattice.Value;
            var size = lattice.size;
            var spacing = lattice.spacing;
            
            var positions = new Vector3[size*size*size];
            var o = GetGridOrigin();
            
            // initialise spheres
            int idx = 0;
            for (int x = 0; x < size; x++)
            {
                for (int y = 0; y < size; y++)
                {
                    for (int z = 0; z < size; z++)
                    {
                        var pos = o + new Vector3(x, y, z) * spacing;

                        positions[idx] = pos;
                        
                        idx++;
                    }
                }
            }
            
            _spheres = new Sphere[positions.Length];
            for (int i = 0; i < positions.Length; i++)
            {
                var s = new Sphere();

                s.position = positions[i];
                s.radius = lattice.sphereRadius;

                s.albedo = lattice.sphereColor.v4();
                s.emission = lattice.sphereEmission.v4();
                s.specular = Vector3.zero;

                _spheres[i] = s;
            }

            _sphereBuffer?.Dispose();
            _sphereBuffer = new ComputeBuffer(_spheres.Length, Strides.SIZE_SPHERE);
        }

        public void SetData(CommandBuffer commands, ComputeShader shader)
        {
            if (_cylinderBuffer == null)
            {
                InitialiseCylinders();
            }
            
            _cylinderBuffer.SetData(_cylinders);
            commands.SetComputeBufferParam(shader, 0, "_Cylinders", _cylinderBuffer);

            if (_sphereBuffer == null)
            {
                InitialiseSpheres();
            }
            
            _sphereBuffer.SetData(_spheres);
            commands.SetComputeBufferParam(shader, 0, "_Spheres", _sphereBuffer);
        }
        
        public void Dispose()
        {
            _cylinderBuffer?.Dispose();
            _sphereBuffer?.Dispose();
        }

        private void OnDisable()
        {
            Dispose();
        }
    }
}
