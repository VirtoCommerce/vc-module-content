using System;
using System.Collections.Generic;

namespace VirtoCommerce.ContentModule.Core.Comparers;
public class OrdinalStringComparer : IComparer<string>
{
    public int Compare(string x, string y)
    {
        if (x == null || y == null)
        {
            return Comparer<string>.Default.Compare(x, y);
        }

        for (var i = 0; i < Math.Min(x.Length, y.Length); i++)
        {
            var charX = x[i];
            var charY = y[i];

            if (charX != charY)
            {
                return charX < charY ? -1 : 1;
            }

            //if (charX == '_' && charY == '.')
            //{
            //    return 1; // Подчеркивание больше точки
            //}
            //if (charX == '.' && charY == '_')
            //{
            //    return -1; // Точка меньше подчеркивания
            //}

            //var result = charX.CompareTo(charY);
            //if (result != 0)
            //{
            //    return result;
            //}
        }

        return x.Length.CompareTo(y.Length);
    }
}
