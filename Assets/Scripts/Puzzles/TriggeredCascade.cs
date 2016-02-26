using System.Collections.Generic;

public class TriggeredCascade : TriggeredBehaviour {

    public List<TriggeredBehaviour> toTrigger;
    
    public override void triggerStart(){
		foreach(TriggeredBehaviour tb in toTrigger)
		{
			if(tb.gameObject != null)
				tb.triggerStart();
		}
	}
	
	public override void triggerExit(){
		foreach(TriggeredBehaviour tb in toTrigger)
		{
			if(tb.gameObject != null)
				tb.triggerExit();
		}
	}
	
	public override void triggerStay(){
        foreach(TriggeredBehaviour tb in toTrigger)
		{
			if(tb.gameObject != null)
				tb.triggerStay();
		}
    }
}
