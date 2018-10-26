﻿using UnityEngine;

namespace GraphicalPrimitive
{
    public class PCPLine2D : APCPLine
    {
        public override LineRenderer GetNewProperLineRenderer()
        {
            return ServiceLocator.PrimitivePlant2D().GetNew2DLineRenderer();
        }
    }
}