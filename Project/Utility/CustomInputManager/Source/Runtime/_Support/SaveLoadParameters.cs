using System.Collections.Generic;

namespace TeamUtility.IO
{
    /// <summary>
    /// Class SaveLoadParameters.
    /// </summary>
    public class SaveLoadParameters
    {
        /// <summary>
        /// The input configurations
        /// </summary>
        public List<InputConfiguration> inputConfigurations;

        /// <summary>
        /// The player one default
        /// </summary>
        public string playerOneDefault;

        /// <summary>
        /// The player two default
        /// </summary>
        public string playerTwoDefault;

        /// <summary>
        /// The player three default
        /// </summary>
        public string playerThreeDefault;

        /// <summary>
        /// The player four default
        /// </summary>
        public string playerFourDefault;

        /// <summary>
        /// Initializes a new instance of the <see cref="SaveLoadParameters"/> class.
        /// </summary>
        public SaveLoadParameters()
        {
            inputConfigurations = new List<InputConfiguration>();
            playerOneDefault = null;
            playerTwoDefault = null;
            playerThreeDefault = null;
            playerFourDefault = null;
        }
    }
}