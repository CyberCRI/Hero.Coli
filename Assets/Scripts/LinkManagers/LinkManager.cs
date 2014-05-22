using UnityEngine;
using System.Collections;


/*DESCRIPTION
 * This class create the links between the Player's Scene, classes and GameObject and the others
 * */

//Create the necessaries links between the Player scene and the other scenes
public class LinkManager : MonoBehaviour {

	// Use this for initialization
	void Awake () {

    GameObject perso = GameObject.Find("Perso");
		Hero hero = perso.GetComponent<Hero>();
		PhenoFickContact pheno = perso.GetComponent<PhenoFickContact>();
    GUITransitioner guiTransitioner = GUITransitioner.get ();


		//Cellcontrol connection
    guiTransitioner.control = perso.GetComponent<CellControl>();


		//Hero connections
		hero.lifeAnimation=GameObject.Find("LifeLogo").GetComponent<LifeLogoAnimation>();
		hero.energyAnimation=GameObject.Find("EnergyLogo").GetComponent<EnergyLogoAnimation>();
		
		GameObject.Find ("LifeIndicator").GetComponent<LifeIndicator>().hero = hero;
		GameObject.Find ("EnergyIndicator").GetComponent<EnergyIndicator>().hero = hero;
    guiTransitioner.hero = hero;


		//PhenoFickcontact connections
		pheno.vectroPanel =GameObject.Find("MediumInfoBackgroundSprite").GetComponent<VectrosityPanel>();
		pheno.moleculeDebug = GameObject.Find("MediumInfoPanelRoom").GetComponent<MoleculeDebug>();


		//Main Camera
    guiTransitioner._mainCameraFollow = GameObject.Find ("Main Camera").GetComponent<cameraFollow>();
  	}
}
