﻿/*
Copyright 2019 Georg Eckert (MIT License)

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to
deal in the Software without restriction, including without limitation the
rights to use, copy, modify, merge, publish, distribute, sublicense, and/or
sell copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in
all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING
FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS
IN THE SOFTWARE.
*/
using ETV;
using GraphicalPrimitive;
using Model;
using UnityEngine;

namespace MetaVisualization
{
    public abstract class AMetaVisFactory : MonoBehaviour
    {
        /// <summary>
        /// Generates a flexible parallel coordinates plot, which connects
        /// both given axes and adapts to them automatically.
        /// </summary>
        /// <param name="data">data set to use</param>
        /// <param name="attIDs">attribute ids</param>
        /// <param name="axisA">first axis to connect</param>
        /// <param name="axisB">second axis to connect</param>
        /// <returns></returns>
        public abstract AETV CreateFlexiblePCP(DataSet data, string[] attIDs, AAxis axisA, AAxis axisB);

        public abstract AETV CreateMetaScatterplot2D(DataSet data, string[] attIDs);

        public abstract AETV CreateMetaHeatmap3D(DataSet data, string[] attIDs, bool manualLength = false, float lengthA = 1f, float lengthB = 1f);

        public abstract AETV CreateMetaFlexibleLinedAxes(DataSet data, string[] attIDs, AAxis axisA, AAxis axisB);
    }
}
