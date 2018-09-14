﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace GraphicalPrimitive
{
    public class AGraphicalPrimitive : MonoBehaviour
    {
        public GameObject pivot;
        public GameObject label;
        public GameObject visBridgePort;
        public GameObject[] visBridgePortPadding;

        private Color primitiveColor = Color.white;

        public void SetColor(Color color)
        {
            primitiveColor = color;
        }

        public void Brush(Color color)
        {
            ApplyColor(color);
        }

        public void Unbrush()
        {
            ApplyColor(primitiveColor);
        }

        public virtual void ApplyColor(Color color) { }
    }
}
