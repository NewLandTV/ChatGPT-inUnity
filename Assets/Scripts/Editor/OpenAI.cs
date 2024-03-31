using System;

namespace ChatGPTInUnity.OpenAI
{
    public static class API
    {
        public const string URL = "https://api.openai.com/v1/chat/completions";
    }

    [Serializable]
    public struct Message
    {
        public string role;
        public string content;
    }

    [Serializable]
    public struct ResponseChoice
    {
        public int index;

        public Message message;
    }

    [Serializable]
    public struct Response
    {
        public string id;

        public ResponseChoice[] choices;
    }

    [Serializable]
    public struct Request
    {
        public string model;

        public Message[] messages;
    }
}