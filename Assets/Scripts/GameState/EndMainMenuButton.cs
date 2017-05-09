public class EndMainMenuButton : ModalButton
{
    public override void press()
    {
        // Debug.Log(this.GetType() + " press()");
        //if commented out, causes "enter" in the main menu at the end of the game to restart the game
        ModalManager.unsetModal();
        GameStateController.get().endGame();
    }
}