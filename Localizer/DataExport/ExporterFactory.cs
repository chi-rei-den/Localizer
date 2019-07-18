using System;
using System.CodeDom;
using Localizer.DataModel;

namespace Localizer.DataExport
{
    public static class ExporterFactory
    {
        // TODO: Should be extensible and elegant
        public static Exporter CreateExporter(Type type, ExportConfig config)
        {
            switch (type.Name)
            {
                case nameof(BasicItemFile):
                    return new BasicItemExporter(config as BasicExportConfig);
                case nameof(BasicNPCFile):
                    return new BasicNPCExporter(config as BasicExportConfig);
                case nameof(BasicBuffFile):
                    return new BasicBuffExporter(config as BasicExportConfig);
                case nameof(BasicCustomFile):
                    return new BasicCustomExporter(config as BasicExportConfig);
                case nameof(LdstrFile):
                    return new LdstrExporter(config as LdstrExportConfig);
                default:
                    return null;
            }
        }
    }
}