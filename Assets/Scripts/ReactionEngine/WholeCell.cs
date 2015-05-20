using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class WholeCell : MonoBehaviour {
    //ribosomes
    public WholeCellVariable r;
    //transporter enzyme
    public WholeCellVariable e_t;
    //metabolic enzyme
    public WholeCellVariable e_m;
    //housekeeping protein
    public WholeCellVariable q;
    //internal nutrient
    public WholeCellVariable s_i;
    //energy, i.e. ATP
    public WholeCellVariable a;
    
    //ribosomes
    public WholeCellVariable mr;
    //transporter enzyme
    public WholeCellVariable me_t;
    //metabolic enzyme
    public WholeCellVariable me_m;
    //housekeeping protein
    public WholeCellVariable mq;
    
    //ribosomes
    public WholeCellVariable cr;
    //transporter enzyme
    public WholeCellVariable ce_t;
    //metabolic enzyme
    public WholeCellVariable ce_m;
    //housekeeping protein
    public WholeCellVariable cq;
    
    public List<WholeCellVariable> _variables = new List<WholeCellVariable>();
    public List<WholeCellVariable> _displayedVariables = new List<WholeCellVariable>();

    void Start() {
        r = new WholeCellVariable("r", "ribosomes", 10000, .05f);
        e_t = new WholeCellVariable("e_t", "t enzymes", 5000, .02f);
        e_m = new WholeCellVariable("e_m", "m enzymes", 2000, .01f);
        q = new WholeCellVariable("q", "hk proteins", 1000, .005f);
        s_i = new WholeCellVariable("s_i", "internal nutrient", 500, .002f);
        a = new WholeCellVariable("a", "ATP", 200, .001f);

        
        mr = new WholeCellVariable("r", "ribosomes", 10000, .05f);
        me_t = new WholeCellVariable("e_t", "t enzymes", 5000, .02f);
        me_m = new WholeCellVariable("e_m", "m enzymes", 2000, .01f);
        mq = new WholeCellVariable("q", "hk proteins", 1000, .005f);

        
        cr = new WholeCellVariable("r", "ribosomes", 10000, .05f);
        ce_t = new WholeCellVariable("e_t", "t enzymes", 5000, .02f);
        ce_m = new WholeCellVariable("e_m", "m enzymes", 2000, .01f);
        cq = new WholeCellVariable("q", "hk proteins", 1000, .005f);

        _variables = new List<WholeCellVariable>(){r, e_t, e_m, q, s_i, a, mr, me_t, me_m, mq, cr, ce_t, ce_m, cq,};
        _displayedVariables = new List<WholeCellVariable>(){r, e_t, e_m, q, s_i, a};
    }

    void Update() {
      foreach(WholeCellVariable variable in _variables)
      {
          variable._value = (1-variable._degradation)*variable._value;
      }
    }
}

public class WholeCellVariable {
    public string _codeName;
    public string _realName;
    public float _value;
    public float _degradation;

    public WholeCellVariable(string codeName, string realName, float value, float degradation) {
        _codeName = codeName;
        _realName = realName;
        _value = value;
        _degradation = degradation;

    }
}
