Sjm.IO
======

A .NET IO helper library.


TeeStream
---------

A stream that proxies another stream and facilitates tapping reads and
writes.

For example, to tap a response stream, conveniently ignoring that ASP.NET
response streams cannot be assigned like this:

    var log = File.OpenWrite("Request.txt");
    var tee = new TeeStream(response.Stream, writeTap: log);
    response.Stream = tee;


Legal
-----

Copyright (c) 2018, Sijmen J. Mulder (<ik@sjmulder.nl>)

Sjm.IO is free software: you can redistribute it and/or modify
it under the terms of the GNU Lesser General Public License as published by
the Free Software Foundation, either version 3 of the License, or
(at your option) any later version.

Sjm.IO is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU General Public License for more details.

You should have received a copy of the GNU Lesser General Public License
along with Sjm.IO.  If not, see <https://www.gnu.org/licenses/>.