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
	
	public string _promoterName;
	public float _productionMax;
	public float _terminatorFactor = 1.0f;
	public string _formula;
	public LinkedList<Product> _products;
	
	public ModuleInfo(
			string promoterName,
			float productionMax,
			float terminatorFactor,
			string formula,
			LinkedList<Product> products) {
		_promoterName = promoterName;
		_productionMax = productionMax;
		_terminatorFactor = terminatorFactor;
		_formula = formula;
		_products = products;
	}
	
	public PromoterProprieties getProprieties() {
			PromoterProprieties proprieties = new PromoterProprieties();
			
  			proprieties.name = _promoterName;
  			proprieties.beta = _productionMax;
  			proprieties.terminatorFactor = _terminatorFactor;
  			proprieties.formula = _formula;
  			proprieties.products = new LinkedList<Product>(_products);
  			proprieties.energyCost = 0;
		
		return proprieties;
	}
	
	public void addToReactionEngine(int mediumID, ReactionEngine reactionEngine) {		
		Debug.Log("module promoter: "+_promoterName);
		
		PromoterProprieties proprieties = getProprieties();
		Debug.Log("proprieties="+proprieties.ToString());
		
		IReaction reaction = Promoter.buildPromoterFromProps(proprieties);
		Debug.Log("reaction="+reaction);
		
		reactionEngine.addReactionToMedium(mediumID, reaction);
	}
	
	
	public override string ToString ()
	{
		string res = "ModuleInfo["
			+"_promoterName = "+_promoterName
			+", _productionMax = "+_productionMax
			+", _terminatorFactor = "+_terminatorFactor
			+", _formula = "+_formula+", Products[";
		IEnumerator<Product> enumerator = _products.GetEnumerator();
		while (enumerator.MoveNext()) {
		  res = res + enumerator.Current.ToString()+", ";
		}
		res+="]";
		return res;
	}
}