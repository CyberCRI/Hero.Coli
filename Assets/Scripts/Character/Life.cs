using UnityEngine;
using System.Collections;

//This class manages the Life

/*How to Manage Life of a character
 * 
 * Life Object must be instanciated in a character 
 * 	The constructor needs to 2 parameters :
 * 		param life : the max and initial life of the character
 * 		param lifeRegen : the initial life regen of the character
 * 
 * What does alter the life of a character, call his Life Object and use AddVariation(float,bool)
 *		param float : the positive value of the change.
 *		param bool  : if the variation is positive (true) or negative (false)
 *
 * ApplyVariation is called in the Update() of the Character
 * */
public class Life
{

    private float _lifeMax;
    private float _life;
    private float _lifeRegen;
    private float _variation;

    private bool _suddenDeath = false;
    //Constructor
    public Life(float life, float lifeRegen)
    {
        _lifeMax = life;
        _life = _lifeMax;
        _lifeRegen = lifeRegen;
    }

    //Add a variation for the next update, the bool positive determines if the variation will be positive or negative
    public void addVariation(float variation)
    {
        _variation += variation;
    }

    public float getLife()
    {
        return _life;
    }

    public void setLife(float life)
    {
        if (life >= _lifeMax)
        {
            life = _lifeMax;
        }
        else if (life <= 0f)
        {
            life = 0f;
        }
        _life = life;
    }

    public void setSuddenDeath(bool b)
    {
        _suddenDeath = b;
    }

    public float getVariation()
    {
        return _variation;
    }

    public void applyVariation()
    {
        _life += _variation;
        if (_life <= 0f || _suddenDeath) _life = 0f;
        else if (_life >= _lifeMax) _life = _lifeMax;

        resetVariation();
    }

    // Reset the variation value : called at the end of the update
    public void resetVariation()
    {
        _variation = 0f;
        if (_suddenDeath)
            _suddenDeath = false;
    }

    public void regenerate(float deltaTime)
    {
        addVariation(deltaTime * _lifeRegen);
    }
}
