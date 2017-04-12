using UnityEngine;
using System.Collections;

public class LawOfMassActionReaction : IReaction
{
    // rate
    private float r = 0;

    private float k;

    // computation of k is undefined, using Arrhenius equation
    // constant
    private float A;
    // activation energy
    private float Ea;
    // universal ideal gas law constant
    private const float R = 8.31446f; // J mol−1 K−1
    // temperature
    private const float T = 293f; // 293°K ≈ 20°C

    //! Default Constructor
    public LawOfMassActionReaction() : base() {}

    //! Default constructor
    public LawOfMassActionReaction(LawOfMassActionReaction r) : base(r) {}

    private void compute_k()
    {
        k = A * Mathf.Exp(-Ea / (R * T));
    }

    private void compute_r(ArrayList molecules)
    {
        r = k;
        Molecule mol;
        foreach (Reactant reactant in _reactants)
        {
            mol = ReactionEngine.getMoleculeFromName(reactant.getName(), molecules);
            r = r * Mathf.Pow(mol.getConcentration(), reactant.n);
        }
    }

    public override void react(ArrayList molecules)
    {
        Molecule mol;
        if (0 == k)
        {
            compute_k();
        }
        compute_r(molecules);
        float delta = r * _reactionSpeed * ReactionEngine.reactionSpeed * Time.deltaTime;
        foreach (Reactant reactant in _reactants)
        {
            mol = ReactionEngine.getMoleculeFromName(reactant.getName(), molecules);
            if (enableSequential)
            {
                mol.subConcentration(delta * reactant.v);
            }
            else
            {
                mol.subNewConcentration(delta * reactant.v);
            }
        }
        foreach (Product product in _products)
        {
            mol = ReactionEngine.getMoleculeFromName(product.getName(), molecules);
            if (enableSequential)
            {
                mol.addConcentration(delta * product.v);
            }
            else
            {
                mol.addNewConcentration(delta * product.v);
            }
        }
    }



    /*
      A LawOfMassActionReaction should respect this syntax:

              <LawOfMassActionReaction>
                <name>Water</name>                        -> Name of the reaction
                <EnergyCost>0.1</EnergyCost>              -> Energy cost of the reaction
                <reactants>
                  <reactant>
                    <name>O</name>                        -> Reactant name
                    <v>1</v>                              -> Reactant stoichiometric coefficient
                    <n>1</n>                              -> Reactant order in the rate equation
                  </reactant>
                  <reactant>
                    <name>H</name>
                    <v>2</v>                              
                    <n>2</n>                              
                  </reactant>
                </reactants>
                <products>
                  <product>
                    <name>H2O</name>                      -> Product name
                    <v>1</v>                              -> Product stoichiometric coefficient
                  </product>
                </products>
              </LawOfMassActionReaction>
     */
}

