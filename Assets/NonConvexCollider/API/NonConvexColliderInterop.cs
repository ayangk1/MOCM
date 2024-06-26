using System;
using System.Runtime.InteropServices;

namespace Plawius.NonConvexCollider
{
    internal static class Interop
    {
        [DllImport("libvhacd")]
        internal static extern int GetMeshEx(IntPtr points, 
                                             int poitns_size, 
                                             IntPtr triangles, 
                                             int triangles_size,
                                             out IntPtr out_points, 
                                             out IntPtr out_triangles, 
                                             out IntPtr indexes, 
                                             out int indexes_cnt, 
                                             Parameters prms);

        [DllImport("libvhacd")]
        internal static extern int ReleaseMemory(IntPtr ptr);
    }
}