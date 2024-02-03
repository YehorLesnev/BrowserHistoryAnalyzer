using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChromeHistoryParser
{
    public class ChromeHistoryParser
    {
        private string chromeHistoryFile = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + @"\Google\Chrome\User Data\Default\History";
        public List<HistoryItem> historyItems { get; private set; } = new List<HistoryItem>();

        /// <summary>
        /// Returns List of HistoryItem objects
        /// Opens SQLiteConnection to user's browser history file db corresponding to \Google\Chrome\User Data\Default\History
        /// </summary>
        /// <returns>List&lt;HistoryItem&gt;</returns>
        /// <exception cref="SQLiteException">'database is locked' means that there's already opened connection to the database (history file)
        /// Closing Chrome browser must solve this error
        /// </exception>
        public List<HistoryItem> GetHistoryItems()
        {
            SQLiteConnection connection = new SQLiteConnection
                ("Data Source=" + chromeHistoryFile + ";Version=3;New=False;Compress=True;");

            connection.Open();

            SQLiteCommand comp = new SQLiteCommand("SELECT * FROM urls ORDER BY last_visit_time DESC", connection);
            comp.CommandType = CommandType.Text;
            SQLiteDataAdapter adapter = new SQLiteDataAdapter(comp);

            DataTable dataset = new DataTable();
            adapter.Fill(dataset);

            foreach (DataRow historyRow in dataset.Rows)
            {
                HistoryItem historyItem = new HistoryItem
                {
                    URL = new Uri(Convert.ToString(historyRow["url"])),
                    Title = Convert.ToString(historyRow["title"])
                };

                //Chrome stores time elapsed since Jan 1, 1601(UTC format) in microseconds
                long utcMicroSeconds = Convert.ToInt64(historyRow["last_visit_time"]);

                //Windows file time UTC is in nanoseconds, so multiplying by 10
                DateTime gmtTime = DateTime.FromFileTimeUtc(10 * utcMicroSeconds);

                //Converting to local time
                DateTime localTime = TimeZoneInfo.ConvertTimeFromUtc(gmtTime, TimeZoneInfo.Local);
                historyItem.VisitedTime = localTime;

                historyItems.Add(historyItem);
            }

            return historyItems;
        }
    }
}