using System;
using System.Collections.Generic;
using System.Text;

namespace Golf
{
    public class Logger
    {
        private List<StrokeRecord> _strikeRecord;

        public Logger()
        {
            _strikeRecord = new List<StrokeRecord>();
        }

        public List<StrokeRecord> StrikeRecord
        {
            get { return this._strikeRecord; }
        }

        public void AddRecord(StrokeRecord data)
        {
            this._strikeRecord.Add(data);
        }


    }
}
