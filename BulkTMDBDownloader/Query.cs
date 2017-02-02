using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BulkTMDBDownloader
{
    class Query
    {
        private const string Uribase = "https://api.themoviedb.org/3/";
        private const string Apikey = "302a28376c434c22df5e3af0f26f2fe8";

        public enum RequestTypes{ Search, Get }
        public Action<string> CallbackAction;
        public RequestTypes RequestType;

        //Search
        private string _keywords;
        private string _year;
        private string _language;
        private string _region;

        //Get
        private string _type;
        private string _id;

        public Query(RequestTypes requestType)
        {
            RequestType = requestType;
            _keywords = null;
            _year = null;
            _language = null;
            _region = null;
            _type = null;
            _id = null;
        }

        /// <summary>
        /// Set keywords
        /// </summary>
        /// <param name="keywords">Keywords</param>
        /// <returns>Query</returns>
        public Query Keywords(string keywords)
        {
            _keywords = keywords;
            return this;
        }

        /// <summary>
        /// Set year
        /// </summary>
        /// <param name="year">Year</param>
        /// <returns>Query</returns>
        public Query Year(string year)
        {
            _year = year;
            return this;
        }

        /// <summary>
        /// Set language
        /// </summary>
        /// <param name="language">Language</param>
        /// <returns>Query</returns>
        public Query Language(string language)
        {
            _language = language;
            return this;
        }

        /// <summary>
        /// Set region
        /// </summary>
        /// <param name="region">Region</param>
        /// <returns>Query</returns>
        public Query Region(string region)
        {
            _region = region;
            return this;
        }

        /// <summary>
        /// Set media type
        /// </summary>
        /// <param name="type">Media Type</param>
        /// <returns>Query</returns>
        public Query Type(string type)
        {
            _type = type;
            return this;
        }

        /// <summary>
        /// Set id
        /// </summary>
        /// <param name="id">ID</param>
        /// <returns>Query</returns>
        public Query Id(string id)
        {
            _id = id;
            return this;
        }

        /// <summary>
        /// Set callback method
        /// </summary>
        /// <param name="callback">Callback method</param>
        /// <returns>Query</returns>
        public Query Callback(Action<string> callback)
        {
            CallbackAction = callback;
            return this;
        }

        /// <summary>
        /// Get the Url for the query
        /// </summary>
        /// <returns>Query URL</returns>
        public string GetUrl()
        {
            return RequestType == RequestTypes.Get ? GetGetUrl() : GetSearchUrl();
        }

        /// <summary>
        /// Get the URL for a search query
        /// </summary>
        /// <returns>Search query URL</returns>
        private string GetSearchUrl()
        {
            string result = $"{Uribase}search/multi?api_key={Apikey}&query={_keywords}";
            result += (_year == null ? "" : $"&year={_year}");
            result += (_language == null ? "" : $"&language={_language}");
            result += (_region == null ? "" : $"&region={_region}");
            return result;
        }

        /// <summary>
        /// Get the URL for a get query
        /// </summary>
        /// <returns>Get query url</returns>
        private string GetGetUrl()
        {
            string result = $"{Uribase}{_type}/{_id}?api_key={Apikey}";
            result += (_language == null ? "" : $"&language={_language}");
            return result;
        }
    }
}
