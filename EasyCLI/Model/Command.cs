using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace EasyCLI.Model
{
    public struct Command
    {
        public object ClassObject;

        public MethodInfo FunctionObject;
        public Param[] Arguments;
    }
}
