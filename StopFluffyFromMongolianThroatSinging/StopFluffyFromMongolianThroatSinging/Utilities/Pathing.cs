using System;
using System.Collections.Generic;
using System.Text;

namespace StopFluffyFromMongolianThroatSinging.Utilities
{
    /// <summary>
    /// Meant For Resolving Paths for the other utilities
    /// </summary>
    public static class Pathing
    {
        public static readonly string InRootPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\StopFluffyFromThroatSinging";
        public static readonly string InTodayPath = InRootPath + "\\" + DateTime.Now.ToString("yyyy-MM-dd");
        public static readonly string InLogPath = InTodayPath + "\\" + "DeoLogger.txt";
    }
}
