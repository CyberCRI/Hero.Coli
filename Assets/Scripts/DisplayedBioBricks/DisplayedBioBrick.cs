using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DisplayedBioBrick : GenericDisplayedBioBrick {
	
	public UILabel                    _label;
	public LastHoveredInfoManager     _lastHoveredInfoManager;
	
	protected static string             prefabURI = "GUI/screen3/BioBricks/DisplayedBioBrickPrefab";
  public    static UnityEngine.Object prefab    = null;
	
	
	public static new DisplayedBioBrick Create(
		Transform parentTransform
		,Vector3 localPosition
		,string spriteName
		,BioBrick biobrick
		,Object externalPrefab = null
		)
	{
		
		string usedSpriteName = ((spriteName!=null)&&(spriteName!=""))?spriteName:getSpriteName(biobrick);
		string nullSpriteName = ((spriteName!=null)&&(spriteName!=""))?"":"(null)=>"+usedSpriteName;
		
		if (null == prefab) prefab = Resources.Load(prefabURI);
		Object prefabToUse = (externalPrefab==null)?prefab:externalPrefab;
		
		Logger.Log("DisplayedBioBrick::Create(parentTransform="+parentTransform
		           + ", localPosition="+localPosition
		           + ", spriteName="+spriteName+nullSpriteName
		           + ", biobrick="+biobrick
		           , Logger.Level.DEBUG
		           );
		
		DisplayedBioBrick result = (DisplayedBioBrick)DisplayedElement.Create(
			parentTransform
			,localPosition
			,usedSpriteName
			,prefabToUse
			);
		
		Initialize(result, biobrick);
		
		return result;
	}
	
	protected static void Initialize(
		DisplayedBioBrick biobrickScript
		,BioBrick biobrick
		) {

		// Logger.Log("DisplayedBioBrick::Initialize("+biobrickScript+", "+biobrick+") starts", Logger.Level.TRACE);
		GenericDisplayedBioBrick.Initialize(biobrickScript, biobrick);
		biobrickScript._label.text = GameplayNames.getBrickRealName(biobrick.getName());
		Logger.Log("DisplayedBioBrick::Initialize ends with biobrickScript._lastHoveredInfoManager="+biobrickScript._lastHoveredInfoManager, Logger.Level.TRACE);

	}

  protected void OnLanguageChanged()
  {
    _label.text = GameplayNames.getBrickRealName(_biobrick.getName());
  }
	
	public new static string getSpriteName(BioBrick brick) {
		return brick.getName();
	}

  protected string getDebugInfos() {
    return "Displayed biobrick id="+_id+", inner biobrick="+_biobrick+", label="+_label.text+" time="+Time.realtimeSinceStartup;
  }

	protected override void OnPress(bool isPressed) {
		Logger.Log("DisplayedBioBrick::OnPress _id="+_id+", isPressed="+isPressed, Logger.Level.INFO);
  }
}
