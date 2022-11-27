using UnityEngine.UI;

namespace Assets.code
{
    class Gui
    {
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

        public float tx = 0;
        public float ty = 0;
        public float tz = 0;

        public float rx = 0;
        public float ry = 0;
        public float rz = 0;

        public float txScroolbarBuf = 0.5f;
        public float tyScroolbarBuf = 0.5f;
        public float tzScroolbarBuf = 0.5f;

        public float rxScroolbarBuf = 0.5f;
        public float ryScroolbarBuf = 0.5f;
        public float rzScroolbarBuf = 0.5f;

        public Gui(Scrollbar _txScroolbar, Text _txText, Scrollbar _tyScroolbar, Text _tyText,
                   Scrollbar _tzScroolbar, Text _tzText, Scrollbar _rxScroolbar, Text _rxText,
                   Scrollbar _ryScroolbar, Text _ryText, Scrollbar _rzScroolbar, Text _rzText)
        {

            txScroolbar = _txScroolbar;
            txText = _txText;
            tyScroolbar = _tyScroolbar;
            tyText = _tyText;
            tzScroolbar = _tzScroolbar;
            tzText = _tzText;
            rxScroolbar = _rxScroolbar;
            rxText = _rxText;
            ryScroolbar = _ryScroolbar;
            ryText = _ryText;
            rzScroolbar = _rzScroolbar;
            rzText = _rzText;

            tx = txScroolbar.value - 0.5f;
            ty = tyScroolbar.value - 0.5f;
            tz = tzScroolbar.value - 0.5f;

            rx = rxScroolbar.value - 0.5f;
            ry = ryScroolbar.value - 0.5f;
            rz = rzScroolbar.value - 0.5f;

            txText.text = "tx = " + tx.ToString();
            tyText.text = "ty = " + ty.ToString();
            tzText.text = "tz = " + tz.ToString();

            rxText.text = "rx = " + rx.ToString();
            ryText.text = "ry = " + ry.ToString();
            rzText.text = "rz = " + rz.ToString();
        }

        public void update()
        {
            if (txScroolbarBuf != txScroolbar.value)
            {
                float change = (txScroolbar.value - txScroolbarBuf) * 100;
                tx = txScroolbar.value - 0.5f;
                txText.text = "tx = " + tx.ToString();
                Globals.hexapod.MoveHexapodBodyXYZ(change, 0, 0);
                txScroolbarBuf = txScroolbar.value;
            }

            if (tyScroolbarBuf != tyScroolbar.value)
            {
                float change = (tyScroolbar.value - tyScroolbarBuf) * 100;
                ty = tyScroolbar.value - 0.5f;
                tyText.text = "ty = " + ty.ToString();
                Globals.hexapod.MoveHexapodBodyXYZ(0, change, 0);
                tyScroolbarBuf = tyScroolbar.value;
            }

            if (tzScroolbarBuf != tzScroolbar.value)
            {
                float change = (tzScroolbar.value - tzScroolbarBuf) * 100;
                tz = tzScroolbar.value - 0.5f;
                tzText.text = "tz = " + tz.ToString();
                Globals.hexapod.MoveHexapodBodyXYZ(0, 0, change);
                tzScroolbarBuf = tzScroolbar.value;
            }

            if (rxScroolbarBuf != rxScroolbar.value)
            {
                float change = (rxScroolbar.value - rxScroolbarBuf) * 80;
                rx = rxScroolbar.value - 0.5f;
                rxText.text = "rx = " + rx.ToString();
                Globals.hexapod.RotateHexapodBodyXYZ(change, 0, 0);
                rxScroolbarBuf = rxScroolbar.value;
            }

            if (ryScroolbarBuf != ryScroolbar.value)
            {
                float change = (ryScroolbar.value - ryScroolbarBuf) * 80;
                ry = ryScroolbar.value - 0.5f;
                ryText.text = "ry = " + ry.ToString();
                Globals.hexapod.RotateHexapodBodyXYZ(0, change, 0);
                ryScroolbarBuf = ryScroolbar.value;
            }

            if (rzScroolbarBuf != rzScroolbar.value)
            {
                float change = (rzScroolbar.value - rzScroolbarBuf) * 80;
                rz = rzScroolbar.value - 0.5f;
                rzText.text = "rz = " + rz.ToString();
                Globals.hexapod.RotateHexapodBodyXYZ(0, 0, change);
                rzScroolbarBuf = rzScroolbar.value;
            }
        }
    }
}