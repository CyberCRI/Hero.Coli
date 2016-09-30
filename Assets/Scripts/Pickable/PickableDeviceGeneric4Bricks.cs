using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PickableDeviceGeneric4Bricks : PickableDevice {

  [SerializeField]
  private string promoterName;
  [SerializeField]
  private float promoterBeta;
  [SerializeField]
  private string promoterFormula;
  [SerializeField]
  private int promoterSize;

  [SerializeField]
  private string rbsName;
  [SerializeField]
  private float rbsFactor;
  [SerializeField]
  private int rbsSize;

  [SerializeField]
  private string geneName;
  [SerializeField]
  private string geneProteinName;
  [SerializeField]
  private int geneSize;

  [SerializeField]
  private string terminatorName;
  [SerializeField]
  private float terminatorFactor;
  [SerializeField]
  private int terminatorSize;

  [SerializeField]
  private string expressionModuleName;

  [SerializeField]
  private string deviceName;

  protected override DNABit produceDNABit()
  {
    PromoterBrick prom = new PromoterBrick(promoterName, promoterBeta, promoterFormula, promoterSize);
    RBSBrick rbs = new RBSBrick(rbsName, rbsFactor, rbsSize);
    GeneBrick gene = new GeneBrick(geneName, geneProteinName, geneSize);
    TerminatorBrick term = new TerminatorBrick(terminatorName, terminatorFactor, terminatorSize);

    LinkedList<BioBrick> bricks = new LinkedList<BioBrick>(new List<BioBrick>(){prom, rbs, gene, term});
    ExpressionModule module = new ExpressionModule(expressionModuleName, bricks);
        
        Device result = Device.buildDevice(deviceName, new LinkedList<ExpressionModule>(new List<ExpressionModule>(){module}));
        return result;
  }
}
