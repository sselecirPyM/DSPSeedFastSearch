using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace DSPSeedFastSearch.GPUAccelerate
{
    public class RingUploadBuffer : UploadBuffer
    {
        public IntPtr cpuResPtr;
        public ulong gpuResPtr;
        public int allocateIndex = 0;


        public void Initialize(ComputeDevice device, int size)
        {
            this.size = size;
            device.CreateUploadBuffer(this, size);
            cpuResPtr = resource.Map(0);
            gpuResPtr = resource.GPUVirtualAddress;
        }

        public int Upload<T>(Span<T> data) where T : struct
        {
            int size1 = data.Length * Marshal.SizeOf(typeof(T));
            int afterAllocateIndex = allocateIndex + ((size1 + 255) & ~255);
            if (afterAllocateIndex > size)
            {
                allocateIndex = 0;
                afterAllocateIndex = allocateIndex + ((size1 + 255) & ~255);
            }
            unsafe
            {
                data.CopyTo(new Span<T>((cpuResPtr + allocateIndex).ToPointer(), data.Length));
            }

            int ofs = allocateIndex;
            allocateIndex = afterAllocateIndex % size;
            return ofs;
        }

        public void SetComputeCBV(CommandList graphicsContext, int offset, int slot)
        {
            graphicsContext.SetComputeCBV(this, offset, slot);
        }
    }
}
