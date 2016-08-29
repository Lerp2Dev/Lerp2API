using System;
using System.Management;
using System.Text;

namespace Lerp2API.Utility
{
    internal class UniqueIdentifiers
    {
        private string UniqueMachineId()
        {
            StringBuilder builder = new StringBuilder();

            string query = "SELECT * FROM Win32_BIOS";
            ManagementObjectSearcher searcher = new ManagementObjectSearcher(query);
            //  This should only find one
            foreach (ManagementObject item in searcher.Get())
            {
                object obj = item["Manufacturer"];
                builder.Append(Convert.ToString(obj));
                builder.Append(':');
                obj = item["SerialNumber"];
                builder.Append(Convert.ToString(obj));
            }

            return builder.ToString();
        }
    }
}