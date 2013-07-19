using System;
using System.Collections;
using UnityEngine;

public class ATPProducer : IReaction
{
  private float _production;

  public void setProduction(float v) { _production = v; }
  public float getProduction() { return _production; }

  public ATPProducer()
  {
    _production = 0f;
  }

  public ATPProducer(ATPProducer r) : base(r)
  {
    _production = r._production;
  }

  public override void react(ArrayList molecules)
  {
    _medium.addEnergy(_production * _reactionSpeed * ReactionEngine.reactionsSpeed);
  }
}