/*
<copyright>
  Copyright (c) 2012 Codefarts
  All rights reserved.
  contact@codefarts.com
  http://www.codefarts.com
</copyright>
*/
namespace Codefarts.GeneralTools.Common
{
    /// <summary>
    /// Extension methods for the <see cref="Vector3"/> type.
    /// </summary>
    public partial struct Vector3
    {
        public static Vector3 MoveTowards(Vector3 current, Vector3 target, float maxDistanceDelta)
        {
            Vector3 vector3 = target - current;
            float single = vector3.Length();
            if (single <= maxDistanceDelta || single == 0f)
            {
                return target;
            }
            
            return current + vector3 / single * maxDistanceDelta;
        }     
    }
}