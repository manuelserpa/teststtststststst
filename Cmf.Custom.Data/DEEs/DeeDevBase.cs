﻿using System;
using System.Collections.Generic;

namespace Cmf.Custom.amsOSRAM.Actions
{
    public abstract class DeeDevBase
    {
        public abstract Boolean DeeTestCondition(Dictionary<String, Object> Input);

        public abstract Dictionary<String, Object> DeeActionCode(Dictionary<String, Object> Input);

        public static void UseReference(String assembly, String nameSpace) { }

    }
}
