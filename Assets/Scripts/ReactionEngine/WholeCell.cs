using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class WholeCell : MonoBehaviour {

    // to control the height of the associated graph
    public float graphHeight;

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

    // time scale
    public float timeScale;

    //////////////////////////////////////////////////
    /// variables
    /// 

    
    //external nutrient
    public WholeCellVariable s;
    //internal nutrient
    public WholeCellVariable s_i;
    //energy, i.e. ATP
    public WholeCellVariable a;

    //ribosomes
    public WholeCellVariable r;
    //transporter enzyme
    public WholeCellVariable e_t;
    //metabolic enzyme
    public WholeCellVariable e_m;
    //housekeeping protein
    public WholeCellVariable q;
    
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
    // metabolism
    private float vimp = 0f;
    private float vcat = 0f;
    // ribosome binding
    float factor1 = 0f;
    float sumr = 0f;
    float sumt = 0f; 
    float summ = 0f;
    float sumq = 0f;


    /////////////////////////////////////////
    /// parameters (unit)
    /// NB: aa denotes number of amino acids
    
    // external nutrient ([molecs])
    // chosen relative to Kt
    public float s0 = 104f;

    //other initial conditions (arbitrary)
    public float s_i0 = 1f;
    public float a0 = 1f;
    public float r0 = 100f;
    public float e_t0 = 1f;
    public float e_m0 = 1f;
    public float q0 = 1f;
    public float mr0 = 10f;
    public float mt0 = 10f;
    public float mm0 = 10f;
    public float mq0 = 10f;
    public float cr0 = 1f;
    public float ct0 = 1f;
    public float cm0 = 1f;
    public float cq0 = 1f;
    
    // mRNA-degradation rate ([min−1 ])
    public float dm = 0.1f;
    
    // nutrient efficiency (none)
    // chosen such that maximal growth rate matches that of E.coli
    public float ns = 0.5f;
    
    // ribosome length ([aa/molecs])
    public float nr = 7459f;
    
    // length of non-ribosomal proteins ([aa/molecs])
    // E.coli’s average
    public float nx = 300f;
    // warning: nt, nm, nq are initialized to nx
    public float nt, nm, nq;
    
    // max. transl. elongation rate ([aa/min molecs])
    public float γmax = 1260f;
    
    // transl. elongation threshold ([molecs/cell])
    // Obtained by parameter optimization
    public float Kγ = 7f;
    
    // max. nutrient import rate ([min−1 ])
    public float vtmax = 726f;
    
    // nutrient import threshold ([molecs])
    public float Kt = 1000f;
    
    // max. enzymatic rate ([min−1 ])
    public float vmmax = 5800f;
    
    // enzymatic threshold ([molecs/cell])
    public float Km = 1000f;
    
    // max. ribosome transcription rate ([molecs/min cell])
    // Obtained by parameter optimization
    public float ωr = 930f;
    
    // max. enzyme transcription rate ([molecs/min cell])
    // Obtained by parameter optimization
    public float ωe = 4.14f;
    // warning: ωt, ωm are initialized to ωe
    public float ωt, ωm;
    
    // max. q-transcription rate ([molecs/min cell])
    // Obtained by parameter optimization
    public float ωq = 948.93f;
    
    // ribosome transcription threshold ([molecs/cell])
    // Obtained by parameter optimization
    public float θr = 426.87f;
    
    // non-ribosomal transcription threshold ([molecs/cell])
    // Obtained by parameter optimization
    public float θnr = 4.38f;
    
    // q-autoinhibition threshold ([molecs/cell])
    // Obtained by parameter optimization
    public float Kq = 152219f;
    
    // q-autoinhibition Hill coeff. (none)
    // for steep auto-inhibition
    public float hq = 4f;
    
    // mRNA-ribosome binding rate ([cell/min molecs])
    // near the diffusion limit
    public float kb = 1f;
    
    // mRNA-ribosome unbinding rate ([min−1 ])
    public float ku = 1f;
    
    // total cell mass ([aa])
    // order of magnitude
    public float M = 108f;
    
    // chloramphenicol-binding rate ([(minμM)−1])
    public float kcm = 0.00599f;

    void Start() {
        initialize ();
    }

    void OnEnable() {
        reset ();
    }

    private WholeCellVariable getNewWholeCellVariable(
        string codeName, string realName,
        float value = 0f,
        float degradation = 0f, 
        float omega = 0f,
        float theta = 0f,
        float import = 0f, 
        float binding = 0f, 
        float translation = 0f, 
        float metabolism = 0f
        )
    {
        return new WholeCellVariable(this, codeName, realName, value, degradation, omega, theta, import, binding, translation, metabolism);
    }

    void initialize() {

        nt = nx;
        nm = nx;
        nq = nx;

        ωt = ωe;
        ωm = ωe;

        
        s = getNewWholeCellVariable("s", "ext. nutrient", s0);
        s._isInternal = false;
        s_i = getNewWholeCellVariable("s_i", "int. nutrient", s_i0);
        a = getNewWholeCellVariable("a", "ATP", a0);
        WholeCellVariable.a = a;
        
        r = getNewWholeCellVariable("r", "ribosomes", r0);
        e_t = getNewWholeCellVariable("e_t", "t enzymes", e_t0);
        e_m = getNewWholeCellVariable("e_m", "m enzymes", e_m0);
        q = getNewWholeCellVariable("q", "hk proteins", q0);
        
        mr = getNewWholeCellVariable("mr", "r free mRNA", mr0, dm, ωr, θr);
        mt = getNewWholeCellVariable("mt", "t free mRNA", mt0, dm, ωt, θnr);
        mm = getNewWholeCellVariable("mm", "m free mRNA", mm0, dm, ωm, θnr);
        mq = getNewWholeCellVariable("mq", "hk free mRNA", mq0, dm, ωq, θnr);
        
        cr = getNewWholeCellVariable("cr", "r bound mRNA", cr0);
        ct = getNewWholeCellVariable("ct", "t bound mRNA", ct0);
        cm = getNewWholeCellVariable("cm", "m bound mRNA", cm0);
        cq = getNewWholeCellVariable("cq", "hk bound mRNA", cq0);

        //TODO change word variable => species
        _variables = new List<WholeCellVariable>(){s, s_i, a, r, e_t, e_m, q, mr, mt, mm, mq, cr, ct, cm, cq};
        _displayedVariables = new List<WholeCellVariable>(){s, s_i, a, r, e_t, e_m, q, mr, mt, mm, mq, cr, ct, cm, cq};
    }

    void reset ()
    {
        if(null != s) {
        
            nt = nx;
            nm = nx;
            nq = nx;

            ωt = ωe;
            ωm = ωe;

            s._value = s0;
            s_i._value = s_i0;
            a._value = a0;

            r._value = r0;
            e_t._value = e_t0;
            e_m._value = e_m0;
            q._value = q0;
              
            mr._value = mr0;
            mt._value = mt0;
            mm._value = mm0;
            mq._value = mq0;
              
            cr._value = cr0;
            ct._value = ct0;
            cm._value = cm0;
            cq._value = cq0;

        }
    }

    void Update() {
        
        //update of computation intermediary variables
        updateλ();
        float elapsedMinutes = timeScale*Time.deltaTime/60f;
        Logger.Log("dt="+elapsedMinutes+"min", Logger.Level.ONSCREEN);

        foreach(WholeCellVariable variable in _variables)
        {
            //reset
            variable._derivative = 0f;
        }
        
        if(computeTranslation){ // translation
            νr = getTranslationRate(cr._value, nr);
            νt = getTranslationRate(ct._value, nt);
            νm = getTranslationRate(cm._value, nm);
            νq = getTranslationRate(cq._value, nq);

            a._derivative += -νr*nr -νt*nt -νm*nm -νq*nq;

            r._derivative   += 2*νr + νt + νm + νq;
            e_t._derivative += νt;
            e_m._derivative += νm;
            q._derivative   += νq;
            
            mr._derivative += νr;
            mt._derivative += νt;
            mm._derivative += νm;
            mq._derivative += νq;
            
            cr._derivative += -νr;
            ct._derivative += -νt;
            cm._derivative += -νm;
            cq._derivative += -νq;
        }

        if(computeMetabolism){ // metabolism
            // primary, raw resource import
            vimp = getMichaelisMentenRate(e_t._value, s._value, vtmax, Kt);
            // primary, raw resource catabolism into secondary, refined resource
            vcat = getMichaelisMentenRate(e_m._value, s_i._value, vmmax, Km);

            s._derivative += -vimp;
            s_i._derivative += vimp - vcat;
            a._derivative += ns * vcat;
        }

        if(computeRibosomeBinding){ // ribosome binding

            factor1 = kb * r._value;

            sumr = factor1 * mr._value - ku * cr._value;
            sumt = factor1 * mt._value - ku * ct._value;
            summ = factor1 * mm._value - ku * cm._value;
            sumq = factor1 * mq._value - ku * cq._value;

            r._derivative += -sumr -sumt -summ -sumq;

            mr._derivative = -sumr;
            mt._derivative = -sumt;
            mm._derivative = -summ;
            mq._derivative = -sumq;
            
            cr._derivative = sumr;
            ct._derivative = sumt;
            cm._derivative = summ;
            cq._derivative = sumq;
        }

        foreach(WholeCellVariable variable in _variables)
        {
            if(computeDilution && variable._isInternal){variable._derivative += -λ*variable._value;} // dilution
            if(computeDegradation){variable._derivative += -variable._degradation*variable._value;} // degradation
            // translation is managed before this loop
            if(computeTranscription){variable._derivative += variable.getTranscription();} // transcription
            // metabolism is managed before this loop
            // ribosome binding is managed before this loop


            // derivative is in min-1
            // Time.deltaTime is in seconds
            float variation = variable._derivative*elapsedMinutes;
            variable._value += variation;

            if(10e-10 > variable._value) { variable._value = 0f; }
        }
    }

    // TODO refactor with ReactionEngine's version
    // e is the enzyme concentration
    // s is the substrate concentration
    // vmax is the maximum rate
    // _K is the reaction constant
    private float getMichaelisMentenRate(float e, float s, float vmax, float _K)
    {
        return e * (vmax*s)/(_K+s);
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
    private static WholeCell _simulator;
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

    // dilution, only for internal species
    public bool _isInternal;


    //derivative, as computation intermediary
    public float _derivative;


    public WholeCellVariable(
        WholeCell simulator,
        string codeName, string realName,
        float value = 0f,
        float degradation = 0f, 
        float omega = 0f,
        float theta = 0f,
        float import = 0f, 
        float binding = 0f, 
        float translation = 0f, 
        float metabolism = 0f
        ) {

        _simulator = simulator;

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
        _isInternal = true;
    }

    public float getTranscription() {
        float result = 0f;
        if(0 != _ω) {
            // autoinhibition
            float _Iq = 1f;
            if(_codeName == "q") {
                _Iq = 1f/(1f + Mathf.Pow(_value/_simulator.Kq,_simulator.hq));
            }

            result = _ω*a._value*_Iq/(_θ+a._value);
        }
        Logger.Log("_w"+_codeName+"="+result, Logger.Level.ONSCREEN);
        return result;
    }
}
