using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PickableDeviceTest : PickableDevice {
    protected override DNABit produceDNABit()
  {
    PromoterBrick prom = new PromoterBrick("PromY2", 75f, "[0.01,2]Y", 12);
    RBSBrick rbs = new RBSBrick("RBS3", 3.0f, 12);
    GeneBrick gene = new GeneBrick("MOV", "MOV", 12);
    TerminatorBrick term = new TerminatorBrick("T1", 1.0f, 12);
    LinkedList<BioBrick> bricks = new LinkedList<BioBrick>(new List<BioBrick>(){prom, rbs, gene, term});
    ExpressionModule module = new ExpressionModule("expr", bricks);
    return Device.buildDevice("DEV", new LinkedList<ExpressionModule>(new List<ExpressionModule>(){module}));
  }
}
