using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public enum MyLayers
{
    Default = 0,
    Obstacles = 8,
    Target = 9,
    ConeOfSight=12,
    Ground=13,
    
}



public class MaterialController : MonoBehaviour
{

    public bool sightHidden = false;
    private bool lastUpdateHiddenState;
    bool isDone = false;

    private MyLayers[] hideableLayers = {MyLayers.Obstacles, MyLayers.Target, MyLayers.Ground};
    private MyLayers[] maskedLayers = {MyLayers.ConeOfSight};
    public List<GameObject> objectsThatCanBeHidden;
    public List<GameObject> maskingObjects;


    // Declaration
    Shader stencilObjectShader;
    Shader stencilMaskShader;
    Shader standardShader;
    
    

    // Start is called before the first frame update
    void Start()
    {
        GetShaders();
        GetGameobjects();
    }

    void GetShaders()
    {
        stencilMaskShader = Shader.Find("Custom/Stencil Mask");
        stencilObjectShader= Shader.Find("Custom/Stencil Object");
        standardShader = Shader.Find("Standard");
    }

    public void GetGameobjects()
    {
        objectsThatCanBeHidden = new List<GameObject>();
        foreach (MyLayers l in hideableLayers)
        {
            List<GameObject> hideableObj = FindGameObjectsInLayer((int) l);
            if (hideableObj!=null)
                objectsThatCanBeHidden.AddRange(hideableObj);
        }
        maskingObjects = new List<GameObject>();
        foreach (MyLayers l in maskedLayers)
        {
            List<GameObject> maskableObj = FindGameObjectsInLayer((int) l);
            if (maskableObj!=null)
                maskingObjects.AddRange(maskableObj);
        }
    }


    List <GameObject> FindGameObjectsInLayer(int layer)
    {
        var goArray = FindObjectsOfType(typeof(GameObject)) as GameObject[];
        var goList = new List<GameObject>();
        for (int i = 0; i < goArray.Length; i++)
        {
            if (goArray[i].layer == layer)
            {
                goList.Add(goArray[i]);
            }
        }
        if (goList.Count == 0)
        {
            return null;
        }
        return goList;
    }
    
    

    // Update is called once per frame
    void Update()
    {
//        if (Input.GetKeyDown("space"))
//        {
//            print("space key was pressed");
//            sightHidden= sightHidden ? false : true;;
//        }
//            
        
        if (lastUpdateHiddenState != sightHidden)
            isDone = false;
            
            
        if (!isDone)
        {
            if (sightHidden)
                ApplyHideMaterials();
            else
                ApplyShowMaterials();

        }

        lastUpdateHiddenState = sightHidden;
        

    }

    void ApplyHideMaterials()
    {
        Debug.Log("Hide Materials");
        
        objectsThatCanBeHidden.RemoveAll(item => item == null);
        maskingObjects.RemoveAll(item => item == null);
        foreach (var go in objectsThatCanBeHidden)
        {
            go.GetComponent<MeshRenderer>().material.shader = stencilObjectShader;
        }
        foreach (var go in maskingObjects)
        {
            go.GetComponent<MeshRenderer>().material.shader = stencilMaskShader;
        }
        isDone = true;
    }

    public void ApplyShowMaterials()
    {
//        Debug.Log("Show Materials");
        objectsThatCanBeHidden.RemoveAll(item => item == null);
        maskingObjects.RemoveAll(item => item == null);
        foreach (var go in objectsThatCanBeHidden)
        {
            go.GetComponent<MeshRenderer>().material.shader = standardShader;
        }
        foreach (var go in maskingObjects)
        {
            go.GetComponent<MeshRenderer>().material.shader = standardShader;
        }
        isDone = true;
    }
}
