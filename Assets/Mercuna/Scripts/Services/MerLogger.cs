// Copyright (C) 2018-2021 Mercuna Developments Limited - All rights reserved
// This source file is part of the Mercuna Middleware
// Use, modification and distribution of this file or any portion thereof
// is only permitted as specified in your Mercuna License Agreement.
using System.IO;
using UnityEngine;

namespace Mercuna
{
    // These must match the enum levels in the native Mercuna library
    public enum ELogLevel { Always, Error, Warning, Info, Debug };

    public class MerLogger
    {
#if UNITY_EDITOR
        private const bool isLoggingToFile = true;
#else
        private const bool isLoggingToFile = false;
#endif

        public static string logFilePath
        {
            get
            {
                return Application.persistentDataPath + "/Mercuna.log";
            }
        }

        internal void ResetLogFile()
        {
            if (isLoggingToFile)
            {
                File.Delete(logFilePath);
            }
        }

        private bool OpenLogFile()
        {
            if (m_streamWriter == null)
            {
                m_streamWriter = new StreamWriter(logFilePath, true);
            }
            return true;
        }

        internal void CloseLogFile()
        {
            if (m_streamWriter != null)
            {
                m_streamWriter.Close();
                m_streamWriter = null;
            }
        }

        internal void FlushLogFile()
        {
            if (m_streamWriter != null)
            {
                m_streamWriter.Flush();
            }
        }

        internal void LogToFile(int level, string msg, bool bFlush)
        {
            if (isLoggingToFile && OpenLogFile())
            {
                m_streamWriter.Write(msg);

                if (bFlush)
                {
                    m_streamWriter.Flush();
                }
            }
        }

        internal void LogToConsole(int level, string msg)
        {
            switch ((ELogLevel)level)
            {
                case ELogLevel.Always:     Debug.Log("Mercuna: " + msg + "\n"); break;
                case ELogLevel.Error:      Debug.LogError("Mercuna: " + msg + "\n"); break;
                case ELogLevel.Warning:    Debug.LogWarning("Mercuna: " + msg + "\n"); break;
                //case ELogLevel.Info:       Debug.Log("Mercuna: " + msg + "\n"); break;
                //case ELogLevel.Debug:      Debug.Log("Mercuna: " + msg + "\n"); break;
            }
        }

        private StreamWriter m_streamWriter;
    }
}
