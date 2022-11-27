using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using UGCS.Sdk.Protocol.Encoding;

namespace SlideMap.Helpers
{
    public class ServiceHelpers
    {
        public static readonly DateTime PosixEpoch =
            new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        public static long CreationTime()
        {
            DateTime now = DateTime.Now;
            DateTime utcTime = now.ToUniversalTime();
            TimeSpan span = utcTime - PosixEpoch;
            return (long)span.TotalMilliseconds;
        }
    }

    public static class ListExtensions
    {
        public static ObservableCollection<T> AsObservable<T>(this List<T> list)
        {
            var result = new ObservableCollection<T>();
            foreach (var item in list)
            {
                result.Add(item);
            }
            return result;
        }
    }

    public static class DoubleExtensions
    {
        public static double ToRadians(this double coordinateDegrees)
        {
            var radian = Math.PI / 180;
            return coordinateDegrees * radian;
        }

        public static double FromRadians(this double coordinateRadians) {
            return coordinateRadians / (Math.PI / 180);
        }
    }

    public static class SegmentDefinitionExtensions
    {
        public static SegmentDefinition AddParameter(this SegmentDefinition segment, string name, string value)
        {
            segment.ParameterValues.Add(new ParameterValue()
            {
                Name = name,
                NameSpecified = true,
                Value = value,
                ValueSpecified = true,
            });
            return segment;
        }
    }
}
