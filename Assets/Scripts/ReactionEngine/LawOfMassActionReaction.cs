using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class LawOfMassActionReaction : IReaction
{

    //!< the list of reactants
    private LinkedList<Product> _reactants;
    private LinkedList<Product> _products;

    // rate
    private float r = 0;

    private float k;

    // computation of k is undefined, using Arrhenius equation
    // constant
    private float A;
    // activation energy
    private float Ea;
    // universal ideal gas law constant
    private float R = 8.31446f; // J mol−1 K−1
    // temperature
    private float T = 293f; // 293°K ≈ 20°C 

    // orders
    private float[] n;
    // stoichiometric coefficients ν (Greek nu)
    private float[] v;

    private void compute_k()
    {
        k = A * Mathf.Exp(-Ea / (R * T));
    }

    private void compute_r(ArrayList molecules)
    {
        r = k;
        int index = 0;
        Molecule mol;
        foreach (Product reactant in _reactants)
        {
            mol = ReactionEngine.getMoleculeFromName(reactant.getName(), molecules);
            r = r * Mathf.Pow(mol.getConcentration(), n[index]);
            index++;
        }
    }

    public override void react(ArrayList molecules)
    {
        int index = 0;
        Molecule mol;
        if (0 == k)
        {
            compute_k();
        }
        compute_r(molecules);
        float delta = r * Time.deltaTime;
        foreach (Product reactant in _reactants)
        {
            mol = ReactionEngine.getMoleculeFromName(reactant.getName(), molecules);
            if (enableSequential)
            {
                mol.subConcentration(delta * v[index]);
            }
            else
            {
                mol.subNewConcentration(delta * v[index]);
            }
            index++;
        }
        index = 0;
        foreach (Product prod in _products)
        {
            mol = ReactionEngine.getMoleculeFromName(prod.getName(), molecules);
            if (enableSequential)
            {
                mol.addConcentration(delta * v[index]);
            }
            else
            {
                mol.addNewConcentration(delta * v[index]);
            }
            index++;
        }
    }
}

