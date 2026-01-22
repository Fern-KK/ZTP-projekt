using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using Tasks = System.Threading.Tasks; //Ten fragment jest bardzo ważny. Zapewnia on brak kolizji między klasą Task a Tasks.Task
using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Globalization;
using System;
using System.IO;
using System.Collections.Generic;
namespace ZTP;
public class ServerConnection
{
    private static ServerConnection? Instance;
    public string? Username; // Tak samo jak poniżej 
    private string? Token; // Pole do potencjalnego przyszłego rozwoju na różnych użytkowników
    private HttpClient PersonalHttpClient = new HttpClient();
    private string BaseLink = "https://logan667.pythonanywhere.com";
    private ServerConnection()
    {}
    public static ServerConnection CreateServerConnection()
    {
        if (Instance == null)
        {
            Instance = new ServerConnection();
            Instance.PersonalHttpClient = new HttpClient();
            Instance.Username = "nexus";
            Instance.Token = "17682096973829nexus";
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

    public async Tasks.Task<bool> FetchContent()
    {
        var payload = new
        {
            username = this.Username,
            token = this.Token
        };
        try
        {
            var response = await PersonalHttpClient.PostAsJsonAsync(
                $"{BaseLink}/api/users/fetch",
                payload
            );

            if (!response.IsSuccessStatusCode) return false;
            var json = await response.Content.ReadFromJsonAsync<JsonElement>();
            var notes = json
                .GetProperty("data")
                .GetProperty("notes");
            foreach(var note in notes.EnumerateArray())
            {
                var new_note = new Note(note.GetProperty("title").GetString(), note.GetProperty("content").GetString());
                new_note.SetCategory(note.GetProperty("category").GetString());
                new_note.SetTags(note.GetProperty("tags").EnumerateArray().Select(t => t.GetString()).ToList());
                new_note.SetId(note.GetProperty("note_id").GetInt32());
                GlobalGroups.AllGroup.Add(new_note);
                GlobalGroups.AllNotesGroup.Add(new_note);
            }
            var tasks = json
                .GetProperty("data")
                .GetProperty("tasks");
            foreach(var task in tasks.EnumerateArray())
            {                
                DateTime deadline = DateTime.ParseExact(task.GetProperty("deadline").GetString(),"dd.MM.yyyy",CultureInfo.InvariantCulture);
                var new_task = new Task(task.GetProperty("title").GetString(), deadline);
                new_task.SetCategory(task.GetProperty("category").GetString ());
                new_task.SetTags(task.GetProperty("tags").EnumerateArray().Select(t => t.GetString()).ToList());
                var priorityString = task.GetProperty("priority").GetString();
                    if (!Enum.TryParse<Priorities>(priorityString, out var priority))
                    {
                        priority = Priorities.None;
                    }
                
                new_task.SetPriority(priority);
                GlobalGroups.AllGroup.Add(new_task);
                GlobalGroups.AllTasksGroup.Add(new_task);
            }
            return json.GetProperty("status").GetString() == "success";
        }
        catch (Exception ex)
        {
            try
            {
                var log = $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}]\n{ex}\n\n";
                File.AppendAllText("errors.txt", log);
            }
            catch
            {
                
            }

            return false;
        }
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
            var id = json
                .GetProperty("data")
                .GetProperty("note_id")
                .GetInt32();
            note.SetId(id);
            return json.GetProperty("status").GetString() == "success";
        }
        catch (Exception ex)
        {
            try
            {
                var log = $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}]\n{ex}\n\n";
                File.AppendAllText("errors.txt", log);
            }
            catch
            {
                
            }

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
        catch (Exception ex)
        {
            try
            {
                var log = $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}]\n{ex}\n\n";
                File.AppendAllText("errors.txt", log);
            }
            catch
            {
                
            }

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
        catch (Exception ex)
        {
            try
            {
                var log = $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}]\n{ex}\n\n";
                File.AppendAllText("errors.txt", log);
            }
            catch
            {
                
            }

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
            content = "-", //gdzie jest Content?
            category = task.Category,
            priority = task.Priority.ToString(),
            deadline = task.EndDate?.ToString("dd.MM.yyyy")
        };

        try
        {
            var response = await PersonalHttpClient.PostAsJsonAsync(
                $"{BaseLink}/api/tasks/new",
                payload
            );

            if (!response.IsSuccessStatusCode) return false;
            var json = await response.Content.ReadFromJsonAsync<JsonElement>();
            var id = json
                .GetProperty("data")
                .GetProperty("task_id")
                .GetInt32();
            task.SetId(id);
            return json.GetProperty("status").GetString() == "success";
        }
        catch (Exception ex)
        {
            try
            {
                var log = $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}]\n{ex}\n\n";
                File.AppendAllText("errors.txt", log);
            }
            catch
            {
                
            }

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
        catch (Exception ex)
        {
            try
            {
                var log = $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}]\n{ex}\n\n";
                File.AppendAllText("errors.txt", log);
            }
            catch
            {
                
            }

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
            content = "-",
            category = task.Category,
            priority = task.Priority.ToString().ToLowerInvariant(),
            deadline = task.EndDate?.ToString("MM-dd-yyyy"),
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
        catch (Exception ex)
        {
            try
            {
                var log = $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}]\n{ex}\n\n";
                File.AppendAllText("errors.txt", log);
            }
            catch
            {
                
            }

            return false;
        }
    }
}