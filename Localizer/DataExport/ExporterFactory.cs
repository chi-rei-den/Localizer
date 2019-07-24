using Localizer.DataModel;
using System;
using System.Collections.Generic;
using System.Reflection;
using MonoMod.Utils;

namespace Localizer.DataExport
{
    public static class ExporterFactory
    {
        
        public static Dictionary<Type, Type> ExpoterCreateActions = new Dictionary<Type, Type>
        {
            [typeof(BasicItemFile)] = typeof(BasicItemExporter),
            [typeof(BasicBuffFile)] = typeof(BasicBuffExporter),
            [typeof(BasicCustomFile)] = typeof(BasicCustomExporter),
            [typeof(BasicNPCFile)] = typeof(BasicNPCExporter),
            [typeof(LdstrFile)] = typeof(LdstrExporter),
        };
        
        public static Exporter CreateExporter(Type type, ExportConfig config)
        {
            if (ExpoterCreateActions.TryGetValue(type, out var exporterType))
            {
                return GetExporterCtor(exporterType).Invoke(new object[]{ config }) as Exporter;
            }

            return null;
        }

        private static ConstructorInfo GetExporterCtor(Type type)
        {
            return type.GetConstructor(new[] { typeof(ExportConfig) });
        }
    }
}