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
using UnityEngine;

public class ETV2DVirtualDevice : MonoBehaviour {

    public GameObject visualization;
    public GameObject VisualizationAnchor;
    public GameObject DeviceAnchor;

    public GameObject borderT, borderB, borderL, borderR, cornerBL, cornerBR, cornerTL, cornerTR, screen;

    public void BindVisualization(GameObject vis)
    {
        SetSize(1,1);
        vis.transform.parent = gameObject.transform;
        DeviceAnchor.transform.localPosition = new Vector3(-.3f, -.3f, .01f);
    }

    public void SetSize(float width, float height)
    {
        cornerBL.transform.localPosition = new Vector3(0.02f, 0.02f, 0);
        cornerBR.transform.localPosition = new Vector3(width - 0.02f, 0.02f, 0);
        cornerTL.transform.localPosition = new Vector3(0.02f, height - 0.02f, 0);
        cornerTR.transform.localPosition = new Vector3(width - 0.02f, height - 0.02f, 0);

        borderT.transform.localPosition = new Vector3(0.5f * width, height-0.01f);
        borderB.transform.localPosition = new Vector3(0.5f * width, 0.01f);
        borderL.transform.localPosition = new Vector3(0.01f, 0.5f * height);
        borderR.transform.localPosition = new Vector3(width-0.01f, 0.5f * height);

        borderT.transform.localScale = new Vector3(width-0.04f, 0.02f, 0.01f);
        borderB.transform.localScale = new Vector3(width-0.04f, 0.02f, 0.01f);
        borderL.transform.localScale = new Vector3(0.02f, height - 0.04f, 0.01f);
        borderR.transform.localScale = new Vector3(0.02f, height - 0.04f, 0.01f);

        screen.transform.localPosition = new Vector3(0.5f*width, 0.5f*height);
        screen.transform.localScale = new Vector3(width-0.04f, height-0.04f, 0.0025f);
    }

	// Use this for initialization
	void Start () {
        
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
