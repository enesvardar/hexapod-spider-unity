using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.code
{
    public enum Direction { forward, back, up, down, left, right, none };

    public enum WalkingStep {sleepy, start, walking1, walking2, stop };

    public enum RotatingStep {sleepy, start, rotating1, rotating2, stop };

    public enum DancingStep { sleepy, dancing, stop };

    class Globals
    {
        public static Hexapod hexapod = new Hexapod();

    }
}
