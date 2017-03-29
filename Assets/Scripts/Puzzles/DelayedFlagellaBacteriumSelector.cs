using UnityEngine;
using System.Collections;

public class DelayedFlagellaBacteriumSelector : BacteriumSelector
{
    [SerializeField]
    private string _redmetricsName;
    [SerializeField]
    private int _requiredFlagellaCount;
    [SerializeField]
    private float _delay;

    protected override bool isBacteriumSelected()
    {
        bool result = CellControl.get(this.GetType().ToString()).hasAtLeastFlagellaCount(_requiredFlagellaCount);
        // Debug.Log(this.GetType() + " isBacteriumSelected with result=" + result);
        return result;
    }

    protected override void getRidOf()
    {
        // Debug.Log(this.GetType() + " getRidOf");
        StartCoroutine(kill());
    }

    private IEnumerator kill()
    {
        yield return new WaitForSeconds(_delay);
        Character.get().kill(new CustomData(CustomDataTag.SOURCE, _redmetricsName));
        yield return null;
    }
}