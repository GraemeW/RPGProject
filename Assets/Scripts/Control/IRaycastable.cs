namespace RPG.Control
{
    public interface IRaycastable
    {
        CursorType GetCursorType();
        bool HandleRaycast(PlayerController callingController, string interactButton = "Fire1");
    }
}