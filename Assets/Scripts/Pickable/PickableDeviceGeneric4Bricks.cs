using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PickableDeviceGeneric4Bricks : PickableDevice {

  public string promoterName;
  public float promoterBeta;
  public string promoterFormula;

  public string rbsName;
  public float rbsFactor;

  public string geneName;
  public string geneProteinName;

  public string terminatorName;
  public float terminatorFactor;

  public string expressionModuleName;

  public string deviceName;

  protected override DNABit produceDNABit()
  {
    PromoterBrick prom = new PromoterBrick(promoterName, promoterBeta, promoterFormula);
    RBSBrick rbs = new RBSBrick(rbsName, rbsFactor);
    GeneBrick gene = new GeneBrick(geneName, geneProteinName);
    TerminatorBrick term = new TerminatorBrick(terminatorName, terminatorFactor);

    LinkedList<BioBrick> bricks = new LinkedList<BioBrick>(new List<BioBrick>(){prom, rbs, gene, term});
    ExpressionModule module = new ExpressionModule(expressionModuleName, bricks);
        
        Device result = Device.buildDevice(deviceName, new LinkedList<ExpressionModule>(new List<ExpressionModule>(){module}));
        return result;
  }
}
