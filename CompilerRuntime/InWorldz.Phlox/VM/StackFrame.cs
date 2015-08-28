using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace InWorldz.Phlox.VM
{
    public class StackFrame
    {
        public const int MemSize = FunctionInfo.MemSize + 8;

        public FunctionInfo FunctionInfo;
        public int ReturnAddress;
        public object[] Locals;

        public StackFrame(FunctionInfo funcInfo, int returnAddress)
        {
            FunctionInfo = funcInfo;
            ReturnAddress = returnAddress;

            int totalLocals = funcInfo.NumberOfArguments + funcInfo.NumberOfLocals;
            Locals = new object[totalLocals];
            
            //fill with sentinel values for null reference checks
            /*for (int i = 0; i < totalLocals; i++)
            {
                Locals[i] = Types.Sentinel.Instance;
            }*/
        }
    }
}
