osu! Beatmap API
===============

An API designed for quick and painless parsing of osu! beatmap files in .NET. This API exposes the 'Beatmap' class for .NET applications to use.
Currently supports reading, creating and saving osu! beatmap file formats up to v12.

What you need to be able to use it:
-----------------------------------
.NET Framework 3.5


Directions for use:
-------------------
To use this API you must add a reference to the DLL file in your project (Project -> Add Reference) and add 'using BMAPI' (or the equivalent for your language) to the top of your project.

If you are working with the source files instead (thus working in C#) and compiling the API along with your program, you do not need to add a reference to the DLL file, however the API files must be added to your project (Right click solution -> Add -> Existing Item).


Usage example:
--------------
In VB.NET:
```VB.NET
'Load the beatmap into a new object
Dim bm As New BMAPI.Beatmap("C:\osu!\Songs\39804 xi - FREEDOM DiVE\xi - FREEDOM DiVE (Nakagawa-Kanon) [FOUR DIMENSIONS].osu")
'Display the approach rate to the user
Console.WriteLine(bm.ApproachRate)
'Change the approach rate to a more 'appropriate' number
bm.ApproachRate = 10
'Let's also change the version to make the map distinguishable from the other maps in the mapset
bm.Version = "Best AR Version"
'Finally, save the beatmap!
bm.Save("C:\osu!\Songs\39804 xi - FREEDOM DiVE\xi - FREEDOM DiVE (Nakagawa-Kanon) [Best AR Version].osu")
```

In C#:
```C#
//Load the beatmap into a new object
BMAPI.Beatmap bm = new BMAPI.Beatmap(@"C:\osu!\Songs\39804 xi - FREEDOM DiVE\xi - FREEDOM DiVE (Nakagawa-Kanon) [FOUR DIMENSIONS].osu");
//Display the approach rate to the user
Console.Writeline(bm.ApproachRate);
//Change the approach rate to a more 'fitting' number
bm.ApproachRate = 10;
//Let's also change the version to make the map distinguishable from the other maps in the mapset
bm.Version = "Best AR Version";
//Finally, save the beatmap!
bm.Save(@"C:\osu!\Songs\39804 xi - FREEDOM DiVE\xi - FREEDOM DiVE (Nakagawa-Kanon) [Best AR Version].osu");
```	
	
License
-------
The MIT License (MIT)

Copyright (c) 2014 smoogipooo

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.