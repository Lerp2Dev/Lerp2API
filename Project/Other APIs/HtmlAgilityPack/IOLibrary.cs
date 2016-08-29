namespace HtmlAgilityPack
{
    using System.IO;
    using System.Runtime.InteropServices;

    [StructLayout(LayoutKind.Sequential, Size = 1)]
    internal struct IOLibrary
    {
        internal static void CopyAlways(string source, string target)
        {
            if (File.Exists(source))
            {
                Directory.CreateDirectory(Path.GetDirectoryName(target));
                MakeWritable(target);
                File.Copy(source, target, true);
            }
        }

        internal static void MakeWritable(string path)
        {
            if (File.Exists(path))
            {
                File.SetAttributes(path, File.GetAttributes(path) & ~FileAttributes.ReadOnly);
            }
        }
    }
}