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
	public string _geneName;
	public float _rbsFactor = 1.0f;
	
	public ModuleInfo(
			string promoterName,
			float productionMax,
			float terminatorFactor,
			string formula,
			string geneName,
			float rbsFactor) {
		_promoterName = promoterName;
		_productionMax = productionMax;
		_terminatorFactor = terminatorFactor;
		_formula = formula;
		_geneName = geneName;
		_rbsFactor = rbsFactor;
	}
	
	public ModuleInfo copy() {
		return new ModuleInfo(
			_promoterName = _promoterName,
			_productionMax = _productionMax,
			_terminatorFactor = _terminatorFactor,
			_formula = _formula,
			_geneName = _geneName,
			_rbsFactor = _rbsFactor);
	}
	
	
	public override string ToString ()
	{
		return "ModuleInfo["
			+"_promoterName = "+_promoterName
			+", _productionMax = "+_productionMax
			+", _terminatorFactor = "+_terminatorFactor
			+", _formula = "+_formula
			+", _geneName = "+_geneName
			+", _rbsFactor = "+_rbsFactor
			+"]";
	}
}