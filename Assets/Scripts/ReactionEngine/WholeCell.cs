using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class WholeCell : MonoBehaviour {

    // enable/disable dilution computation
    public bool computeDilution;
    // enable/disable degradation computation
    public bool computeDegradation;
    // enable/disable translation computation
    public bool computeTranslation;
    // enable/disable transcription computation
    public bool computeTranscription;
    // enable/disable metabolism computation
    public bool computeMetabolism;
    // enable/disable ribosome binding computation
    public bool computeRibosomeBinding;

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
    public WholeCellVariable mt;
    //metabolic enzyme
    public WholeCellVariable mm;
    //housekeeping protein
    public WholeCellVariable mq;
    
    //ribosomes
    public WholeCellVariable cr;
    //transporter enzyme
    public WholeCellVariable ct;
    //metabolic enzyme
    public WholeCellVariable cm;
    //housekeeping protein
    public WholeCellVariable cq;

    public List<WholeCellVariable> _variables = new List<WholeCellVariable>();
    public List<WholeCellVariable> _displayedVariables = new List<WholeCellVariable>();
    
    
    // computation intermediaries
    // dilution
    private float λ = 0f;
    // translation
    private float νr = 0f;
    private float νt = 0f;
    private float νm = 0f;
    private float νq = 0f;



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
    private static float ωr = 930f;
    
    // max. enzyme transcription rate ([molecs/min cell])
    // Obtained by parameter optimization
    private static float ωe = 4.14f;
    private static float ωt = ωe, ωm = ωe;
    
    // max. q-transcription rate ([molecs/min cell])
    // Obtained by parameter optimization
    private static float ωq = 948.93f;
    
    // ribosome transcription threshold ([molecs/cell])
    // Obtained by parameter optimization
    private static float θr = 426.87f;
    
    // non-ribosomal transcription threshold ([molecs/cell])
    // Obtained by parameter optimization
    private static float θnr = 4.38f;
    
    // q-autoinhibition threshold ([molecs/cell])
    // Obtained by parameter optimization
    public static float Kq = 152219f;
    
    // q-autoinhibition Hill coeff. (none)
    // for steep auto-inhibition
    public static float hq = 4f;
    
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

        s_i = new WholeCellVariable("s_i", "internal nutrient", 500);
        a = new WholeCellVariable("a", "ATP", 200);
        WholeCellVariable.a = a;

        r = new WholeCellVariable("r", "ribosomes", 1);
        e_t = new WholeCellVariable("e_t", "t enzymes", 1);
        e_m = new WholeCellVariable("e_m", "m enzymes", 1);
        q = new WholeCellVariable("q", "hk proteins", 1);

        mr = new WholeCellVariable("mr", "r free mRNA", 1000, dm, ωr, θr);
        mt = new WholeCellVariable("mt", "t free mRNA", 100, dm, ωt, θnr);
        mm = new WholeCellVariable("mm", "m free mRNA", 10, dm, ωm, θnr);
        mq = new WholeCellVariable("mq", "hk free mRNA", 1, dm, ωq, θnr);
        
        cr = new WholeCellVariable("cr", "r bound mRNA", 1);
        ct = new WholeCellVariable("ct", "t bound mRNA", 1);
        cm = new WholeCellVariable("cm", "m bound mRNA", 1);
        cq = new WholeCellVariable("cq", "hk bound mRNA", 1);

        //TODO change word variable => species
        _variables = new List<WholeCellVariable>(){r, e_t, e_m, q, s_i, a, mr, mt, mm, mq, cr, ct, cm, cq};
        _displayedVariables = new List<WholeCellVariable>(){r, e_t, e_m, q, s_i, a, mr, mt, mm, mq, cr, ct, cm, cq};
    }

    void Update() {
        
        //update of computation intermediary variables
        updateλ();
        float elapsedMinutes = Time.deltaTime/60f;
        Logger.Log("dt="+elapsedMinutes+"min", Logger.Level.ONSCREEN);

        foreach(WholeCellVariable variable in _variables)
        {
            //reset
            variable._derivative = 0;
        }
        
        if(computeTranslation){ // translation
            νr = getTranslationRate(cr._value, nr);
            νt = getTranslationRate(ct._value, nt);
            νm = getTranslationRate(cm._value, nm);
            νq = getTranslationRate(cq._value, nq);

            a._derivative -= νr*nr + νt*nt + νm*nm + νq*nq;

            r._derivative   += 2*νr + νt + νm + νq;
            e_t._derivative += νt;
            e_m._derivative += νm;
            q._derivative   += νq;
            
            mr._derivative += νr;
            mt._derivative += νt;
            mm._derivative += νm;
            mq._derivative += νq;
            
            cr._derivative -= νr;
            ct._derivative -= νt;
            cm._derivative -= νm;
            cq._derivative -= νq;
        }

        foreach(WholeCellVariable variable in _variables)
        {
            if(computeDilution){variable._derivative += -λ*variable._value;} // dilution
            if(computeDegradation){variable._derivative += -variable._degradation*variable._value;} // degradation
            // translation is managed before this loop
            if(computeTranscription){variable._derivative += variable.getTranscription();} // transcription
            if(computeMetabolism){}// metabolism
            if(computeRibosomeBinding){}// ribosome binding


            // derivative is in min-1
            // Time.deltaTime is in seconds
            float variation = variable._derivative*elapsedMinutes;
            variable._value += variation;

            if(10e-10 > variable._value) { variable._value = 0f; }
        }
    }

    private float getTranslationRate(float cx, float nx)
    {
        return cx * γ() / nx;
    }

    void updateλ() {
        float Rt = cr._value + ct._value + cm._value + cq._value;
        λ = γ()*Rt/M;

        Logger.Log("lambda="+λ, Logger.Level.ONSCREEN);
    }

    private float γ(){
        return γmax*a._value/(Kγ+a._value);
    }
}

public class WholeCellVariable {
    public static WholeCellVariable a;
    public string _codeName;
    public string _realName;
    public float _value;

    // degradation (ratio per second)
    public float _degradation;

    // max. transcription rate ([molecs/min cell]), used for the computation of transcription
    public float _ω;
    // transcription threshold ([molecs/cell]), used for the computation of transcription
    public float _θ;

    // import (amount per second)
    public float _import;

    // ribosome binding
    public float _binding;

    // translation
    public float _translation;

    // metabolism
    public float _metabolism;


    //derivative, as computation intermediary
    public float _derivative;


    public WholeCellVariable(string codeName, string realName, float value,
                             float degradation = 0f, 
                             float omega = 0f,
                             float theta = 0f,
                             float import = 0f, 
                             float binding = 0f, 
                             float translation = 0f, 
                             float metabolism = 0f
                             ) {

        _codeName = codeName;
        _realName = realName;
        _value = value;

        _degradation = degradation;
        _ω = omega;
        _θ = theta;
        _import = import;
        _binding = binding;
        _translation = translation;
        _metabolism = metabolism;

        _derivative = 0f;
    }

    public float getTranscription() {
        float result = 0f;
        if(0 != _ω) {
            // autoinhibition
            float _Iq = 1f;
            if(_codeName == "q") {
                _Iq = 1f/(1f + Mathf.Pow(_value/WholeCell.Kq,WholeCell.hq));
            }

            result = _ω*a._value*_Iq/(_θ+a._value);
        }
        Logger.Log("_w"+_codeName+"="+result, Logger.Level.ONSCREEN);
        return result;
    }
}
