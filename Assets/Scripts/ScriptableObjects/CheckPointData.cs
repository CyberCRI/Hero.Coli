using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class DeviceEquippedData
{
    public DeviceData deviceData;
    public bool equipped = false;

    public override string ToString()
    {
        return this.GetType() + "[deviceData=" + deviceData + "]";
    }
}

[CreateAssetMenu(fileName = "CheckPoint", menuName = "Data/CheckPointData", order = 22)]
public class CheckPointData : BiobrickListData
{
    public CheckPointData previous = null;
    public DeviceEquippedData[] deviceDataList;
    public int requiredSlots;

    bool isListCircular()
    {
        if (previous == null)
        {
            return false;
        }
        var first = this;
        var second = previous;
        while (first != null && second != null && second.previous != null)
        {
            if (Object.ReferenceEquals(first, second))
            {
                return true;
            }
            first = first.previous;
            second = second.previous.previous;
        }
        return false;
    }

    void OnValidate()
    {
        // Prevents the creation of a circular list
        if (isListCircular())
        {
            Debug.LogError(this.GetType() + " Circular list !");
            previous = null;
        }
    }

    public override string ToString()
    {
        string biobrickDataListString = Logger.ToString<BiobrickDataCount>(biobrickDataList);
        string deviceDataListString = Logger.ToString<DeviceEquippedData>(deviceDataList);

        return this.GetType() + "[biobricks=" + biobrickDataListString + ", devices=" + deviceDataListString + "]";
    }
}

