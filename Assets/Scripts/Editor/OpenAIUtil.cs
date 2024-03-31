using System.Threading;
using UnityEditor;
using UnityEngine;
using UnityEngine.Networking;

namespace ChatGPTInUnity.OpenAI
{
    public static class OpenAIUtil
    {
        public static string CreateChatRequestBody(string prompt)
        {
            Message message = new Message() { role = "user", content = prompt };
            Request request = new Request() { model = "gpt-3.5-turbo", messages = new Message[] { message } };

            Debug.Log(request.messages[0].content);

            return JsonUtility.ToJson(request);
        }

        public static string InvokeChat(string prompt)
        {
            CommandSettings settings = CommandSettings.instance;

            // Post
            using UnityWebRequest post = UnityWebRequest.Post(API.URL, CreateChatRequestBody(prompt));

            post.timeout = settings.timeout;

            post.SetRequestHeader("Content-Type", "application/json");
            post.SetRequestHeader("Authorization", $"Bearer {settings.apiKey}");

            UnityWebRequestAsyncOperation request = post.SendWebRequest();

            // Progress bar
            for (float progress = 0f; !request.isDone; progress += 0.01f)
            {
                EditorUtility.DisplayProgressBar("Commands", "Generating...", progress);
                Thread.Sleep(100);
            }

            EditorUtility.ClearProgressBar();

            // Response extraction
            Response data = JsonUtility.FromJson<Response>(post.downloadHandler.text);

            return data.choices[0].message.content;
        }
    }
}