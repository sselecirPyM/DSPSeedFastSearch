using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using DSPSeedFastSearch.GPUAccelerate;

namespace DSPSeedFastSearch.GPUCompute
{
    public class ComputeContext
    {
        public ComputeDevice device = new ComputeDevice();
        public CommandList commandList = new CommandList();
        public RootSignature rootSignature = new RootSignature();
        public RWBuffer rwBuffer = new RWBuffer();
        public ReadBackBuffer readBackBuffer = new ReadBackBuffer();
        public RingUploadBuffer uploadBuffer = new RingUploadBuffer();

        ComputeShader currentComputeShader;

        int defaultSize = 65536 * 512;

        public void Init()
        {
            device.Init();
            commandList.Init(device);
            device.CreateRootSignature(rootSignature, new[] { RootSignatureParamP.CBV, RootSignatureParamP.UAV, });
            device.CreateRWBuffer(rwBuffer, defaultSize);
            device.CreateReadBackBuffer(readBackBuffer, defaultSize);
            uploadBuffer.Initialize(device, 65536);

        }

        public void Begin()
        {
            commandList.BeginCommand();
            commandList.SetDescriptorHeapDefault();
            commandList.SetComputeRootSignature(rootSignature);
            //commandList.SetComputeShader(currentComputeShader);
        }

        public void SetShader(ComputeShader computeShader)
        {
            currentComputeShader = computeShader;
        }

        public void End()
        {
            commandList.Copy(rwBuffer, readBackBuffer);
            commandList.EndCommand();
            commandList.Execute();
            device.WaitForGpu();
        }

        public void GetResult<T>(Span<T> data) where T : struct
        {
            readBackBuffer.CopyTo(MemoryMarshal.Cast<T, byte>(data));
        }

        public void Upload<T>(Span<T> data,int slot) where T : struct
        {
            int offset = uploadBuffer.Upload(data);
            uploadBuffer.SetComputeCBV(commandList, offset, slot);
        }
    }
}
