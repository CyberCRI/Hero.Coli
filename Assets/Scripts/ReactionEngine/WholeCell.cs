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

    public List<WholeCellVariable> _variables = new List<WholeCellVariable>();

    void Start() {
        r = new WholeCellVariable("ribosomes", 0);
        e_t = new WholeCellVariable("t enzymes", 0);
        e_m = new WholeCellVariable("m enzymes", 0);
        q = new WholeCellVariable("hk proteins", 0);
        s_i = new WholeCellVariable("internal nutrient", 0);
        a = new WholeCellVariable("ATP", 0);

        _variables = new List<WholeCellVariable>(){r, e_t, e_m, q, s_i, a};
    }

    void Update() {
    }
}

public class WholeCellVariable{
    public string _name;
    public float _value;

    public WholeCellVariable(string name, float value){
        _name = name;
        _value = value;
    }
}
