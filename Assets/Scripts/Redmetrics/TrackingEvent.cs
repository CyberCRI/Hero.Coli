//v1.51
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
    SWITCH,             // changed game level adventure / sandbox
    UNEQUIP,            // unequipped a device using a listed device
    SELECT,             // selected a slot
    ADD,                // added a brick to the crafting zone using available bricks, or equipped a device through this process
    REMOVE,             // removed a brick from the crafting zone using craft zone bricks, or unequipped a device through this process or using a craft result device                         

    // main menu
    SELECTMENU,
    CONFIGURE,
    GOTOMOOC,
    GOTOURL,

    // backend events
    SWITCHFROMGAMEVERSION,
    SWITCHTOGAMEVERSION
}