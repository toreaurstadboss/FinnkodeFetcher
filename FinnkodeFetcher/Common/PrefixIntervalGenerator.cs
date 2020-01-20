using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace FinnkodeFetcher.Common
{
    public static class PrefixIntervalGenerator
    {

        public static List<string> GenerateSubchapters(string prefixFrom, string prefixTo)
        {
            var subchapters = new List<string>();
            if (string.IsNullOrEmpty(prefixFrom) || string.IsNullOrEmpty(prefixTo) || prefixFrom.Length != 2 || prefixTo.Length != 2){
                return subchapters;
            }
            prefixFrom = prefixFrom.ToUpper(); 
            prefixTo = prefixTo.ToUpper(); //make upper cases 
	
            var matchingAlphaNumericCode = new Regex("^[A-Z][0-9]$");
            var isMatchPrefixFrom = matchingAlphaNumericCode.IsMatch(prefixFrom);
            var isMatchPrefixTo = matchingAlphaNumericCode.IsMatch(prefixTo);
            if (!isMatchPrefixFrom || !isMatchPrefixTo) {
                return subchapters; //if the prefixes are not of the right form, return empty list of subchapters  
            }

            var prefixFromMainChapter = prefixFrom[0];
            var prefixFromSubChapter = prefixFrom[1];
            var prefixToMainChapter = prefixTo[0]; 
            var prefixToSubChapter = prefixTo[1];
            int chapterIntervalLength = Math.Max(1, 1 + ((int) prefixToMainChapter - (int) prefixFromMainChapter));

            for (int i = 0; i < chapterIntervalLength; i++)
            {
                for (int j = 0; j <= 9; j++)
                {
                    int offsetFrom = (int)prefixFromMainChapter;
                    string subchapterToAdd = (char)(offsetFrom + i) + j.ToString();
                    if ((char)subchapterToAdd[0] == prefixFromMainChapter && (char)subchapterToAdd[1] <= prefixFromSubChapter-1){
                        continue;				
                    }

                    if ((char)subchapterToAdd[0] == prefixToMainChapter && (char)subchapterToAdd[1] == prefixToSubChapter)
                    {
                        //do not generate more sub chapters 
                        subchapters.Add(subchapterToAdd);
                        break;
                    }
                    else
                    {
                        subchapters.Add(subchapterToAdd);
                    }
                }	
            }
            return subchapters;
        }
    }
}