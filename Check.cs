using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MrHuo.Controls
{
    internal class Check
    {
        public static void True(bool b, Action trueAction, Action falseAction = null)
        {
            if (b && trueAction != null)
            {
                trueAction();
            }
            else if (falseAction != null)
            {
                falseAction();
            }
        }

        public static void NullOrEmpty(object obj, string objName, Exception ex = null)
        {
            True(obj == null || string.IsNullOrEmpty(obj.ToString()), () =>
            {
                True(ex == null, () =>
                {
                    throw new ArgumentNullException(objName);
                }, () =>
                {
                    throw new ArgumentNullException(objName, ex);
                });
            });
        }
        public static void NullOrEmpty(object obj, string objName, string errorMessage)
        {
            True(obj == null || string.IsNullOrEmpty(obj.ToString()), () =>
            {
                True(string.IsNullOrEmpty(errorMessage), () =>
                {
                    throw new ArgumentNullException(objName);
                }, () =>
                {
                    throw new ArgumentNullException(objName, errorMessage);
                });
            });
        }
    }
}