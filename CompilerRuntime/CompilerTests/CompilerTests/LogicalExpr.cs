using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using NUnit.Framework;
using Antlr.Runtime;

namespace CompilerTests.CompilerUnitTests
{
    class LogicalExpr : BaseTest
    {
        [Test]
        public void TestGoodLogicalExpr()
        {
            string test = @"
                        integer f() {
                            return 0;
                        }

                        default
                        {
                            touch_start(integer num) {
                                if (num) {
                                }
                                
                                while (num) {}
                                
                                integer i;
                                for (i = 0; i < 4; i++) {}
                                
                                do {} while(TRUE);

                                if (f() == -1) {}

                                vector v = <1<2,3,4>;
                            }
                        }";

            Compiler.Compile(new ANTLRStringStream(test));
            Assert.IsTrue(Listener.ErrorCount == 0);
        }

        [Test]
        public void TestGoodCastedLogicalExpr()
        {
            string test = @"
                        default
                        {
                            touch_start(integer num) {
                                integer i = !(integer)""1"";
                                integer j = !(integer)~1;
                            }
                        }";

            Compiler.Compile(new ANTLRStringStream(test));
            Assert.IsTrue(Listener.ErrorCount == 0);
        }

        [Test]
        public void AlphaVendBug01()
        {
            string test = @"
                        integer test() {
                            string input;
                            list cats = llCSV2List(input);
                            integer len = llGetListLength(cats);
                            if(llListFindList(cats, [""all""]) != -1) return TRUE;
                            else return FALSE;
                        }

                        default { state_entry() {} }";

            Compiler.Compile(new ANTLRStringStream(test));
            Assert.IsTrue(Listener.ErrorCount == 0);
        }

        [Test]
        public void TestConditionsWithExtraneousParens()
        {
            string test = @"
                        default
                        {
                            touch_start(integer num) {
                                if ((num)) {
                                }

                                while ((num != 5)) {
                                }
                            }
                        }";

            Compiler.Compile(new ANTLRStringStream(test));
            Assert.IsTrue(Listener.ErrorCount == 0);
        }

        [Test]
        public void TestLTGTStringCompareHasErrors()
        {
            string test = @"
                        default
                        {
                            touch_start(integer num) {
                                string a;
                                string b;
                                integer compare = ""a"" < ""b"";
                            }
                        }";

            Compiler.Compile(new ANTLRStringStream(test));
            Assert.IsTrue(Listener.ErrorCount == 1);
        }
    }
}
