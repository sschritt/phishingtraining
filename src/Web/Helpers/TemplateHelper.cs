using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PhishingTraining.Web.Helpers
{
    public class TemplateHelper
    {
        public static bool HasTemplateFile(Guid TemplateId, string StoragePath)
        {
            if (TemplateId == Guid.Empty)
                return false;

            var templateFilename = $"{TemplateId}.zip";
            var filepath = StoragePath + "/" + templateFilename;
            return System.IO.File.Exists(filepath);
        }

        internal static bool HasPlainTemplateFile(Guid TemplateId, string StoragePath)
        {
            if (TemplateId == Guid.Empty)
                return false;

            var templateFilename = $"{TemplateId}.txt";
            var filepath = StoragePath + "/" + templateFilename;
            return System.IO.File.Exists(filepath);
        }
    }
}
