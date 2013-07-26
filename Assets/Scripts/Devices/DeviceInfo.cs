
using System.Collections;
using System.Collections.Generic;

public class DeviceInfo {
	public int _id = -1;
	public string _name;
	public string _spriteName;
	public List<ModuleInfo> _modules;
	
	public DeviceInfo(int id, string name, string spriteName, List<ModuleInfo> modules) {
		_id = id;
		_name = name;
		_spriteName = spriteName;
		_modules = modules;
	}
	
	//helper for simple devices
	public DeviceInfo(
		int id,		
		string deviceName,
		string spriteName,
		string promoterName,
		float productionMax,
		float terminatorFactor,
		string formula,
		string product,
		float rbs) {
		ModuleInfo moduleInfo = new ModuleInfo(
			"pLac",
			10.0f,
			1.0f,
			"![0.8,2]LacI",
			"GFP",
			1.0f
			);
		List<ModuleInfo> modules = new List<ModuleInfo>();
		modules.Add(moduleInfo);
				
		_id = id;
		_name = deviceName;
		_spriteName = spriteName;
		_modules = modules;
	}
	
	public override string ToString ()
	{
		string res = "DeviceInfo[_id:"+_id+", _name:"+_name+", _spriteName:"+_spriteName+", ";
		_modules.ForEach(module => res = res+module.ToString()+",");
		return res+"]";
	}
}

