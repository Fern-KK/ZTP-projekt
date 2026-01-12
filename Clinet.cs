using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using Tasks = System.Threading.Tasks; //UWAGA KUREWSKO WAŻNY FRAGMENT PANOWIE
//ŻEBY NIE KOLIDOWAŁO Z KLASĄ Task
using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System;
using System.Collections.Generic;

namespace ZTP;
public class ServerConnection
{
    private static ServerConnection? Instance;
    public string? Username;
    private string? Token;
    private HttpClient? PersonalHttpClient;
    private string BaseLink = "https://logan667.pythonanywhere.com/";
    private ServerConnection()
    {}
    public ServerConnection CreateServerConnection()
    {
        if (Instance == null)
        {
            Instance = new ServerConnection();
            Instance.PersonalHttpClient = new HttpClient();
        }
        return Instance;
    }
    public async Tasks.Task<bool> SignIn(string username, string password)
    {
        var payload = new
        {
            username = username,
            password = password
        };
        var res = await this.PersonalHttpClient.PostAsJsonAsync($"{BaseLink}/api/user/new", payload);
        if (!res.IsSuccessStatusCode) return false;
        var json = await res.Content.ReadFromJsonAsync<JsonElement>();
        if (json.GetProperty("status").GetString() != "success") return false;
        return true;
    }
    public async Tasks.Task<bool> LogIn(string username, string password)
    {
        var payload = new
        {
            username = username,
            password = password
        };
        var res = await this.PersonalHttpClient.PostAsJsonAsync($"{BaseLink}/api/user/verify_login",payload);
        if (!res.IsSuccessStatusCode) return false;
        var json = await res.Content.ReadFromJsonAsync<JsonElement>();
        if (json.GetProperty("status").GetString() != "success") return false;
        this.Username = json.GetProperty("data").GetProperty("username").GetString();
        this.Token = json.GetProperty("data").GetProperty("token").GetString();
        return true;
    }

    public void FetchContent()
    {
        
    }
    
    public async Tasks.Task<bool> NewNote(Note note)
    {
        var payload = new
        {
            username = this.Username,
            token = this.Token,
            title = note.Name,
            content = note.Content,
            category = note.Category,
            tags = note.Tags
        };

        try
        {
            var response = await PersonalHttpClient.PostAsJsonAsync(
                $"{BaseLink}/api/notes/new",
                payload
            );

            if (!response.IsSuccessStatusCode) return false;
            var json = await response.Content.ReadFromJsonAsync<JsonElement>();
            note.NoteId = json
                .GetProperty("data")
                .GetProperty("note_id")
                .GetInt32();

            return json.GetProperty("status").GetString() == "success";
        }
        catch
        {
            return false;
        }
    }
    public async Tasks.Task<bool> DeleteNote(int NoteId)
    {
        var payload = new
        {
            username = this.Username,
            token = this.Token,
            note_id = NoteId
        };
        try
        {
            var req = new HttpRequestMessage(HttpMethod.Delete,
            $"{BaseLink}/api/notes/delete")
            {
                Content = JsonContent.Create(payload)
            };

            var res = await PersonalHttpClient.SendAsync(req);
            return res.IsSuccessStatusCode;
        }
        catch
        {
            return false;
        }
    }
    public async Tasks.Task<bool> UpdateNote(Note note, int NoteId)
    {
        var payload = new
        {
            username = this.Username,
            token = this.Token,
            title = note.Name,
            content = note.Content,
            category = note.Category,
            tags = note.Tags,
            note_id = NoteId
        };
        try
        {
            var req = new HttpRequestMessage(HttpMethod.Patch,
            $"{BaseLink}/api/notes/update")
            {
                Content = JsonContent.Create(payload)
            };

            var res = await PersonalHttpClient.SendAsync(req);
            return res.IsSuccessStatusCode;
        }
        catch
        {
            return false;
        }
    }
    public async Tasks.Task<bool> NewTask(Task task)
    {
        var payload = new
        {
            username = this.Username,
            token = this.Token,
            title = task.Name,
            content = "Task content missing", //gdzie jest Content?
            category = task.Category,
            priority = task.Priority.ToString().ToLowerInvariant(),
            deadline = task.EndDate?.ToString("yyyy-MM-dd")
        };

        try
        {
            var response = await PersonalHttpClient.PostAsJsonAsync(
                $"{BaseLink}/api/tasks/new",
                payload
            );

            if (!response.IsSuccessStatusCode) return false;
            var json = await response.Content.ReadFromJsonAsync<JsonElement>();
            task.TaskId = json
                .GetProperty("data")
                .GetProperty("note_id")
                .GetInt32();

            return json.GetProperty("status").GetString() == "success";
        }
        catch
        {
            return false;
        }
    }
    public async Tasks.Task<bool> DeleteTask(int TaskId)
    {
        var payload = new
        {
            username = this.Username,
            token = this.Token,
            task_id = TaskId
        };
        try
        {
            var req = new HttpRequestMessage(HttpMethod.Delete,
            $"{BaseLink}/api/tasks/delete")
            {
                Content = JsonContent.Create(payload)
            };

            var res = await PersonalHttpClient.SendAsync(req);
            return res.IsSuccessStatusCode;
        }
        catch
        {
            return false;
        }
    }
    public async Tasks.Task<bool> UpdateTask(Task task, int TaskId)
    {
        var payload = new
        {
            username = this.Username,
            token = this.Token,
            title = task.Name,
            content = "Task content missing", //gdzie jest Content?
            category = task.Category,
            priority = task.Priority.ToString().ToLowerInvariant(),
            deadline = task.EndDate?.ToString("yyyy-MM-dd"),
            task_id = TaskId
        };
        try
        {
            var req = new HttpRequestMessage(HttpMethod.Patch,
            $"{BaseLink}/api/notes/update")
            {
                Content = JsonContent.Create(payload)
            };

            var res = await PersonalHttpClient.SendAsync(req);
            return res.IsSuccessStatusCode;
        }
        catch
        {
            return false;
        }
    }
}