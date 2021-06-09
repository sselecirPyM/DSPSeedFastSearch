using System;
using System.Collections.Generic;
using System.Text;
using Vortice.Direct3D12;

namespace DSPSeedFastSearch.GPUAccelerate
{
    public class CommandList : IDisposable
    {
        public ComputeDevice device;
        public ID3D12GraphicsCommandList5 commandList;
        public RootSignature currentRootSignature;
        public void Init(ComputeDevice device)
        {
            ThrowIfFailed(device.device.CreateCommandList(0, CommandListType.Direct, device.GetCommandAllocator(), null, out commandList));
            commandList.Close();
            this.device = device;
        }

        public void SetDescriptorHeapDefault()
        {
            commandList.SetDescriptorHeaps(1, new[] { device.cbvsrvuavHeap });
        }

        public void SetComputeRootSignature(RootSignature rootSignature)
        {
            currentRootSignature = rootSignature;
            commandList.SetComputeRootSignature(rootSignature.rootSignature);
        }

        public void SetComputeShader(ComputeShader computeShader)
        {
            if (computeShader.pipelineStates.TryGetValue(currentRootSignature, out var pipelineState))
            {
                commandList.SetPipelineState(pipelineState);
            }
            else
            {
                ComputePipelineStateDescription description = new ComputePipelineStateDescription();
                description.RootSignature = currentRootSignature.rootSignature;
                description.ComputeShader = computeShader.byteCode;
                pipelineState = device.device.CreateComputePipelineState<ID3D12PipelineState>(description);
                computeShader.pipelineStates[currentRootSignature] = pipelineState;
                commandList.SetPipelineState(pipelineState);
            }
        }

        public void SetComputeCBV(Buffer buffer, int offset, int slot)
        {
            buffer.StateChange(commandList, ResourceStates.GenericRead);
            commandList.SetComputeRootConstantBufferView(currentRootSignature.cbv[slot], buffer.resource.GPUVirtualAddress + (ulong)offset);
        }

        public void SetComputeUAV(Buffer buffer, int offset, int slot)
        {
            buffer.StateChange(commandList, ResourceStates.UnorderedAccess);
            commandList.SetComputeRootUnorderedAccessView(currentRootSignature.uav[slot], buffer.resource.GPUVirtualAddress + (ulong)offset);
        }

        public void Copy(Buffer source, Buffer destination)
        {
            source.StateChange(commandList, ResourceStates.GenericRead);
            destination.StateChange(commandList, ResourceStates.CopyDestination);
            commandList.CopyResource(destination.resource, source.resource);
        }

        public void Dispatch(int x, int y, int z)
        {
            commandList.Dispatch(x, y, z);
        }

        public void BeginCommand()
        {
            commandList.Reset(device.GetCommandAllocator());
        }

        public void EndCommand()
        {
            commandList.Close();
        }

        public void Execute()
        {
            device.commandQueue.ExecuteCommandList(commandList);
        }

        private void ThrowIfFailed(SharpGen.Runtime.Result hr)
        {
            if (hr != SharpGen.Runtime.Result.Ok)
            {
                throw new NotImplementedException();
            }
        }

        private void CBVSRVUAVHandle(out CpuDescriptorHandle cpuHandle, out GpuDescriptorHandle gpuHandle)
        {
            CpuDescriptorHandle handle = device.cbvsrvuavHeap.GetCPUDescriptorHandleForHeapStart();
            handle.Ptr += device.cbvsrvuavAllocatedCount * device.cbvsrvuavHeapIncrementSize;
            GpuDescriptorHandle gpuHandle1 = device.cbvsrvuavHeap.GetGPUDescriptorHandleForHeapStart();
            gpuHandle1.Ptr += (ulong)(device.cbvsrvuavAllocatedCount * device.cbvsrvuavHeapIncrementSize);

            device.cbvsrvuavAllocatedCount = (device.cbvsrvuavAllocatedCount + 1) % device.CBVSRVUAVDescriptorCount;
            cpuHandle = handle;
            gpuHandle = gpuHandle1;
        }

        public void Dispose()
        {
            commandList?.Dispose();
            commandList = null;
        }
    }
}
