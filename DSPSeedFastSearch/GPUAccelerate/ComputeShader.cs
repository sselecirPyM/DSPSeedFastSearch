using System;
using System.Collections.Generic;
using System.Text;
using Vortice.Direct3D12;

namespace DSPSeedFastSearch.GPUAccelerate
{
    public class ComputeShader : IDisposable
    {
        public byte[] byteCode;
        public Dictionary<RootSignature, ID3D12PipelineState> pipelineStates = new Dictionary<RootSignature, ID3D12PipelineState>();
        public void Dispose()
        {
            foreach(var pair in pipelineStates)
            {
                pair.Value.Dispose();
            }
            pipelineStates.Clear();
        }
    }
}
