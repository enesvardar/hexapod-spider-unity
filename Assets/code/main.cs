using Assets.code;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;


public class main : MonoBehaviour
{

    //show in inspector
    public Direction direction;

    public Scrollbar txScroolbar;
    public Text txText;
    public Scrollbar tyScroolbar;
    public Text tyText;
    public Scrollbar tzScroolbar;
    public Text tzText;

    public Scrollbar rxScroolbar;
    public Text rxText;
    public Scrollbar ryScroolbar;
    public Text ryText;
    public Scrollbar rzScroolbar;
    public Text rzText;

    Gui gui;

    bool keyFlagW = false;
    bool keyFlagR = false;
    bool keyFlagD = false;

    public bool walkFlag = false;
    public bool rotateFlag = false;
    public bool danceFlag = false;

    // Start is called before the first frame update
    void Start()
    {
        gui = new Gui(txScroolbar, txText, tyScroolbar, tyText, tzScroolbar, tzText, rxScroolbar, rxText, ryScroolbar, ryText, rzScroolbar, rzText);
    }

    // Update is called once per frame
    void Update()
    {

        if (Input.GetKey(KeyCode.R) == true)
        {
            rotateFlag = true;
            keyFlagR = true;
        }
        else if (Input.GetKey(KeyCode.R) == false && keyFlagR == true)
        {
            rotateFlag = false;
            keyFlagR = false;
        }

        if (Input.GetKey(KeyCode.W) == true)
        {
            walkFlag = true;
            direction = Direction.forward;
            keyFlagW = true;
        }
        else if (Input.GetKey(KeyCode.S) == true)
        {
            walkFlag = true;
            direction = Direction.back;
            keyFlagW = true;
        }
        else if (Input.GetKey(KeyCode.D) == true)
        {
            walkFlag = true;
            direction = Direction.right;
            keyFlagW = true;
        }
        else if (Input.GetKey(KeyCode.A) == true)
        {
            walkFlag = true;
            direction = Direction.left;
            keyFlagW = true;
        }
        else if (keyFlagW == true)
        {
            walkFlag = false;
            keyFlagW = false;
        }

        if (Input.GetKey(KeyCode.Q) == true)
        {
            danceFlag = true;
            keyFlagD = true;
        }
        else if (keyFlagD == true)
        {
            danceFlag = false;
            keyFlagD = false;
        }

        Globals.hexapod.walking(direction, walkFlag);
        Globals.hexapod.rotating(rotateFlag);
        Globals.hexapod.dancing(danceFlag);

        gui.update();
        Globals.hexapod.update();
    }
}
