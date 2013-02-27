﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Kinect;

namespace Schnittstellen
{
    public interface Triggerable
    {
        void triggerAction(Skeleton[] skeletonData);
    }
}
