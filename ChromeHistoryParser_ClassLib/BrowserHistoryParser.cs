using System.Data;
using System.Data.SQLite;

namespace BrowserHistoryParser_ClassLib
{
    public class BrowserHistoryParser
    {
        private readonly string _chromeHistoryFile = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + @"\Google\Chrome\User Data\Default\History";
        private readonly string _edgeHistoryFile = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + @"\Microsoft\Edge\User Data\Default\History";
        private readonly string _firefoxFolder = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\Mozilla\Firefox\Profiles\";
        private readonly string _firefoxHistoryFileName = @"places.sqlite";

        public IEnumerable<HistoryItem> GetAllHistoryItems()
        {
            List<HistoryItem> historyItems = new List<HistoryItem>();
            historyItems.AddRange(GetChromeHistoryItems());
            historyItems.AddRange(GetEdgeHistoryItems());
            historyItems.AddRange(GetFirefoxHistoryItems());

            return historyItems;
        }

        /// <summary>
        /// Returns List of HistoryItem objects
        /// <para>
        /// Opens SQLiteConnection to user's Google Chrome browser history file db corresponding to \Google\Chrome\User Data\Default\History
        /// </para>
        /// </summary>
        /// <remarks>
        /// Throws SQLiteException 'database is locked' exception if there's already opened connection to the database (history file).
        /// Closing Chrome browser must solve this error
        /// </remarks>
        /// <returns>List&lt;HistoryItem&gt;</returns>
        /// <exception cref="SQLiteException">'database is locked' means that there's already opened connection to the database (history file)</exception>
        public IEnumerable<HistoryItem> GetChromeHistoryItems()
        {
            ICollection<HistoryItem> historyItems = new List<HistoryItem>();

            SQLiteConnection connection = new SQLiteConnection
                ("Data Source=" + _chromeHistoryFile + ";Version=3;New=False;Compress=True;");

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
                    Title = Convert.ToString(historyRow["title"]),
                    BrowserName = BrowserName.Chrome
                };

                //Chrome stores time elapsed since Jan 1, 1601(UTC format) in microseconds
                long utcMicroSeconds = Convert.ToInt64(historyRow["last_visit_time"]);

                //Windows file time UTC is in nanoseconds, so multiplying by 10
                DateTime gmtTime = DateTime.FromFileTimeUtc(10 * utcMicroSeconds);

                //Converting to local time
                DateTime localTime = TimeZoneInfo.ConvertTimeFromUtc(gmtTime, TimeZoneInfo.Local);
                historyItem.VisitedTime = localTime;

                historyItem.VisitCount = (long)historyRow["visit_count"];
                historyItem.TypedCount = (long)historyRow["typed_count"];
                historyItem.Id = (long)historyRow["id"];

                historyItems.Add(historyItem);

            }

            return historyItems;
        }

        /// <summary>
        /// Returns List of HistoryItem objects that contain (or not) one of strings in given array in their URL
        /// <para>
        /// Opens SQLiteConnection to user's Google Chrome browser history file db corresponding to \Google\Chrome\User Data\Default\History
        /// </para>
        /// </summary>
        /// <remarks>
        ///     <para>
        ///     Throws SQLiteException 'database is locked' exception if there's already opened connection to the database (history file).
        ///     Closing Chrome browser must solve this error
        ///     </para>
        ///     <para>
        ///         <example>
        ///             <code>
        ///             var historyItemsList = GetСhromeHistoryItems(new string[]{"cat", "dog" }, true);
        ///             //This will return all HistoryItems that contain word 'cat' or 'dog' in URL
        ///             </code>
        ///         </example>
        ///     </para>
        /// </remarks>
        /// <param name="containOneOf">Array of strings that may (or not) contain in URL</param>
        /// <param name="mustContain">Determines whether the HistoryItem URL must contain one of the strings in the array</param>
        /// <returns>List&lt;HistoryItem&gt;</returns>
        /// <exception cref="SQLiteException">'database is locked' means that there's already opened connection to the database (history file)</exception>
        public IEnumerable<HistoryItem> GetChromeHistoryItems(string[] containOneOf, bool mustContain)
        {
            ICollection<HistoryItem> historyItems = new List<HistoryItem>();

            SQLiteConnection connection = new SQLiteConnection
                ("Data Source=" + _chromeHistoryFile + ";Version=3;New=False;Compress=True;");

            connection.Open();
            string query = "SELECT * FROM urls";

            if (containOneOf.Length > 0)
            {
                if (mustContain)
                {
                    query += $" WHERE url LIKE '%{containOneOf[0]}%'";

                    for (int i = 1; i < containOneOf.Length; ++i)
                    {
                        query += $" OR url LIKE '%{containOneOf[i]}%'";
                    }
                }
                else
                {
                    query += $" WHERE url NOT LIKE '%{containOneOf[0]}%'";

                    for (int i = 1; i < containOneOf.Length; ++i)
                    {
                        query += $" AND url NOT LIKE '%{containOneOf[i]}%'";
                    }
                }
            }

            query += " ORDER BY last_visit_time DESC";

            SQLiteCommand comp = new SQLiteCommand(query, connection);
            comp.CommandType = CommandType.Text;
            SQLiteDataAdapter adapter = new SQLiteDataAdapter(comp);

            DataTable dataset = new DataTable();
            adapter.Fill(dataset);

            foreach (DataRow historyRow in dataset.Rows)
            {
                HistoryItem historyItem = new HistoryItem
                {
                    URL = new Uri(Convert.ToString(historyRow["url"])),
                    Title = Convert.ToString(historyRow["title"]),
                    BrowserName = BrowserName.Chrome
                };

                //Chrome stores time elapsed since Jan 1, 1601(UTC format) in microseconds
                long utcMicroSeconds = Convert.ToInt64(historyRow["last_visit_time"]);

                //Windows file time UTC is in nanoseconds, so multiplying by 10
                DateTime gmtTime = DateTime.FromFileTimeUtc(10 * utcMicroSeconds);

                //Converting to local time
                DateTime localTime = TimeZoneInfo.ConvertTimeFromUtc(gmtTime, TimeZoneInfo.Local);
                historyItem.VisitedTime = localTime;

                historyItem.VisitCount = (long)historyRow["visit_count"];
                historyItem.TypedCount = (long)historyRow["typed_count"];
                historyItem.Id = (long)historyRow["id"];

                historyItems.Add(historyItem);
            }

            return historyItems;
        }

        /// <summary>
        /// Returns List of HistoryItem objects whose URL contain one of strings in the first array 
        /// and don't contain any of strings in the second array
        /// <para>
        /// Opens SQLiteConnection to user's Google Chrome browser history file db corresponding to \Google\Chrome\User Data\Default\History
        /// </para>
        /// </summary>
        /// <remarks>
        ///     <para>
        ///     Throws SQLiteException 'database is locked' exception if there's already opened connection to the database (history file).
        ///     Closing Chrome browser must solve this error
        ///     </para>
        ///     <para>
        ///         <example>
        ///             <code>
        ///             var historyItemsList = GetСhromeHistoryItems(new string[]{"cat", "dog"}, new string[]{"bird", "fish"});
        ///             //This will return all HistoryItems that contain word 'cat' or 'dog' and don't contain 'bird' and 'fish' in URL
        ///             </code>
        ///         </example>
        ///     </para>
        /// </remarks>
        /// <param name="containOneOf">Array of strings that may contain in URL</param>
        /// <param name="dontContainAnyOf">Array of strings that don't contain in any URL</param>
        /// <returns>List&lt;HistoryItem&gt;</returns>
        /// <exception cref="SQLiteException">'database is locked' means that there's already opened connection to the database (history file)</exception>
        public IEnumerable<HistoryItem> GetChromeHistoryItems(string[] containOneOf, string[] dontContainAnyOf)
        {
            ICollection<HistoryItem> historyItems = new List<HistoryItem>();

            SQLiteConnection connection = new SQLiteConnection
                ("Data Source=" + _chromeHistoryFile + ";Version=3;New=False;Compress=True;");

            connection.Open();
            string query = "SELECT * FROM urls";

            if (containOneOf.Length > 0 || dontContainAnyOf.Length > 0)
            {
                query += " WHERE";

                if (containOneOf.Length > 0)
                {
                    query += " (";
                    query += $" url LIKE '%{containOneOf[0]}%'";

                    for (int i = 1; i < containOneOf.Length; ++i)
                    {
                        query += $" OR url LIKE '%{containOneOf[i]}%'";
                    }

                    query += " )";
                }

                if (containOneOf.Length > 0 && dontContainAnyOf.Length > 0)
                {
                    query += " AND";
                }

                if (dontContainAnyOf.Length > 0)
                {
                    query += " (";
                    query += $" url NOT LIKE '%{dontContainAnyOf[0]}%'";

                    for (int i = 1; i < dontContainAnyOf.Length; ++i)
                    {
                        query += $" AND url NOT LIKE '%{dontContainAnyOf[i]}%'";
                    }

                    query += " )";
                }
            }

            query += " ORDER BY last_visit_time DESC";

            SQLiteCommand comp = new SQLiteCommand(query, connection);
            comp.CommandType = CommandType.Text;
            SQLiteDataAdapter adapter = new SQLiteDataAdapter(comp);

            DataTable dataset = new DataTable();
            adapter.Fill(dataset);

            foreach (DataRow historyRow in dataset.Rows)
            {
                HistoryItem historyItem = new HistoryItem
                {
                    URL = new Uri(Convert.ToString(historyRow["url"])),
                    Title = Convert.ToString(historyRow["title"]),
                    BrowserName = BrowserName.Chrome
                };

                //Chrome stores time elapsed since Jan 1, 1601(UTC format) in microseconds
                long utcMicroSeconds = Convert.ToInt64(historyRow["last_visit_time"]);

                //Windows file time UTC is in nanoseconds, so multiplying by 10
                DateTime gmtTime = DateTime.FromFileTimeUtc(10 * utcMicroSeconds);

                //Converting to local time
                DateTime localTime = TimeZoneInfo.ConvertTimeFromUtc(gmtTime, TimeZoneInfo.Local);
                historyItem.VisitedTime = localTime;

                historyItem.VisitCount = (long)historyRow["visit_count"];
                historyItem.TypedCount = (long)historyRow["typed_count"];
                historyItem.Id = (long)historyRow["id"];

                historyItems.Add(historyItem);
            }

            return historyItems;
        }

        /// <summary>
        /// Returns List of HistoryItem objects
        /// <para>
        /// Opens SQLiteConnection to user's Microsoft Edge browser history file db corresponding to \Microsoft\Edge\User Data\Default\History
        /// </para>
        /// </summary>
        /// <remarks>
        /// Throws SQLiteException 'database is locked' exception if there's already opened connection to the database (history file).
        /// Closing Edge browser and ending Edge processes must solve this error
        /// </remarks>
        /// <returns>List&lt;HistoryItem&gt;</returns>
        /// <exception cref="SQLiteException">'database is locked' means that there's already opened connection to the database (history file)</exception>
        public IEnumerable<HistoryItem> GetEdgeHistoryItems()
        {
            ICollection<HistoryItem> historyItems = new List<HistoryItem>();

            SQLiteConnection connection = new SQLiteConnection
                ("Data Source=" + _edgeHistoryFile + ";Version=3;New=False;Compress=True;");

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
                    Title = Convert.ToString(historyRow["title"]),
                    BrowserName = BrowserName.Edge
                };

                //Similar to Chrome, Edge stores time elapsed since Jan 1, 1601(UTC format) in microseconds
                long utcMicroSeconds = Convert.ToInt64(historyRow["last_visit_time"]);

                //Windows file time UTC is in nanoseconds, so multiplying by 10
                DateTime gmtTime = DateTime.FromFileTimeUtc(10 * utcMicroSeconds);

                //Converting to local time
                DateTime localTime = TimeZoneInfo.ConvertTimeFromUtc(gmtTime, TimeZoneInfo.Local);
                historyItem.VisitedTime = localTime;

                historyItem.VisitCount = (long)historyRow["visit_count"];
                historyItem.TypedCount = (long)historyRow["typed_count"];
                historyItem.Id = (long)historyRow["id"];

                historyItems.Add(historyItem);

            }

            return historyItems;
        }

        /// <summary>
        /// Returns List of HistoryItem objects that contain (or not) one of strings in given array in their URL
        /// <para>
        /// Opens SQLiteConnection to user's Microsoft Edge browser history file db corresponding to \Microsoft\Edge\User Data\Default\History
        /// </para>
        /// </summary>
        /// <remarks>
        ///     <para>
        ///     Throws SQLiteException 'database is locked' exception if there's already opened connection to the database (history file).
        ///     Closing Edge browser must solve this error
        ///     </para>
        ///     <para>
        ///         <example>
        ///             <code>
        ///             var historyItemsList = GetEdgeHistoryItems(new string[]{"cat", "dog" }, true);
        ///             //This will return all HistoryItems that contain word 'cat' or 'dog' in URL
        ///             </code>
        ///         </example>
        ///     </para>
        /// </remarks>
        /// <param name="containOneOf">Array of strings that may (or not) contain in URL</param>
        /// <param name="mustContain">Determines whether the HistoryItem URL must contain one of the strings in the array</param>
        /// <returns>List&lt;HistoryItem&gt;</returns>
        /// <exception cref="SQLiteException">'database is locked' means that there's already opened connection to the database (history file)</exception>
        public IEnumerable<HistoryItem> GetEdgeHistoryItems(string[] containOneOf, bool mustContain)
        {
            ICollection<HistoryItem> historyItems = new List<HistoryItem>();

            SQLiteConnection connection = new SQLiteConnection
                ("Data Source=" + _edgeHistoryFile + ";Version=3;New=False;Compress=True;");

            connection.Open();
            string query = "SELECT * FROM urls";

            if (containOneOf.Length > 0)
            {
                if (mustContain)
                {
                    query += $" WHERE url LIKE '%{containOneOf[0]}%'";

                    for (int i = 1; i < containOneOf.Length; ++i)
                    {
                        query += $" OR url LIKE '%{containOneOf[i]}%'";
                    }
                }
                else
                {
                    query += $" WHERE url NOT LIKE '%{containOneOf[0]}%'";

                    for (int i = 1; i < containOneOf.Length; ++i)
                    {
                        query += $" AND url NOT LIKE '%{containOneOf[i]}%'";
                    }
                }
            }

            query += " ORDER BY last_visit_time DESC";

            SQLiteCommand comp = new SQLiteCommand(query, connection);
            comp.CommandType = CommandType.Text;
            SQLiteDataAdapter adapter = new SQLiteDataAdapter(comp);

            DataTable dataset = new DataTable();
            adapter.Fill(dataset);

            foreach (DataRow historyRow in dataset.Rows)
            {
                HistoryItem historyItem = new HistoryItem
                {
                    URL = new Uri(Convert.ToString(historyRow["url"])),
                    Title = Convert.ToString(historyRow["title"]),
                    BrowserName = BrowserName.Edge
                };

                //Similar to Chrome, Edge stores time elapsed since Jan 1, 1601(UTC format) in microseconds
                long utcMicroSeconds = Convert.ToInt64(historyRow["last_visit_time"]);

                //Windows file time UTC is in nanoseconds, so multiplying by 10
                DateTime gmtTime = DateTime.FromFileTimeUtc(10 * utcMicroSeconds);

                //Converting to local time
                DateTime localTime = TimeZoneInfo.ConvertTimeFromUtc(gmtTime, TimeZoneInfo.Local);
                historyItem.VisitedTime = localTime;

                historyItem.VisitCount = (long)historyRow["visit_count"];
                historyItem.TypedCount = (long)historyRow["typed_count"];
                historyItem.Id = (long)historyRow["id"];

                historyItems.Add(historyItem);

            }

            return historyItems;
        }

        /// <summary>
        /// Returns List of HistoryItem objects whose URL contain one of strings in the first array 
        /// and don't contain any of strings in the second array
        /// <para>
        /// Opens SQLiteConnection to user's Microsoft Edge browser history file db corresponding to \Microsoft\Edge\User Data\Default\History
        /// </para>
        /// </summary>
        /// <remarks>
        ///     <para>
        ///     Throws SQLiteException 'database is locked' exception if there's already opened connection to the database (history file).
        ///     Closing Edge browser must solve this error
        ///     </para>
        ///     <para>
        ///         <example>
        ///             <code>
        ///             var historyItemsList = GetEdgeHistoryItems(new string[]{"cat", "dog"}, new string[]{"bird", "fish"});
        ///             //This will return all HistoryItems that contain word 'cat' or 'dog' and don't contain 'bird' and 'fish' in URL
        ///             </code>
        ///         </example>
        ///     </para>
        /// </remarks>
        /// <param name="containOneOf">Array of strings that may contain in URL</param>
        /// <param name="dontContainAnyOf">Array of strings that don't contain in any URL</param>
        /// <returns>List&lt;HistoryItem&gt;</returns>
        /// <exception cref="SQLiteException">'database is locked' means that there's already opened connection to the database (history file)</exception>
        public IEnumerable<HistoryItem> GetEdgeHistoryItems(string[] containOneOf, string[] dontContainAnyOf)
        {
            ICollection<HistoryItem> historyItems = new List<HistoryItem>();

            SQLiteConnection connection = new SQLiteConnection
                ("Data Source=" + _edgeHistoryFile + ";Version=3;New=False;Compress=True;");

            connection.Open();
            string query = "SELECT * FROM urls";

            if (containOneOf.Length > 0 || dontContainAnyOf.Length > 0)
            {
                query += " WHERE";

                if (containOneOf.Length > 0)
                {
                    query += " (";
                    query += $" url LIKE '%{containOneOf[0]}%'";

                    for (int i = 1; i < containOneOf.Length; ++i)
                    {
                        query += $" OR url LIKE '%{containOneOf[i]}%'";
                    }

                    query += " )";
                }

                if (containOneOf.Length > 0 && dontContainAnyOf.Length > 0)
                {
                    query += " AND";
                }

                if (dontContainAnyOf.Length > 0)
                {
                    query += " (";
                    query += $" url NOT LIKE '%{dontContainAnyOf[0]}%'";

                    for (int i = 1; i < dontContainAnyOf.Length; ++i)
                    {
                        query += $" AND url NOT LIKE '%{dontContainAnyOf[i]}%'";
                    }

                    query += " )";
                }
            }

            query += " ORDER BY last_visit_time DESC";

            SQLiteCommand comp = new SQLiteCommand(query, connection);
            comp.CommandType = CommandType.Text;
            SQLiteDataAdapter adapter = new SQLiteDataAdapter(comp);

            DataTable dataset = new DataTable();
            adapter.Fill(dataset);

            foreach (DataRow historyRow in dataset.Rows)
            {
                HistoryItem historyItem = new HistoryItem
                {
                    URL = new Uri(Convert.ToString(historyRow["url"])),
                    Title = Convert.ToString(historyRow["title"]),
                    BrowserName = BrowserName.Edge
                };

                //Similar to Chrome, Edge stores time elapsed since Jan 1, 1601(UTC format) in microseconds
                long utcMicroSeconds = Convert.ToInt64(historyRow["last_visit_time"]);

                //Windows file time UTC is in nanoseconds, so multiplying by 10
                DateTime gmtTime = DateTime.FromFileTimeUtc(10 * utcMicroSeconds);

                //Converting to local time
                DateTime localTime = TimeZoneInfo.ConvertTimeFromUtc(gmtTime, TimeZoneInfo.Local);
                historyItem.VisitedTime = localTime;

                historyItem.VisitCount = (long)historyRow["visit_count"];
                historyItem.TypedCount = (long)historyRow["typed_count"];
                historyItem.Id = (long)historyRow["id"];

                historyItems.Add(historyItem);

            }

            return historyItems;
        }

        /// <summary>
        /// Returns List of HistoryItem objects
        /// <para>
        /// Opens SQLiteConnection to user's Mozilla Firefox browser history file db corresponding to \Mozilla\Firefox\Profiles\%PROFILE_NAME%\places.sqlite
        /// </para>
        /// </summary>
        /// <remarks>
        /// Throws SQLiteException 'database is locked' exception if there's already opened connection to the database (history file).
        /// Closing Firefox browser must solve this error
        /// </remarks>
        /// <returns>List&lt;HistoryItem&gt;</returns>
        /// <exception cref="SQLiteException">'database is locked' means that there's already opened connection to the database (history file)</exception>
        public IEnumerable<HistoryItem> GetFirefoxHistoryItems()
        {
            ICollection<HistoryItem> historyItems = new List<HistoryItem>();

            string firefoxHistoryFile = FindFirefoxProfileFolderPath(_firefoxFolder) + "\\" + _firefoxHistoryFileName;

            SQLiteConnection connection = new SQLiteConnection
                ("Data Source=" + firefoxHistoryFile + ";Version=3;New=False;Compress=True;");

            connection.Open();

            SQLiteCommand comp = new SQLiteCommand("SELECT * FROM moz_places ORDER BY last_visit_date DESC", connection);
            comp.CommandType = CommandType.Text;
            SQLiteDataAdapter adapter = new SQLiteDataAdapter(comp);

            DataTable dataset = new DataTable();
            adapter.Fill(dataset);

            foreach (DataRow historyRow in dataset.Rows)
            {
                HistoryItem historyItem = new HistoryItem
                {
                    URL = new Uri(Convert.ToString(historyRow["url"])),
                    Title = Convert.ToString(historyRow["title"]),
                    BrowserName = BrowserName.Firefox
                };

                //Firefox stores time elapsed since the Unix epoch (January 1, 1970) in microseconds
                var time = historyRow["last_visit_date"];
                if (time is not DBNull)
                {
                    long utcMicroSeconds = Convert.ToInt64(time);

                    //Windows file time UTC is in nanoseconds, so multiplying by 10
                    DateTime gmtTime = DateTime.FromFileTimeUtc(10 * utcMicroSeconds);

                    //Converting to local time
                    DateTime epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
                    DateTime localTime = epoch.AddSeconds(utcMicroSeconds / 1000000).ToLocalTime();
                    historyItem.VisitedTime = localTime;
                }

                historyItem.VisitCount = (long)historyRow["visit_count"];
                historyItem.TypedCount = (long)historyRow["typed"];
                historyItem.Id = (long)historyRow["id"];

                historyItems.Add(historyItem);

            }

            return historyItems;
        }

        /// <summary>
        /// Returns List of HistoryItem objects that contain (or not) one of strings in given array in their URL.
        /// <para>
        /// Opens SQLiteConnection to user's Mozilla Firefox browser history file db corresponding to \Mozilla\Firefox\Profiles\%PROFILE_NAME%\places.sqlite
        /// </para>
        /// </summary>
        /// <remarks>
        ///     <para>
        ///     Throws SQLiteException 'database is locked' exception if there's already opened connection to the database (history file).
        ///     Closing Firefox browser must solve this error
        ///     </para>
        ///     <para>
        ///         <example>
        ///             <code>
        ///             var historyItemsList = GetFirefoxHistoryItems(new string[]{"cat", "dog" }, true);
        ///             //This will return all HistoryItems that contain word 'cat' or 'dog' in URL
        ///             </code>
        ///         </example>
        ///     </para>
        /// </remarks>
        /// <param name="containOneOf">Array of strings that may (or not) contain in URL</param>
        /// <param name="mustContain">Determines whether the HistoryItem URL must contain one of the strings in the array</param>
        /// <returns>List&lt;HistoryItem&gt;</returns>
        /// <exception cref="SQLiteException">'database is locked' means that there's already opened connection to the database (history file)</exception>
        public IEnumerable<HistoryItem> GetFirefoxHistoryItems(string[] containOneOf, bool mustContain)
        {
            ICollection<HistoryItem> historyItems = new List<HistoryItem>();

            string firefoxHistoryFile = FindFirefoxProfileFolderPath(_firefoxFolder) + "\\" + _firefoxHistoryFileName;

            SQLiteConnection connection = new SQLiteConnection
                ("Data Source=" + firefoxHistoryFile + ";Version=3;New=False;Compress=True;");

            connection.Open();
            string query = "SELECT * FROM moz_places";

            if (containOneOf.Length > 0)
            {
                if (mustContain)
                {
                    query += $" WHERE url LIKE '%{containOneOf[0]}%'";

                    for (int i = 1; i < containOneOf.Length; ++i)
                    {
                        query += $" OR url LIKE '%{containOneOf[i]}%'";
                    }
                }
                else
                {
                    query += $" WHERE url NOT LIKE '%{containOneOf[0]}%'";

                    for (int i = 1; i < containOneOf.Length; ++i)
                    {
                        query += $" AND url NOT LIKE '%{containOneOf[i]}%'";
                    }
                }
            }

            query += " ORDER BY last_visit_date DESC";

            SQLiteCommand comp = new SQLiteCommand(query, connection);
            comp.CommandType = CommandType.Text;
            SQLiteDataAdapter adapter = new SQLiteDataAdapter(comp);

            DataTable dataset = new DataTable();
            adapter.Fill(dataset);

            foreach (DataRow historyRow in dataset.Rows)
            {
                HistoryItem historyItem = new HistoryItem
                {
                    URL = new Uri(Convert.ToString(historyRow["url"])),
                    Title = Convert.ToString(historyRow["title"]),
                    BrowserName = BrowserName.Firefox
                };

                //Firefox stores time elapsed since the Unix epoch (January 1, 1970) in microseconds
                var time = historyRow["last_visit_date"];
                if (time is not DBNull)
                {
                    long utcMicroSeconds = Convert.ToInt64(time);

                    //Windows file time UTC is in nanoseconds, so multiplying by 10
                    DateTime gmtTime = DateTime.FromFileTimeUtc(10 * utcMicroSeconds);

                    //Converting to local time
                    DateTime epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
                    DateTime localTime = epoch.AddSeconds(utcMicroSeconds / 1000000).ToLocalTime();
                    historyItem.VisitedTime = localTime;
                }

                historyItem.VisitCount = (long)historyRow["visit_count"];
                historyItem.TypedCount = (long)historyRow["typed"];
                historyItem.Id = (long)historyRow["id"];

                historyItems.Add(historyItem);

            }

            return historyItems;
        }

        /// <summary>
        /// Returns List of HistoryItem objects whose URL contain one of strings in the first array 
        /// and don't contain any of strings in the second array
        /// <para>
        /// Opens SQLiteConnection to user's Mozilla Firefox browser history file db corresponding to \Mozilla\Firefox\Profiles\%PROFILE_NAME%\places.sqlite
        /// </para>
        /// </summary>
        /// <remarks>
        ///     <para>
        ///     Throws SQLiteException 'database is locked' exception if there's already opened connection to the database (history file).
        ///     Closing Firefox browser must solve this error
        ///     </para>
        ///     <para>
        ///         <example>
        ///             <code>
        ///             var historyItemsList = GetFirefoxHistoryItems(new string[]{"cat", "dog"}, new string[]{"bird", "fish"});
        ///             //This will return all HistoryItems that contain word 'cat' or 'dog' and don't contain 'bird' and 'fish' in URL
        ///             </code>
        ///         </example>
        ///     </para>
        /// </remarks>
        /// <param name="containOneOf">Array of strings that may contain in URL</param>
        /// <param name="dontContainAnyOf">Array of strings that don't contain in any URL</param>
        /// <returns>List&lt;HistoryItem&gt;</returns>
        /// <exception cref="SQLiteException">'database is locked' means that there's already opened connection to the database (history file)</exception>
        public IEnumerable<HistoryItem> GetFirefoxHistoryItems(string[] containOneOf, string[] dontContainAnyOf)
        {
            ICollection<HistoryItem> historyItems = new List<HistoryItem>();

            string firefoxHistoryFile = FindFirefoxProfileFolderPath(_firefoxFolder) + "\\" + _firefoxHistoryFileName;

            SQLiteConnection connection = new SQLiteConnection
                ("Data Source=" + firefoxHistoryFile + ";Version=3;New=False;Compress=True;");

            connection.Open();
            string query = "SELECT * FROM moz_places";

            if (containOneOf.Length > 0 || dontContainAnyOf.Length > 0)
            {
                query += " WHERE";

                if (containOneOf.Length > 0)
                {
                    query += " (";
                    query += $" url LIKE '%{containOneOf[0]}%'";

                    for (int i = 1; i < containOneOf.Length; ++i)
                    {
                        query += $" OR url LIKE '%{containOneOf[i]}%'";
                    }

                    query += " )";
                }

                if (containOneOf.Length > 0 && dontContainAnyOf.Length > 0)
                {
                    query += " AND";
                }

                if (dontContainAnyOf.Length > 0)
                {
                    query += " (";
                    query += $" url NOT LIKE '%{dontContainAnyOf[0]}%'";

                    for (int i = 1; i < dontContainAnyOf.Length; ++i)
                    {
                        query += $" AND url NOT LIKE '%{dontContainAnyOf[i]}%'";
                    }

                    query += " )";
                }
            }

            query += " ORDER BY last_visit_date DESC";

            SQLiteCommand comp = new SQLiteCommand(query, connection);
            comp.CommandType = CommandType.Text;
            SQLiteDataAdapter adapter = new SQLiteDataAdapter(comp);

            DataTable dataset = new DataTable();
            adapter.Fill(dataset);

            foreach (DataRow historyRow in dataset.Rows)
            {
                HistoryItem historyItem = new HistoryItem
                {
                    URL = new Uri(Convert.ToString(historyRow["url"])),
                    Title = Convert.ToString(historyRow["title"]),
                    BrowserName = BrowserName.Firefox
                };

                //Firefox stores time elapsed since the Unix epoch (January 1, 1970) in microseconds
                var time = historyRow["last_visit_date"];
                if (time is not DBNull)
                {
                    long utcMicroSeconds = Convert.ToInt64(time);

                    //Windows file time UTC is in nanoseconds, so multiplying by 10
                    DateTime gmtTime = DateTime.FromFileTimeUtc(10 * utcMicroSeconds);

                    //Converting to local time
                    DateTime epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
                    DateTime localTime = epoch.AddSeconds(utcMicroSeconds / 1000000).ToLocalTime();
                    historyItem.VisitedTime = localTime;
                }

                historyItem.VisitCount = (long)historyRow["visit_count"];
                historyItem.TypedCount = (long)historyRow["typed"];
                historyItem.Id = (long)historyRow["id"];

                historyItems.Add(historyItem);
            }

            return historyItems;
        }

        private string FindFirefoxProfileFolderPath(string folderPath)
        {
            string[] directories = Directory.GetDirectories(folderPath);
            string folder = directories[0];

            for (int i = 1; i < directories.Length; i++)
            {
                if (Directory.GetFiles(folder, ".sqlite").Length > 0) break;
                folder = directories[i];
            }

            return folder;
        }
    }
}