using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using ProtoBuf;

namespace Halcyon.Phlox.Serialization
{
    /// <summary>
    /// Serialized version of a VM stackframe
    /// </summary>
    [ProtoContract]
    public class SerializedStackFrame
    {
        [ProtoMember(1)]
        public VM.FunctionInfo FunctionInfo;

        [ProtoMember(2)]
        public int ReturnAddress;

        [ProtoMember(3)]
        public SerializedLSLPrimitive[] Locals;

        public SerializedStackFrame()
        {
        }

        public static SerializedStackFrame FromStackFrame(VM.StackFrame frame)
        {
            if (frame == null) return null;

            SerializedStackFrame serFrame = new SerializedStackFrame();
            serFrame.FunctionInfo = frame.FunctionInfo;

            serFrame.ReturnAddress = frame.ReturnAddress;

            serFrame.Locals = new SerializedLSLPrimitive[frame.Locals.Length];
            for (int i = 0; i < serFrame.Locals.Length; i++)
            {
                serFrame.Locals[i] = SerializedLSLPrimitive.FromPrimitive(frame.Locals[i]);
            }

            return serFrame;
        }

        public VM.StackFrame ToStackFrame()
        {
            VM.StackFrame frame = new VM.StackFrame(this.FunctionInfo, this.ReturnAddress);
            frame.Locals = SerializedLSLPrimitive.ToPrimitiveList(this.Locals);

            return frame;
        }
    }
}
