using CinemaApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CinemaApi
{
    public static class Helpers
    {
        public static void UpdatePartial(object currObj, object newObj)
        {
            if (currObj.GetType() != newObj.GetType())
            {
                return;
            }
            foreach (var prop in currObj.GetType().GetProperties())
            {
                if (prop.Name == "Id") continue;
                var newObjPropValue = prop.GetValue(newObj);
                if (newObjPropValue != null)
                {
                    if (prop.Name.EndsWith("Id") && Convert.ToDouble(newObjPropValue) == 0) continue;
                    prop.SetValue(currObj, newObjPropValue);
                }
            }
        }
    }
}
