using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Piece : MonoBehaviour
{
    public float gap_width;
    public float gap_height;
    public float width;
    public float height;
    public int half_horizen;
    public int half_vertical;
    public GameObject target;
    public GameObject mask;
    public GameObject gameController;

    // Start is called before the first frame update
    void Start()
    {
        Vector3 og = target.transform.position;
        Quaternion og_rot = target.transform.rotation;
        int id = 1;
        for (int i = -half_horizen; i <= half_horizen; i++)
        {
            for (int j = -half_vertical; j <= half_vertical; j++)
            {
                if (i == 0 && j == 0)
                {
                    id += 1;
                }
                // continue;
                Vector3 add = new Vector3(i * (width + gap_width) + j * ((width + gap_width) / 2), j * (height+ gap_height), 0);
                GameObject newObj = Instantiate(target, og + add, og_rot);
                newObj.transform.SetParent(mask.transform, false);
                newObj.name = "hex(" + i + "," + j + ")," + id;
                newObj.SetActive(true);
                id += 1;
            }
        }
        gameController.GetComponent<Game>().placeInitialPieces();
    }
}
