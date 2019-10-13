using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace StopFluffyFromMongolianThroatSinging.Utilities
{
    /// <summary>
    /// Writting Custom Pretty Logs.
    /// </summary>
    internal class DeoLogger
    {
        private List<string> lstLogInMemory = new List<string>();

        public DeoLogger()
        {
            StartLog();
            AddLine("Startup Health Test");
            StopLog();
        }

        #region Log Output

        /// <summary>
        /// Appends Line to the the In-memory Log
        /// </summary>
        /// <returns></returns>
        public void AddLine(string line)
        {
            lstLogInMemory.Add(line);
        }

        #endregion Log Output


        #region Operational

        /// <summary>
        /// Creates Header info on the Memory Log
        /// </summary>
        public void StartLog()
        {
            lstLogInMemory = new List<string>();
            lstLogInMemory.Add("---------------------------------------------");
            lstLogInMemory.Add($"Start DeoLogger @ {DateTime.Now.ToLongTimeString()}");
        }

        /// <summary>
        /// Creates Footer info on the Memory Log + Dump on Filestream
        /// </summary>
        public void StopLog()
        {
            lstLogInMemory.Add($"Stop DeoLogger @ {DateTime.Now.ToLongTimeString()}");
            lstLogInMemory.Add("---------------------------------------------");
            DumpIntoFileStream(Pathing.InLogPath, 1);
            lstLogInMemory = new List<string>();
        }

        private bool DumpIntoFileStream(string path, int fileTry)
        {
            string internalpath = path + $" #{fileTry}";
            try
            {
                File.AppendAllLines(internalpath, lstLogInMemory);
                return true;
            }
            catch (Exception ex)
            {
                if (fileTry == 5)
                    throw ex;
                else
                    return DumpIntoFileStream(path, fileTry + 1);
            }
        }

        #endregion Operational
    }
}
