using UnityEngine;
using System.Collections.Generic;

public class ModuleInfo {
	
	/*
	<promoter>
      <name>pLac</name>
      <productionMax>10</productionMax>
      <terminatorFactor>1</terminatorFactor>
      <formula>![0.8,2]LacI</formula>
      <operon>
		<gene>
		  <name>GFP</name>
		  <RBSFactor>1</RBSFactor>
		</gene>
      </operon>
    </promoter>
    */
	
	//promoter
	public string _promoterName;
	public float _productionMax;
	public float _terminatorFactor = 1.0f;
	public string _formula;
	
	//operon
	public string _proteinName;
	public float _rbsFactor = 1.0f;
	
	public ModuleInfo(
			string promoterName,
			float productionMax,
			float terminatorFactor,
			string formula,
			string proteinName,
			float rbsFactor) {
		_promoterName = promoterName;
		_productionMax = productionMax;
		_terminatorFactor = terminatorFactor;
		_formula = formula;
		_proteinName = proteinName;
		_rbsFactor = rbsFactor;
	}
	
	public PromoterProprieties getProprieties() {
			PromoterProprieties proprieties = new PromoterProprieties();
			
  			proprieties.name = _promoterName;
  			proprieties.beta = _productionMax;
  			proprieties.terminatorFactor = _terminatorFactor;
  			proprieties.formula = _formula;
		
		//
			LinkedList<Product> products = new LinkedList<Product>();
			products.AddLast(new Product(_proteinName, _rbsFactor));
  			proprieties.products = products;
  			proprieties.energyCost = 0;
		
		return proprieties;
	}
	
	public void addToReactionEngine(int mediumID, ReactionEngine reactionEngine) {		
		Debug.Log("module promoter: "+_promoterName);
		
		PromoterProprieties proprieties = getProprieties();
		Debug.Log("proprieties="+proprieties);
		
		IReaction reaction = Promoter.buildPromoterFromProps(proprieties);
		Debug.Log("reaction="+reaction);
		
		reactionEngine.addReactionToMedium(mediumID, reaction);
	}
	
	
	public override string ToString ()
	{
		return "ModuleInfo["
			+"_promoterName = "+_promoterName
			+", _productionMax = "+_productionMax
			+", _terminatorFactor = "+_terminatorFactor
			+", _formula = "+_formula
			+", _proteinName = "+_proteinName
			+", _rbsFactor = "+_rbsFactor
			+"]";
	}
}