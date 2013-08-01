using System;
using System.Collections.Generic;
using UnityEngine;

public class Device
{
  private string                        _name;
  private LinkedList<ExpressionModule>	_modules;
  
  public string getName() { return _name; }
  public void setName(string v) { _name = v; }
  public LinkedList<ExpressionModule> getBioBricks() { return _modules; }

  public int getSize()
  {
    int sum = 0;

    foreach (ExpressionModule em in _modules)
      sum += em.getSize();
    return sum;
  }
  
  private Device()
  {}

  private Device(string name, LinkedList<ExpressionModule> modules)
  {
    _name = name;
    _modules = new LinkedList<ExpressionModule>();
    foreach (ExpressionModule em in modules)
      _modules.AddLast(new ExpressionModule(em));
  }
	
  public override string ToString ()
  {
		string modulesString = "Modules: [";
		foreach (ExpressionModule module in _modules) {
			modulesString += (module.ToString() + ", ");
		}
		modulesString += "]";
		
		return string.Format ("[Device: name: {0}, modules: {1}]", _name, modulesString);
  }

  public LinkedList<Product> getProductsFromBiobricks(LinkedList<BioBrick> list)
  {
    LinkedList<Product> products = new LinkedList<Product>();
    Product prod;
    RBSBrick rbs;
    GeneBrick gene;
    float RBSf = 0f;
    string molName = "Unknown";
    int i = 0;

    foreach (BioBrick b in list)
      {
        rbs = b as RBSBrick;
        if (rbs != null)
          RBSf = rbs.getRBSFactor();
        else
          {
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
              Debug.Log("This case should never arrive. Bad Biobrick in operon.");
          }
        i++;
      }
    while (i > 0)
      list.RemoveFirst();
    return products;
  }

  public PromoterProprieties getPromoterReaction(ExpressionModule em, int id)
  {
    PromoterProprieties prom = new PromoterProprieties();
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

    prom.energyCost = getSize();
    return prom;
  }

  public LinkedList<PromoterProprieties> getPromoterReactions()
  {
    LinkedList<ExpressionModule> modules = new LinkedList<ExpressionModule>(_modules);
    LinkedList<PromoterProprieties> reactions = new LinkedList<PromoterProprieties>();
    PromoterProprieties reaction;
    int i = 0;

    foreach (ExpressionModule em in modules)
      {
        reaction = getPromoterReaction(em, i);
        if (reaction != null)
          reactions.AddLast(reaction);
        i++;
      }
    if (reactions.Count == 0)
      return null;
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
    if (module == null)
      return false;

    LinkedList<BioBrick> bricks = new LinkedList<BioBrick>(module.getBioBricks());
    if (bricks == null)
      return false;
    if (bricks.Count == 0 || bricks.First.Value == null)
      return false;
    if (checkPromoter(bricks.First.Value) == false)
      return false;
    bricks.RemoveFirst();

    if (checkOperon(bricks) == false)
      return false;

    if (bricks.Count == 0 || bricks.First.Value != null || checkTerminator(bricks.First.Value) == false)
      return false;
    bricks.RemoveFirst();
    if (bricks.Count != 0)
      return false;
    return true;
  }

  private static bool checkDeviceValidity(LinkedList<ExpressionModule> device)
  {
    foreach (ExpressionModule em in device)
      if (checkModuleValidity(em) == false)
        return false;
    return true;
  }

  public static Device buildDevice(string name, LinkedList<ExpressionModule> bricks)
  {
    if (bricks == null || checkDeviceValidity(bricks) == false)
      return null;
    Device device = new Device(name, bricks);
    return device;
  }
	
	//helper for simple devices
	public static Device buildDevice(string name,
		float beta,//promoter
		string formula,//promoter
		float rbsFactor,//rbs
		string proteinName,//gene
		float terminatorFactor//terminator
		) {
		
		BioBrick[] bioBrickArray = {
			new PromoterBrick(beta, formula),
			new RBSBrick(rbsFactor),
			new GeneBrick(proteinName),
			new TerminatorBrick(terminatorFactor)
		};
		
		LinkedList<BioBrick> bricks = new LinkedList<BioBrick>(bioBrickArray);
		
		ExpressionModule[] modulesArray = { new ExpressionModule(name+"_module", bricks) };
		LinkedList<ExpressionModule> modules = new LinkedList<ExpressionModule>(modulesArray);
		
		return Device.buildDevice(name, modules);
	}
}