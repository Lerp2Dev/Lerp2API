using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Lerp2API.SafeECalls
{
    public class SafeECall
    {
        public const string error = "Attemp to call a Unity method outside from it, please, set at your startup, 'LerpedCore.safeECallEnabled' as true.";
    }
}
