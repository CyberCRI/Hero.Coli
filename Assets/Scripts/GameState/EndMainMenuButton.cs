public class EndMainMenuButton : ModalButton
{
    private const string studyURLKey = "STUDY.LEARN.LINK"; 
    
    public override void press ()
    {
        // Debug.Log(this.GetType() + " press()");
        //URLOpener.open(studyURLKey, false);        
        
        //if commented out, causes "enter" in the main menu at the end of the game to restart the game
        ModalManager.unsetModal ();
        GameStateController.get ().endGame();
    }
}

