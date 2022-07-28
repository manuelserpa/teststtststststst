
namespace Cmf.Custom.OntoFDC
{
    internal static class ConvertHelper
    {
        /// <summary>
        /// Parses a string and converts it to Boolean  
        /// </summary>
        /// <param name="attributeValue">a string to Parse</param>
        /// <returns>Returns a null if not able to parse</returns>
        public static object ConvertToBoolean(string attributeValue)
        {
            if (attributeValue == null)
            {
                return null;
            }

            switch (attributeValue.ToLower().Trim())
            {
                case "true":
                case "t":
                case "1":
                case "yes":
                case "y":
                    return true;
                case "0":
                case "false":
                case "f":
                case "no":
                case "n":
                    return false;
                default:
                    return null;
            }
        }
    }
}
