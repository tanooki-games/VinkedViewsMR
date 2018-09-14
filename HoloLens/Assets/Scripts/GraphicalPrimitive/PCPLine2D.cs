﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GraphicalPrimitive
{
    public class PCPLine2D : AGraphicalPrimitive
    {
        public LineRenderer lineRenderer;
        
        public override void ApplyColor(Color color)
        {
            lineRenderer.startColor = color;
            lineRenderer.endColor = color;
        }
    }
}