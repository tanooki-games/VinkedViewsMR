﻿using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;
using UnityEngine.Networking;

/// <summary>
/// Main class for visualization generation from databases.
/// </summary>
public class VisualizationFactory : NetworkBehaviour
{
    // ........................................................................ Fields to populate in editor

    // Prefabs
    public Transform NewETVPlaceHolder;
    public GameObject ObjectCollection;
    public GameObject CubeIconVariable;
    public GameObject NetworkAnchorPrefab;

    [SerializeField]
    public DataProvider dataProvider;
    public Material lineMaterial;
    public VisFactoryInteractionReceiver interactionReceiver;

    public AETVFactoryMethod[] factoryMethodHooks;

    public IDictionary<VisType, AETVFactoryMethod> generators;



    // ........................................................................ Private properties

    private IList<GameObject> activeVisualizations;
    


    // ........................................................................ MonoBehaviour methods

    void Awake()
    {
        activeVisualizations = new List<GameObject>();
        generators = new Dictionary<VisType, AETVFactoryMethod>();
        foreach(var method in factoryMethodHooks)
        {
            var key = VisType.SingleAxis3D;
            Enum.TryParse(method.gameObject.name, true, out key);
            generators.Add(key, method);
        }
    }

    // ........................................................................ Visualization generation
    // SingleAxis3D, BarChart2D, BarChart3D, BarMap3D, PCP2D, PCP3D, ScatterXY2D, ScatterXYZ3D, LineXY2D


    /// <summary>
    /// Generates visualizations from one attribute.
    /// </summary>
    /// <param name="dataSetID">data set to use</param>
    /// <param name="variable">attribute to visualize</param>
    /// <param name="visType">visualization type to generate</param>
    /// <returns></returns>
    public GameObject GenerateVisFrom(int dataSetID, string variable, VisType visType)
    {
        return GenerateVisFrom(dataSetID, new string[] { variable }, visType);
    }

    /// <summary>
    /// Generates visualizations from several attributes.
    /// </summary>
    /// <param name="dataSetID">data set to use</param>
    /// <param name="variable">attribute to visualize</param>
    /// <param name="visType">visualization type to generate</param>
    /// <returns></returns>
    public GameObject GenerateVisFrom(int dataSetID, string[] variables, VisType visType)
    {
        try
        {
            var vis = generators[visType].GenerateVisualization(dataSetID, variables);
            
            vis = Services.ETVFactory2D().PutETVOnAnchor(vis);
            vis.transform.position = NewETVPlaceHolder.position;
            AddNetworkAnchor(vis, dataSetID, variables, visType);


            GameManager.gameManager.PersistETV(vis, dataSetID, variables, visType);

            return vis;
        } catch(Exception e)
        {
            Debug.Log("Creation of requested Visualization failed.");
            Debug.LogError(e.Message);
            Debug.LogError(e.StackTrace);
            return new GameObject("Creation Failed");
        }
    }



    // ........................................................................ internal creation methods
    
        
        
    public void AddNetworkAnchor(GameObject etv, int dataSetID, string[] attributes, VisType visType)
    {
        if(isServer)
        {
            var networkAnchor = Instantiate(NetworkAnchorPrefab);
            if(networkAnchor.GetComponent<NetworkAnchor>() != null)
            {
                NetworkServer.Spawn(networkAnchor);
                networkAnchor.GetComponent<NetworkAnchor>().Init(dataSetID, attributes, visType);
                networkAnchor.GetComponent<NetworkAnchor>().ETV = etv;
            }
        }

    }




    // ........................................................................ Helper methods

    /// <summary>
    /// List which visualizations are suitable for the combination of provided attributes.
    /// </summary>
    /// <param name="dataSetID">ID of DataSet to use.</param>
    /// <param name="variables">Names of the attributes to visualize.</param>
    /// <returns>List of suitable visualizations.</returns>
    public VisType[] ListPossibleVisualizations(int dataSetID, string[] variables)
    {
        int[] nomIDs, ordIDs, ivlIDs, ratIDs;

        AttributeProcessor.ExtractAttributeIDs(dataProvider.dataSets[dataSetID], variables, out nomIDs, out ordIDs, out ivlIDs, out ratIDs);

        var suitableVisTypes = new List<VisType>();

        foreach(var visType in generators.Keys)
        {
            if(generators[visType].CheckIfSuitable(dataSetID, variables))
            {
                suitableVisTypes.Add(visType);
            }
        }

        if(suitableVisTypes.Count == 0)
            return new VisType[] { VisType.PCP2D };
        else
            return suitableVisTypes.ToArray();
    }

    /// <summary>
    /// Checks whether the given attributes of the given DataSet are suitable for
    /// the visualization type in question
    /// </summary>
    /// <param name="dataSetID">ID of the data set</param>
    /// <param name="attributes">attributes in question</param>
    /// <param name="visType">planned visualization type</param>
    /// <returns>if suitable</returns>
    public bool CheckIfSuitable(int dataSetID, string[] attributes, VisType visType)
    {
        return CheckIfSuitable(dataSetID, attributes, new VisType[] { visType });
    }
    public bool CheckIfSuitable(int dataSetID, string[] attributes, VisType[] visTypes)
    {
        // SingleAxis3D, BarChart2D, BarChart3D, BarMap3D, PCP2D, PCP3D, ScatterXY2D, ScatterXYZ3D, LineXY2D
        var suitables = ListPossibleVisualizations(dataSetID, attributes);

        bool suitable = true;

        foreach(var t in visTypes)
        {
            suitable &= suitables.Contains(t);
        }

        if(!suitable)
        {
            var atts = "";
            var vists = "";
            foreach(var att in attributes) atts += (att + ", ");
            foreach(var vist in visTypes) vists += (vist.ToString() + ", ");
            Debug.LogWarning("Visualization type(s) " + vists + " not suitable for attribute(s) " + atts);
        }

        return suitable;
    }

    public void AddNewVisualization(GameObject visualization)
    {
        if(visualization.GetComponent<ETVAnchor>() != null)
        {
            activeVisualizations.Add(visualization);
        } else
        {
            Debug.LogWarning("Given GameObject is not an anchored visualization!");
        }
    }

    
}


