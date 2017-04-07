using System;
using System.Collections.Generic;
using UnityEngine;

public class Device : DNABit
{
    //TODO load that value from file
    private static float _energyPerBasePair = 0.005f;

    private static int _idCounter;
    private int _id;

    //TODO factorize code with ExpressionModule's
    public string displayedName { get; set; }
    private string _internalName;
    public override string getInternalName()
    {
        // Debug.Log(this.GetType() + " getInternalName()");
        if (string.IsNullOrEmpty(_internalName))
        {
            _internalName = generateInternalName();
        }
        return _internalName;
    }

    private LinkedList<ExpressionModule> _modules;
    public LinkedList<ExpressionModule> getExpressionModules() { return _modules; }

    public PromoterBrick.Regulation getRegulation()
    {
        try
        {
            return ((PromoterBrick)_modules.First.Value.getBioBricks().First.Value).getRegulation();
        }
        catch
        {
            Debug.LogWarning(this.GetType() + " getRegulation error on " + this);
            return PromoterBrick.Regulation.CONSTANT;
        }
    }
    
    // _level
    private const float _level0 = .06f;
    private const float _level1 = .126f;
    private const float _level2 = .23f;
    private const float _level3 = .4f;
    private const float _level01Threshold = (_level0 + _level1) / 2;
    private const float _level12Threshold = (_level1 + _level2) / 2;
    private const float _level23Threshold = (_level2 + _level3) / 2;
    private int _levelIndex = -1;
    public int levelIndex
    {
        get
        {
            if (-1 == _levelIndex)
            {
                initializeLevelIndex();
            }
            return _levelIndex;
        }
        private set
        {
            _levelIndex = value;
        }
    }
    private void initializeLevelIndex()
    {
        float expressionLevel = getExpressionLevel();

        if (expressionLevel < _level01Threshold)
        {
            levelIndex = 0;
        }
        else if (expressionLevel < _level12Threshold)
        {
            levelIndex = 1;
        }
        else if (expressionLevel < _level23Threshold)
        {
            levelIndex = 2;
        }
        else
        {
            levelIndex = 3;
        }
    }
    protected int _length;
    public void updateLength()
    {
        int sum = 0;

        foreach (ExpressionModule em in _modules)
            sum += em.getSize();

        // Debug.Log("Size " + sum + " for " + this);
        _length = sum;
    }
    public override int getLength()
    {
        if (0 == _length)
        {
            updateLength();
        }
        return _length;
    }
    public override string getTooltipTitleKey()
    {
        // Debug.Log(this.GetType() + " getTooltipTitleKey " + this);
        return GameplayNames.generateRealNameFromBricks(this);
    }
    public override string getTooltipExplanation()
    {
        return TooltipManager.produceExplanation(this);
    }

    private void idInit()
    {
        _id = _idCounter;
        _idCounter += 1;
    }

    private Device()
    {
        idInit();
    }

    //generates internal name from expression modules sequence
    private string generateInternalName()
    {
        // Debug.Log(this.GetType() + " generateName(bricks)");
        return generateInternalName(_modules);
    }

    private static string generateInternalName(LinkedList<ExpressionModule> modules)
    {
        // Debug.Log(this.GetType() + " Device generateInternalName(modules="+Logger.ToString(modules)+")");

        string name = "";

        //TODO extract this
        string separator = "+";

        if (isValid(modules))
        {
            LinkedList<ExpressionModule> ems = new LinkedList<ExpressionModule>(modules);
            while (1 != ems.Count)
            {
                string toAppend = ems.First.Value.getInternalName() + separator;
                name += toAppend;
                ems.RemoveFirst();
            }
            name += ems.First.Value.getInternalName();
            return name;
        }
        else
        {
            Debug.LogWarning("Device generateInternalName got invalid expression modules sequence");
            return "";
        }
    }

    private Device(string name, LinkedList<ExpressionModule> modules)
    {
        // Debug.Log(this.GetType() + " Device("+name+", modules="+Logger.ToString(modules)+")");

        idInit();
        displayedName = name;
        _internalName = generateInternalName(modules);

        _modules = new LinkedList<ExpressionModule>();
        foreach (ExpressionModule em in modules)
        {
            _modules.AddLast(new ExpressionModule(em));
        }
    }

    //returns the code name of the first - 'upstream' - protein produced by the device
    public string getFirstGeneProteinName()
    {
        foreach (ExpressionModule module in _modules)
        {
            foreach (BioBrick brick in module.getBioBricks())
            {
                if (brick.getType() == BioBrick.Type.GENE)
                {
                    // Debug.Log("((GeneBrick)brick).getProteinName()="+((GeneBrick)brick).getProteinName());
                    return ((GeneBrick)brick).getProteinName();
                }
            }
        }
        return null;
    }

    public string getFirstGeneBrickName()
    {
        foreach (ExpressionModule module in _modules)
        {
            foreach (BioBrick brick in module.getBioBricks())
            {
                if (brick.getType() == BioBrick.Type.GENE)
                {
                    // Debug.Log("brick.getInternalName()="+brick.getInternalName());
                    return brick.getInternalName();
                }
            }
        }
        return null;
    }

    public LinkedList<BioBrick> getBioBricks()
    {
        LinkedList<BioBrick> result = new LinkedList<BioBrick>();
        foreach (ExpressionModule module in _modules)
        {
            result.AppendRange(module.getBioBricks());
        }
        return result;
    }

    public float getExpressionLevel()
    {
        foreach (ExpressionModule module in _modules)
        {
            foreach (BioBrick brick in module.getBioBricks())
            {
                if (brick.getType() == BioBrick.Type.RBS)
                {
                    return ((RBSBrick)brick).getRBSFactor();
                }
            }
        }
        return 0f;
    }

    public override string ToString()
    {
        return string.Format("[Device: id : {0}, name: {1}, [ExpressionModules: {2}]", _id, displayedName, Logger.ToString(_modules));
    }

    private LinkedList<Product> getProductsFromBiobricks(LinkedList<BioBrick> list)
    {
        // Debug.Log(this.GetType() + " getProductsFromBioBricks([list: "+Logger.ToString(list)+"])");
        LinkedList<Product> products = new LinkedList<Product>();
        Product prod;
        RBSBrick rbs;
        GeneBrick gene;
        float RBSf = 0f;
        string molName = "Unknown";
        int i = 0;

        foreach (BioBrick b in list)
        {
            // Debug.Log(this.GetType() + " getProductsFromBioBricks: starting treatment of "+b.ToString());
            rbs = b as RBSBrick;
            if (rbs != null)
            {
                // Debug.Log(this.GetType() + " getProductsFromBioBricks: rbs spotted");
                RBSf = rbs.getRBSFactor();
            }
            else
            {
                // Debug.Log(this.GetType() + " getProductsFromBioBricks: not an rbs");
                gene = b as GeneBrick;
                if (gene != null)
                {
                    molName = gene.getProteinName();
                    prod = new Product(molName, RBSf);
                    products.AddLast(prod);
                }
                else
                {
                    if (b as TerminatorBrick == null)
                        Debug.LogWarning(this.GetType() + " getProductsFromBioBricks This case should never happen. Bad Biobrick in operon.");
                    else
                    {
                        break;
                    }
                }
            }
            i++;
        }
        while (i > 0)
        {
            list.RemoveFirst();
            i--;
        }
        return products;
    }

    private PromoterProperties getPromoterReaction(ExpressionModule em, int id)
    {
        // Debug.Log(this.GetType() + " getPromoterReaction("+em.ToString()+", "+id+")");
        PromoterProperties prom = new PromoterProperties();

        prom.energyCost = _energyPerBasePair * em.getSize();
        //promoter only
        //prom.energyCost = _energyPerBasePair*em.getBioBricks().First.Value.getSize();

        LinkedList<BioBrick> bricks = em.getBioBricks();

        //TODO fix this: create good properties' name
        prom.name = _internalName + id;
        PromoterBrick p = bricks.First.Value as PromoterBrick;
        prom.formula = p.getFormula();
        prom.beta = p.getBeta();
        bricks.RemoveFirst();

        prom.products = getProductsFromBiobricks(bricks);

        TerminatorBrick tb = bricks.First.Value as TerminatorBrick;
        prom.terminatorFactor = tb.getTerminatorFactor();
        bricks.RemoveFirst();

        if (bricks.Count != 0)
        {
            // Debug.Log(this.GetType() + " getPromoterReaction Warning: bricks.Count ="+bricks.Count);
        }
        return prom;
    }


    private LinkedList<PromoterProperties> getPromoterReactions()
    {
        // Debug.Log(this.GetType() + " getPromoterReactions() starting... device="+this);

        //cf issue #224
        //previously:
        //LinkedList<ExpressionModule> modules = new LinkedList<ExpressionModule>(_modules);
        //caused early deletion problem
        LinkedList<ExpressionModule> modules = new LinkedList<ExpressionModule>();
        foreach (ExpressionModule module in _modules)
        {
            modules.AddLast(new ExpressionModule(module));
        }

        LinkedList<PromoterProperties> reactions = new LinkedList<PromoterProperties>();
        PromoterProperties reaction;
        // Debug.Log(this.GetType() + " getPromoterReactions() built #modules="+modules.Count+" and #reactions="+reactions.Count);

        foreach (ExpressionModule em in modules)
        {
            // Debug.Log(this.GetType() + " getPromoterReactions() analyzing em="+em);
            reaction = getPromoterReaction(em, em.GetHashCode());
            if (reaction != null)
                reactions.AddLast(reaction);
        }
        if (reactions.Count == 0)
            return null;
        return reactions;
    }

    public LinkedList<IReaction> getReactions()
    {
        // Debug.Log(this.GetType() + " getReactions(); device="+this);

        LinkedList<IReaction> reactions = new LinkedList<IReaction>();
        LinkedList<PromoterProperties> props = new LinkedList<PromoterProperties>(getPromoterReactions());
        foreach (PromoterProperties promoterProps in props)
        {
            // Debug.Log(this.GetType() + " getReactions() adding prop "+promoterProps);
            reactions.AddLast(PromoterReaction.buildPromoterFromProps(promoterProps));
        }

        // Debug.Log(this.GetType() + " getReactions() with device="+this+" returns "+Logger.ToString<IReaction>(reactions));
        return reactions;
    }

    public static bool isValid(Device device)
    {
        return (null != device && isValid(device._modules));
    }

    private static bool isValid(LinkedList<ExpressionModule> modules)
    {
        if (null == modules)
        {
            // Debug.Log(this.GetType() + " Device isValid null==modules");
            return false;
        }

        if (0 == modules.Count)
        {
            // Debug.Log(this.GetType() + " Device isValid 0==modules.Count");
            return false;
        }

        foreach (ExpressionModule em in modules)
            if (!em.isValid())
            {
                Debug.LogWarning("Device isValid module " + em + " is invalid");
                return false;
            }
        return true;
    }

    public static Device buildDevice(LinkedList<BioBrick> bricks)
    {
        // Debug.Log(this.GetType() + " Device buildDevice(bricks) with bricks="+bricks);

        ExpressionModule em = new ExpressionModule(bricks);
        return buildDevice(em);
    }

    public static Device buildDevice(ExpressionModule em)
    {
        // Debug.Log(this.GetType() + " Device buildDevice(em) with em="+em+")");
        LinkedList<ExpressionModule> modules = new LinkedList<ExpressionModule>();
        modules.AddLast(em);
        return buildDevice(em.getInternalName(), modules);
    }

    public static Device buildDevice(LinkedList<ExpressionModule> modules)
    {
        // Debug.Log(this.GetType() + " Device buildDevice(modules)");
        if (!isValid(modules))
        {
            Debug.LogWarning("Device buildDevice(modules) failed: modules==null or modules are invalid");
            return null;
        }
        //TODO take into account all modules in device name
        return buildDevice(modules.First.Value.getInternalName(), modules);
    }

    public static Device buildDevice(string name, LinkedList<ExpressionModule> modules)
    {
        // Debug.Log(this.GetType() + " Device buildDevice(name, modules) with name="+name);
        if (!isValid(modules))
        {
            Debug.LogWarning("Device buildDevice(name, modules) failed: modules==null or modules are invalid");
            return null;
        }

        Device device = new Device(name, modules);
        // Debug.Log(this.GetType() + " Device buildDevice returns "+device);
        return device;
    }

    //copy factory
    public static Device buildDevice(Device device)
    {
        if (device == null)
        {
            Debug.LogWarning("Device buildDevice device == null");
            return null;
        }
        return buildDevice(device.getInternalName(), device._modules);
    }

    //helper for simple devices
    public static Device buildDevice(string name,
        float beta,//promoter
        string formula,//promoter
        float rbsFactor,//rbs
        string proteinName,//gene
        float terminatorFactor,//terminator
    int pSize = 0,
    int rSize = 0,
    int gSize = 0,
    int tSize = 0
        )
    {

        string nullName = (name == null) ? "(null)" : "";
        // Debug.Log(this.GetType() + " Device buildDevice(name="+name+nullName
        //   +", beta="+beta
        //   +", formula='"+formula
        //   +"', rbsFactor="+rbsFactor
        //   +", proteinName="+proteinName
        //   +", terminatorFactor="+terminatorFactor
        //   +") starting...");

        string notNullName = name;
        if (notNullName == null)
        {
            notNullName = "device" + _idCounter;
        }

        BioBrick[] bioBrickArray = {
            new PromoterBrick(notNullName+"_promoter", beta, formula, pSize),
            new RBSBrick(notNullName+"_rbs", rbsFactor, rSize),
            new GeneBrick(notNullName+"_gene", proteinName, gSize),
            new TerminatorBrick(notNullName+"_terminator", terminatorFactor, tSize)
        };

        LinkedList<BioBrick> bricks = new LinkedList<BioBrick>(bioBrickArray);

        ExpressionModule[] modulesArray = { new ExpressionModule(notNullName + "_module", bricks) };

        LinkedList<ExpressionModule> modules = new LinkedList<ExpressionModule>(modulesArray);

        return Device.buildDevice(notNullName, modules);
    }

    public bool hasSameBricks(Device device)
    {
        if (!Device.isValid(device)
          || (device._modules.Count != _modules.Count)
          )
        {
            return false;
        }

        IEnumerator<ExpressionModule> enumerator1 = device._modules.GetEnumerator();
        IEnumerator<ExpressionModule> enumerator2 = _modules.GetEnumerator();

        while (enumerator1.MoveNext() && enumerator2.MoveNext())
        {
            if (!enumerator1.Current.hasSameBricks(enumerator2.Current))
            {
                return false;
            }
        }
        return true;
    }

    public override bool Equals(System.Object obj)
    {
        if (obj == null)
        {
            return false;
        }

        Device d = obj as Device;
        if ((System.Object)d == null)
        {
            return false;
        }
        // bool sameBricks = this.hasSameBricks(d);
        // if(sameBricks)
        //   Debug.Log(this.GetType() + " equals returns " + sameBricks + " between " + this + " and " + d);
        return this.hasSameBricks(d);
    }
}