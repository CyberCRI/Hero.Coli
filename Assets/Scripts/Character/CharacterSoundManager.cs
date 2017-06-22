using UnityEngine;

public class CharacterSoundManager: MonoBehaviour {
	public float fadeToMovementTime;
	public float fadeToIdleTime;

	public PlayableSound respawnSound;

	public PlayableSound deathSound;
	public PlayableSound deathMineSound;
	public PlayableSound deathCrushSound;
	public PlayableSound deathNoEnergySound;
	public PlayableSound deathSuicideSound;
	public PlayableSound deathEnemySound;
	public PlayableSound deathAmpicilinSound;

	public PlayableSound hurtAntibioticsSound;
	public PlayableSound hurtEnergySound;

	public PlayableSound divisionSound;

	public PlayableSound pushSound;

	void OnEnable()
	{
		Character.onCharacterRespawn += OnCharacterRespawn;
		Character.onCharacterDivision += OnCharacterDivision;
		Character.onCharacterDeath += OnCharacterDeath;
		Character.onHurtEnergy += OnHurtEnergy;
		Character.onHurtAntibiotics += OnHurtAntibiotics;
		CellControl.onCellMove += OnCellMove;
		PushableBox.onPlayerPushRock += OnPlayerPushRock;
	}

	void OnDisable()
	{
		Character.onCharacterDivision -= OnCharacterDivision;
		Character.onCharacterRespawn -= OnCharacterRespawn;
		Character.onCharacterDeath -= OnCharacterDeath;
		Character.onHurtEnergy -= OnHurtEnergy;
		Character.onHurtAntibiotics -= OnHurtAntibiotics;
		CellControl.onCellMove -= OnCellMove;
		PushableBox.onPlayerPushRock -= OnPlayerPushRock;
	}

	/// <summary>
	/// Called whenever the player respawn
	/// </summary>
	public void OnCharacterRespawn ()
	{
		respawnSound.Play ();
		ArcadeManager.instance.playAnimation(ArcadeManager.Animation.bacterium_respawn);
	}

	/// <summary>
	/// Called whenever the player dies
	/// </summary>
	/// <param name="deathData">The death data that holds the type of death</param>
	public void OnCharacterDeath (CustomDataValue deathData)
	{
		switch (deathData)
        {
            case CustomDataValue.ENEMY:
                deathEnemySound.Play();
				// ArcadeManager.instance.playAnimation(ArcadeManager.Animation.bacterium_death_enemy);
                break;
            case CustomDataValue.MINE:
                deathMineSound.Play();
				// ArcadeManager.instance.playAnimation(ArcadeManager.Animation.bacterium_death_enemy);
                break;
            case CustomDataValue.CRUSHED:
                deathCrushSound.Play();
				// ArcadeManager.instance.playAnimation(ArcadeManager.Animation.bacterium_death_crushed);
                break;
            case CustomDataValue.SUICIDEBUTTON:
                deathSuicideSound.Play();
				// ArcadeManager.instance.playAnimation(ArcadeManager.Animation.bacterium_death_suicide);
                break;
            case CustomDataValue.NOENERGY:
                deathNoEnergySound.Play();
				// ArcadeManager.instance.playAnimation(ArcadeManager.Animation.bacterium_death_energy);
                break;
            case CustomDataValue.AMPICILLIN:
            case CustomDataValue.AMPICILLINWALL1:
            case CustomDataValue.AMPICILLINWALL2:
                deathAmpicilinSound.Play();
				// ArcadeManager.instance.playAnimation(ArcadeManager.Animation.bacterium_death_antibiotics);
                break;
            default:
                deathSound.Play();
				// ArcadeManager.instance.playAnimation(ArcadeManager.Animation.bacterium_death);
                break;
        }
		ArcadeManager.instance.playAnimation(ArcadeManager.Animation.bacterium_death);
	}

	/// <summary>
	/// Called whenever the character is being hurt or stops being hurt by antibiotics
	/// </summary>
	/// <param name="isBeingHurt">If set to <c>true</c> character is being hurt.</param>
	public void OnHurtAntibiotics (bool isBeingHurt)
	{
		if (isBeingHurt)
		{
			hurtAntibioticsSound.PlayIfNotPlayed ();
			ArcadeManager.instance.playAnimation(ArcadeManager.Animation.bacterium_hurt_antibiotics_start);
		}
		else
		{
			hurtAntibioticsSound.StopAll ();
			ArcadeManager.instance.playAnimation(ArcadeManager.Animation.bacterium_hurt_antibiotics_end);
		}
	}

	/// <summary>
	/// Called whenever the character is being hurt or stops being hurt by energy
	/// </summary>
	/// <param name="isBeingHurt">If set to <c>true</c> character is being hurt.</param>
	public void OnHurtEnergy (bool isBeingHurt)
	{
		if (isBeingHurt)
		{
			hurtEnergySound.PlayIfNotPlayed ();
			ArcadeManager.instance.playAnimation(ArcadeManager.Animation.bacterium_hurt_energy_start);
		}
		else
		{
			hurtEnergySound.StopAll ();
			ArcadeManager.instance.playAnimation(ArcadeManager.Animation.bacterium_hurt_energy_end);
		}
	}

	/// <summary>
	/// Called whenever the player moves
	/// </summary>
	/// <param name="isMoving">If set to <c>true</c> character is moving.</param>
	public void OnCellMove (bool isMoving)
	{
		if (isMoving)
			SoundManager.instance.ActivateMovementAudioMix (fadeToMovementTime);
		else
			SoundManager.instance.ActivateIdleAudioMix (fadeToIdleTime);
	}

	public void OnCharacterDivision ()
	{
		divisionSound.Play ();
		ArcadeManager.instance.playAnimation(ArcadeManager.Animation.bacterium_divide);
	}
		
	public void OnPlayerPushRock (bool canPush)
	{
		if (canPush)
			pushSound.Play ();
		else
			pushSound.StopAll ();
	}
}

