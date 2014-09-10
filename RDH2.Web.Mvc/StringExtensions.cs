using System;
using System.Text;

namespace RDH2.Web.Mvc
{
    /// <summary>
    /// StringExtensions encapsulates some extensions
    /// to the base String object.
    /// </summary>
    public static class StringExtensions
    {
        /// <summary>
        /// TitleReduce takes the input, pulls out special characters,
        /// and adds hyphens between words.
        /// </summary>
        /// <param name="input">The String to reduce</param>
        /// <returns>String of reduced data</returns>
        public static String TitleReduce(this String input)
        {
            //Declare a StringBuilder to hold the output
            StringBuilder rtn = new StringBuilder();

            //Get the String as lowercase
            String lowered = input.ToLower();

            //Iterate through the String and strip non-characters
            //while adding hyphens for spaces
            foreach (Char character in lowered)
            {
                //If the character is a letter, add it.  Otherwise, 
                //if the character is a space, hyphen it.
                if (Char.IsLetterOrDigit(character) == true)
                    rtn.Append(character);
                else if (Char.IsWhiteSpace(character) == true)
                    rtn.Append('-');
            }

            //Return the result
            return rtn.ToString();
        }
    }
}
