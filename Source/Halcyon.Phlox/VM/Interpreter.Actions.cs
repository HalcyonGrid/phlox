using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using OpenMetaverse;
using Halcyon.Phlox.Types;
using System.Text.RegularExpressions;
using System.Globalization;

namespace Halcyon.Phlox.VM
{
    public partial class Interpreter
    {
        public void SafeOperandsPush(object obj)
        {
            if (obj == null)
                throw new VMException("Attempt to push null operand.\n" + DumpState());

            /*if (obj is Sentinel)
                throw new VMException("Attempt to push sentinel operand.\n" + DumpState());*/

            _state.Operands.Push(obj);
        }

        private string DumpState()
        {
            StringBuilder stateinfo = new StringBuilder();

            stateinfo.AppendLine(String.Format("IP: {0}", _state.IP));

            string frameinfo = "Top Frame: ";
            string frameinfoLocals = "  Locals: None";
            if (_state.TopFrame != null)
            {
                frameinfo = String.Format("Top Frame: Name: {0}, Addr: {1}, Locals {2}", _state.TopFrame.FunctionInfo.Name,
                        _state.TopFrame.FunctionInfo.Address,
                        _state.TopFrame.FunctionInfo.NumberOfArguments + _state.TopFrame.FunctionInfo.NumberOfLocals);

                StringBuilder locals = new StringBuilder();
                foreach (object obj in _state.TopFrame.Locals)
                {
                    locals.AppendLine(String.Format("  Local: {0}, Type: {1}", obj, obj != null ? obj.GetType().FullName : "null"));
                }

                frameinfoLocals = locals.ToString();
            }

            stateinfo.AppendLine(frameinfo);
            stateinfo.AppendLine(frameinfoLocals);

            string eventinfo = "Running Event: None";
            string eventinfoArgs = "  Args: None";
            if (_state.RunningEvent != null)
            {
                eventinfo = String.Format("Running Event: Name: {0}", _state.RunningEvent.EventType.ToString());

                StringBuilder args = new StringBuilder();
                foreach (object obj in _state.RunningEvent.Args)
                {
                    args.AppendLine(String.Format("  Arg: {0}, Type: {1}", obj, obj != null ? obj.GetType().FullName : "null"));
                }

                eventinfoArgs = args.ToString();
            }

            stateinfo.AppendLine(eventinfo);
            stateinfo.AppendLine(eventinfoArgs);

            return stateinfo.ToString();
        }

        private void _Load(object[] varList)
        {
            int index = this.GetIntOperand();
            object local = varList[index];
            SafeOperandsPush(local);
        }

        private void _LoadSub(object[] varList)
        {
            int index = this.GetIntOperand();
            int subIndex = this.GetIntOperand();

            object local = varList[index];

            if (local is Vector3)
            {
                Vector3 vlocal = (Vector3)local;
                switch (subIndex)
                {
                    case 0:
                        SafeOperandsPush(vlocal.X);
                        break;

                    case 1:
                        SafeOperandsPush(vlocal.Y);
                        break;

                    case 2:
                        SafeOperandsPush(vlocal.Z);
                        break;

                    default:
                        throw new VMException("Op_LoadSub: Invalid subscript index for vector");
                }
            }
            else if (local is Quaternion)
            {
                Quaternion qlocal = (Quaternion)local;
                switch (subIndex)
                {
                    case 0:
                        SafeOperandsPush(qlocal.X);
                        break;

                    case 1:
                        SafeOperandsPush(qlocal.Y);
                        break;

                    case 2:
                        SafeOperandsPush(qlocal.Z);
                        break;

                    case 3:
                        SafeOperandsPush(qlocal.W);
                        break;

                    default:
                        throw new VMException("Op_LoadSub: Invalid subscript index for rotation");
                }
            }
            else
            {
                throw new VMException("Op_LoadSub: Subscript access on non-subscriptable type");
            }
        }

        private void Op_Load()
        {
            this._Load(_state.TopFrame.Locals);
        }

        private void Op_LoadSub()
        {
            this._LoadSub(_state.TopFrame.Locals);
        }

        private void _Store(object[] destList)
        {
            int index = this.GetIntOperand();
            object currLocal = destList[index];
            object newLocal = _state.Operands.Pop();

            destList[index] = newLocal;

            _state.MemInfo.ReplaceStored(currLocal, newLocal);
        }

        private void Op_Store()
        {
            this._Store(_state.TopFrame.Locals);
        }

        enum SubScriptOp
        {
            NONE,
            INC,
            DEC
        }

        /// <summary>
        /// Assigns a new value to a subscript of an existing value
        /// </summary>
        /// <param name="destList">The list to retrieve the value from</param>
        /// <param name="destIndex">The index of the item to change</param>
        /// <param name="subIndex">The subscript index to change</param>
        /// <param name="subScriptValue">The new value for the subscript</param>
        /// <returns>The old subscript value</returns>
        private float _AssignSubscript(object[] destList, int destIndex, int subIndex, object subScriptValue,
            SubScriptOp op)
        {
            object currLocal = destList[destIndex];

            if (currLocal is Vector3)
            {
                Vector3 vlocal = (Vector3)currLocal;

                switch (subIndex)
                {
                    case 0:
                        if (op == SubScriptOp.INC)
                        {
                            subScriptValue = vlocal.X + 1;
                        }
                        else if (op == SubScriptOp.DEC)
                        {
                            subScriptValue = vlocal.X - 1;
                        }

                        destList[destIndex] = new Vector3((float)subScriptValue, vlocal.Y, vlocal.Z);
                        return vlocal.X;

                    case 1:
                        if (op == SubScriptOp.INC)
                        {
                            subScriptValue = vlocal.Y + 1;
                        }
                        else if (op == SubScriptOp.DEC)
                        {
                            subScriptValue = vlocal.Y - 1;
                        }

                        destList[destIndex] = new Vector3(vlocal.X, (float)subScriptValue, vlocal.Z);
                        return vlocal.Y;

                    case 2:
                        if (op == SubScriptOp.INC)
                        {
                            subScriptValue = vlocal.Z + 1;
                        }
                        else if (op == SubScriptOp.DEC)
                        {
                            subScriptValue = vlocal.Z - 1;
                        }

                        destList[destIndex] = new Vector3(vlocal.X, vlocal.Y, (float)subScriptValue);
                        return vlocal.Z;

                    default:
                        throw new VMException("Op_LoadSub: Invalid subscript index for vector");
                }
            }
            else if (currLocal is Quaternion)
            {
                Quaternion qlocal = (Quaternion)currLocal;
                switch (subIndex)
                {
                    case 0:
                        if (op == SubScriptOp.INC)
                        {
                            subScriptValue = qlocal.X + 1;
                        }
                        else if (op == SubScriptOp.DEC)
                        {
                            subScriptValue = qlocal.X - 1;
                        }

                        destList[destIndex] = new Quaternion((float)subScriptValue, qlocal.Y, qlocal.Z, qlocal.W);
                        return qlocal.X;

                    case 1:
                        if (op == SubScriptOp.INC)
                        {
                            subScriptValue = qlocal.Y + 1;
                        }
                        else if (op == SubScriptOp.DEC)
                        {
                            subScriptValue = qlocal.Y - 1;
                        }

                        destList[destIndex] = new Quaternion(qlocal.X, (float)subScriptValue, qlocal.Z, qlocal.W);
                        return qlocal.Y;

                    case 2:
                        if (op == SubScriptOp.INC)
                        {
                            subScriptValue = qlocal.Z + 1;
                        }
                        else if (op == SubScriptOp.DEC)
                        {
                            subScriptValue = qlocal.Z - 1;
                        }

                        destList[destIndex] = new Quaternion(qlocal.X, qlocal.Y, (float)subScriptValue, qlocal.W);
                        return qlocal.Z;

                    case 3:
                        if (op == SubScriptOp.INC)
                        {
                            subScriptValue = qlocal.W + 1;
                        }
                        else if (op == SubScriptOp.DEC)
                        {
                            subScriptValue = qlocal.W - 1;
                        }

                        destList[destIndex] = new Quaternion(qlocal.X, qlocal.Y, qlocal.Z, (float)subScriptValue);
                        return qlocal.W;

                    default:
                        throw new VMException("Op_LoadSub: Invalid subscript index for rotation");
                }
            }
            else
            {
                throw new VMException("Op_LoadSub: Subscript access on non-subscriptable type");
            }
        }

        private void _StoreSub(object[] destList)
        {
            int index = this.GetIntOperand();
            int subIndex = this.GetIntOperand();

            object subScriptValue = _state.Operands.Pop();
            _AssignSubscript(destList, index, subIndex, subScriptValue, SubScriptOp.NONE);
        }

        private void Op_StoreSub()
        {
            this._StoreSub(_state.TopFrame.Locals);
        }

        private void Op_Gload()
        {
            this._Load(_state.Globals);
        }

        private void Op_GloadSub()
        {
            this._LoadSub(_state.Globals);
        }

        private void Op_Gstore()
        {
            this._Store(_state.Globals);
        }

        private void Op_GstoreSub()
        {
            this._StoreSub(_state.Globals);
        }

        private void Op_Iconst()
        {
            int constVal = this.GetIntOperand();
            SafeOperandsPush(constVal);
        }

        private void Op_Fconst()
        {
            int constIndex = this.GetIntOperand();
            SafeOperandsPush(_script.ConstPool[constIndex]);
        }

        private void Op_Sconst()
        {
            int constIndex = this.GetIntOperand();
            SafeOperandsPush((string)_script.ConstPool[constIndex]);
        }

        private void Op_Vconst()
        {
            int constIndex = this.GetIntOperand();
            SafeOperandsPush(_script.ConstPool[constIndex]);
        }

        private void Op_Rconst()
        {
            int constIndex = this.GetIntOperand();
            SafeOperandsPush(_script.ConstPool[constIndex]);
        }

        private void Op_Lconst()
        {
            int constIndex = this.GetIntOperand();
            SafeOperandsPush(_script.ConstPool[constIndex]);
        }

        private void Op_Iadd()
        {
            int b = (int)_state.Operands.Pop();
            int a = (int)_state.Operands.Pop();

            SafeOperandsPush(a + b);
        }

        private void Op_Isub()
        {
            int b = (int)_state.Operands.Pop();
            int a = (int)_state.Operands.Pop();

            SafeOperandsPush(a - b);
        }

        private void Op_Imul()
        {
            int b = (int)_state.Operands.Pop();
            int a = (int)_state.Operands.Pop();

            SafeOperandsPush(a * b);
        }

        private void Op_Idiv()
        {
            int b = (int)_state.Operands.Pop();
            int a = (int)_state.Operands.Pop();

            SafeOperandsPush(a / b);
        }

        private void Op_Imod()
        {
            int b = (int)_state.Operands.Pop();
            int a = (int)_state.Operands.Pop();

            SafeOperandsPush(a % b);
        }

        private void Op_Ibor()
        {
            int b = (int)_state.Operands.Pop();
            int a = (int)_state.Operands.Pop();

            SafeOperandsPush(a | b);
        }

        private void Op_Iband()
        {
            int b = (int)_state.Operands.Pop();
            int a = (int)_state.Operands.Pop();

            SafeOperandsPush(a & b);
        }

        private void Op_Ibxor()
        {
            int b = (int)_state.Operands.Pop();
            int a = (int)_state.Operands.Pop();

            SafeOperandsPush(a ^ b);
        }

        private void Op_Irsh()
        {
            int b = (int)_state.Operands.Pop();
            int a = (int)_state.Operands.Pop();

            SafeOperandsPush(a >> b);
        }

        private void Op_Ilsh()
        {
            int b = (int)_state.Operands.Pop();
            int a = (int)_state.Operands.Pop();

            SafeOperandsPush(a << b);
        }

        private void _IPreinc(object[] destList)
        {
            int index = this.GetIntOperand();
            int oldVal = (int)destList[index];
            int newVal = oldVal + 1;

            destList[index] = newVal;

            SafeOperandsPush(newVal);
        }

        private void _IPostInc(object[] destList)
        {
            int index = this.GetIntOperand();
            int oldVal = (int)destList[index];
            int newVal = oldVal + 1;

            destList[index] = newVal;

            SafeOperandsPush(oldVal);
        }

        private void _IPreDec(object[] destList)
        {
            int index = this.GetIntOperand();
            int oldVal = (int)destList[index];
            int newVal = oldVal - 1;

            destList[index] = newVal;

            SafeOperandsPush(newVal);
        }

        private void _IPostDec(object[] destList)
        {
            int index = this.GetIntOperand();
            int oldVal = (int)destList[index];
            int newVal = oldVal - 1;

            destList[index] = newVal;

            SafeOperandsPush(oldVal);
        }

        private void _FPreInc(object[] destList)
        {
            int index = this.GetIntOperand();
            float oldVal = (float)destList[index];
            float newVal = oldVal + 1;

            destList[index] = newVal;

            SafeOperandsPush(newVal);
        }

        private void _FPostInc(object[] destList)
        {
            int index = this.GetIntOperand();
            float oldVal = (float)destList[index];
            float newVal = oldVal + 1;

            destList[index] = newVal;

            SafeOperandsPush(oldVal);
        }

        private void _FPreDec(object[] destList)
        {
            int index = this.GetIntOperand();
            float oldVal = (float)destList[index];
            float newVal = oldVal - 1;

            destList[index] = newVal;

            SafeOperandsPush(newVal);
        }

        private void _FPostDec(object[] destList)
        {
            int index = this.GetIntOperand();
            float oldVal = (float)destList[index];
            float newVal = oldVal - 1;

            destList[index] = newVal;

            SafeOperandsPush(oldVal);
        }

        private void Op_Ipreinc_l()
        {
            this._IPreinc(_state.TopFrame.Locals);
        }

        private void Op_Ipostinc_l()
        {
            this._IPostInc(_state.TopFrame.Locals);
        }

        private void Op_Ipredec_l()
        {
            this._IPreDec(_state.TopFrame.Locals);
        }

        private void Op_Ipostdec_l()
        {
            this._IPostDec(_state.TopFrame.Locals);
        }

        private void Op_Ipreinc_g()
        {
            this._IPreinc(_state.Globals);
        }

        private void Op_Ipostinc_g()
        {
            this._IPostInc(_state.Globals);
        }

        private void Op_Ipredec_g()
        {
            this._IPreDec(_state.Globals);
        }

        private void Op_Ipostdec_g()
        {
            this._IPostDec(_state.Globals);
        }

        private void Op_Fpreinc_l()
        {
            this._FPreInc(_state.TopFrame.Locals);
        }

        private void Op_Fpostinc_l()
        {
            this._FPostInc(_state.TopFrame.Locals);
        }

        private void Op_Fpredec_l()
        {
            this._FPreDec(_state.TopFrame.Locals);
        }

        private void Op_Fpostdec_l()
        {
            this._FPostDec(_state.TopFrame.Locals);
        }

        private void Op_Fpreinc_g()
        {
            this._FPreInc(_state.Globals);
        }

        private void Op_Fpostinc_g()
        {
            this._FPostInc(_state.Globals);
        }

        private void Op_Fpredec_g()
        {
            this._FPreDec(_state.Globals);
        }

        private void Op_Fpostdec_g()
        {
            this._FPostDec(_state.Globals);
        }

        private void _FPreIncSub(object[] destList)
        {
            int index = this.GetIntOperand();
            int subIndex = this.GetIntOperand();

            float prevVal = _AssignSubscript(destList, index, subIndex, null, SubScriptOp.INC);
            SafeOperandsPush(prevVal + 1);
        }

        private void _FPostIncSub(object[] destList)
        {
            int index = this.GetIntOperand();
            int subIndex = this.GetIntOperand();

            float prevVal = _AssignSubscript(destList, index, subIndex, null, SubScriptOp.INC);
            SafeOperandsPush(prevVal);
        }

        private void _FPreDecSub(object[] destList)
        {
            int index = this.GetIntOperand();
            int subIndex = this.GetIntOperand();

            float prevVal = _AssignSubscript(destList, index, subIndex, null, SubScriptOp.DEC);
            SafeOperandsPush(prevVal - 1);
        }

        private void _FPostDecSub(object[] destList)
        {
            int index = this.GetIntOperand();
            int subIndex = this.GetIntOperand();

            float prevVal = _AssignSubscript(destList, index, subIndex, null, SubScriptOp.DEC);
            SafeOperandsPush(prevVal);
        }

        private void Op_Fpreinc_l_sub()
        {
            _FPreIncSub(_state.TopFrame.Locals);
        }

        private void Op_Fpostinc_l_sub()
        {
            _FPostIncSub(_state.TopFrame.Locals);
        }

        private void Op_Fpredec_l_sub()
        {
            _FPreDecSub(_state.TopFrame.Locals);
        }

        private void Op_Fpostdec_l_sub()
        {
            _FPostDecSub(_state.TopFrame.Locals);
        }

        private void Op_Fpreinc_g_sub()
        {
            _FPreIncSub(_state.Globals);
        }

        private void Op_Fpostinc_g_sub()
        {
            _FPostIncSub(_state.Globals);
        }

        private void Op_Fpredec_g_sub()
        {
            _FPreDecSub(_state.Globals);
        }

        private void Op_Fpostdec_g_sub()
        {
            _FPostDecSub(_state.Globals);
        }

        private void Op_Ineg()
        {
            int i = (int)_state.Operands.Pop();
            SafeOperandsPush(-i);
        }

        private void Op_Ilnot()
        {
            int i = (int)_state.Operands.Pop();
            i = i == 0 ? 1 : 0;

            SafeOperandsPush(i);
        }

        private void Op_Ilor()
        {
            int b = (int)_state.Operands.Pop();
            int a = (int)_state.Operands.Pop();

            if (b != 0 || a != 0)
            {
                SafeOperandsPush(1);
            }
            else
            {
                SafeOperandsPush(0);
            }
        }

        private void Op_Iland()
        {
            int b = (int)_state.Operands.Pop();
            int a = (int)_state.Operands.Pop();

            if (b != 0 && a != 0)
            {
                SafeOperandsPush(1);
            }
            else
            {
                SafeOperandsPush(0);
            }
        }

        private void Op_Ilt()
        {
            int b = (int)_state.Operands.Pop();
            int a = (int)_state.Operands.Pop();

            if (a < b)
            {
                SafeOperandsPush(1);
            }
            else
            {
                SafeOperandsPush(0);
            }
        }

        private void Op_Igt()
        {
            int b = (int)_state.Operands.Pop();
            int a = (int)_state.Operands.Pop();

            if (a > b)
            {
                SafeOperandsPush(1);
            }
            else
            {
                SafeOperandsPush(0);
            }
        }

        private void Op_Ilte()
        {
            int b = (int)_state.Operands.Pop();
            int a = (int)_state.Operands.Pop();

            if (a <= b)
            {
                SafeOperandsPush(1);
            }
            else
            {
                SafeOperandsPush(0);
            }
        }

        private void Op_Igte()
        {
            int b = (int)_state.Operands.Pop();
            int a = (int)_state.Operands.Pop();

            if (a >= b)
            {
                SafeOperandsPush(1);
            }
            else
            {
                SafeOperandsPush(0);
            }
        }

        private void Op_Ieq()
        {
            int b = (int)_state.Operands.Pop();
            int a = (int)_state.Operands.Pop();

            if (a == b)
            {
                SafeOperandsPush(1);
            }
            else
            {
                SafeOperandsPush(0);
            }
        }

        private void Op_Ineq()
        {
            int b = (int)_state.Operands.Pop();
            int a = (int)_state.Operands.Pop();

            if (a != b)
            {
                SafeOperandsPush(1);
            }
            else
            {
                SafeOperandsPush(0);
            }
        }

        private void Op_Fadd()
        {
            float b = (float)_state.Operands.Pop();
            float a = (float)_state.Operands.Pop();

            SafeOperandsPush(a + b);
        }

        private void Op_Fsub()
        {
            float b = (float)_state.Operands.Pop();
            float a = (float)_state.Operands.Pop();

            SafeOperandsPush(a - b);
        }

        private void Op_Fmul()
        {
            float b = (float)_state.Operands.Pop();
            float a = (float)_state.Operands.Pop();

            SafeOperandsPush(a * b);
        }

        private void Op_Fdiv()
        {
            float b = (float)_state.Operands.Pop();
            float a = (float)_state.Operands.Pop();

            SafeOperandsPush(a / b);
        }

        private void Op_Fneg()
        {
            float a = (float)_state.Operands.Pop();

            SafeOperandsPush(-a);
        }

        private void Op_Flt()
        {
            float b = (float)_state.Operands.Pop();
            float a = (float)_state.Operands.Pop();

            if (a < b)
            {
                SafeOperandsPush(1);
            }
            else
            {
                SafeOperandsPush(0);
            }
        }

        private void Op_Fgt()
        {
            float b = (float)_state.Operands.Pop();
            float a = (float)_state.Operands.Pop();

            if (a > b)
            {
                SafeOperandsPush(1);
            }
            else
            {
                SafeOperandsPush(0);
            }
        }

        private void Op_Flte()
        {
            float b = (float)_state.Operands.Pop();
            float a = (float)_state.Operands.Pop();

            if (a <= b)
            {
                SafeOperandsPush(1);
            }
            else
            {
                SafeOperandsPush(0);
            }
        }

        private void Op_Fgte()
        {
            float b = (float)_state.Operands.Pop();
            float a = (float)_state.Operands.Pop();

            if (a >= b)
            {
                SafeOperandsPush(1);
            }
            else
            {
                SafeOperandsPush(0);
            }
        }

        private void Op_Feq()
        {
            float b = (float)_state.Operands.Pop();
            float a = (float)_state.Operands.Pop();

            if (a == b)
            {
                SafeOperandsPush(1);
            }
            else
            {
                SafeOperandsPush(0);
            }
        }

        private void Op_Fneq()
        {
            float b = (float)_state.Operands.Pop();
            float a = (float)_state.Operands.Pop();

            if (a != b)
            {
                SafeOperandsPush(1);
            }
            else
            {
                SafeOperandsPush(0);
            }
        }

        private void Op_Vadd()
        {
            Vector3 b = (Vector3)_state.Operands.Pop();
            Vector3 a = (Vector3)_state.Operands.Pop();

            SafeOperandsPush(a + b);
        }

        private void Op_Vsub()
        {
            Vector3 b = (Vector3)_state.Operands.Pop();
            Vector3 a = (Vector3)_state.Operands.Pop();

            SafeOperandsPush(a - b);
        }

        private void Op_Vmul()
        {
            Vector3 b = (Vector3)_state.Operands.Pop();
            Vector3 a = (Vector3)_state.Operands.Pop();

            SafeOperandsPush(Vector3.Dot(a, b));
        }

        private void Op_Vcross()
        {
            Vector3 b = (Vector3)_state.Operands.Pop();
            Vector3 a = (Vector3)_state.Operands.Pop();

            SafeOperandsPush(a % b);
        }

        private void Op_Veq()
        {
            Vector3 b = (Vector3)_state.Operands.Pop();
            Vector3 a = (Vector3)_state.Operands.Pop();

            if (a == b)
            {
                SafeOperandsPush(1);
            }
            else
            {
                SafeOperandsPush(0);
            }
        }

        private void Op_Vneq()
        {
            Vector3 b = (Vector3)_state.Operands.Pop();
            Vector3 a = (Vector3)_state.Operands.Pop();

            if (a != b)
            {
                SafeOperandsPush(1);
            }
            else
            {
                SafeOperandsPush(0);
            }
        }

        private void Op_Sconcat()
        {
            string b = (string)_state.Operands.Pop();
            string a = (string)_state.Operands.Pop();

            SafeOperandsPush(a + b);
        }

        private void Op_Seq()
        {
            string b = (string)_state.Operands.Pop();
            string a = (string)_state.Operands.Pop();

            if (a == b)
            {
                SafeOperandsPush(1);
            }
            else
            {
                SafeOperandsPush(0);
            }
        }

        private void Op_Sneq()
        {
            string b = (string)_state.Operands.Pop();
            string a = (string)_state.Operands.Pop();

            if (a != b)
            {
                SafeOperandsPush(1);
            }
            else
            {
                SafeOperandsPush(0);
            }
        }

        private void Op_Radd()
        {
            Quaternion b = (Quaternion)_state.Operands.Pop();
            Quaternion a = (Quaternion)_state.Operands.Pop();

            SafeOperandsPush(a + b);
        }

        private void Op_Rsub()
        {
            Quaternion b = (Quaternion)_state.Operands.Pop();
            Quaternion a = (Quaternion)_state.Operands.Pop();

            SafeOperandsPush(a - b);
        }

        private Quaternion _QuatMul(Quaternion b, Quaternion a)
        {
            return Quaternion.Negate(a * b);
        }

        private void Op_Rmul()
        {
            Quaternion b = (Quaternion)_state.Operands.Pop();
            Quaternion a = (Quaternion)_state.Operands.Pop();

            SafeOperandsPush(_QuatMul(a, b));
        }

        private void Op_Rdiv()
        {
            Quaternion b = (Quaternion)_state.Operands.Pop();
            Quaternion a = (Quaternion)_state.Operands.Pop();

            Quaternion binv = new Quaternion(b.X, b.Y, b.Z, -b.W);
            SafeOperandsPush(Quaternion.Negate(_QuatMul(a, binv)));
        }

        private void Op_Req()
        {
            Quaternion b = (Quaternion)_state.Operands.Pop();
            Quaternion a = (Quaternion)_state.Operands.Pop();

            if (a == b)
            {
                SafeOperandsPush(1);
            }
            else
            {
                SafeOperandsPush(0);
            }
        }

        private void Op_Rneq()
        {
            Quaternion b = (Quaternion)_state.Operands.Pop();
            Quaternion a = (Quaternion)_state.Operands.Pop();

            if (a != b)
            {
                SafeOperandsPush(1);
            }
            else
            {
                SafeOperandsPush(0);
            }
        }

        private Vector3 _VrMul(Vector3 a, Quaternion b)
        {
            Quaternion vq = new Quaternion(a.X, a.Y, a.Z, 0);
            Quaternion nq = new Quaternion(-b.X, -b.Y, -b.Z, b.W);

            Quaternion result = _QuatMul(nq, _QuatMul(vq, b));

            return new Vector3(result.X, result.Y, result.Z);
        }

        private void Op_Vrmul()
        {
            Quaternion b = (Quaternion)_state.Operands.Pop();
            Vector3 a = (Vector3)_state.Operands.Pop();

            SafeOperandsPush(_VrMul(a, b));
        }

        private void Op_Vimul()
        {
            int b = (int)_state.Operands.Pop();
            Vector3 a = (Vector3)_state.Operands.Pop();

            SafeOperandsPush(a * (float)b);
        }

        private void Op_Vfmul()
        {
            float b = (float)_state.Operands.Pop();
            Vector3 a = (Vector3)_state.Operands.Pop();

            SafeOperandsPush(a * b);
        }

        private void Op_Pop()
        {
            _state.Operands.Pop();
        }

        private void Op_ListPrepend()
        {
            LSLList b = (LSLList)_state.Operands.Pop();
            object a = (object)_state.Operands.Pop();

            SafeOperandsPush(b.Prepend(a));
        }

        private void Op_ListAppend()
        {
            object b = _state.Operands.Pop();
            LSLList a = (LSLList)_state.Operands.Pop();

            SafeOperandsPush(a.Append(b));
        }

        private void Op_Jmp()
        {
            int index = this.GetIntOperand();
            _state.IP = index;
        }

        private void _Call(int funcIndex)
        {
            FunctionInfo fi = (FunctionInfo)_script.ConstPool[funcIndex];
            StackFrame f = new StackFrame(fi, _state.IP);

            // push new stack frame for parameters and locals
            if (f == null)
                throw new VMException("Attempt to push null call frame.");

            _state.Calls.Push(f); 

            // move args from operand stack to top frame on call stack
            for (int a = fi.NumberOfArguments - 1; a >= 0; a--) 
            { 
                f.Locals[a] = _state.Operands.Pop(); 
            }

            //tell the memory tracker about this call
            _state.MemInfo.AddCall(f);

            _state.TopFrame = f;
            _state.IP = fi.Address; // branch to function
        }

        private void Op_Call()
        {
            int constIndex = this.GetIntOperand();
            this._Call(constIndex);
        }

        private void Op_Ret()
        {
            StackFrame frame = _state.Calls.Pop();

            if (_state.Calls.Count > 0)
            {
                _state.TopFrame = _state.Calls.Peek();
            }
            else
            {
                _state.TopFrame = null;
            }


            _state.IP = frame.ReturnAddress;

            _state.MemInfo.CompleteCall(frame);

            //return address is 0 indicates an event call
            if (frame.ReturnAddress == 0)
            {
                this.Op_Halt();
            }
        }

        private void Op_Syscall()
        {
            int syscallIndex = this.GetIntOperand();
            _syscallShim.Call(syscallIndex);
        }

        private void Op_Halt()
        {
            _state.RunState = RuntimeState.Status.Waiting;
        }

        private int _CastToInt(string s)
        {
            return Util.Encoding.CastToInt(s);
        }

        
        private float _CastToFloat(string s)
        {
            return Util.Encoding.CastToFloat(s);
        }

        private void Op_Icast()
        {
            object a = (object)_state.Operands.Pop();

            //the only valid types for an integer cast are
            //float, string, and integer
            if (a is string)
            {
                SafeOperandsPush(_CastToInt((string)a));
            }
            else if (a is float)
            {
                SafeOperandsPush((int)(float)a);
            }
            else if (a is int)
            {
                SafeOperandsPush((int)a);
            }
            else
            {
                throw new VMException("Invalid integer cast");
            }
        }

        private void Op_Fcast()
        {
            object a = (object)_state.Operands.Pop();

            //the only valid types for an float cast are
            //float, string, and integer
            if (a is string)
            {
                SafeOperandsPush(_CastToFloat((string)a));
            }
            else if (a is int)
            {
                SafeOperandsPush((float)(int)a);
            }
            else if (a is float)
            {
                SafeOperandsPush((float)a);
            }
            else
            {
                throw new VMException("Invalid floating point cast");
            }
        }

        private string _PrimitiveToString(object primitive)
        {
            //all types are valid for a string cast
            if (primitive is int)
            {
                return Convert.ToString((int)primitive);
            }
            else if (primitive is float)
            {
                return Util.Encoding.FloatToStringWith6FractionalDigits((float)primitive);
            }
            else if (primitive is Vector3)
            {
                Vector3 vPrimitive = (Vector3)primitive;
                return Util.Encoding.Vector3ToStringWith5FractionalDigits(vPrimitive);
            }
            else if (primitive is Quaternion)
            {
                Quaternion rPrimitive = (Quaternion)primitive;
                return Util.Encoding.QuaternionToStringWith5FractionalDigits(rPrimitive);
            }
            else if (primitive is string)
            {
                return (string)primitive;
            }
            else
            {
                throw new VMException("Invalid string cast");
            }
        }

        private string _LSLListToString(LSLList list)
        {
            StringBuilder contents = new StringBuilder();
            for (int index = 0; index < list.Data.Length; ++index)
            {
                contents.Append(list.GetLSLStringItem(index));
            }

            return contents.ToString();
        }

        private void Op_Scast()
        {
            object a = (object)_state.Operands.Pop();

            //all types are valid for a string cast
            if (a is int)
            {
                SafeOperandsPush(_PrimitiveToString(a));
            }
            else if (a is float)
            {
                SafeOperandsPush(_PrimitiveToString(a));
            }
            else if (a is Vector3)
            {
                SafeOperandsPush(_PrimitiveToString(a));
            }
            else if (a is Quaternion)
            {
                SafeOperandsPush(_PrimitiveToString(a));
            }
            else if (a is LSLList)
            {
                SafeOperandsPush(_LSLListToString((LSLList)a));
            }
            else if (a is string)
            {
                SafeOperandsPush((string)a);
            }
            else
            {
                throw new VMException("Invalid string cast");
            }
        }

        private void Op_Vcast()
        {
            object a = (object)_state.Operands.Pop();

            //only string and vector are valid for a vector cast
            if (a is string)
            {
                Vector3 ret;
                if (Vector3.TryParse((string)a, out ret))
                {
                    SafeOperandsPush(ret);
                }
                else
                {
                    SafeOperandsPush(Vector3.Zero);
                }
            }
            else if (a is Vector3)
            {
                SafeOperandsPush(a);
            }
            else
            {
                throw new VMException("Invalid vector cast");
            }
        }

        private void Op_Rcast()
        {
            object a = (object)_state.Operands.Pop();

            //only string and rotation are valid for a rotation cast
            if (a is string)
            {
                Quaternion ret;

                if (Quaternion.TryParse((string)a, out ret))
                {
                    SafeOperandsPush(ret);
                }
                else
                {
                    SafeOperandsPush(Quaternion.Identity);
                }
                
            }
            else if (a is Quaternion)
            {
                SafeOperandsPush(a);
            }
            else
            {
                throw new VMException("Invalid rotation cast");
            }
        }

        private void Op_Lcast()
        {
            object a = (object)_state.Operands.Pop();

            //anything can be casted to a list. This creates a new
            //list with a single element in it
            if (!(a is LSLList))
            {
                SafeOperandsPush(new LSLList(a));
            }
            else 
            {
                SafeOperandsPush(a);
            }
        }

        private void Op_BuildVec()
        {
            float z = (float)_state.Operands.Pop();
            float y = (float)_state.Operands.Pop();
            float x = (float)_state.Operands.Pop();

            SafeOperandsPush(new Vector3(x, y, z));
        }

        private void Op_BuildRot()
        {
            float w = (float)_state.Operands.Pop();
            float z = (float)_state.Operands.Pop();
            float y = (float)_state.Operands.Pop();
            float x = (float)_state.Operands.Pop();

            SafeOperandsPush(new Quaternion(x, y, z, w));
        }

        private void Op_BuildList()
        {
            int numMembers = this.GetIntOperand();

            object[] members = new object[numMembers];

            for (int i = numMembers - 1; i >= 0; --i)
            {
                members[i] = _state.Operands.Pop();
            }

            SafeOperandsPush(new LSLList(members));
        }

        private void Op_Trace()
        {
            object top = _state.Operands.Pop();

            _traceDestination.WriteLine(top.ToString());
        }

        private void Op_Brt()
        {
            int result = (int)_state.Operands.Pop();

            if (result != 0)
            {
                int branchAddress = this.GetIntOperand();
                _state.IP = branchAddress;
            }
            else
            {
                this.DiscardIntOperand();
            }
        }

        private void Op_Brf()
        {
            int result = (int)_state.Operands.Pop();

            if (result == 0)
            {
                int branchAddress = this.GetIntOperand();
                _state.IP = branchAddress;
            }
            else
            {
                this.DiscardIntOperand();
            }
        }

        private void Op_StateChg()
        {
            int stateId = this.GetIntOperand();
            this.OnStateChg(this, stateId);
            _syscallShim.OnStateChange();
        }

        private void Op_Vneg()
        {
            Vector3 v = (Vector3)_state.Operands.Pop();
            SafeOperandsPush(-v);
        }

        private void Op_Rneg()
        {
            Quaternion r = (Quaternion)_state.Operands.Pop();
            SafeOperandsPush(-r);
        }

        private void Op_Ibunot()
        {
            int i = (int)_state.Operands.Pop();
            SafeOperandsPush(~i);
        }

        private void Op_Vidiv()
        {
            int rhs = (int)_state.Operands.Pop();
            Vector3 lhs = (Vector3)_state.Operands.Pop();

            SafeOperandsPush(lhs / rhs);
        }

        private void Op_Vfdiv()
        {
            float rhs = (float)_state.Operands.Pop();
            Vector3 lhs = (Vector3)_state.Operands.Pop();

            SafeOperandsPush(lhs / rhs);
        }

        private void Op_Vrdiv()
        {
            Quaternion rhs = (Quaternion)_state.Operands.Pop();
            Vector3 lhs = (Vector3)_state.Operands.Pop();

            rhs.W = -rhs.W;

            SafeOperandsPush(_VrMul(lhs, rhs));
        }

        private void Op_Leq()
        {
            LSLList rhs = (LSLList)_state.Operands.Pop();
            LSLList lhs = (LSLList)_state.Operands.Pop();

            SafeOperandsPush(lhs.Members.Count == rhs.Members.Count ? 1 : 0);
        }

        private void Op_Iinit_g()
        {
            int gidx = this.GetIntOperand();
            
            int newVal = 0;
            _state.MemInfo.ReplaceStored(_state.Globals[gidx], newVal);

            _state.Globals[gidx] = newVal;
        }

        private void Op_Finit_g()
        {
            int gidx = this.GetIntOperand();

            float newVal = 0.0f;
            _state.MemInfo.ReplaceStored(_state.Globals[gidx], newVal);

            _state.Globals[gidx] = newVal;
        }

        private void Op_Vinit_g()
        {
            int gidx = this.GetIntOperand();

            Vector3 newVal = Vector3.Zero;
            _state.MemInfo.ReplaceStored(_state.Globals[gidx], newVal);

            _state.Globals[gidx] = newVal;
        }

        private void Op_Rinit_g()
        {
            int gidx = this.GetIntOperand();

            Quaternion newVal = Quaternion.Identity;
            _state.MemInfo.ReplaceStored(_state.Globals[gidx], newVal);

            _state.Globals[gidx] = newVal;
        }

        private void Op_Sinit_g()
        {
            int gidx = this.GetIntOperand();

            string newVal = String.Empty;
            _state.MemInfo.ReplaceStored(_state.Globals[gidx], newVal);

            _state.Globals[gidx] = newVal;
        }

        private void Op_Linit_g()
        {
            int gidx = this.GetIntOperand();

            LSLList newVal = new LSLList();
            _state.MemInfo.ReplaceStored(_state.Globals[gidx], newVal);

            _state.Globals[gidx] = newVal;
        }

        private void Op_Iinit_l()
        {
            int lidx = this.GetIntOperand();

            int newVal = 0;
            _state.MemInfo.ReplaceStored(_state.TopFrame.Locals[lidx], newVal);

            _state.TopFrame.Locals[lidx] = newVal;
        }

        private void Op_Finit_l()
        {
            int lidx = this.GetIntOperand();

            float newVal = 0.0f;
            _state.MemInfo.ReplaceStored(_state.TopFrame.Locals[lidx], newVal);

            _state.TopFrame.Locals[lidx] = newVal;
        }

        private void Op_Vinit_l()
        {
            int lidx = this.GetIntOperand();

            Vector3 newVal = Vector3.Zero;
            _state.MemInfo.ReplaceStored(_state.TopFrame.Locals[lidx], newVal);

            _state.TopFrame.Locals[lidx] = newVal;
        }

        private void Op_Rinit_l()
        {
            int lidx = this.GetIntOperand();

            Quaternion newVal = Quaternion.Identity;
            _state.MemInfo.ReplaceStored(_state.TopFrame.Locals[lidx], newVal);

            _state.TopFrame.Locals[lidx] = newVal;
        }

        private void Op_Sinit_l()
        {
            int lidx = this.GetIntOperand();

            string newVal = String.Empty;
            _state.MemInfo.ReplaceStored(_state.TopFrame.Locals[lidx], newVal);

            _state.TopFrame.Locals[lidx] = newVal;
        }

        private void Op_Linit_l()
        {
            int lidx = this.GetIntOperand();

            LSLList newVal = new LSLList();
            _state.MemInfo.ReplaceStored(_state.TopFrame.Locals[lidx], newVal);

            _state.TopFrame.Locals[lidx] = newVal;
        }

        private void Op_Lneq()
        {
            LSLList rhs = (LSLList)_state.Operands.Pop();
            LSLList lhs = (LSLList)_state.Operands.Pop();

            SafeOperandsPush(lhs.Members.Count == rhs.Members.Count ? 0 : 1);
        }

        private const string ZERO_GUID = "00000000-0000-0000-0000-000000000000";

        private void Op_Kinit_g()
        {
            int gidx = this.GetIntOperand();

            string newVal = ZERO_GUID;
            _state.MemInfo.ReplaceStored(_state.Globals[gidx], newVal);

            _state.Globals[gidx] = newVal;
        }

        private void Op_Kinit_l()
        {
            int lidx = this.GetIntOperand();

            string newVal = ZERO_GUID;
            _state.MemInfo.ReplaceStored(_state.TopFrame.Locals[lidx], newVal);

            _state.TopFrame.Locals[lidx] = newVal;
        }

        private void Op_Booleval()
        {
            object operand = _state.Operands.Pop();

            switch (RuntimeMirror.VarTypeFromRuntimeType(operand.GetType()))
            {
                case VarType.Integer:
                    SafeOperandsPush((((int)operand) != 0) ? 1 : 0);
                    break;

                case VarType.Float:
                    SafeOperandsPush((((float)operand) != 0.0f) ? 1 : 0);
                    break;

                case VarType.List:
                    SafeOperandsPush((((LSLList)operand).Members.Count != 0) ? 1 : 0);
                    break;

                case VarType.Rotation:
                    SafeOperandsPush((((Quaternion)operand) != Quaternion.Identity) ? 1 : 0);
                    break;

                case VarType.String:
                    string sOperand = (string)operand;
                    SafeOperandsPush((sOperand != String.Empty && sOperand != ZERO_GUID) ? 1 : 0);
                    break;

                case VarType.Vector:
                    SafeOperandsPush((((Vector3)operand) != Vector3.Zero) ? 1 : 0);
                    break;

                default:
                    throw new VMException(String.Format("VM was unable to perform boolean evaluation on the given operand '{0}'", operand));
            }
        }
    }
}
