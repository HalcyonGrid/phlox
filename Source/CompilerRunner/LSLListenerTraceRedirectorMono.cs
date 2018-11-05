using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Halcyon.Phlox.Types;

/// <summary>
/// Redirects all trace text from the parsers to an LSLListener
/// </summary>
public class LSLListenerTraceRedirectorMono : TextWriter
{
    private ILSLListener _listener;

    StringBuilder _lineSoFar = new StringBuilder();

    public override Encoding Encoding
    {
        get { return Encoding.UTF8; }
    }


    public LSLListenerTraceRedirectorMono(ILSLListener listener)
    {
        _listener = listener;
    }

    public override void Write(char value)
    {
        if (value == CoreNewLine[CoreNewLine.Length - 1])
        {
            if (_lineSoFar.Length > 0)
            {
                _lineSoFar.Append(value);
                _listener.Error(_lineSoFar.ToString());
                _lineSoFar.Length = 0;
            }
        }
        else
        {
            _lineSoFar.Append(value);
        }
    }

    public override void Write(string value)
    {
        if (_lineSoFar.Length > 0)
        {
            _listener.Error(_lineSoFar.ToString() + value);
            _lineSoFar.Length = 0;
        }
        else
        {
            _listener.Error(value);
        }
    }
}