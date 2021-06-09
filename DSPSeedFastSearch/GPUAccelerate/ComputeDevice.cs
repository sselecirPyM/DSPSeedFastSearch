using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using Vortice.Direct3D12;
using Vortice.Direct3D12.Debug;
using Vortice.Dxc;
using Vortice.DXGI;
using Vortice.Mathematics;

namespace DSPSeedFastSearch.GPUAccelerate
{
    public struct ResourceDelayDestroy
    {
        public ulong destroyFrame;
        public ID3D12Resource resource;
        public ID3D12PipelineState pipelineState;
    }

    public class ComputeDevice : IDisposable
    {
        //public static ComputeDevice Instance { get => _device; }
        //public static ComputeDevice _device;

        //public static void StaticInit()
        //{
        //    if (_device == null)
        //    {
        //        _device = new ComputeDevice();
        //        _device.Init();
        //    }
        //}
        public ID3D12Device2 device;
        public IDXGIAdapter adapter;
        public IDXGIFactory7 dxgiFactory;

        public ID3D12CommandQueue commandQueue;
        public List<ID3D12CommandAllocator> commandAllocators;
        public EventWaitHandle waitHandle;
        public ID3D12Fence fence;
        public ID3D12DescriptorHeap cbvsrvuavHeap;
        public int cbvsrvuavHeapIncrementSize;
        public int cbvsrvuavAllocatedCount;

        public Queue<ResourceDelayDestroy> delayDestroy = new Queue<ResourceDelayDestroy>();
        public int CBVSRVUAVDescriptorCount = 65536;
        public int executeIndex = 0;
        public ulong executeCount = 3;
        public int bufferCount = 3;

        public void Init()
        {
#if DEBUG
            if (D3D12.D3D12GetDebugInterface<ID3D12Debug>(out var pDx12Debug).Success)
            {
                pDx12Debug.EnableDebugLayer();
            }
#endif
            ThrowIfFailed(DXGI.CreateDXGIFactory1(out dxgiFactory));
            int index1 = 0;
            while (true)
            {
                var hr = dxgiFactory.EnumAdapterByGpuPreference(index1, GpuPreference.HighPerformance, out adapter);
                if (hr == SharpGen.Runtime.Result.Ok)
                {
                    break;
                }
                index1++;
            }
            ThrowIfFailed(D3D12.D3D12CreateDevice(this.adapter, out device));
            CommandQueueDescription description;
            description.Flags = CommandQueueFlags.None;
            description.Type = CommandListType.Direct;
            description.NodeMask = 0;
            description.Priority = 0;
            ThrowIfFailed(device.CreateCommandQueue(description, out commandQueue));
            DescriptorHeapDescription descriptorHeapDescription;
            descriptorHeapDescription.DescriptorCount = CBVSRVUAVDescriptorCount;
            descriptorHeapDescription.Type = DescriptorHeapType.ConstantBufferViewShaderResourceViewUnorderedAccessView;
            descriptorHeapDescription.Flags = DescriptorHeapFlags.ShaderVisible;
            descriptorHeapDescription.NodeMask = 0;
            ThrowIfFailed(device.CreateDescriptorHeap(descriptorHeapDescription, out cbvsrvuavHeap));

            waitHandle = new EventWaitHandle(false, EventResetMode.AutoReset);
            commandAllocators = new List<ID3D12CommandAllocator>();
            for (int i = 0; i < bufferCount; i++)
            {
                ThrowIfFailed(device.CreateCommandAllocator(CommandListType.Direct, out ID3D12CommandAllocator commandAllocator));
                commandAllocators.Add(commandAllocator);
            }
            ThrowIfFailed(device.CreateFence(executeCount, FenceFlags.None, out fence));
            executeCount++;

            cbvsrvuavHeapIncrementSize = device.GetDescriptorHandleIncrementSize(DescriptorHeapType.ConstantBufferViewShaderResourceViewUnorderedAccessView);
        }

        public ID3D12CommandAllocator GetCommandAllocator()
        {
            return commandAllocators[executeIndex];
        }

        public void CreateRootSignature(RootSignature rootSignature, IList<RootSignatureParamP> types)
        {
            //static samplers
            StaticSamplerDescription[] samplerDescription = new StaticSamplerDescription[4];
            samplerDescription[0] = new StaticSamplerDescription(ShaderVisibility.All, 0, 0)
            {
                AddressU = TextureAddressMode.Clamp,
                AddressV = TextureAddressMode.Clamp,
                AddressW = TextureAddressMode.Clamp,
                BorderColor = StaticBorderColor.OpaqueBlack,
                ComparisonFunction = ComparisonFunction.Never,
                Filter = Filter.MinMagMipLinear,
                MipLODBias = 0,
                MaxAnisotropy = 0,
                MinLOD = 0,
                MaxLOD = float.MaxValue,
                ShaderVisibility = ShaderVisibility.All,
                RegisterSpace = 0,
                ShaderRegister = 0,
            };
            samplerDescription[1] = samplerDescription[0];
            samplerDescription[2] = samplerDescription[0];
            samplerDescription[3] = samplerDescription[0];

            samplerDescription[1].ShaderRegister = 1;
            samplerDescription[2].ShaderRegister = 2;
            samplerDescription[3].ShaderRegister = 3;
            samplerDescription[1].MaxAnisotropy = 16;
            samplerDescription[1].Filter = Filter.Anisotropic;
            samplerDescription[2].ComparisonFunction = ComparisonFunction.Less;
            samplerDescription[2].Filter = Filter.ComparisonMinMagMipLinear;
            samplerDescription[3].Filter = Filter.MinMagMipPoint;

            RootParameter1[] rootParameters = new RootParameter1[types.Count];

            int cbvCount = 0;
            int srvCount = 0;
            int uavCount = 0;
            rootSignature.cbv.Clear();
            rootSignature.srv.Clear();
            rootSignature.uav.Clear();

            for (int i = 0; i < types.Count; i++)
            {
                RootSignatureParamP t = types[i];
                switch (t)
                {
                    case RootSignatureParamP.CBV:
                        rootParameters[i] = new RootParameter1(RootParameterType.ConstantBufferView, new RootDescriptor1(cbvCount, 0), ShaderVisibility.All);
                        rootSignature.cbv[cbvCount] = i;
                        cbvCount++;
                        break;
                    case RootSignatureParamP.SRV:
                        rootParameters[i] = new RootParameter1(RootParameterType.ShaderResourceView, new RootDescriptor1(srvCount, 0), ShaderVisibility.All);
                        rootSignature.srv[srvCount] = i;
                        srvCount++;
                        break;
                    case RootSignatureParamP.UAV:
                        rootParameters[i] = new RootParameter1(RootParameterType.UnorderedAccessView, new RootDescriptor1(uavCount, 0), ShaderVisibility.All);
                        rootSignature.uav[uavCount] = i;
                        uavCount++;
                        break;
                    case RootSignatureParamP.CBVTable:
                        rootParameters[i] = new RootParameter1(new RootDescriptorTable1(new DescriptorRange1(DescriptorRangeType.ConstantBufferView, 1, cbvCount)), ShaderVisibility.All);
                        rootSignature.cbv[cbvCount] = i;
                        cbvCount++;
                        break;
                    case RootSignatureParamP.SRVTable:
                        rootParameters[i] = new RootParameter1(new RootDescriptorTable1(new DescriptorRange1(DescriptorRangeType.ShaderResourceView, 1, srvCount)), ShaderVisibility.All);
                        rootSignature.srv[srvCount] = i;
                        srvCount++;
                        break;
                    case RootSignatureParamP.UAVTable:
                        rootParameters[i] = new RootParameter1(new RootDescriptorTable1(new DescriptorRange1(DescriptorRangeType.UnorderedAccessView, 1, uavCount)), ShaderVisibility.All);
                        rootSignature.uav[uavCount] = i;
                        uavCount++;
                        break;
                }
            }

            RootSignatureDescription1 rootSignatureDescription = new RootSignatureDescription1();
            rootSignatureDescription.StaticSamplers = samplerDescription;
            rootSignatureDescription.Flags = RootSignatureFlags.AllowInputAssemblerInputLayout;
            rootSignatureDescription.Parameters = rootParameters;

            rootSignature.rootSignature = device.CreateRootSignature<ID3D12RootSignature>(0, rootSignatureDescription);
        }

        public void CreateUploadBuffer(UploadBuffer buffer, int size)
        {
            DestroyResource(buffer.resource);
            buffer.resourceStates = ResourceStates.GenericRead;
            buffer.resource = device.CreateCommittedResource<ID3D12Resource>(
                HeapProperties.UploadHeapProperties,
                HeapFlags.None,
                ResourceDescription.Buffer(new ResourceAllocationInfo((ulong)size, 0)),
                ResourceStates.GenericRead);
            buffer.size = size;
        }

        public void CreateRWBuffer(RWBuffer buffer, int size)
        {
            DestroyResource(buffer.resource);
            buffer.resourceStates = ResourceStates.GenericRead;
            buffer.resource = device.CreateCommittedResource<ID3D12Resource>(
                HeapProperties.DefaultHeapProperties,
                HeapFlags.None,
                ResourceDescription.Buffer(new ResourceAllocationInfo((ulong)size, 0), ResourceFlags.AllowUnorderedAccess),
                ResourceStates.GenericRead);
            buffer.size = size;
        }


        public void CreateReadBackBuffer(ReadBackBuffer buffer, int size)
        {
            DestroyResource(buffer.resource);
            buffer.resourceStates = ResourceStates.CopyDestination;
            buffer.resource = device.CreateCommittedResource<ID3D12Resource>(
                HeapProperties.ReadbackHeapProperties,
                HeapFlags.None,
                ResourceDescription.Buffer(new ResourceAllocationInfo((ulong)size, 0)),
                ResourceStates.CopyDestination);
            buffer.size = size;
        }

        public void DestroyResource(ID3D12PipelineState res)
        {
            if (res != null)
                delayDestroy.Enqueue(new ResourceDelayDestroy { pipelineState = res, destroyFrame = executeCount });
        }

        public void DestroyResource(ID3D12Resource res)
        {
            if (res != null)
                delayDestroy.Enqueue(new ResourceDelayDestroy { resource = res, destroyFrame = executeCount });
        }

        private void DestroyResourceInternal(ulong completedFrame)
        {
            while (delayDestroy.Count > 0)
                if (delayDestroy.Peek().destroyFrame <= completedFrame)
                {
                    var p = delayDestroy.Dequeue();
                    p.pipelineState?.Dispose();
                    p.resource?.Dispose();
                }
                else
                    break;
        }

        public void WaitForGpu()
        {
            commandQueue.Signal(fence, executeCount);
            fence.SetEventOnCompletion(executeCount, waitHandle);
            waitHandle.WaitOne();
            DestroyResourceInternal(fence.CompletedValue);
            executeCount++;
        }

        void ThrowIfFailed(SharpGen.Runtime.Result hr)
        {
            if (hr != SharpGen.Runtime.Result.Ok)
            {
                throw new NotImplementedException(hr.ToString());
            }
        }

        public void Dispose()
        {
            WaitForGpu();
            while (delayDestroy.Count > 0)
            {
                var p = delayDestroy.Dequeue();
                p.pipelineState?.Dispose();
                p.resource?.Dispose();
            }
            foreach (var commandAllocator in commandAllocators)
                commandAllocator.Dispose();
            dxgiFactory?.Dispose();
            commandQueue?.Dispose();
            cbvsrvuavHeap?.Dispose();
            fence?.Dispose();
            device?.Dispose();
            adapter?.Dispose();
        }
    }
}
