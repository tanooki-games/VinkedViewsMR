﻿namespace ETV
{
    public class PCP2DLineGenerator : APCPLineGenerator
    {
        public override AGraphicalPrimitiveFactory GetProperFactory()
        {
            return ServiceLocator.PrimitivePlant2D();
        }
    }
}
