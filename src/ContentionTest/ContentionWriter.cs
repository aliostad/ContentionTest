using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ContentionTest
{
    public class ContentionWriter : IDisposable
    {

        private string _fileName = string.Empty;
        private int _howMuch;
        private Stream _stream;
        private Stopwatch _stopwatch = new Stopwatch();
   

        public ContentionWriter(int howMuch)
        {
            _howMuch = howMuch;
        }

        public Stopwatch Stopwatch
        {
            get { return _stopwatch; }
            set { _stopwatch = value; }
        }

        public Task WriteAsync(byte[] buffer)
        {
            var random = new Random();
            _fileName = GetFileName();
            _stream = new FileStream(_fileName, FileMode.Create);
            _stopwatch.Start();
            return Task.Factory.FromAsync(_stream.BeginWrite, _stream.EndWrite, buffer, random.Next(_howMuch), _howMuch, null);
        }

        public void Write(byte[] buffer)
        {
            var random = new Random();
            _fileName = GetFileName();
            _stream = new FileStream(_fileName, FileMode.Create);
            _stream.Write(buffer, random.Next(_howMuch), _howMuch);
        }

        private string GetFileName()
        {
            return "c:\\temp\\" + Guid.NewGuid().ToString("N");
        }

        public void Dispose()
        {
            _stream.Dispose();
            if(!string.IsNullOrEmpty(_fileName))
                File.Delete(_fileName);

        }
    }
}
