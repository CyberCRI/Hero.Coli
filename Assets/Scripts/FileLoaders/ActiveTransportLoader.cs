using UnityEngine;
using System;
using System.Xml;
using System.IO;
using System.Collections;
using System.Collections.Generic;

/*!
  \brief This class load properties from XML files

  \important
  A file should respect this convention:

        <activeTransports>
          <ATProp>
            <name>name</name>                  -> The name
            <Medium>1<Medium>                  -> In which medium the reaction is done.
            <MediumSrc>1</MediumSrc>           -> Medium where the reaction takes place
            <MediumDst>2</MediumDst>           -> Medium where the product will be released
            <EnergyCost>0.1</EnergyCost>       -> Energy that a reaction costs
            <substrate>S</substrate>           -> The Molecule that will be consumed in the SrcMedium
            <enzyme>E</enzyme>                 -> The Molecule which will play the role of the tunnel
            <Kcat>1</Kcat>                     -> Reaction constant of enzymatic reaction
            <effector>False</effector>         -> The Molecule that will inhibit the reaction or activate the reaction (False value is accepted, meaning that there is no effector)
            <alpha>1</alpha>                   -> Alpha parameter. Describes the competitivity of the effector (see execEnzymeReaction in EnzymeReaction)
            <beta>0</beta>                     -> Beta parameter. Describes the extend of inhibition or the extend of activation (see execEnzymeReaction in EnzymeReaction)
            <Km>1.3</Km>                       -> Km parameter. Affinity between substrate and enzyme
            <Ki>0.0000001</Ki>                 -> Ki parameter. Affinity between effector and enzyme
            <Products>                         -> List of the products
              <name>O</name>                   -> Name of the product molecule
            </Products>
          </ATProp>
        </activeTransports>
  \sa EnzymeReaction  
 */


public class ActiveTransportLoader : XmlLoaderImpl
{    
  public override string xmlTag
  {
    get
    {
        return "ATProp";
    }
  }
}