using System;
using System.Collections.Generic;
using System.Text;
using Vortice.Direct3D12;

namespace DSPSeedFastSearch.GPUAccelerate
{
    public class ReadBackBuffer : Buffer
    {
        public void CopyTo(Span<byte> destination)
        {
            IntPtr ptr = resource.Map(0);
            unsafe
            {
                new Span<byte>(ptr.ToPointer(),Math.Min( size,destination.Length)).CopyTo(destination);
            }
            resource.Unmap(0);
        }
    }
}
