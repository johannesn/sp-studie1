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
            // attraction type; id; enter time; exit time
            return getTypeString(type) + ";" + id + ";" + (entered ? time.ToShortTimeString() : "") + ";" + (!entered ? time.ToShortTimeString() : "");
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
            String path = Directory.GetCurrentDirectory() + "\\Log " + System.DateTime.Now.ToShortDateString() + " " + System.DateTime.Now.ToShortTimeString().Replace(':', '.') + ".txt";
            System.Console.WriteLine(path);
            file = new StreamWriter(@path);
            this.rows = new List<Row>();
        }

        public void addRow(AttractionTypes type, int id, DateTime time, bool entered)
        {
            Monitor.Enter(this.rows);
            this.rows.Add(new Row(id, time, type, entered));
            Monitor.PulseAll(this.rows);
            Monitor.Exit(this.rows);
        }

        protected override void  OnDoWork(DoWorkEventArgs e)
        {
            List<Row> writingRows = new List<Row>();
            while (!CancellationPending)
            {
                Monitor.Enter(this.rows);
                while(this.rows.Count <= 0 && !CancellationPending)
                {
                    Monitor.Wait(this.rows);
                }
                writingRows.AddRange(this.rows);
                this.rows.RemoveRange(0, this.rows.Count);
                Monitor.Exit(this.rows);
                foreach (Row row in writingRows)
                {
                    file.WriteLine(row.getString());
                }
                writingRows.RemoveRange(0, writingRows.Count);
                file.Flush();
            }
        }
    }
}
