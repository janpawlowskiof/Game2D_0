using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class IntegerAssertion : EditorWindow
{
    private bool autoIntegerCorrection = false;
    private bool autoMatchAABBToScale = false;
    private bool drawPlatformStops = false;
    
    [MenuItem("Window/Integer Assertion")]
    static void CreateWindow()
    {
        EditorWindow.GetWindow<IntegerAssertion>();
    }

    private void OnGUI()
    {
        autoIntegerCorrection = EditorGUILayout.Toggle("Auto Integer Correction (Slow)", autoIntegerCorrection);
        autoMatchAABBToScale = EditorGUILayout.Toggle("Auto Match AABB To Scale (Slow)", autoMatchAABBToScale);
        drawPlatformStops = EditorGUILayout.Toggle("Draw platform stops", drawPlatformStops);
        
        if (GUILayout.Button("Run Integer Correction"))
        {
            IntegerCorrection();
        }
        
        if (GUILayout.Button("Set References To Player on Pickups"))
        {
            AssignPlayerReferencesOnPickups();
        }
        
        if (GUILayout.Button("scale = AABB"))
        {
            MatchScaleToAABB();
        }
        
        if (GUILayout.Button("AABB = scale"))
        {
            MatchAABBToScale();
        }
        
        if (GUILayout.Button("Select AABBs"))
        {
            SelectAABBS();
        }
    }

    void SelectAABBS()
    {
        Selection.objects = FindObjectsOfType<AABB>().Select(x => x.gameObject).ToArray();
    }
    
    void MatchScaleToAABB()
    {
        foreach (var gameObject in Selection.gameObjects)
        {
            var aabb = gameObject.GetComponent<AABB>();
            if (aabb)
            {
                gameObject.transform.localScale = new Vector3(aabb.W, aabb.H, gameObject.transform.localScale.z);
            }
        }
    }
    
    void MatchAABBToScale()
    {
        foreach (var gameObject in Selection.gameObjects)
        {
            var aabb = gameObject.GetComponent<AABB>();
            if (aabb)
            {
                aabb.halfExtentsInt.x = Mathf.FloorToInt(gameObject.transform.localScale.x / 2);
                aabb.halfExtentsInt.y = Mathf.FloorToInt(gameObject.transform.localScale.y / 2);
            }
        }
        
        MatchScaleToAABB();
    }

    void IntegerCorrection()
    {
        var aabbs = FindObjectsOfType<AABB>();

        foreach (var aabb in aabbs)
        {
            var current = aabb.transform;
            while (current != null)
            {
                var position1 = current.transform.position;
                position1 = new Vector3(Mathf.Floor(position1.x),
                    Mathf.Floor(position1.y), 0);
                current.transform.position = position1;
                
                current = current.parent;
            }
        }
    }

    void DrawPlatformsStopsInEditor()
    {
        var platforms = FindObjectsOfType<PlatformSolid>();

        foreach (var platformSolid in platforms)
        {
            if (platformSolid.stopA == null || platformSolid.stopB == null)
            {
                Debug.Log($"Platform {platformSolid} has no stop attached!");
                continue;
            }
            
            platformSolid.stopA.Draw(Color.magenta);
            platformSolid.stopB.Draw(Color.magenta);
        }
    }

    void AssignPlayerReferencesOnPickups()
    {
        var pickups = FindObjectsOfType<PickupActor>();
        var player = FindObjectOfType<PlayerActor>();
        
        foreach (var pickupActor in pickups)
        {
            pickupActor.player = player;
        }
    }

    private void Update()
    {
        if (drawPlatformStops)
           DrawPlatformsStopsInEditor();
        
        if (autoIntegerCorrection)
            IntegerCorrection();
        
        if(autoMatchAABBToScale)
            MatchAABBToScale();
    }
}
