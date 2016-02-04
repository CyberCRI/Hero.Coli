using UnityEngine;
using System.Collections;

//v1.32
public enum TrackingEvent {
    //standard events
    DEFAULT,
    CREATEPLAYER,
    START,
    END,
    WIN,
    FAIL,
    RESTART,
    GAIN,
    LOSE,

    //example events
    JUMP,
    BOUNCE,

    //specific events
    COMPLETE,
    CRAFT,
    DEATH,
    EQUIP,
    PICKUP,
    REACH,
    SWITCH,
    UNEQUIP,
	//main menu
	SELECTMENU,
    CONFIGURE,
	GOTOMOOC,
    GOTOURL,
    //backend events
    SWITCHFROMGAMEVERSION,
    SWITCHTOGAMEVERSION
}