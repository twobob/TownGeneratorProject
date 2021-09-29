using UnityEngine;
using System.Collections;
using System.Linq;

namespace ExtensionMethods
{

    static class ElevenStringHelpers
    {

        public static string ReplaceAll(this string seed, char[] chars, char replacementCharacter)
        {
            return chars.Aggregate(seed, (str, cItem) => str.Replace(cItem, replacementCharacter));
        }


        public static string Elvenize(this string seed)
        {

            char[] firstflip = new char[] { 'i', 'o' };
            char[] secondflip = new char[] { 'e', 'u' };

            return seed
                  .ReplaceAll(firstflip, (RandomGen.FlipACoin()) ? 'e' : 'y')
                  .ReplaceAll(secondflip, (RandomGen.FlipACoin()) ? 'a' : 'o');
        }
    }
}