using UnityEngine;

namespace DuckGame.Utils
{
    /// <summary>
    /// Helper class to document and manage the namespace structure
    /// </summary>
    public static class ScriptNamespaceHelper
    {
        /// <summary>
        /// Lists all the namespaces used in the project
        /// </summary>
        public static readonly string[] ProjectNamespaces = {
            "DuckGame.Core",
            "DuckGame.Managers", 
            "DuckGame.Controllers",
            "DuckGame.Systems",
            "DuckGame.UI",
            "DuckGame.Utils"
        };
        
        /// <summary>
        /// Gets the recommended namespace for a given script type
        /// </summary>
        /// <param name="scriptType">Type of script (Manager, Controller, System, etc.)</param>
        /// <returns>The recommended namespace</returns>
        public static string GetRecommendedNamespace(string scriptType)
        {
            switch (scriptType.ToLower())
            {
                case "manager":
                    return "DuckGame.Managers";
                case "controller":
                    return "DuckGame.Controllers";
                case "system":
                    return "DuckGame.Systems";
                case "ui":
                    return "DuckGame.UI";
                case "util":
                case "helper":
                    return "DuckGame.Utils";
                default:
                    return "DuckGame.Core";
            }
        }
        
        /// <summary>
        /// Validates that a script is in the correct namespace
        /// </summary>
        /// <param name="scriptName">Name of the script</param>
        /// <param name="currentNamespace">Current namespace</param>
        /// <returns>True if the namespace is appropriate for the script type</returns>
        public static bool ValidateNamespace(string scriptName, string currentNamespace)
        {
            if (scriptName.Contains("Manager"))
                return currentNamespace == "DuckGame.Managers";
            if (scriptName.Contains("Controller"))
                return currentNamespace == "DuckGame.Controllers";
            if (scriptName.Contains("System"))
                return currentNamespace == "DuckGame.Systems";
            if (scriptName.Contains("UI"))
                return currentNamespace == "DuckGame.UI";
            if (scriptName.Contains("Util") || scriptName.Contains("Helper"))
                return currentNamespace == "DuckGame.Utils";
            
            return true;
        }
    }
} 