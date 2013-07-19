using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

public class Group {
	static int groupCount = 1;
	static Dictionary<Transform, Transform> previousChildren;
	
	[MenuItem ("Pixelplacement/Group/Group Selected %g")]
    static void MakeGroup() {
		Undo.CreateSnapshot();
		
		//cache selected objects in scene:
		Transform[] selectedObjects = Selection.transforms;
		
		//early exit if nothing is selected:
		if ( selectedObjects.Length == 0 ) {
			return;
		}
				
		//parent construction and hierarchy structure decision:
		bool nestParent = false;
		Undo.RegisterSceneUndo( "Create Group" );
		Transform _parent = new GameObject( "Group " + groupCount++ ).transform; //naming convention mirrors Photoshop's
		Transform coreParent = selectedObjects[0].parent;
		foreach ( Transform item in selectedObjects ) {
			if ( item.parent != coreParent ) {
				nestParent = false;
				break;
			}else{
				nestParent = true;	
			}
		}
		if ( nestParent ) {
			_parent.parent = coreParent;
		}
		
		//place group's pivot on the active transform in the scene:
		_parent.position = Selection.activeTransform.position;
		
		//set selected objects as children of the group:
		foreach ( Transform item in selectedObjects ) {
			item.parent = _parent;
		}
    }	
}

