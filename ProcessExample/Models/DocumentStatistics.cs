
namespace ProcessExample.Models
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Runtime.Serialization;

    /// <summary>
    /// Data class model to contain relevant information for a document
    /// </summary>
    /// <remarks>
    /// Author: Andrew Tangeman
    /// Date: 05/12/2017
    /// </remarks>
    [DataContract]
    public class DocumentStatistics
    {
        /// <summary>
        /// Gets or sets document count
        /// </summary>
        [DataMember(Name = "DocumentCount", Order = 0)]
        public int DocumentCount { get; set; }

        /// <summary>
        /// Gets or sets document list
        /// </summary>
        [DataMember(Name = "Documents", Order = 1)]
        public List<string> Documents { get; set; }

        /// <summary>
        /// Gets or sets word counts.
        /// </summary>
        [DataMember(Name = "WordCounts", Order = 2)]
        public Dictionary<string, int> WordCounts { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="DocumentStatistics"/> class.
        /// </summary>
        public DocumentStatistics()
        {
            DocumentCount = 0;
            Documents = new List<string>();
            WordCounts = new Dictionary<string, int>();
        }

        /// <summary>
        /// Method to count words in a document.
        /// </summary>
        /// <param name="wordList">String array wordlist to use to obtain final count</param>
        public void CountWords(string[] wordList)
        {

            string[] wordListIterable = wordList;
            foreach (string word in wordListIterable)
            {
                if (!WordCounts.ContainsKey(word) && word.Length > 0)
                {
                    WordCounts.Add(word, 1);
                }
                else
                {
                    int v = int.MinValue;
                    if (WordCounts.TryGetValue(word, out v) && word.Length > 0)
                    {
                        WordCounts[word] = 1 + v;
                    }
                }
            }
        }
    }
}
