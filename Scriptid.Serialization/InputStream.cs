using System;
using System.IO;

namespace Scriptid.Serialization
{
    internal class InputStream : IDisposable
    {
        private readonly StringReader _reader;

        public InputStream(string input)
        {
            _reader = new StringReader(input);
        }

        public int Position { get; private set; }

        public int Line { get; private set; }

        public int Column { get; private set; }

        public int Next()
        {
            Position++;
            var ch = _reader.Read();

            if (ch == '\n')
            {
                Line++;
                Column = 0;
            }
            else
            {
                Column++;
            }

            return ch;
        }

        public int Peek()
            => _reader.Peek();

        public bool Eof()
            => Peek() == -1;

        public void Croak(string msg)
            => throw new Exception($"{msg} (L{Line}:C{Column})");

        public void Dispose()
        {
            _reader?.Dispose();
        }
    }
}
