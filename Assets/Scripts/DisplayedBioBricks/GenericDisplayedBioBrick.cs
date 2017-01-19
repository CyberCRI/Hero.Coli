using UnityEngine;
using System.Collections.Generic;


/*!
\brief This class enables the generic display of BioBricks
\details The texture used is by default set according to the BioBrick type

\author Raphael GOUJET
*/

public class GenericDisplayedBioBrick : DisplayedElement
{
    private const string _prefabURI = "GUI/screen1/Devices/TinyBioBrickIconPrefab";
    protected static UnityEngine.Object _genericPrefab = null;


    public static Dictionary<BioBrick.Type, string> spriteNamesDico = new Dictionary<BioBrick.Type, string>() {
    {BioBrick.Type.GENE,        "gene"},
    {BioBrick.Type.PROMOTER,    "promoter"},
    {BioBrick.Type.RBS,         "RBS"},
    {BioBrick.Type.TERMINATOR,  "terminator"},
    {BioBrick.Type.UNKNOWN,     "unknown"}
  };

    public BioBrick _biobrick;

    public static GenericDisplayedBioBrick Create(
      Transform parentTransform
      , Vector3 localPosition
      , BioBrick biobrick
      , Object externalPrefab = null
      )
    {

        if (_genericPrefab == null) _genericPrefab = Resources.Load(_prefabURI);
        Object prefabToUse = (externalPrefab == null) ? _genericPrefab : externalPrefab;

        // Debug.Log("GenericDisplayedBioBrick Create(parentTransform="+parentTransform
        //   + ", localPosition="+localPosition
        //   + ", biobrick="+biobrick
        //   );

        GenericDisplayedBioBrick result = (GenericDisplayedBioBrick)DisplayedElement.Create(
          parentTransform
          , localPosition
          , getSpriteName(biobrick)
          , prefabToUse
          );

        Initialize(result, biobrick);

        return result;
    }

    protected static void Initialize(
      GenericDisplayedBioBrick biobrickScript
      , BioBrick biobrick
    )
    {
        // Debug.Log("GenericDisplayedBioBrick Initialize("+biobrickScript+", "+biobrick+") starts");
        biobrickScript._biobrick = biobrick;
    }

    public static string getSpriteName(BioBrick brick)
    {
        return spriteNamesDico[brick.getType()];
    }

    protected string getDebugInfos()
    {
        return "Displayed biobrick id=" + _id + ", inner biobrick=" + _biobrick + " time=" + Time.realtimeSinceStartup;
    }

    public override void OnPress(bool isPressed)
    {
        // Debug.Log(this.GetType() + " OnPress _id="+_id+", isPressed="+isPressed);
    }

    void OnHover(bool isOver)
    {
        // Debug.Log(this.GetType() + " OnHover("+isOver+") brick=" + _biobrick.getInternalName());
        TooltipManager.displayTooltip(isOver, _biobrick, transform.position);
    }
}
