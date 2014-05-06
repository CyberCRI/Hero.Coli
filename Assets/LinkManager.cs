using UnityEngine;
using System.Collections;


/*DESCRIPTION
 * This class create the links between the Player's Scene, classes and GameObject and the others
 * */

//Create the necessaries links between the Player scene and the other scenes
public class LinkManager : MonoBehaviour {

	// Use this for initialization
	void Awake () {

		 Hero _hero = GameObject.Find("Perso").GetComponent<Hero>();
		PhenoFickContact _pheno = GameObject.Find ("Perso").GetComponent<PhenoFickContact>();


		//Cellcontrol connection
		GUITransitioner.get ().control = GameObject.Find("Perso").GetComponent<CellControl>();


		//Hero connections
		_hero.lifeAnimation=GameObject.Find("LifeLogo").GetComponent<LifeLogoAnimation>();
		_hero.energyAnimation=GameObject.Find("EnergyLogo").GetComponent<EnergyLogoAnimation>();
		
		GameObject.Find ("LifeIndicator").GetComponent<LifeIndicator>().hero = _hero;
		GameObject.Find ("EnergyIndicator").GetComponent<EnergyIndicator>().hero = _hero;
		GUITransitioner.get ().hero = _hero;


		//PhenoFickcontact connections
		_pheno.vectroPanel =GameObject.Find("MediumInfoBackgroundSprite").GetComponent<VectrosityPanel>();
		_pheno.moleculeDebug = GameObject.Find("MediumInfoPanelRoom").GetComponent<MoleculeDebug>();


		//Main Camera
		GUITransitioner.get()._mainCameraFollow = GameObject.Find ("Main Camera").GetComponent<cameraFollow>();
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
