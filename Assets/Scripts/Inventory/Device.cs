using System;
using System.Collections.Generic;
using UnityEngine;

public class Device: DNABit
{
  private static int                    _idCounter;
  private int                           _id;
  private string                        _name;
  private LinkedList<ExpressionModule>	_modules;

  private static float                  _energyPerBasePair = 0.01f;
  
  public string getName() { return _name; }
  public void setName(string v) { _name = v; }
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

  private Device(string name, LinkedList<ExpressionModule> modules)
  {
    idInit();
    _name = name;
    _modules = new LinkedList<ExpressionModule>();
    foreach (ExpressionModule em in modules)
      _modules.AddLast(new ExpressionModule(em));
  }

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

  public override string ToString ()
  {		
		return string.Format ("[Device: id : {0}, name: {1}, [ExpressionModules: {2}]", _id, _name, Logger.ToString(_modules));
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

  private PromoterProprieties getPromoterReaction(ExpressionModule em, int id)
  {
    Logger.Log("Device::getPromoterReaction("+em.ToString()+", "+id+")", Logger.Level.TRACE);
    PromoterProprieties prom = new PromoterProprieties();

    prom.energyCost = _energyPerBasePair*em.getSize();
    //promoter only
    //prom.energyCost = _energyPerBasePair*em.getBioBricks().First.Value.getSize();

    LinkedList<BioBrick> bricks = em.getBioBricks();

    prom.name = _name + id;
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

  private LinkedList<PromoterProprieties> getPromoterReactions()
  {
    Logger.Log("Device::getPromoterReactions() starting... device="+this, Logger.Level.TRACE);
    LinkedList<ExpressionModule> modules = new LinkedList<ExpressionModule>(_modules);
    LinkedList<PromoterProprieties> reactions = new LinkedList<PromoterProprieties>();
    PromoterProprieties reaction;
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
    LinkedList<PromoterProprieties> props = new LinkedList<PromoterProprieties>(getPromoterReactions());
    foreach (PromoterProprieties promoterProps in props) {
      Logger.Log("Device::getReactions() adding prop "+promoterProps, Logger.Level.TRACE);
      reactions.AddLast(PromoterReaction.buildPromoterFromProps(promoterProps));
    }
		
	Logger.Log("Device::getReactions() with device="+this+" returns "+Logger.ToString<IReaction>(reactions), Logger.Level.INFO);
    return reactions;
  }


  private static bool checkPromoter(BioBrick b)
  {
    return (b.getType() == BioBrick.Type.PROMOTER);
  }

  private static bool checkRBS(BioBrick b)
  {
    return (b.getType() == BioBrick.Type.RBS);
  }

  private static bool checkGene(BioBrick b)
  {
    return (b.getType() == BioBrick.Type.GENE);
  }

  private static bool checkTerminator(BioBrick b)
  {
    return (b.getType() == BioBrick.Type.TERMINATOR);
  }  

  private static bool checkRBSGene(LinkedList<BioBrick> list)
  {
    if (list == null)
      return false;
    if (list.Count == 0 || list.First.Value == null)
      return false;
    if (checkRBS(list.First.Value) == false)
      return false;
    list.RemoveFirst();
    if (list.Count == 0 || list.First.Value == null)
      return false;
    if (checkGene(list.First.Value) == false)
      return false;
    list.RemoveFirst();
    return true;
  }

  private static bool checkOperon(LinkedList<BioBrick> bricks)
  {
    bool b = false;

    while (checkRBSGene(bricks))
      b = true;
    return b;
  }

  private static bool checkModuleValidity(ExpressionModule module)
  {
    Logger.Log("Device::checkModuleValidity("+module.ToString()+")", Logger.Level.TRACE);
    if (module == null) {
      Logger.Log("Device::checkModuleValidity failed (module == null)", Logger.Level.WARN);
      return false;
    }

    LinkedList<BioBrick> bricks = new LinkedList<BioBrick>(module.getBioBricks());
    if (bricks == null) {
      Logger.Log("Device::checkModuleValidity failed (bricks == null)", Logger.Level.WARN);
      return false;
    }
    if (bricks.Count == 0 || bricks.First.Value == null) {
      Logger.Log("Device::checkModuleValidity failed (bricks.Count == 0 || bricks.First.Value == null)", Logger.Level.WARN);
      return false;
    }
    if (checkPromoter(bricks.First.Value) == false) {
      Logger.Log("Device::checkModuleValidity failed (checkPromoter(bricks.First.Value) == false)", Logger.Level.WARN);
      return false;
    }
    bricks.RemoveFirst();

    if (checkOperon(bricks) == false) {
      Logger.Log("Device::checkModuleValidity failed (checkOperon(bricks) == false)", Logger.Level.WARN);
      return false;
    }

    bool b1 = (bricks.Count == 0);
    bool b2 = (bricks.First.Value == null);
    bool b3 = (checkTerminator(bricks.First.Value) == false);
    if (b1 || b2 || b3) {
      if (b1) Logger.Log("Device::checkModuleValidity failed (bricks.Count == 0) = true", Logger.Level.WARN);
      if (b2) Logger.Log("Device::checkModuleValidity failed (bricks.First.Value != null) = true", Logger.Level.WARN);
      if (b3) Logger.Log("Device::checkModuleValidity failed (checkTerminator(bricks.First.Value) == false) = true", Logger.Level.WARN);
      return false;
    }
    bricks.RemoveFirst();
    if (bricks.Count != 0) {
      Logger.Log("Device::checkModuleValidity failed (bricks.Count != 0)", Logger.Level.WARN);
      return false;
    }
    return true;
  }

  private static bool checkDeviceValidity(LinkedList<ExpressionModule> modules)
  {
    foreach (ExpressionModule em in modules)
      if (checkModuleValidity(em) == false)
        return false;
    return true;
  }

  public static Device buildDevice(string name, LinkedList<ExpressionModule> modules)
  {
    if (modules == null || checkDeviceValidity(modules) == false) {
      return null;
	  }
    Device device = new Device(name, modules);
    return device;
  }

  public static Device buildDevice(Device device)
  {
    if (device == null)
    {
      Logger.Log("Device::buildDevice device == null", Logger.Level.WARN);
      return null;
    }
    return buildDevice(device.getName(), device._modules);
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
}