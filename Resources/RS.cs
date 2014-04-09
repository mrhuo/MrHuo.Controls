using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Resources;
using System.Globalization;
using System.Web;
using System.Reflection;
using System.IO;
using MrHuo.Controls.WebControls;

namespace MrHuo.Controls
{
    internal static class RS
    {
        private static ResourceManager rm;
        internal static string get(string key)
        {
            if (key == null)
                throw new ArgumentNullException("key");
            try
            {
                rm = new ResourceManager("MrHuo.Controls.Resources.Resource", typeof(MrHuo.Controls.RestResult).Assembly);
                return rm.GetString(key,System.Globalization.CultureInfo.CurrentCulture);
            }
            catch (Exception ex)
            {
                return key.Substring(key.LastIndexOf("_") + 1) + "<!--" + ex.ToString() + "-->";
            }
        }
    }
}