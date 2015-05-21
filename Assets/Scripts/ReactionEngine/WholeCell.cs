using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class WholeCell : MonoBehaviour {

    //////////////////////////////////////////////////
    /// variables
    /// 

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


    /////////////////////////////////////////
    /// parameters (unit)
    /// NB: aa denotes number of amino acids
    
    // external nutrient ([molecs])
    // chosen relative to Kt
    private static float s = 104f;
    
    // mRNA-degradation rate ([min−1 ])
    private static float dm = 0.1f;
    
    // nutrient efficiency (none)
    // chosen such that maximal growth rate matches that of E.coli
    private static float ns = 0.5f;
    
    // ribosome length ([aa/molecs])
    private static float nr = 7459f;
    
    // length of non-ribosomal proteins ([aa/molecs])
    // E.coli’s average
    private static float nx = 300f;
    private static float nt = nx, nm = nx, nq = nx;
    
    // max. transl. elongation rate ([aa/min molecs])
    private static float γmax = 1260f;
    
    // transl. elongation threshold ([molecs/cell])
    // Obtained by parameter optimization
    private static float Kγ = 7f;
    
    // max. nutrient import rate ([min−1 ])
    private static float vt = 726f;
    
    // nutrient import threshold ([molecs])
    private static float Kt = 1000f;
    
    // max. enzymatic rate ([min−1 ])
    private static float vm = 5800f;
    
    // enzymatic threshold ([molecs/cell])
    private static float Km = 1000f;
    
    // max. ribosome transcription rate ([molecs/min cell])
    // Obtained by parameter optimization
    private static float wr = 930f;
    
    // max. enzyme transcription rate ([molecs/min cell])
    // Obtained by parameter optimization
    private static float wx = 4.14f;
    private static float we = wx, wt = wx, wm = wx;
    
    // max. q-transcription rate ([molecs/min cell])
    // Obtained by parameter optimization
    private static float wq = 948.93f;
    
    // ribosome transcription threshold ([molecs/cell])
    // Obtained by parameter optimization
    private static float θr = 426.87f;
    
    // non-ribosomal transcription threshold ([molecs/cell])
    // Obtained by parameter optimization
    private static float θnr = 4.38f;
    
    // q-autoinhibition threshold ([molecs/cell])
    // Obtained by parameter optimization
    private static float Kq = 152219f;
    
    // q-autoinhibition Hill coeff. (none)
    // for steep auto-inhibition
    private static float hq = 4f;
    
    // mRNA-ribosome binding rate ([cell/min molecs])
    // near the diffusion limit
    private static float kb = 1f;
    
    // mRNA-ribosome unbinding rate ([min−1 ])
    private static float ku = 1f;
    
    // total cell mass ([aa])
    // order of magnitude
    private static float M = 108f;
    
    // chloramphenicol-binding rate ([(minμM)−1])
    private static float kcm = 0.00599f;



    void Start() {

        s_i = new WholeCellVariable("s_i", "internal nutrient", 500, .002f);
        a = new WholeCellVariable("a", "ATP", 200, .001f);

        r = new WholeCellVariable("r", "ribosomes", 10000, .05f, 100);
        e_t = new WholeCellVariable("e_t", "t enzymes", 5000, .02f, 50);
        e_m = new WholeCellVariable("e_m", "m enzymes", 2000, .01f, 20);
        q = new WholeCellVariable("q", "hk proteins", 1000, .005f, 10);

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
            // dilution
            variable._value = (1-variable._dilution)*variable._value;
            // degradation
            variable._value = (1-variable._degradation)*variable._value;
            // transcription
            variable._value = variable._value + variable._transcription;
      }
    }
}

public class WholeCellVariable {
    public string _codeName;
    public string _realName;
    public float _value;

    // dilution, aka lambda (ratio per second)
    public float _dilution;

    // degradation (ratio per second)
    public float _degradation;

    // transcription, aka omega (amount per second)
    public float _transcription;

    // import (amount per second)
    public float _import;

    // ribosome binding
    public float _binding;

    // translation
    public float _translation;

    // metabolism
    public float _metabolism;

    public WholeCellVariable(string codeName, string realName, float value, 
                             float dilution, 
                             float degradation = 0f, 
                             float transcription = 0f,
                             float import = 0f, 
                             float binding = 0f, 
                             float translation = 0f, 
                             float metabolism = 0f
                             ) {

        _codeName = codeName;
        _realName = realName;
        _value = value;

        _dilution = dilution;
        _degradation = degradation;
        _transcription = transcription;
        _import = import;
        _binding = binding;
        _translation = translation;
        _metabolism = metabolism;
    }
}
