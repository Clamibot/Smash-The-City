using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// This script scales the player after the player is spawned into the scene. If the player is scaled up in the editor, the hands appear in the wrong location.
public class PlayerScaler : MonoBehaviour
{
    private GameObject player;
    public float scaleX, scaleY, scaleZ;
    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.Find("PlayerRobot");
        StartCoroutine(Scale());
    }

    IEnumerator Scale()
    {
        yield return new WaitForSeconds(0.1f);
        player.transform.localScale = new Vector3(scaleX, scaleY, scaleZ);
    }
}
