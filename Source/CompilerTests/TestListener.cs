using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Halcyon.Phlox.Compiler;
using Halcyon.Phlox.Types;

namespace CompilerTests
{
    class TestListener : ILSLListener
    {
        int _errorCount = 0;
        StringBuilder _errorMessagesCollector = new StringBuilder();

        public int ErrorCount
        {
            get
            {
                return _errorCount;
            }
        }

        int _infoCount = 0;
        public int InfoCount
        {
            get
            {
                return _infoCount;
            }
        }

        private System.IO.StringWriter _traceDestination = new System.IO.StringWriter();
        public System.IO.StringWriter TraceDestination
        {
            get
            {
                return _traceDestination;
            }
        }

        #region ILSLListener Members

        public void Info(string message)
        {
            Console.WriteLine("info: " + message);
            _infoCount++;
        }

        public void Error(string message)
        {
            Console.Error.WriteLine("error: " + message);
            _errorMessagesCollector.Append(message);
            _errorCount++;
        }

        public bool HasErrors()
        {
            return _errorCount > 0 || _traceDestination.ToString() != String.Empty;
        }

        #endregion

        public void Reset()
        {
            _errorCount = 0;
            _infoCount = 0;
            _errorMessagesCollector.Length = 0;
        }

        public bool MessagesContain(string search)
        {
            return _errorMessagesCollector.ToString().Contains(search) ||
                _traceDestination.ToString().Contains(search);
        }

        public void CompilationFinished()
        {
        }
    }
}
