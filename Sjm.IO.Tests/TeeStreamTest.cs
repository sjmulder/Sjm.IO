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

using System.IO;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Sjm.IO.Tests
{
    [TestClass]
    public class TeeStreamTest
    {
        [TestMethod]
        public void Read_NoTap()
        {
            byte[] data = Encoding.ASCII.GetBytes("Hello, World!");

            var tee = new TeeStream(new MemoryStream(data));
            var sink = new MemoryStream();
            tee.CopyTo(sink);

            CollectionAssert.AreEqual(data, sink.ToArray());
        }

        [TestMethod]
        public void Read_Tap()
        {
            byte[] data = Encoding.ASCII.GetBytes("Hello, World!");

            var tap = new MemoryStream();
            var tee = new TeeStream(new MemoryStream(data), readTap: tap);
            var sink = new MemoryStream();
            tee.CopyTo(sink);

            CollectionAssert.AreEqual(data, sink.ToArray());
            CollectionAssert.AreEqual(data, tap.ToArray());
        }

        [TestMethod]
        public void Read_Partial()
        {
            byte[] data = Encoding.ASCII.GetBytes("Hello, World!");

            var tap = new MemoryStream();
            var tee = new TeeStream(new MemoryStream(data), readTap: tap);

            byte[] buf = new byte[100];
            int nread = tee.Read(buf, 0, 100);

            Assert.AreEqual(data.Length, nread);
            CollectionAssert.AreEqual(data, tap.ToArray());
        }

        [TestMethod]
        public void Read_Offset()
        {
            byte[] data = Encoding.ASCII.GetBytes("Hello, World!");

            var tap = new MemoryStream();
            var tee = new TeeStream(new MemoryStream(data), readTap: tap);

            byte[] buf = new byte[50 + data.Length];
            int nread = tee.Read(buf, 50, data.Length);

            Assert.AreEqual(data.Length, nread);
            CollectionAssert.AreEqual(data, tap.ToArray());
        }

        [TestMethod]
        public void Write_NoTap()
        {
            byte[] data = Encoding.ASCII.GetBytes("Hello, World!");

            var sink = new MemoryStream();
            var tee = new TeeStream(sink);
            tee.Write(data, 0, data.Length);

            CollectionAssert.AreEqual(data, sink.ToArray());
        }

        [TestMethod]
        public void Write_Tap()
        {
            byte[] data = Encoding.ASCII.GetBytes("Hello, World!");

            var sink = new MemoryStream();
            var tap = new MemoryStream();
            var tee = new TeeStream(sink, writeTap: tap);
            tee.Write(data, 0, data.Length);

            CollectionAssert.AreEqual(data, sink.ToArray());
            CollectionAssert.AreEqual(data, tap.ToArray());
        }

        [TestMethod]
        public void Write_Offset()
        {
            byte[] data = Encoding.ASCII.GetBytes("Hello, World!");
            byte[] buf;

            using (var mem = new MemoryStream())
            {
                mem.Write(new byte[50]);
                mem.Write(data);
                buf = mem.ToArray();
            }

            var sink = new MemoryStream();
            var tap = new MemoryStream();
            var tee = new TeeStream(sink, writeTap: tap);
            tee.Write(buf, 50, data.Length);

            CollectionAssert.AreEqual(data, sink.ToArray());
            CollectionAssert.AreEqual(data, tap.ToArray());
        }
    }
}
