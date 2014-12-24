using System;
using System.Collections.Generic;
using UnityEngine;

public class Device: DNABit
{
    //TODO load that value from file
    private static float                  _energyPerBasePair = 0.005f;

    private static int                    _idCounter;
    private int                           _id;

    //TODO factorize code with ExpressionModule's
    public string displayedName { get; set; }
    private string _internalName;
    public string getInternalName() {
        Logger.Log("ExpressionModule::getInternalName()", Logger.Level.DEBUG);
        if(string.IsNullOrEmpty(_internalName))
        {
            _internalName = generateInternalName();
        }
        return _internalName;
    }

    private LinkedList<ExpressionModule>	_modules;
    public LinkedList<ExpressionModule> getExpressionModules() { return _modules; }

    public int getSize()
    {
        int sum = 0;

        foreach (ExpressionModule em in _modules)
            sum += em.getSize();
        return sum;
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
        Logger.Log("Device::generateName(bricks)", Logger.Level.DEBUG);
        return generateInternalName(_modules);
    }

    private static string generateInternalName(LinkedList<ExpressionModule> modules)
    {
        Logger.Log("Device::generateInternalName(modules="+Logger.ToString(modules)+")", Logger.Level.INFO);

        string name = "";

        //TODO extract this
        string separator = "+";

        if(isValid(modules))
        {
            LinkedList<ExpressionModule> ems = new LinkedList<ExpressionModule>(modules);
            while(1 != ems.Count)
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
            Logger.Log("Device::generateInternalName got invalid expression modules sequence", Logger.Level.WARN);
            return "";
        }
    }

  private Device(string name, LinkedList<ExpressionModule> modules)
  {
    Logger.Log("Device::Device("+name+", modules="+Logger.ToString(modules)+")", Logger.Level.DEBUG);

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
        if(brick.getType() == BioBrick.Type.GENE)
        {
          return ((GeneBrick)brick).getProteinName();
        }
      }
    }
    return null;
  }

  public float getExpressionLevel()
  {
    foreach (ExpressionModule module in _modules)
    {
      foreach (BioBrick brick in module.getBioBricks())
      {
        if(brick.getType() == BioBrick.Type.RBS)
        {
          return ((RBSBrick)brick).getRBSFactor();
        }
      }
    }
    return 0f;
  }

  public override string ToString ()
  {		
		return string.Format ("[Device: id : {0}, name: {1}, [ExpressionModules: {2}]", _id, displayedName, Logger.ToString(_modules));
  }

  private LinkedList<Product> getProductsFromBiobricks(LinkedList<BioBrick> list)
  {
    Logger.Log("Device::getProductsFromBioBricks([list: "+Logger.ToString(list)+"])", Logger.Level.DEBUG);
    LinkedList<Product> products = new LinkedList<Product>();
    Product prod;
    RBSBrick rbs;
    GeneBrick gene;
    float RBSf = 0f;
    string molName = "Unknown";
    int i = 0;

    foreach (BioBrick b in list)
      {
        Logger.Log("Device::getProductsFromBioBricks: starting treatment of "+b.ToString(), Logger.Level.TRACE);
        rbs = b as RBSBrick;
        if (rbs != null) {
          Logger.Log("Device::getProductsFromBioBricks: rbs spotted", Logger.Level.TRACE);
          RBSf = rbs.getRBSFactor();
      }
        else
          {
            Logger.Log("Device::getProductsFromBioBricks: not an rbs", Logger.Level.TRACE);
            gene = b as GeneBrick;
            if (gene != null)
              {
                molName = gene.getProteinName();
                prod = new Product();
                prod.setName(molName);
                prod.setQuantityFactor(RBSf);
                products.AddLast(prod);
              }
            else
              {
                if (b as TerminatorBrick == null)
                  Logger.Log("Device::getProductsFromBioBricks This case should never happen. Bad Biobrick in operon.", Logger.Level.WARN);
                else {
                 break;
                }
              }
          }
        i++;
      }
    while (i > 0) {
      list.RemoveFirst();
      i--;
    }
    return products;
  }

  private PromoterProperties getPromoterReaction(ExpressionModule em, int id)
  {
    Logger.Log("Device::getPromoterReaction("+em.ToString()+", "+id+")", Logger.Level.TRACE);
    PromoterProperties prom = new PromoterProperties();

    prom.energyCost = _energyPerBasePair*em.getSize();
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

    if(bricks.Count != 0) {
      Logger.Log("Device::getPromoterReaction Warning: bricks.Count ="+bricks.Count, Logger.Level.TRACE);
    }
    return prom;
  }


    private LinkedList<PromoterProperties> getPromoterReactions()
    {
        Logger.Log("Device::getPromoterReactions() starting... device="+this, Logger.Level.TRACE);

        //cf issue #224
        //previously:
        //LinkedList<ExpressionModule> modules = new LinkedList<ExpressionModule>(_modules);
        //caused early deletion problem
        LinkedList<ExpressionModule> modules = new LinkedList<ExpressionModule>();
        foreach(ExpressionModule module in _modules)
        {
            modules.AddLast(new ExpressionModule(module));
        }

        LinkedList<PromoterProperties> reactions = new LinkedList<PromoterProperties>();
        PromoterProperties reaction;
        Logger.Log("Device::getPromoterReactions() built #modules="+modules.Count+" and #reactions="+reactions.Count, Logger.Level.TRACE);

        foreach (ExpressionModule em in modules)
        {
            Logger.Log("Device::getPromoterReactions() analyzing em="+em, Logger.Level.TRACE);
            reaction = getPromoterReaction(em, em.GetHashCode());
            if (reaction != null)
                reactions.AddLast(reaction);
        }
        if (reactions.Count == 0)
            return null;
        return reactions;
    }

    public LinkedList<IReaction> getReactions() {
        Logger.Log ("Device::getReactions(); device="+this, Logger.Level.TRACE);
		
        LinkedList<IReaction> reactions = new LinkedList<IReaction>();	
        LinkedList<PromoterProperties> props = new LinkedList<PromoterProperties>(getPromoterReactions());
        foreach (PromoterProperties promoterProps in props) {
            Logger.Log("Device::getReactions() adding prop "+promoterProps, Logger.Level.TRACE);
            reactions.AddLast(PromoterReaction.buildPromoterFromProps(promoterProps));
        }
		
        Logger.Log("Device::getReactions() with device="+this+" returns "+Logger.ToString<IReaction>(reactions), Logger.Level.INFO);
        return reactions;
    }

    private static bool isValid(LinkedList<ExpressionModule> modules)
    {
        if(null == modules)
        {
            Logger.Log("Device::isValid null==modules", Logger.Level.DEBUG);
            return false;
        }

        if(0 == modules.Count)
        {
            Logger.Log("Device::isValid 0==modules.Count", Logger.Level.DEBUG);
            return false;
        }

        foreach (ExpressionModule em in modules)
            if (!em.isValid())
        {
            Logger.Log("Device::isValid module "+em+" is invalid", Logger.Level.WARN);
            return false;
        }
        return true;
    }

    public static Device buildDevice(LinkedList<BioBrick> bricks)
    {
        Logger.Log("Device::buildDevice(bricks) with bricks="+bricks, Logger.Level.INFO);

        ExpressionModule em = new ExpressionModule(bricks);
        return buildDevice(em);
    }

    public static Device buildDevice(ExpressionModule em)
    {
        Logger.Log("Device::buildDevice(em) with em="+em+")", Logger.Level.INFO);
        LinkedList<ExpressionModule> modules = new LinkedList<ExpressionModule>();
        modules.AddLast(em);
        return buildDevice(em.getInternalName(), modules);
    }

    public static Device buildDevice(LinkedList<ExpressionModule> modules)
    {
        Logger.Log("Device::buildDevice(modules)", Logger.Level.INFO);
        if(!isValid(modules))
        {
            Logger.Log("Device::buildDevice(modules) failed: modules==null or modules are invalid", Logger.Level.WARN);
            return null;
        }
        //TODO take into account all modules in device name
        return buildDevice(modules.First.Value.getInternalName(), modules);
    }

    //warning: can lead to same devices but with different names
    public static Device buildDevice(string name, LinkedList<ExpressionModule> modules)
    {
        Logger.Log("Device::buildDevice(name, modules) with name="+name, Logger.Level.INFO);
        if (!isValid(modules))
        {
            Logger.Log("Device::buildDevice(name, modules) failed: modules==null or modules are invalid", Logger.Level.WARN);
            return null;
        }

        Device device = new Device(name, modules);
        Logger.Log("Device::buildDevice returns "+device, Logger.Level.INFO);
        return device;
    }

    //copy factory
  public static Device buildDevice(Device device)
  {
    if (device == null)
    {
      Logger.Log("Device::buildDevice device == null", Logger.Level.WARN);
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
		float terminatorFactor//terminator
		) {

    string nullName = (name==null)?"(null)":"";
		Logger.Log("Device::buildDevice(name="+name+nullName
      +", beta="+beta
      +", formula='"+formula
      +"', rbsFactor="+rbsFactor
      +", proteinName="+proteinName
      +", terminatorFactor="+terminatorFactor
      +") starting...", Logger.Level.TRACE);

    string notNullName = name;
    if(notNullName==null) {
      notNullName = "device"+_idCounter;
    }
		
		BioBrick[] bioBrickArray = {
			new PromoterBrick(notNullName+"_promoter", beta, formula),
			new RBSBrick(notNullName+"_rbs", rbsFactor),
			new GeneBrick(notNullName+"_gene", proteinName),
			new TerminatorBrick(notNullName+"_terminator", terminatorFactor)
		};

		LinkedList<BioBrick> bricks = new LinkedList<BioBrick>(bioBrickArray);
		
		ExpressionModule[] modulesArray = { new ExpressionModule(notNullName+"_module", bricks) };

		LinkedList<ExpressionModule> modules = new LinkedList<ExpressionModule>(modulesArray);
		
		return Device.buildDevice(notNullName, modules);
	}

  public bool hasSameBricks(Device device) {
    if(device._modules.Count != _modules.Count) {
      Logger.Log("Device::hasSameBricks("+device+") of "+this+" differ on count: "+device._modules.Count+"≠"+_modules.Count, Logger.Level.TRACE);
      return false;
    }

    IEnumerator<ExpressionModule> enumerator1 = device._modules.GetEnumerator();
    IEnumerator<ExpressionModule> enumerator2 = _modules.GetEnumerator();

    while(enumerator1.MoveNext() && enumerator2.MoveNext()) {
      if(!enumerator1.Current.hasSameBricks(enumerator2.Current)) {
        Logger.Log("Device::hasSameBricks("+device+") of "+this+" differ on "+enumerator1.Current+"≠"+enumerator2.Current, Logger.Level.TRACE);
        return false;
      }
    }
    Logger.Log("Device::hasSameBricks("+device+") of "+this+" coincide", Logger.Level.TRACE);
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
    
    return this.hasSameBricks(d);
  }
}