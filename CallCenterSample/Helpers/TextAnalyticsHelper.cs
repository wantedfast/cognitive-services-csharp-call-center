// 
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license.
// 
// Microsoft Cognitive Services: http://www.microsoft.com/cognitive
// 
// Microsoft Cognitive Services Github:
// https://github.com/Microsoft/Cognitive
// 
// Copyright (c) Microsoft Corporation
// All rights reserved.
// 
// MIT License:
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
// 
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED ""AS IS"", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
// 

using Azure;
using Azure.AI.TextAnalytics;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CallCenterSample.Helpers
{
    public class TextAnalyticsHelper
    {
        private static TextAnalyticsClient TextAnalyticsClient;

        private static string _apiKey;
        private static string _endpoint;

        public static string ApiKey
        {
            get { return _apiKey; }

            set
            {
                bool isChanged = _apiKey != value;
                _apiKey = value;

                if (isChanged)
                {
                    InitializeTextAnalyticsClient();
                }
            }
        }

        public static string Endpoint
        {
            get { return _endpoint; }

            set
            {
                bool isChanged = _endpoint != value;
                _endpoint = value;

                if (isChanged)
                {
                    InitializeTextAnalyticsClient();
                }
            }
        }

        private static void InitializeTextAnalyticsClient()
        {
            Argument.AssertNotNullOrEmpty(Endpoint, nameof(Endpoint));
            Argument.AssertNotNullOrEmpty(ApiKey, nameof(ApiKey));
            TextAnalyticsClient = new TextAnalyticsClient(new Uri(Endpoint), new AzureKeyCredential(ApiKey));
        }

        public static async Task<DetectLanguageResult> GetDetectedLanguageAsync(string input)
        {
            DetectLanguageResult languageResult = new DetectLanguageResult() { Language = new Dictionary<string, string>() };
            Argument.AssertNotNullOrEmpty(input, nameof(input));

            DetectedLanguage result = await TextAnalyticsClient.DetectLanguageAsync(input);

            languageResult.Language.Add("iso6391Name", result.Iso6391Name);
            languageResult.Language.Add("name", result.Name);
            languageResult.Language.Add("score", result.ConfidenceScore.ToString());

            return languageResult;
        }

        public static async Task<SentimentResult> GetTextSentimentAsync(string input)
        {
            SentimentResult sentimentResult = new SentimentResult() { Score = 0.5 };
            Argument.AssertNotNullOrEmpty(input, nameof(input));

            DocumentSentiment result = await TextAnalyticsClient.AnalyzeSentimentAsync(input, "en");

            sentimentResult.Score = result.ConfidenceScores.Neutral;

            return sentimentResult;
        }

        public static async Task<KeyPhrasesResult> GetKeyPhrasesAsync(string input)
        {
            KeyPhrasesResult keyPhrasesResult = new KeyPhrasesResult() { KeyPhrases = new List<string>() };
            Argument.AssertNotNullOrEmpty(input, nameof(input));

            KeyPhraseCollection result = await TextAnalyticsClient.ExtractKeyPhrasesAsync(input, "en");

            foreach (string keyPhrase in result)
            {
                keyPhrasesResult.KeyPhrases.Add(keyPhrase);
            }

            return keyPhrasesResult;
        }
    }

    public class SentimentResult
    {
        public double Score { get; set; }
    }

    public class DetectLanguageResult
    {
        public Dictionary<string, string> Language { get; set; }
    }

    public class KeyPhrasesResult
    {
        public IList<string> KeyPhrases { get; set; }
    }

    public class Argument
    {
        public static void AssertNotNullOrEmpty(string value, string name)
        {
            if (value is null)
            {
                throw new ArgumentNullException(name);
            }

            if (value.Length == 0)
            {
                throw new ArgumentException($"{name} cannot be empty.");
            }
        }
    }
}
