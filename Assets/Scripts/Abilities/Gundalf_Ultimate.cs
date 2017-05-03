using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gundalf_U : AbstractUltimate {

    private float startMoveHeight;

    protected override void OnTriggerDown()
    {
        //Check if controllers are [distance] over the headposition and one higher than the other
        if (Vector2.Distance(new Vector2(leftHand.position.x,leftHand.position.z),new Vector2(rightHand.position.x,rightHand.position.z))<0.2)//Headposition und Vergleich fehlt hier noch!!!!
        {
            startMoveHeight = (leftHand.position.y + rightHand.position.y) / 2;
            //@HackerPlayer: Ultimate wurde erkannt und aktiviert
            Debug.Log("Ultimate-Tracking started! #For_Gun OnTriggerDown()");
        }
        else
        {
            IsTriggerDown = false;
            //@HackerPlayer: Ultimate wurde abgebrochen. Ein normaler Move soll ausgeführt werden
            Debug.Log("Ultimate-Tracking failed! #For_Gun OnTriggerDown()");
        }
        //--> set UltimateActive (moveHeightStart=transform.position.y)
        //--> unselect selected Enemies
    }
    protected override void OnTriggerUp()
    {
        float moveHeightEnd = (leftHand.position.y + rightHand.position.y) / 2;
        //Check if the controllers are [distance] higher as the position before OnGripsDown()
        if (moveHeightEnd - startMoveHeight < 0.5)
        {
            Debug.Log("Ultimate-Tracking successfull! #For_Gun OnTriggerUp()");
            //--> Do Force push! 
            
        }
        else
        {
            Debug.Log("Ultimate-Tracking failed! #For_Gun OnTriggerUp()");
            startMoveHeight = 100;
        }
        IsTriggerDown = false;
    }
}
