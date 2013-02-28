using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.ComponentModel;
using System.Threading;

namespace Studie1
{
    class Row
    {
        int id;
        DateTime time;
        AttractionTypes type;
        bool entered;

        public Row(int id, DateTime time, AttractionTypes type, bool entered)
        {
            this.id = id;
            this.time = time;
            this.type = type;
            this.entered = entered;
        }

        public String getString()
        {
            // id; attraction type; enter time; exit time
            return id + ";" + getTypeString(type) + ";" + (entered ? time.ToShortTimeString() : "") + ";" + (!entered ? time.ToShortTimeString() : "");
        }

        public String getTypeString(AttractionTypes type)
        {
            switch (type)
            {
                case AttractionTypes.ANIMATED: return "0";
                case AttractionTypes.AUDITIV: return "1";
                case AttractionTypes.AVATAR: return "2";
                case AttractionTypes.STATIC: return "3";
                case AttractionTypes.NONE: return "999999";
                default: return "999999";
            }
        }
    }

    class Logger : BackgroundWorker
    {
        private StreamWriter file;
        private List<Row> rows;

        public Logger()
        {
            file = new StreamWriter(@"Log"+System.DateTime.Now.ToShortDateString()+System.DateTime.Now.ToShortTimeString()+".txt");
            this.rows = new List<Row>();
        }

        public void addRow(AttractionTypes type, int id, DateTime time, bool entered)
        {
            lock(this.rows)
            {
                this.rows.Add(new Row(id,time,type,entered));
                Monitor.PulseAll(this.rows);
            }
        }

        protected override void OnDoWork(DoWorkEventArgs e)
        {
            List<Row> writingRows = new List<Row>();
            while (!CancellationPending)
            {
                lock (this.rows)
                {
                    if (this.rows.Count <= 0)
                    {
                        Monitor.Wait(this.rows);
                    }
                    writingRows.AddRange(this.rows);
                    this.rows.RemoveRange(0, this.rows.Count);
                }
                foreach (Row row in writingRows)
                {
                    file.WriteLine(row.getString());
                }
            }
        }
    }
}
