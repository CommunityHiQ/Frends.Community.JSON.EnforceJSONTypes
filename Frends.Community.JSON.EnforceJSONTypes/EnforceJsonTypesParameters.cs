namespace Frends.Community.JSON.EnforceJSONTypes
{
    /// <summary>
    /// Parameters for EnforceJsonTypes task
    /// </summary>
    public class EnforceJsonTypesParameters
    {
        /// <summary>
        /// JSON data type rules to enforce
        /// </summary>
        public JsonTypeRule[] Rules { get; set; }
    }
}