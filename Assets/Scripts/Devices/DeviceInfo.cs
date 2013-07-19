
using System.Collections;
using System.Collections.Generic;

public class DeviceInfo {
	public int _id = -1;
	public string _name;
	public List<ModuleInfo> _modules;
	
	public DeviceInfo(int id, string name, List<ModuleInfo> modules) {
		_id = id;
		_name = name;
		_modules = modules;
	}
	
	public override string ToString ()
	{
		string res = "DeviceInfo[_id:"+_id+",_name:"+_name+",";
		_modules.ForEach(module => res = res+module.ToString()+",");
		return res+"]";
	}
}

