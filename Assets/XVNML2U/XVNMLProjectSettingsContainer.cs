using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XVNML2U.Data
{
    public static class XVNMLProjectSettingsContainer
    {
        public static XVNMLProjectSettings ActiveProjectSettings { get; private set; }

        internal static void Set(XVNMLProjectSettings settings)
        {
            ActiveProjectSettings = settings;
        }
    }
}
