//v1.51
public enum TrackingEvent
{
    // standard events
    DEFAULT,
    CREATEPLAYER,
    START,
    END,
    WIN,
    FAIL,
    RESTART,
    GAIN,
    LOSE,

    // example events
    JUMP,
    BOUNCE,

    // specific events
    COMPLETE,           // finished the game
    CRAFT,              // successfully crafted a new device
    DEATH,              // player died
    EQUIP,              // using listed device
    PICKUP,             // picked up a device or a brick
    REACH,              // reached a checkpoint
    SWITCH,             // changed game level
    UNEQUIP,            // using listed device
    SELECT,             // selected a slot
    ADD,                // using available bricks
    REMOVE,             // using craft result device / craft zone bricks                        

    // main menu
    SELECTMENU,
    CONFIGURE,
    GOTOMOOC,
    GOTOURL,

    // backend events
    SWITCHFROMGAMEVERSION,
    SWITCHTOGAMEVERSION
}