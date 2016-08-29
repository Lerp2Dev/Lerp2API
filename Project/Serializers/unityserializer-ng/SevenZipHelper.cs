using ICSharpCode.SharpZipLib.Zip.Compression.Streams;
using System;
using System.IO;

public static class CompressionHelper
{
    public static string technique = "ZipStream";

    public static string Compress(byte[] data)
    {
        using (var m = new MemoryStream())
        {
            switch (technique)
            {
                case "ZipStream":
                    using (var br = new BinaryWriter(m))
                    using (var z = new DeflaterOutputStream(m))
                    {
                        br.Write(data.Length);
                        z.Write(data, 0, data.Length);
                        z.Flush();
                    }
                    break;
            }
            return technique + ":" + Convert.ToBase64String(m.GetBuffer());
        }
    }

    public static byte[] Decompress(string data)
    {
        byte[] output = null;
        if (data.StartsWith("ZipStream:"))
        {
            using (var m = new MemoryStream(Convert.FromBase64String(data.Substring(10))))
            using (var z = new InflaterInputStream(m))
            using (var br = new BinaryReader(m))
            {
                var length = br.ReadInt32();
                output = new byte[length];
                z.Read(output, 0, length);
            }
        }
        return output;
    }
}