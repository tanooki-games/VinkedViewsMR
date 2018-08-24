﻿using GraphicalPrimitive;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Graphical3DPrimitiveFactory : AGraphicalPrimitiveFactory
{
    public GameObject cone;
    public GameObject bar3D;
    public GameObject axis3D;

    public override GameObject CreateAxis(Color color, string variableName, string variableEntity, Vector3 direction, float length, bool tipped = true, float width = 0.01F)
    {
        GameObject axis;

        axis = Instantiate(axis3D);
        axis.GetComponent<Axis>().SetTipped(tipped);
        axis.GetComponent<Axis>().SetSize(width, length);
        axis.GetComponent<Axis>().ChangeColor(color);

        direction.Normalize();
        Quaternion rotateTo = Quaternion.FromToRotation(Vector3.up, direction);
        axis.transform.localRotation = rotateTo;

        return axis;
    }

    public override GameObject CreateGrid(Color color, Vector3 rowDir, Vector3 colDir, bool rows = true, int rowCount = 10, float rowResolution = 0.1F, float xAxisLength = 1, bool cols = true, int colCount = 10, float colResolution = 0.1F, float yAxisLength = 1, float width = 0.005F)
    {
        throw new System.NotImplementedException();
    }

    public override GameObject CreateBar(float value, float rangeToNormalizeTo, float width, float depth)
    {
        GameObject bar = Instantiate(bar3D);
        bar.GetComponent<Bar3D>().SetSize(width, value / rangeToNormalizeTo, depth);
        
        return bar;
    }
}