using UnityEngine;

public class DisplayedBioBrick : GenericDisplayedBioBrick {
	
    [SerializeField]
    protected UISprite _jigsawSprite;
    [SerializeField]
    protected UILocalize _biobrickLabel;
    
	public static UnityEngine.Object prefab    = null;
    
	protected new const string prefabURI      = "GUI/screen3/BioBricks/DisplayedBioBrickPrefab";
    
    private const string _jigsawSpriteNamePrefix     = "jigsaw_";
    private const string _promoterJigsawSpriteName   = _jigsawSpriteNamePrefix + "promoter";
    private const string _rbsJigsawSpriteName        = _jigsawSpriteNamePrefix + "rbs";
    private const string _geneJigsawSpriteName       = _jigsawSpriteNamePrefix + "gene";
    private const string _terminatorJigsawSpriteName = _jigsawSpriteNamePrefix + "terminator";
    
    private const string _textBackgroundSpriteSuffix = "_text";
    private const string _geneTextBackgroundSprite = "gene"+_textBackgroundSpriteSuffix;
    private const string _promoterTextBackgroundSpritePrefix = "promoter";
    private const string _promoterConstantTextBackgroundSpritStem = "";
    private const string _promoterActivatedTextBackgroundSpritStem = "_+";
    private const string _promoterRepressedTextBackgroundSpritStem = "_-";
    private const string _promoterBothTextBackgroundSpritStem = "_+-";
    private const string _promoterConstantTextBackgroundSprite          = _promoterTextBackgroundSpritePrefix+_promoterConstantTextBackgroundSpritStem+_textBackgroundSpriteSuffix;
    private const string _promoterActivatedTextBackgroundSprite         = _promoterTextBackgroundSpritePrefix+_promoterActivatedTextBackgroundSpritStem+_textBackgroundSpriteSuffix;
    private const string _promoterRepressedConstTextBackgroundSprite    = _promoterTextBackgroundSpritePrefix+_promoterRepressedTextBackgroundSpritStem+_textBackgroundSpriteSuffix;
    private const string _promoterBothConstTextBackgroundSprite         = _promoterTextBackgroundSpritePrefix+_promoterBothTextBackgroundSpritStem+_textBackgroundSpriteSuffix;
	
    
	public static new DisplayedBioBrick Create(
		Transform parentTransform
		,Vector3 localPosition
		,BioBrick biobrick
		,Object externalPrefab = null
		)
	{
		
		if (null == prefab) prefab = Resources.Load(prefabURI);
		Object prefabToUse = (externalPrefab==null)?prefab:externalPrefab;
		
		// Debug.Log("DisplayedBioBrick Create(parentTransform="+parentTransform
		//            + ", localPosition="+localPosition
		//            + ", biobrick="+biobrick
		//            );
		
		DisplayedBioBrick result = (DisplayedBioBrick)DisplayedElement.Create(
			parentTransform
			,localPosition
			,getSpriteName(biobrick)
			,prefabToUse
			);
		
		result.Initialize(biobrick);
		
		return result;
	}

    public void Initialize(BioBrick biobrick)
    {
        // Debug.Log(this.GetType() + " Initialize(" + biobrick + ")");
        // Debug.Log(this.GetType() + " Initialize(" + biobrick + ") starts");
        GenericDisplayedBioBrick.Initialize(this, biobrick);
        setJigsawSprite();
        setBioBrickOverlay();
        setBioBrickIcon();
        // Debug.Log(this.GetType() + " this.transform.localPosition="+this.transform.localPosition);
        // Debug.Log(this.GetType() + " this.transform.localScale="+this.transform.localScale);

        this.transform.localScale = Vector3.one;
        this.transform.localPosition = new Vector3(this.transform.localPosition.x, this.transform.localPosition.y, 0);
    }

    protected virtual void setJigsawSprite()
    {
        if (null != _jigsawSprite)
        {
            string jigsawSpriteName = "";
            switch (_biobrick.getType())
            {
                case BioBrick.Type.PROMOTER:
                    jigsawSpriteName = _promoterJigsawSpriteName;
                    break;
                case BioBrick.Type.RBS:
                    jigsawSpriteName = _rbsJigsawSpriteName;
                    break;
                case BioBrick.Type.GENE:
                    jigsawSpriteName = _geneJigsawSpriteName;
                    break;
                case BioBrick.Type.TERMINATOR:
                    jigsawSpriteName = _terminatorJigsawSpriteName;
                    break;
                default:
                    jigsawSpriteName = _promoterJigsawSpriteName;
                    break;
            }
            _jigsawSprite.spriteName = jigsawSpriteName;
        }
    }

    private void setBioBrickOverlay()
    {
        // Debug.Log(this.GetType() + " setBioBrickOverlay " + this._biobrick);
        if (BioBrick.Type.TERMINATOR == _biobrick.getType())
        {
            if (null != _biobrickLabel)
            {
                GameObject.Destroy(_biobrickLabel.gameObject);
            }
        }
        else
        {
            DisplayedDevice.setMoleculeOverlay(this._biobrick.getInternalName(), _biobrickLabel, true);
        }
    }
    
    private void setBioBrickIcon()
    {
        string backgroundSpriteName = null;
        switch (_biobrick.getType())
        {
            case BioBrick.Type.PROMOTER:
                PromoterBrick promoter = (PromoterBrick)_biobrick;
                PromoterBrick.Regulation regulation = promoter.getRegulation();
                switch (regulation)
                {
                    case PromoterBrick.Regulation.CONSTANT:
                        backgroundSpriteName = _promoterConstantTextBackgroundSprite;
                        break;
                    case PromoterBrick.Regulation.ACTIVATED:
                        backgroundSpriteName = _promoterActivatedTextBackgroundSprite;
                        break;
                    case PromoterBrick.Regulation.REPRESSED:
                        backgroundSpriteName = _promoterRepressedConstTextBackgroundSprite;
                        break;
                    case PromoterBrick.Regulation.BOTH:
                        backgroundSpriteName = _promoterBothConstTextBackgroundSprite;
                        break;
                    default:
                        backgroundSpriteName = _promoterConstantTextBackgroundSprite;
                        break;
                }
                break;
            case BioBrick.Type.RBS:
                backgroundSpriteName = getSpriteName(_biobrick);
                break;
            case BioBrick.Type.GENE:
                backgroundSpriteName = _geneTextBackgroundSprite;
                break;
            case BioBrick.Type.TERMINATOR:
                backgroundSpriteName = getSpriteName(_biobrick);
                break;
            default:
                backgroundSpriteName = _geneTextBackgroundSprite;
                break;
        }
        if (!string.IsNullOrEmpty(backgroundSpriteName))
        {
            setSprite(backgroundSpriteName);
        }
    }
	
	public new static string getSpriteName(BioBrick brick) {
		return brick.getName();
	}

  protected new string getDebugInfos() {
    return "Displayed biobrick id="+_id+", inner biobrick="+_biobrick+",  time="+Time.realtimeSinceStartup;
  }

	public override void OnPress(bool isPressed) {
		// Debug.Log(this.GetType() + " OnPress _id="+_id+", isPressed="+isPressed);
  }
}
