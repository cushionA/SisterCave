//using Rewired;
using UnityEngine;
public class InputTest : MonoBehaviour
{
    //Player _player;
  //  int playerId = 0;
    void Awake()
    {
//        _player = ReInput.players.GetPlayer(playerId);
    }
    void Update()
    {
      //  Debug.Log($"コントローラーの名前{ReInput.controllers.Controllers[2].name}");
        if (GManager.instance.InputR.GetButtonRepeating(2)){
            Debug.Log("お肉");
        }
        if (GManager.instance.InputR.GetAxisRaw(MainUI.instance.rewiredAction16) > 0)
        {
            Debug.Log("人でなし");
        }
    }
}