using System;
using System.Reflection;

namespace Lerp2Console
{
    public class Instancials
    {
        private Assembly asm;
        
        public Instancials(string asmPath)
        {
            asm = Assembly.LoadFile(asmPath);
        }

        public dynamic CreateStatic(string asmFullPath)
        {
            return DynamicStaticTypeMembers.Create(asm.GetType(asmFullPath));
        }

        public dynamic CreateInstance(string asmFullPath)
        {
            return Activator.CreateInstance(asm.GetType(asmFullPath));
        }
    }
}
