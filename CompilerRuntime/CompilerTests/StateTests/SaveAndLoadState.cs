using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

using NUnit.Framework;
using Antlr.Runtime;

using MCompilerFrontend = InWorldz.Phlox.Glue.CompilerFrontend;
using InWorldz.Phlox.VM;
using InWorldz.Phlox.Types;
using InWorldz.Phlox.Serialization;
using InWorldz.Phlox.Util;

using OpenMetaverse;

namespace CompilerTests.SateTests
{
    [TestFixture]
    class SaveAndLoadState
    {
        [TestCase]
        public void TestLoadOldProtobuf1CompiledScript()
        {
            CompiledScript cs;
            using (var file = System.IO.File.OpenRead("StateTests\\v1script.plx"))
            {
                SerializedScript deser = ProtoBuf.Serializer.Deserialize<SerializedScript>(file);
                cs = deser.ToCompiledScript();
            }

            Assert.AreEqual(cs.AssetId, UUID.Parse("e6225f55-5051-4949-95a4-688e86dec5e5"));
        }

        [TestCase]
        public void TestLoadOldProtobuf1State()
        {
            RuntimeState deserRunState;

            using (var file = System.IO.File.OpenRead("StateTests\\phloxstate.plxs"))
            {
                SerializedRuntimeState deser = ProtoBuf.Serializer.Deserialize<SerializedRuntimeState>(file);
                deserRunState = deser.ToRuntimeState();
            }

            RuntimeState state = new RuntimeState(5);

            state.IP = 1;
            state.LSLState = 2;
            state.Operands = new Stack<object>();
            state.Operands.Push(new Vector3(1.0f, 2.0f, 3.0f));

            state.Globals[0] = 1;
            state.Globals[1] = 2.0f;
            state.Globals[2] = new Vector3(3.0f, 3.0f, 3.0f);
            state.Globals[3] = new LSLList(new object[] { "4.0", new Vector3(5.1f, 6.1f, 7.1f), new Quaternion(8.1f, 9.1f, 10.1f) });
            state.Globals[4] = new Quaternion(5.0f, 5.0f, 5.0f, 5.0f);

            state.Calls = new Stack<StackFrame>();
            state.Calls.Push(new StackFrame(new FunctionInfo { Address = 5, Name = "funk", NumberOfArguments = 0, NumberOfLocals = 0 }, 0));

            state.TopFrame = new StackFrame(new FunctionInfo { Address = 8, Name = "funk2", NumberOfArguments = 1, NumberOfLocals = 1 }, 0);
            state.TopFrame.Locals = new object[] { 2, 1.1f, new Vector3(1.0f, 2.0f, 3.0f) };

            state.Calls.Push(state.TopFrame);

            state.MemInfo = new MemoryInfo { MemoryUsed = 1000 };
            state.EventQueue = new C5.LinkedList<PostedEvent>();
            state.EventQueue.Push(new PostedEvent
            {
                Args = new object[] { 4 },
                DetectVars = new DetectVariables[1] { 
                    new DetectVariables { Grab = new Vector3(1,2,3), Group = "Group", Key = UUID.Zero.ToString(),
                        LinkNumber = 1, Name = "Name", Owner = "f1d932c0-7236-11e2-bcfd-0800200c9a66", Pos = new Vector3(4.5f, 5.6f, 6.7f),
                        Rot = new Quaternion(7.6f, 6.5f, 5.4f, 4.3f), TouchBinormal = new Vector3(3.2f, 2.1f, 1.0f), TouchFace = 8,
                        TouchNormal = new Vector3(9,4,3), TouchPos = new Vector3(7,6,5), TouchST = new Vector3(10.9f, 9.8f, 8.7f),
                        TouchUV = new Vector3(7.6f, 6.5f, 5.4f), Type = 0, Vel = new Vector3(999.8f, 888.7f, 777.6f) }
                },
                EventType = SupportedEventList.Events.ATTACH,
                TransitionToState = -1
            });

            state.RunState = RuntimeState.Status.Running;

            state.GeneralEnable = true;

            state.NextWakeup = Clock.GetLongTickCount();

            state.TimerInterval = 1000;
            state.ActiveListens = new Dictionary<int, ActiveListen>();
            state.ActiveListens.Add(2, new ActiveListen { Channel = 1, Handle = 2, Key = "", Message = "msg", Name = "blah" });

            Assert.AreEqual(deserRunState.IP, state.IP);
            Assert.AreEqual(deserRunState.LSLState, state.LSLState);
            Assert.AreEqual(deserRunState.Operands.Pop(), state.Operands.Pop());
            Assert.AreEqual(deserRunState.Globals, state.Globals);

            StackFrame origTopFrame = state.Calls.Pop();
            StackFrame deserFrame = deserRunState.Calls.Pop();

            Assert.AreEqual(origTopFrame.Locals, deserFrame.Locals);
            Assert.AreEqual(origTopFrame.ReturnAddress, deserFrame.ReturnAddress);
            Assert.AreEqual(origTopFrame.FunctionInfo.Address, deserFrame.FunctionInfo.Address);
            Assert.AreEqual(origTopFrame.FunctionInfo.Name, deserFrame.FunctionInfo.Name);
            Assert.AreEqual(origTopFrame.FunctionInfo.NumberOfArguments, deserFrame.FunctionInfo.NumberOfArguments);
            Assert.AreEqual(origTopFrame.FunctionInfo.NumberOfLocals, deserFrame.FunctionInfo.NumberOfLocals);

            Assert.AreEqual(state.MemInfo.MemoryUsed, deserRunState.MemInfo.MemoryUsed);

            PostedEvent origEvent = state.EventQueue.Pop();
            PostedEvent deserEvent = deserRunState.EventQueue.Pop();

            Assert.AreEqual(origEvent.Args, deserEvent.Args);
            Assert.AreEqual(origEvent.EventType, deserEvent.EventType);
            Assert.AreEqual(origEvent.TransitionToState, deserEvent.TransitionToState);

            CompareDetectVariables(origEvent.DetectVars, deserEvent.DetectVars);

            Assert.AreEqual(state.RunState, deserRunState.RunState);
            Assert.AreEqual(state.GeneralEnable, deserRunState.GeneralEnable);
            //Assert.AreEqual(state.NextWakeup, deserRunState.NextWakeup);
            Assert.AreEqual(state.TimerInterval, deserRunState.TimerInterval);

            origTopFrame = state.TopFrame;
            deserFrame = deserRunState.TopFrame;

            Assert.AreEqual(origTopFrame.Locals, deserFrame.Locals);
            Assert.AreEqual(origTopFrame.ReturnAddress, deserFrame.ReturnAddress);
            Assert.AreEqual(origTopFrame.FunctionInfo.Address, deserFrame.FunctionInfo.Address);
            Assert.AreEqual(origTopFrame.FunctionInfo.Name, deserFrame.FunctionInfo.Name);
            Assert.AreEqual(origTopFrame.FunctionInfo.NumberOfArguments, deserFrame.FunctionInfo.NumberOfArguments);
            Assert.AreEqual(origTopFrame.FunctionInfo.NumberOfLocals, deserFrame.FunctionInfo.NumberOfLocals);

            Assert.AreEqual(state.ActiveListens.ToArray(), deserRunState.ActiveListens.ToArray());
        }

        private void CompareDetectVariables(DetectVariables[] detectVariables1, DetectVariables[] detectVariables2)
        {
            Assert.AreEqual(detectVariables1.Length, detectVariables2.Length);

            int numElements = Math.Min(detectVariables1.Length, detectVariables2.Length);
            for (int i = 0; i < numElements; i++)
            {
                Assert.AreEqual(detectVariables1[i].Grab, detectVariables2[i].Grab);
                Assert.AreEqual(detectVariables1[i].Group, detectVariables2[i].Group);
                Assert.AreEqual(detectVariables1[i].Key, detectVariables2[i].Key);
                Assert.AreEqual(detectVariables1[i].LinkNumber, detectVariables2[i].LinkNumber);
                Assert.AreEqual(detectVariables1[i].Name, detectVariables2[i].Name);
                Assert.AreEqual(detectVariables1[i].Owner, detectVariables2[i].Owner);
                Assert.AreEqual(detectVariables1[i].Pos, detectVariables2[i].Pos);
                Assert.AreEqual(detectVariables1[i].Rot, detectVariables2[i].Rot);
                Assert.AreEqual(detectVariables1[i].TouchBinormal, detectVariables2[i].TouchBinormal);
                Assert.AreEqual(detectVariables1[i].TouchFace, detectVariables2[i].TouchFace);
                Assert.AreEqual(detectVariables1[i].TouchNormal, detectVariables2[i].TouchNormal);
                Assert.AreEqual(detectVariables1[i].TouchPos, detectVariables2[i].TouchPos);
                Assert.AreEqual(detectVariables1[i].TouchST, detectVariables2[i].TouchST);
                Assert.AreEqual(detectVariables1[i].TouchUV, detectVariables2[i].TouchUV);
                Assert.AreEqual(detectVariables1[i].Type, detectVariables2[i].Type);
                Assert.AreEqual(detectVariables1[i].Vel, detectVariables2[i].Vel);

            }
        }

        [TestCase]
        public void TestPlainSaveAndLoadState()
        {
            RuntimeState state = new RuntimeState(5);

            state.IP = 1;
            state.LSLState = 2;
            state.Operands = new Stack<object>();
            state.Operands.Push(new Vector3(1.0f, 2.0f, 3.0f));

            state.Globals[0] = 1;
            state.Globals[1] = 2.0f;
            state.Globals[2] = new Vector3(3.0f, 3.0f, 3.0f);
            state.Globals[3] = new LSLList(new object[] { "4.0", new Vector3(5.1f, 6.1f, 7.1f), new Quaternion(8.1f, 9.1f, 10.1f) });
            state.Globals[4] = new Quaternion(5.0f, 5.0f, 5.0f, 5.0f);

            state.Calls = new Stack<StackFrame>();
            state.Calls.Push(new StackFrame(new FunctionInfo { Address = 5, Name = "funk", NumberOfArguments = 0, NumberOfLocals = 0}, 0));

            state.TopFrame = state.Calls.Peek();

            state.MemInfo = new MemoryInfo { MemoryUsed = 1000 };
            state.EventQueue = new C5.LinkedList<PostedEvent>();
            state.EventQueue.Push(new PostedEvent { Args = new object[] {4}, DetectVars = new DetectVariables[0], 
                EventType = SupportedEventList.Events.ATTACH, TransitionToState = -1 });

            state.RunState = RuntimeState.Status.Running;

            state.GeneralEnable = true;

            state.NextWakeup = Clock.GetLongTickCount();

            state.TimerInterval = 1000;
            state.ActiveListens = new Dictionary<int, ActiveListen>();
            state.ActiveListens.Add(2, new ActiveListen { Channel = 1, Handle = 2, Key = "", Message = "msg", Name = "blah" });
            
            SerializedRuntimeState serRunState = SerializedRuntimeState.FromRuntimeState(state);

            MemoryStream memStream = new MemoryStream();
            ProtoBuf.Serializer.Serialize(memStream, serRunState);

            memStream.Seek(0, SeekOrigin.Begin);

            SerializedRuntimeState deser = ProtoBuf.Serializer.Deserialize<SerializedRuntimeState>(memStream);

            RuntimeState deserRunState = deser.ToRuntimeState();

            Assert.AreEqual(deserRunState.IP, state.IP);
            Assert.AreEqual(deserRunState.LSLState, state.LSLState);
            Assert.AreEqual(deserRunState.Operands.Pop(), state.Operands.Pop());
            Assert.AreEqual(deserRunState.Globals, state.Globals);

            StackFrame origTopFrame = state.Calls.Pop();
            StackFrame deserFrame = deserRunState.Calls.Pop();

            Assert.AreEqual(origTopFrame.Locals, deserFrame.Locals);
            Assert.AreEqual(origTopFrame.ReturnAddress, deserFrame.ReturnAddress);
            Assert.AreEqual(origTopFrame.FunctionInfo.Address, deserFrame.FunctionInfo.Address);
            Assert.AreEqual(origTopFrame.FunctionInfo.Name, deserFrame.FunctionInfo.Name);
            Assert.AreEqual(origTopFrame.FunctionInfo.NumberOfArguments, deserFrame.FunctionInfo.NumberOfArguments);
            Assert.AreEqual(origTopFrame.FunctionInfo.NumberOfLocals, deserFrame.FunctionInfo.NumberOfLocals);

            Assert.AreEqual(state.MemInfo.MemoryUsed, deserRunState.MemInfo.MemoryUsed);

            PostedEvent origEvent = state.EventQueue.Pop();
            PostedEvent deserEvent = deserRunState.EventQueue.Pop();

            Assert.AreEqual(origEvent.Args, deserEvent.Args);
            Assert.AreEqual(origEvent.EventType, deserEvent.EventType);
            Assert.AreEqual(origEvent.TransitionToState, deserEvent.TransitionToState);

            Assert.AreEqual(state.RunState, deserRunState.RunState);
            Assert.AreEqual(state.GeneralEnable, deserRunState.GeneralEnable);
            //Assert.AreEqual(state.NextWakeup, deserRunState.NextWakeup);
            Assert.AreEqual(state.TimerInterval, deserRunState.TimerInterval);

            origTopFrame = state.TopFrame;
            deserFrame = deserRunState.TopFrame;

            Assert.AreEqual(origTopFrame.Locals, deserFrame.Locals);
            Assert.AreEqual(origTopFrame.ReturnAddress, deserFrame.ReturnAddress);
            Assert.AreEqual(origTopFrame.FunctionInfo.Address, deserFrame.FunctionInfo.Address);
            Assert.AreEqual(origTopFrame.FunctionInfo.Name, deserFrame.FunctionInfo.Name);
            Assert.AreEqual(origTopFrame.FunctionInfo.NumberOfArguments, deserFrame.FunctionInfo.NumberOfArguments);
            Assert.AreEqual(origTopFrame.FunctionInfo.NumberOfLocals, deserFrame.FunctionInfo.NumberOfLocals);

            Assert.AreEqual(state.ActiveListens.ToArray(), deserRunState.ActiveListens.ToArray());
        }

        [TestCase]
        public void TestManyIterationsOfVectorSerialization()
        {
            Random rand = new Random();

            for (int i = 0; i < 1000; i++)
            {
                Vector3 vec = new Vector3((float)rand.NextDouble() * 0.000000000001f, (float)rand.NextDouble() * 10000000000000.0f, (float)rand.NextDouble() * 10000000000000.0f);
                SerializedLSLPrimitive primitive = new SerializedLSLPrimitive();
                primitive.Value = vec;

                Assert.IsTrue(primitive.IsValid());

                using (MemoryStream memStream = new MemoryStream())
                {
                    ProtoBuf.Serializer.Serialize(memStream, primitive);

                    memStream.Seek(0, SeekOrigin.Begin);

                    SerializedLSLPrimitive deser = ProtoBuf.Serializer.Deserialize<SerializedLSLPrimitive>(memStream);

                    Assert.IsTrue(deser.IsValid());
                }
            }
        }

        [TestCase]
        public void TestSerializePrimitiveToFile()
        {
            Random rand = new Random();

            SerializedStackFrame frame = new SerializedStackFrame();
            SerializedLSLPrimitive vec = new SerializedLSLPrimitive();
            Vector3 rvec = new Vector3((float)rand.NextDouble() * 0.000000000001f, (float)rand.NextDouble() * 10000000000000.0f, (float)rand.NextDouble() * 10000000000000.0f);
            vec.Value = rvec;

            frame.Locals = new SerializedLSLPrimitive[] { vec };
            frame.ReturnAddress = 999;
            frame.FunctionInfo = new FunctionInfo { Address = 9282, Name = "blam", NumberOfArguments = 1, NumberOfLocals = 0 };

            using (FileStream outStream = File.OpenWrite("primitive.txt"))
            {
                ProtoBuf.Serializer.Serialize(outStream, frame);
            }
            
        }

        [TestCase]
        public void TestPlainSaveAndLoadStateExtremes()
        {
            RuntimeState state = new RuntimeState(5);

            state.IP = 1;
            state.LSLState = 2;
            state.Operands = new Stack<object>();
            state.Operands.Push(new Vector3(float.PositiveInfinity, float.PositiveInfinity, float.PositiveInfinity));
            state.Operands.Push(new Vector3(float.MaxValue, float.MinValue, float.PositiveInfinity));

            state.Globals[0] = Int32.MaxValue;
            state.Globals[1] = 2.0f;
            state.Globals[2] = new Vector3(3.0f, 3.0f, 3.0f);
            state.Globals[3] = new LSLList(new object[] { "4.0", 2.5f, 5.5f});
            state.Globals[4] = new Quaternion(float.NegativeInfinity, 5.0f, 5.0f, 5.0f);

            state.Calls = new Stack<StackFrame>();
            state.Calls.Push(new StackFrame(new FunctionInfo { Address = 5, Name = "funk", NumberOfArguments = 0, NumberOfLocals = 0 }, 0));
            state.TopFrame = new StackFrame(new FunctionInfo { Address = 8, Name = "funk2", NumberOfArguments = 1, NumberOfLocals = 1 }, 0);
            state.TopFrame.Locals = new object[] { 2, 1.1f, new Vector3(1.0f, 2.0f, 3.0f) };
            state.Calls.Push(state.TopFrame);
            

            state.MiscAttributes[(int)RuntimeState.MiscAttr.Control] =  new object[] { 500, 1, 0 };

            state.MemInfo = new MemoryInfo { MemoryUsed = 1000 };
            state.EventQueue = new C5.LinkedList<PostedEvent>();
            state.EventQueue.Push(new PostedEvent
            {
                Args = new object[] { 4 },
                DetectVars = new DetectVariables[0],
                EventType = SupportedEventList.Events.ATTACH,
                TransitionToState = -1
            });

            state.RunState = RuntimeState.Status.Running;

            state.GeneralEnable = true;

            state.NextWakeup = Clock.GetLongTickCount();

            state.TimerInterval = 1000;

            state.ActiveListens = new Dictionary<int, ActiveListen>();
            state.ActiveListens.Add(2, new ActiveListen { Channel = 1, Handle = 2, Key = "", Message = "msg", Name = "blah" });

            SerializedRuntimeState serRunState = SerializedRuntimeState.FromRuntimeState(state);

            MemoryStream memStream = new MemoryStream();
            ProtoBuf.Serializer.Serialize(memStream, serRunState);

            memStream.Seek(0, SeekOrigin.Begin);

            SerializedRuntimeState deser = ProtoBuf.Serializer.Deserialize<SerializedRuntimeState>(memStream);

            RuntimeState deserRunState = deser.ToRuntimeState();

            Assert.AreEqual(deserRunState.IP, state.IP);
            Assert.AreEqual(deserRunState.LSLState, state.LSLState);

            Vector3 deserOp1 = (Vector3)deserRunState.Operands.Pop();
            Vector3 stateOp1 = (Vector3)state.Operands.Pop();

            Assert.AreEqual(deserOp1.X, stateOp1.X);
            Assert.AreEqual(deserOp1.Y, stateOp1.Y);
            Assert.That(float.IsInfinity(deserOp1.Z));
            Assert.That(float.IsInfinity(stateOp1.Z));
            Assert.AreEqual(deserRunState.Globals, state.Globals);

            StackFrame origTopFrame;
            StackFrame deserFrame;
            TestNextFrame(state, deserRunState, out origTopFrame, out deserFrame);
            TestNextFrame(state, deserRunState, out origTopFrame, out deserFrame);

            origTopFrame = state.TopFrame;
            deserFrame = deserRunState.TopFrame;

            Assert.AreEqual(origTopFrame.Locals, deserFrame.Locals);
            Assert.AreEqual(origTopFrame.ReturnAddress, deserFrame.ReturnAddress);
            Assert.AreEqual(origTopFrame.FunctionInfo.Address, deserFrame.FunctionInfo.Address);
            Assert.AreEqual(origTopFrame.FunctionInfo.Name, deserFrame.FunctionInfo.Name);
            Assert.AreEqual(origTopFrame.FunctionInfo.NumberOfArguments, deserFrame.FunctionInfo.NumberOfArguments);
            Assert.AreEqual(origTopFrame.FunctionInfo.NumberOfLocals, deserFrame.FunctionInfo.NumberOfLocals);

            Assert.AreEqual(state.ActiveListens.ToArray(), deserRunState.ActiveListens.ToArray());
            Assert.AreEqual(state.MiscAttributes, deserRunState.MiscAttributes);

        }

        private static void TestNextFrame(RuntimeState state, RuntimeState deserRunState, out StackFrame origTopFrame, out StackFrame deserFrame)
        {
            origTopFrame = state.Calls.Pop();
            deserFrame = deserRunState.Calls.Pop();

            Assert.AreEqual(origTopFrame.Locals, deserFrame.Locals);
            Assert.AreEqual(origTopFrame.ReturnAddress, deserFrame.ReturnAddress);
            Assert.AreEqual(origTopFrame.FunctionInfo.Address, deserFrame.FunctionInfo.Address);
            Assert.AreEqual(origTopFrame.FunctionInfo.Name, deserFrame.FunctionInfo.Name);
            Assert.AreEqual(origTopFrame.FunctionInfo.NumberOfArguments, deserFrame.FunctionInfo.NumberOfArguments);
            Assert.AreEqual(origTopFrame.FunctionInfo.NumberOfLocals, deserFrame.FunctionInfo.NumberOfLocals);

            Assert.AreEqual(state.MemInfo.MemoryUsed, deserRunState.MemInfo.MemoryUsed);

            if (state.EventQueue.Count > 0)
            {
                PostedEvent origEvent = state.EventQueue.Pop();
                PostedEvent deserEvent = deserRunState.EventQueue.Pop();

                Assert.AreEqual(origEvent.Args, deserEvent.Args);
                Assert.AreEqual(origEvent.EventType, deserEvent.EventType);
                Assert.AreEqual(origEvent.TransitionToState, deserEvent.TransitionToState);
            }

            Assert.AreEqual(state.RunState, deserRunState.RunState);
            Assert.AreEqual(state.GeneralEnable, deserRunState.GeneralEnable);
            Assert.AreEqual(state.TimerInterval, deserRunState.TimerInterval);
        }
    }
}
