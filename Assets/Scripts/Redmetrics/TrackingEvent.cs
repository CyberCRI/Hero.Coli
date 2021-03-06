﻿//v1.52
public enum TrackingEvent
{
    // standard events
    DEFAULT,
    CREATEPLAYER,
    START,              // launched the game
    END,                // quit the game
    WIN,
    FAIL,
    RESTART,
    GAIN,
    LOSE,

    // example events
    JUMP,
    BOUNCE,

    // specific events
    COMPLETE,           // successfully finished the game
    CRAFT,              // successfully crafted a new device
    DEATH,              // player died
    EQUIP,              // equipped a device using a listed device
    PICKUP,             // picked up a device or a brick
    REACH,              // reached a checkpoint
    NEWFURTHEST,        // reached a new furthest chapter
    NEWOWNRECORD,       // beat own best completion time on a chapter
    NEWWORLDRECORD,     // beat world best completion time on a chapter
    SWITCH,             // changed game level adventure / sandbox
    UNEQUIP,            // unequipped a device using a listed device
    SELECT,             // selected a slot
    ADD,                // added a brick to the crafting zone using available bricks, or equipped a device through this process
    REMOVE,             // removed a brick from the crafting zone using craft zone bricks, or unequipped a device through this process or using a craft result device                         
    HINT,               // a hint message was displayed

    // main menu
    SELECTMENU,
    CONFIGURE,
    GOTOMOOC,
    GOTOSTUDY,
    GOTOURL,

    // alternative configuration routes
    WEBCONFIGURE,
    ADMINCONFIGURE,

    // backend events
    SWITCHFROMGAMEVERSION,
    SWITCHTOGAMEVERSION
}