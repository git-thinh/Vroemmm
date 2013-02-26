using Google.Apis.Calendar.v3.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml;

namespace Vroemmm.Logic
{
    public static class EventExtentions
    {
        public static DateTime? EndDateTime(this Event evnt)
        {
            if(evnt.End == null) return null;

            var date = evnt.End.DateTime ?? evnt.End.Date ?? string.Empty;

            if (string.IsNullOrEmpty(date)) return null;

            return XmlConvert.ToDateTime(date, XmlDateTimeSerializationMode.Local);
        }

        public static DateTime? StartDateTime(this Event evnt)
        {
            if (evnt.Start == null) return null;

            var date = evnt.Start.DateTime ?? evnt.Start.Date ?? string.Empty;

            if (string.IsNullOrEmpty(date)) return null;

            return XmlConvert.ToDateTime(date, XmlDateTimeSerializationMode.Local);
        }
    }
}