// Copyright (c) 2018, Sijmen J. Mulder <ik@sjmulder.nl>
//
// This file is part of Sjm.IO.
//
// Sjm.IO is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
//
// Sjm.IO is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU Lesser General Public License
// along with this program.  If not, see <https://www.gnu.org/licenses/>.

using System;
using System.IO;

namespace Sjm.IO
{
    /// <summary>
    /// A stream that proxies another stream and facilitates tapping reads and
    /// writes.
    /// </summary>
    /// <example>
    /// Tap a response stream, conveniently ignoring that ASP.NET response
    /// streams cannot be assigned like this:
    /// <code>
    /// var log = File.OpenWrite("Request.txt");
    /// var tee = new TeeStream(response.Stream, writeTap: log);
    /// response.Stream = tee;
    /// </code>
    /// </example>
    public class TeeStream : Stream
    {
        Stream _source;
        Stream _readTap;
        Stream _writeTap;

        /// <summary>
        /// Construct a new TeeStream proxying <paramref name="source"/>.
        /// All data read from <paramref name="source"/> is also written to
        /// <paramref name="readTap"/>, if set, and all data written to
        /// <paramref name="source"/> is also written to
        /// <paramref name="writeTap" />, if set.
        /// </summary>
        public TeeStream(Stream source, Stream readTap = null,
            Stream writeTap = null)
        {
            _source = source;
            _readTap = readTap;
            _writeTap = writeTap;
        }

        public override bool CanRead => _source.CanRead;
        public override bool CanSeek => false;
        public override bool CanWrite => _source.CanWrite;
        public override long Length => _source.Length;

        public override long Position
        {
            get => _source.Position;
            set => throw new InvalidOperationException(nameof(TeeStream) +
                " is not seekable");
        }

        public override void Flush()
        {
            _source.Flush();
            _writeTap?.Flush();
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            int nread = _source.Read(buffer, offset, count);
            _readTap?.Write(buffer, offset, nread);
            return nread;
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            throw new InvalidOperationException(nameof(TeeStream) +
                " is not seekable");
        }

        public override void SetLength(long value)
        {
            throw new InvalidOperationException("Cannot set length on " +
                nameof(TeeStream));
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            _source.Write(buffer, offset, count);
            _writeTap?.Write(buffer, offset, count);
        }
    }
}
