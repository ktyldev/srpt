using Ktyl.Util;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Rendering;

namespace Ktyl.Rendering
{
    [CreateAssetMenu(menuName = "ktyl/Rendering/Special Relativity Settings")]
    public class SpecialRelativitySettings : ScriptableObject
    {
        public float B => (float)_b;
        [SerializeField] private SerialDouble _b;

        public float Lorentz => 1.0f / math.sqrt(1.0f - B * B);

        public float DopplerWeight => _dopplerToggle ? 1 : 0;
        [SerializeField] private SerialBool _dopplerToggle;

        public float SearchlightWeight => _searchlightToggle ? 1 : 0;
        [SerializeField] private SerialBool _searchlightToggle;

        public float WavelengthScaling => _wavelengthScalingToggle ? 1 : 0;
        [SerializeField] private SerialBool _wavelengthScalingToggle;

        public void SetShaderRelativityParameters(CommandBuffer commands, ComputeShader shader)
        {
            commands.SetComputeFloatParam(shader, "_Velocity", B);
            commands.SetComputeFloatParam(shader, "_Lorentz", Lorentz);
            commands.SetComputeFloatParam(shader, "_Doppler", DopplerWeight);
            commands.SetComputeFloatParam(shader, "_Searchlight", SearchlightWeight);
            commands.SetComputeFloatParam(shader, "_WavelengthScaling", WavelengthScaling);
        }
    }
}