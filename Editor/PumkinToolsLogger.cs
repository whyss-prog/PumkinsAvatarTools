using System;
using Pumkin.HelperFunctions;
using UnityEngine;

namespace Pumkin.AvatarTools
{
    public static class PumkinToolsLogger
    {
        struct CopierExceptionContext
        {
            public string operationName;
            public string sourcePath;
            public string targetPath;
            public string componentType;
            public bool hasData;
        }

        static CopierExceptionContext _copierExceptionContext;

        /// <summary>
        /// Logs a message to console with a blue PumkinsAvatarTools: prefix.
        /// </summary>
        /// <param name="logFormat">Same as string.format</param>
        public static void Log(string message, LogType logType = LogType.Log, params string[] logFormat)
        {
            string msg = message;
            try
            {
                if(logFormat.Length > 0)
                    message = string.Format(message, logFormat);
                message = "<color=blue>PumkinsAvatarTools</color>: " + message;
            }
            catch
            {
                message = msg;
                logType = LogType.Warning;
            }
            switch(logType)
            {
                case LogType.Error:
                    Debug.LogError(message);
                    break;
                case LogType.Warning:
                    Debug.LogWarning(message);
                    break;
                case LogType.Exception:
                    Debug.LogException(new Exception(message));
                    break;
                case LogType.Assert:
                    Debug.LogAssertion(message);
                    break;
                default:
                    Debug.Log(message);
                    break;
            }
        }
        
        /// <summary>
        /// Logs a message to console with a red PumkinsAvatarTools: prefix. Only if verbose logging is enabled.
        /// </summary>
        /// <param name="logFormat">Same as string.format()</param>
        public static void LogVerbose(string message, LogType logType = LogType.Log, params string[] logFormat)
        {
            if(!PumkinsAvatarTools.Settings.verboseLoggingEnabled)
                return;

            if(logFormat.Length > 0)
                message = string.Format(message, logFormat);
            message = "<color=red>PumkinsAvatarTools</color>: " + message;

            switch(logType)
            {
                case LogType.Error:
                    Debug.Log(message);
                    break;
                case LogType.Warning:
                    Debug.LogWarning(message);
                    break;
                case LogType.Exception:
                    Debug.LogException(new Exception(message));
                    break;
                case LogType.Assert:
                    Debug.LogAssertion(message);
                    break;
                default:
                    Debug.Log(message);
                    break;
            }
        }

        public static void ClearCopierExceptionContext()
        {
            _copierExceptionContext = default;
        }

        public static void SetCopierExceptionContext(string operationName, Transform source, Transform sourceRoot, Transform target, Transform targetRoot, Type componentType)
        {
            _copierExceptionContext = new CopierExceptionContext
            {
                operationName = operationName ?? string.Empty,
                sourcePath = GetHierarchyPath(source, sourceRoot),
                targetPath = GetHierarchyPath(target, targetRoot),
                componentType = componentType != null ? componentType.Name : string.Empty,
                hasData = true
            };
        }

        public static void LogCopierException(string shortMessage, Exception ex)
        {
            Log($"{shortMessage}: {ex.Message}", LogType.Error);

            if(!PumkinsAvatarTools.Settings.verboseLoggingEnabled)
                return;

            if(_copierExceptionContext.hasData)
            {
                LogVerbose("Copier exception context => Operation: {0}, Component: {1}, Source: {2}, Target: {3}, Exception: {4}: {5}\n{6}",
                    LogType.Error,
                    _copierExceptionContext.operationName,
                    _copierExceptionContext.componentType,
                    _copierExceptionContext.sourcePath,
                    _copierExceptionContext.targetPath,
                    ex.GetType().Name,
                    ex.Message,
                    ex.StackTrace);
            }
            else
            {
                LogVerbose("Copier exception context unavailable => Exception: {0}: {1}\n{2}",
                    LogType.Error,
                    ex.GetType().Name,
                    ex.Message,
                    ex.StackTrace);
            }
        }

        static string GetHierarchyPath(Transform trans, Transform root)
        {
            if(!trans)
                return "<null>";

            if(!root)
                root = trans.root;

            string relativePath = Helpers.GetTransformPath(trans, root, true);

            if(string.IsNullOrEmpty(relativePath))
                return root ? root.name : trans.name;

            string rootName = root ? root.name : string.Empty;
            return string.IsNullOrEmpty(rootName) ? relativePath : $"{rootName}/{relativePath}";
        }
    }
}
