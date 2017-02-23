using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;
using System.Security;

namespace Lerp2Console
{
    [SuppressUnmanagedCodeSecurity()]
    public sealed class NativeMethods
    {

        /// ----------------------------------------------------------------------------------------------------
        /// <summary>
        /// Creates a symbolic link.
        /// </summary>
        /// ----------------------------------------------------------------------------------------------------
        /// <remarks>
        /// <see href="https://msdn.microsoft.com/en-us/library/windows/desktop/aa363866%28v=vs.85%29.aspx"/>
        /// </remarks>
        /// ----------------------------------------------------------------------------------------------------
        /// <param name="dstFilePath">
        /// The path of the symbolic link to be created.
        /// </param>
        /// 
        /// <param name="srcFilePath">
        /// The path of the target for the symbolic link to be created.
        /// <para></para>
        /// If <paramref name="srcFilePath"/> has a device name associated with it, 
        /// the link is treated as an absolute link; 
        /// otherwise, the link is treated as a relative link.
        /// </param>
        /// 
        /// <param name="flags">
        /// Indicates whether the link target is a file or is a directory.
        /// </param>
        /// ----------------------------------------------------------------------------------------------------
        /// <returns>
        /// If the function succeeds, the return value is <see langword="True"/>.
        /// <para></para>
        /// If the function fails, the return value is <see langword="False"/>.
        /// <para></para>
        /// To get extended error information, call <see cref="Marshal.GetLastWin32Error"/>.
        /// <para></para>
        /// </returns>
        /// ----------------------------------------------------------------------------------------------------
        [SuppressMessage("Microsoft.Interoperability", "CA1401:PInvokesShouldNotBeVisible", Justification = "Visible for API")]
        [DllImport("Kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.I1)]
        public static extern bool CreateSymbolicLink(string dstFilePath, string srcFilePath, [MarshalAs(UnmanagedType.I4)] NativeEnums.SymbolicLinkFlags flags);

        /// <summary>
        /// Prevents a default instance of the <see cref="NativeMethods"/> class from being created.
        /// </summary>
        private NativeMethods()
        {
        }

    }

    public sealed class NativeEnums
    {

        /// ----------------------------------------------------------------------------------------------------
        /// <summary>
        /// Indicates whether a symbolic link is a file or is a directory.
        /// </summary>
        /// ----------------------------------------------------------------------------------------------------
        /// <remarks>
        /// <see href="https://msdn.microsoft.com/en-us/library/windows/desktop/aa363866%28v=vs.85%29.aspx"/>
        /// </remarks>
        /// ----------------------------------------------------------------------------------------------------
        public enum SymbolicLinkFlags : int
        {

            /// <summary>
            /// The link target is a file.
            /// </summary>
            File = 0x0,

            /// <summary>
            /// The link target is a directory.
            /// </summary>
            Directory = 0x1

        }

        /// <summary>
        /// Prevents a default instance of the <see cref="NativeEnums"/> class from being created.
        /// </summary>
        private NativeEnums()
        {
        }

    }
}
