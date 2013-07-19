using System.Collections;

public class DeviceInfo {
	
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
	
	//id
	public int _id = -1;
	
	//promoter
	public string _promoterName;
	public float _productionMax;
	public float _terminatorFactor = 1.0f;
	public float _formulaFactor1;
	public float _formulaFactor2;
	public string _formulaName;
	
	//operon
	public string _geneName;
	public float _rbsFactor = 1.0f;
	
	public DeviceInfo(
			string promoterName,
			float productionMax,
			float terminatorFactor,
			float formulaFactor1,
			float formulaFactor2,
			string formulaName,
			string geneName,
			float rbsFactor) {
		_promoterName = promoterName;
		_productionMax = productionMax;
		_terminatorFactor = terminatorFactor;
		_formulaFactor1 = formulaFactor1;
		_formulaFactor2 = formulaFactor2;
		_formulaName = formulaName;
		_geneName = geneName;
		_rbsFactor = rbsFactor;
	}
	
	public DeviceInfo copy() {
		return new DeviceInfo(
			_promoterName = _promoterName,
			_productionMax = _productionMax,
			_terminatorFactor = _terminatorFactor,
			_formulaFactor1 = _formulaFactor1,
			_formulaFactor2 = _formulaFactor2,
			_formulaName = _formulaName,
			_geneName = _geneName,
			_rbsFactor = _rbsFactor);
	}
	
	public override string ToString ()
	{
		return "DeviceInfo["
			+"_promoterName = "+_promoterName
			+", _productionMax = "+_productionMax
			+", _terminatorFactor = "+_terminatorFactor
			+", _formulaFactor1 = "+_formulaFactor1
			+", _formulaFactor2 = "+_formulaFactor2
			+", _formulaName = "+_formulaName
			+", _geneName = "+_geneName
			+", _rbsFactor = "+_rbsFactor
			+"]";
	}
}


