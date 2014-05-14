using UnityEngine;
using System.Collections;

//This class manage the Life script

/*How to Manage Life of a character
 * 
 * Life Object must be instanciate in a character 
 * 	The constructor need to 2 parameters :
 * 		param life : the life max and initial of the character
 * 		param lifeRegen : the initial life regen of the character
 * 
 * What does alter the life of a character, call his Life Object and use AddVariation(float,bool)
 *		param float : the positive value of the change.
 *		param bool  : if the alter is positive (true) or negative (false)
 *
 * ApplyVariation is called in the Update()of the Character
 * */
public class Life : MonoBehaviour {

	private float _lifeMax;
	private float _life;
	private float _lifeRegen;
	private float _variation;



	//Constructor
	public Life(float life, float lifeRegen)
	{
		_lifeMax = life;
		_life = _lifeMax;
		_lifeRegen = lifeRegen;
	}
	

	//Add a variation for the next update, the bool positif determine if the vaiation will be positive or negative
	public void addVariation (float variation, bool positif)
	{
		_variation += (positif == true)? variation : - variation;
	}

	public float getLife()
	{
		return _life;
	}

	public float getVariation()
	{
		return _variation;
	}

	public void applyVariation()
	{
		_life += _variation;
		if(_life  <= 0f) _life = 0f;
		else if(_life  >= _lifeMax) _life = _lifeMax;

		ResetVariation();
	}

	// Reset the variation value : called at the end of the update
	public void ResetVariation()
	{
		_variation = 0f;
	}
}
