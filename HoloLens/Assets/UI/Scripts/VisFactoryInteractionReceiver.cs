﻿using HoloToolkit.Unity.InputModule;
using HoloToolkit.Unity.Receivers;
using System.Collections.Generic;
using UnityEngine;
using HoloToolkit.Unity.Collections;
using Model;
using System.Linq;
using static CubeIconVariable;
using HoloToolkit.Unity.Buttons;

/// <summary>
/// Handles interaction of the VisualizationFactory's buttons.
/// </summary>
public class VisFactoryInteractionReceiver : InteractionReceiver
{
    public GameObject HoloButtonPrefab;
    public GameObject screenAnchor;
    public GameObject newETVPlatform;
    public GameObject CubeIconVariablePrefab;
    public GameObject ObjectCollection;
    public GameObject VisTypeChoicePanel;

    public IDictionary<string, GameObject> currentlyChosenAttributes = new Dictionary<string, GameObject>();
    public int currentlyChosenDataBase = 0;
    public IList<GameObject> currentlyActiveVisChoicePanelButtons = new List<GameObject>();

    protected override void InputDown(GameObject obj, InputEventData eventData)
    {
        Debug.Log(obj.name + " : InputDown");

        switch(obj.name)
        {
            case "ButtonDataSets":
                DeactivateAllInteractibles();
                ActivateInteractables(new int[] { 1, 2, 4, 10 });
                break;
            case "Back":
                DeactivateAllInteractibles();
                ActivateInteractables(new int[] { 0, 10 });
                break;
            case "ButtonDataSet1":
                currentlyChosenDataBase = 0;
                DeactivateAllInteractibles();
                ActivateInteractables(new int[] { 5, 6, 7, 8, 9, 10 });
                break;
            case "ButtonDataSet2":
                currentlyChosenDataBase = 1;
                DeactivateAllInteractibles();
                ActivateInteractables(new int[] { 5, 6, 8, 9, 10 });
                break;
            case "ButtonDataSet3":
                currentlyChosenDataBase = 2;
                DeactivateAllInteractibles();
                ActivateInteractables(new int[] { 5, 6, 7, 8, 9, 10 });
                break;
            case "Button1D":
                SetupGalery(currentlyChosenDataBase, 1);
                break;
            case "Button2D":
                SetupGalery(currentlyChosenDataBase, 2);
                break;
            case "Button3D":
                SetupGalery(currentlyChosenDataBase, 3);
                break;
            case "ButtonND":
                SetupGalery(currentlyChosenDataBase, 4);
                break;
            case "Back2":
                DeactivateAllInteractibles();
                ActivateInteractables(new int[] { 1,2,3,4, 10 });
                ClearGallery();
                break;
            case "CubeIconVariable":
                HideAllIconSubButtons();
                if(obj.GetComponent<CubeIconVariable>() != null)
                {
                    var cubecon = obj.GetComponent<CubeIconVariable>();
                    cubecon.Select();

                    if(cubecon.selected)
                    {
                        currentlyChosenAttributes.Add(cubecon.varNames[0], obj);
                    }else
                    {
                        currentlyChosenAttributes.Remove(cubecon.varNames[0]);
                    }

                    ShowVisTypeButtons();
                }
                break;
            case "VisTypeButton":
                Services.VisFactory().GenerateVisFrom(
                    currentlyChosenDataBase,
                    currentlyChosenAttributes.Keys.ToArray(),
                    obj.GetComponent<VisTypeButton>().visType
                    );
                break;
            case "ButtonQuitApp":
                Debug.Log("Quitting Application");
                Application.Quit();
                break;
            default:
                break;
        }
    }

    /// <summary>
    /// Hides sub buttons of the shelf items.
    /// </summary>
    private void HideAllIconSubButtons()
    {
        VisTypeChoicePanel.SetActive(false);
    }

    /// <summary>
    /// Hides all shelf items except the given one.
    /// </summary>
    /// <param name="exception"></param>
    public void ClearAllBut(GameObject exception)
    {
        for(int i = 0; i < ObjectCollection.transform.childCount; i++)
        {
            var g = ObjectCollection.transform.GetChild(i);
            if(!g.gameObject.Equals(exception))
                Destroy(g.gameObject);
        }
    }

    /// <summary>
    /// Removes all shelf items from the gallery.
    /// </summary>
    private void ClearGallery()
    {
        for(int i = 0; i < ObjectCollection.transform.childCount; i++)
        {
            var g = ObjectCollection.transform.GetChild(i);
            interactables.Remove(g.gameObject);
            Destroy(g.gameObject);
        }
    }

    /// <summary>
    /// Deactivates all interactables of this receiver.
    /// </summary>
    private void DeactivateAllInteractibles()
    {
        foreach(GameObject g in interactables)
        {
            if(g != null) g.SetActive(false);
        }
    }

    /// <summary>
    /// Activates the interactables with the given IDs.
    /// </summary>
    /// <param name="IDs"></param>
    private void ActivateInteractables(int[] IDs)
    {
        foreach(var id in IDs)
        {
            interactables[id].SetActive(true);
        }
    }

    /// <summary>
    /// Assembles a variable galery from the DataBase of the given ID and provides all
    /// combinations of the given dimension.
    /// </summary>
    /// <param name="dataBaseID"></param>
    /// <param name="dimension"></param>
    private void SetupGalery(int dataBaseID, int dimension)
    {
        switch(dimension)
        {
            case 3: Setup3DGalery(dataBaseID); break;
            case 2: Setup2DGalery(dataBaseID); break;
            case 1: Setup1DGalery(dataBaseID); break;
            default: SetupNDGalery(dataBaseID); break;
        }

        ObjectCollection.GetComponent<ObjectCollection>().UpdateCollection();
    }

    /// <summary>
    /// Creates a galery of all attributes.
    /// </summary>
    /// <param name="dbID"></param>
    private void Setup1DGalery(int dbID)
    {
        DataSet ds = Services.DataBase().dataSets[dbID];

        var ms = new List<AttributeStats>();
        ms.AddRange(ds.nominalStatistics.Values);
        ms.AddRange(ds.ordinalStatistics.Values);
        ms.AddRange(ds.intervalStatistics.Values);
        ms.AddRange(ds.rationalStatistics.Values);
        

        foreach(AttributeStats m in ms)
        {
            interactables.Add(Create1DIconAndInsert(m.name, m.type));
        }
    }
    
    /// <summary>
    /// Generates a galery by combining all available attributes to 2D pairs.
    /// </summary>
    /// <param name="dbID"></param>
    private void Setup2DGalery(int dbID)
    {
        DataSet ds = Services.DataBase().dataSets[dbID];

        var ms = new List<AttributeStats>();
        ms.AddRange(ds.nominalStatistics.Values);
        ms.AddRange(ds.ordinalStatistics.Values);
        ms.AddRange(ds.intervalStatistics.Values);
        ms.AddRange(ds.rationalStatistics.Values);

        var keys = new List<IconKey2D>();
        string debug = "";

        foreach(var m1 in ms)
        {
            foreach(var m2 in ms)
            {
                var key = new IconKey2D(m1, m2);
                if(!m1.name.Equals(m2.name))
                {
                    keys.Add(key);
                    interactables.Add(Create2DIconAndInsert(m1.name, m2.name, m1.type, m2.type));
                    debug += m1.name + " x " + m2.name + "\n";
                }
            }
        }

        

        foreach(var m in ms)
        {
            
        }

        Debug.Log(debug);
    }

    /// <summary>
    /// Generates a galery by combining all available attributes to 3D groups.
    /// </summary>
    /// <param name="dbID"></param>
    private void Setup3DGalery(int dbID)
    {
        DataSet ds = Services.DataBase().dataSets[dbID];
        
        var ms = new List<AttributeStats>();
        ms.AddRange(ds.nominalStatistics.Values);
        ms.AddRange(ds.ordinalStatistics.Values);
        ms.AddRange(ds.intervalStatistics.Values);
        ms.AddRange(ds.rationalStatistics.Values);

        var keys = new List<IconKey>();


        foreach(var m1 in ms)
        {
            foreach(var m2 in ms)
            {
                foreach(var m3 in ms)
                {
                    var key = new IconKey(m1, m2, m3);
                    if(!keys.Contains(key) && !(m1.Equals(m2) || m2.Equals(m3) || m3.Equals(m1)))
                    {
                        keys.Add(key);
                        interactables.Add(Create3DIconAndInsert(m1.name, m2.name, m3.name, m1.type, m2.type, m3.type));
                    }
                }
            }
        }
    }

    /// <summary>
    /// Generates a galery by grouping attributes all together or by level of measurement.
    /// </summary>
    /// <param name="dbID"></param>
    private void SetupNDGalery(int dbID)
    {
        DataSet ds = Services.DataBase().dataSets[dbID];

        // 1 Icon for all variables
        var allVars = new List<string>();
        var allLoms = new List<LoM>();
        foreach(string key in ds.nominalStatistics.Keys)
        {
            allVars.Add(key);
            allLoms.Add(ds.nominalStatistics[key].type);
        }
        foreach(string key in ds.ordinalStatistics.Keys)
        {
            allVars.Add(key);
            allLoms.Add(ds.ordinalStatistics[key].type);
        }
        foreach(string key in ds.intervalStatistics.Keys)
        {
            allVars.Add(key);
            allLoms.Add(ds.intervalStatistics[key].type);
        }
        foreach(string key in ds.rationalStatistics.Keys)
        {
            allVars.Add(key);
            allLoms.Add(ds.rationalStatistics[key].type);
        }

        if(allVars.Count > 0)
            interactables.Add(CreateNDIconAndInsert(allVars.ToArray(), allLoms.ToArray(), "All Attr."));

        // 1 Icon for all nominal variables
        allVars.Clear();
        allLoms.Clear();

        foreach(string key in ds.nominalStatistics.Keys)
        {
            allVars.Add(key);
            allLoms.Add(ds.nominalStatistics[key].type);
        }

        if(allVars.Count > 0)
            interactables.Add(CreateNDIconAndInsert(allVars.ToArray(), allLoms.ToArray(), "All Nom."));

        // 1 Icon for all ordinal variables
        allVars.Clear();
        allLoms.Clear();

        foreach(string key in ds.ordinalStatistics.Keys)
        {
            allVars.Add(key);
            allLoms.Add(ds.ordinalStatistics[key].type);
        }

        if(allVars.Count > 0)
            interactables.Add(CreateNDIconAndInsert(allVars.ToArray(), allLoms.ToArray(), "All Ord."));

        // 1 Icon for all interval variables
        allVars.Clear();
        allLoms.Clear();

        foreach(string key in ds.intervalStatistics.Keys)
        {
            allVars.Add(key);
            allLoms.Add(ds.intervalStatistics[key].type);
        }

        if(allVars.Count > 0)
            interactables.Add(CreateNDIconAndInsert(allVars.ToArray(), allLoms.ToArray(), "All Intv."));

        // 1 Icon for all ratio variables
        allVars.Clear();
        allLoms.Clear();

        foreach(string key in ds.rationalStatistics.Keys)
        {
            allVars.Add(key);
            allLoms.Add(ds.rationalStatistics[key].type);
        }

        if(allVars.Count > 0)
            interactables.Add(CreateNDIconAndInsert(allVars.ToArray(), allLoms.ToArray(), "All Ratio"));

    }

    private GameObject Create1DIconAndInsert(string name, LoM lom)
    {
       return CreateIconAndInsertInCollection(new string[] { name }, new LoM[] { lom });
    }

    private GameObject Create2DIconAndInsert(string name1, string name2, LoM lom1, LoM lom2)
    {
        return CreateIconAndInsertInCollection(new string[] { name1, name2 }, new LoM[] { lom1, lom2 });
    }

    private GameObject Create3DIconAndInsert(string name1, string name2, string name3, LoM lom1, LoM lom2, LoM lom3)
    {
        return CreateIconAndInsertInCollection(new string[] { name1, name2, name3 }, new LoM[] { lom1, lom2, lom3 });
    }

    private GameObject CreateNDIconAndInsert(string[] names, LoM[] loms, string name)
    {
        return CreateMultiIconAndInsertInCollection(names, loms, name);
    }

    private GameObject CreateIconAndInsertInCollection(string[] names, LoM[] loms)
    {
        GameObject etvIcon = Instantiate(CubeIconVariablePrefab);
        CubeIconVariable civ = etvIcon.GetComponent<CubeIconVariable>();
        civ.Init(names, loms, currentlyChosenDataBase);
        etvIcon.transform.parent = ObjectCollection.transform;

        return etvIcon;
    }

    private GameObject CreateMultiIconAndInsertInCollection(string[] names, LoM[] loms, string name)
    {
        GameObject etvIcon = Instantiate(CubeIconVariablePrefab);
        CubeIconVariable civ = etvIcon.GetComponent<CubeIconVariable>();
        civ.InitMulti(names, loms, currentlyChosenDataBase, name);
        etvIcon.transform.parent = ObjectCollection.transform;

        return etvIcon;
    }

    /// <summary>
    /// Initializes the sub buttons to choose possible visualization type from.
    /// </summary>
    public void ShowVisTypeButtons()
    {
        foreach(var b in currentlyActiveVisChoicePanelButtons)
        {
            interactables.Remove(b);
            Destroy(b);
        }

        currentlyActiveVisChoicePanelButtons.Clear();

        if(currentlyChosenAttributes.Count == 0)
        {
            // Nothing selected, remove buttons, if there are any
            VisTypeChoicePanel.SetActive(false);

        } else
        {
            // Show matching buttons
            var suitableVisTypes = Services.VisFactory().ListPossibleVisualizations(currentlyChosenDataBase, currentlyChosenAttributes.Keys.ToArray());
            VisTypeChoicePanel.SetActive(true);
            
            for(int i = 0; i < suitableVisTypes.Length; i++)
            {
                var visType = suitableVisTypes[i];
                var button = Instantiate(HoloButtonPrefab);

                button.AddComponent<VisTypeButton>().visType = visType;

                button.name = visType.ToString();
                button.GetComponent<CompoundButtonText>().Text = visType.ToString();

                button.transform.parent = VisTypeChoicePanel.transform;
                button.transform.localPosition = new Vector3(.15f * i - suitableVisTypes.Length*.15f/2f, 0, 0);
                button.transform.localRotation = Quaternion.Euler(Vector3.zero);
                
                interactables.Add(button);
                currentlyActiveVisChoicePanelButtons.Add(button);
            }
        }
    }


    // ........................................................................ Inner Classes

    private class IconKey2D
    {
        AttributeStats[] ks;

        public IconKey2D(AttributeStats k1, AttributeStats k2)
        {
            ks = new AttributeStats[2];
            ks[0] = k1;
            ks[0] = k2;
        }
    }

    private class IconKey
    {
        AttributeStats[] ks;

        public IconKey(AttributeStats k1, AttributeStats k2, AttributeStats k3)
        {
            ks = new AttributeStats[3];
            ks[0] = k1;
            ks[0] = k2;
            ks[0] = k3;
        }
    }
}