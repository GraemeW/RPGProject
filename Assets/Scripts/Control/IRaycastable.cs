namespace RPG.Control
{
    public interface IRaycastable
    {
        CursorType GetCursorType();
        bool HandleRaycast(PlayerController callingController, string interactButtonOne = "Fire1", string interactButtonTwo = "Fire2");
    }
}