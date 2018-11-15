﻿using System;
using System.Collections.Generic;
using ETV;
using GraphicalPrimitive;
using MetaVisualization;
using UnityEngine;

/// <summary>
/// Uncoupling update-class. Observed axes which span a meta-visualization
/// and tells it about changes in them. When one of the axes gets destroyed 
/// of conditions for the meta-visualization are not met anymore, it
/// destroys the meta-visualization and itself.
/// </summary>
public class MetaVisUpdater : MonoBehaviour, IAxisObserver, IDisposable
{
    private int dataSetID;
    private AETV metaVisualization;
    private AxisPair spanningAxes;
    private bool disposed = false;
    private bool initialized = false;
    private MetaVisType type;
    private IDictionary<AAxis, AAxis> shadowAxes = new Dictionary<AAxis, AAxis>();
    private ISet<IObserver<MetaVisUpdater>> observers = new HashSet<IObserver<MetaVisUpdater>>();

    public void Init(AETV etv, AxisPair axes, MetaVisType type, int dsID)
    {
        metaVisualization = etv;
        spanningAxes = axes;
        this.type = type;
        dataSetID = dsID;
        initialized = true;

        FindShadowAxes();
        Observe(axes.A);
        Observe(axes.B);
    }

    private void FindShadowAxes()
    {
        // FlexiblePCP does not contain shadow axes
        try
        {
            // for both axes of the pair, add their shadow-counterparts to the list
            foreach(var ax in new AAxis[] { spanningAxes.A, spanningAxes.B })
            {
                if(metaVisualization.registeredAxes.ContainsKey(ax.attributeName))
                {
                    shadowAxes.Add(ax, metaVisualization.registeredAxes[ax.attributeName]);
                }
            }

        } catch(Exception e)
        {
            Debug.LogException(e);
        }
    }

    private void TryHidingCloseAxes()
    {
        foreach(var originalAxis in new AAxis[] { spanningAxes.A, spanningAxes.B })
        {
            if(shadowAxes.ContainsKey(originalAxis))
            {
                var shadowAxis = shadowAxes[originalAxis];
                try
                {
                    var visible = !AMetaVisSystem.CheckIfNearEnoughToHideAxis(originalAxis, shadowAxis);
                    shadowAxis.SetVisibility(visible);

                    var distProjOnOrigBase = AMetaVisSystem.ProjectedDistanceToAxis(shadowAxis.GetAxisBaseGlobal(), originalAxis);
                    var distProjOnOrigTip = AMetaVisSystem.ProjectedDistanceToAxis(shadowAxis.GetAxisTipGlobal(), originalAxis);

                    // if one axis is parallel it's metavis axis and the other is not,
                    // stick to the parallel one
                    if(distProjOnOrigBase < .01f && distProjOnOrigTip < .01f)
                    {
                        metaVisualization.transform.position = originalAxis.GetAxisBaseGlobal();
                    }

                } catch(Exception e)
                {
                    Debug.LogError("Checking vicinity of original and meta axis failed, because of exception.");
                    Debug.LogException(e);
                }
            }
        }
    }
    

    public void Ignore(AAxis observable)
    {
        // Not neccessary, drestroys itself
        // when OnDispose() is called by observable.
    }

    public void Observe(AAxis observable)
    {
        observable.Subscribe(this);
    }

    public void OnChange(AAxis observable)
    {
        // Update meta-visualization, when spanning axes
        // change.
        if(!disposed && initialized)
        {
            // if meta-visualization conditions are not met anymore
            if(!AMetaVisSystem.CheckIfNearEnough(spanningAxes) || 
                !type.Equals(AMetaVisSystem.WhichMetaVis(spanningAxes, dataSetID)))
            {
                Dispose();
            } else
            {
                metaVisualization.UpdateETV();
                // Update meta-visualization
                TryHidingCloseAxes();
            }
        }
    }

    public void OnDispose(AAxis observable)
    {
        Dispose();
    }

    public void Dispose()
    {
        disposed = true;
        // Tell meta-visualization to destroy itself,
        // since AETV doesn't observe the spanning axes,
        // it doesn't know otherwise, when to destroy itself.
        metaVisualization.Dispose();

        // Tell MetaVisSystem to release combination
        // for new meta-visualizations
        Services.MetaVisSys().ReleaseCombination(spanningAxes);

        // Self destruction
        Destroy(gameObject);
    }
}