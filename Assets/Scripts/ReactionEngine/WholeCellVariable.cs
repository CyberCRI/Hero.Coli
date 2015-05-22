using UnityEngine;
using System.Collections;

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
        //Logger.Log("_w"+_codeName+"="+result, Logger.Level.ONSCREEN);
        return result;
    }
}
