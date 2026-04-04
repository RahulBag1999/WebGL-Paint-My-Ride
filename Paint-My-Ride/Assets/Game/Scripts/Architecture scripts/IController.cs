using System;

public interface IController
{
    void Init(PopupHandler popupHandler, EssentialConfigData essentialConfigData, Action<GameStates, object> stateChanged);
    void ChangeGameState(GameStates newGameState, object data = null);
    void RegisterInputs();
    void UnregisterInputs();
    void Cleanup();
}