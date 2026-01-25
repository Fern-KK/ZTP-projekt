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
using ZTP.Composite;

namespace ZTP;

/*
                                            ︵‿︵‿୨♡୧‿︵‿︵
                                    Hello and welcome to my codebase
                                'm a big time C# antifan, so please be patient 
        ServerConnection class is an simple way to communicate with out python server (https://logan667.pythonanywhere.com)
            It uses a Singleton design pattern, guaranteeing that there is no more than one HttpClient per aplication
                                            ︵‿︵‿୨♡୧‿︵‿︵
*/
public class ServerConnection
{
    private static ServerConnection? Instance;
    public string? Username; // TThis field is not in use right now, it's for future development
    private string? Token; // This one just as the username field
    private HttpClient PersonalHttpClient = new HttpClient();
    private string BaseLink = "https://logan667.pythonanywhere.com"; //the base link leading to our server. Paste it into google, and see what happens
    private ServerConnection()
    {} //private constructor, as Singleton should have
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
    } // This method allows client proces to get an instance of ServerConnection

    //Below: user handling code. Not in use right now, because
    //accounts are the future feture
    //For now, we are using the "nexus" account, which we can treat as a sort of admin account
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
    //FetchContent function fetches all the task, all the notes and all the tasklists from the server
    //It sends users username and token, which they normaly would get from server using LogIn function
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
                DataManager.AllGroup.Add(new_note);
                DataManager.AllNotesGroup.Add(new_note);
            }
            var tasks = json
                .GetProperty("data")
                .GetProperty("tasks");
            foreach(var task in tasks.EnumerateArray())
            {   
                var deadline = DateTime.TryParseExact(task.GetProperty("deadline").GetString(), "dd.MM.yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out var d) ? d : (DateTime?)null;

                var new_task = new Task(task.GetProperty("title").GetString(), deadline);
                new_task.SetCategory(task.GetProperty("category").GetString ());
                new_task.SetTags(task.GetProperty("tags").EnumerateArray().Select(t => t.GetString()).ToList());
                var priorityString = task.GetProperty("priority").GetString();
                if (!Enum.TryParse<Priorities>(priorityString, out var priority))
                {
                    priority = Priorities.None;
                }
                new_task.SetPriority(priority);
                DataManager.AllGroup.Add(new_task);
                DataManager.AllTasksGroup.Add(new_task);     
                
            }
            var task_lists = json
                .GetProperty("data")
                .GetProperty("task_lists");
            foreach(var task_list in task_lists.EnumerateArray())
            {
                var new_task_list = new TaskList(task_list.GetProperty("title").GetString());
                var priorityString = task_list.GetProperty("priority").GetString();
                if (!Enum.TryParse<Priorities>(priorityString, out var priority))
                {
                    priority = Priorities.None;
                }
                new_task_list.SetPriority(priority);
                if(task_list.GetProperty("category").GetString() != null)
                {
                    new_task_list.SetCategory(task_list.GetProperty("category").GetString()); 
                }
                new_task_list.SetTags(task_list.GetProperty("tags").EnumerateArray().Select(t => t.GetString()).ToList());
                foreach (var task in task_list.GetProperty("tasks").EnumerateArray())
                {
                    var deadline = DateTime.TryParseExact(task.GetProperty("deadline").GetString(), "dd.MM.yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out var d) ? d : (DateTime?)null;
                    var new_task_list_task = new Task(task.GetProperty("title").ToString(), deadline);
                    new_task_list.Add(new_task_list_task);
                }
                DataManager.AllGroup.Add(new_task_list);
                DataManager.AllTasksGroup.Add(new_task_list);
            }



            return json.GetProperty("status").GetString() == "success";
        }
        //This style of error handling is used by every function of our code
        // In case of any errors, they are logged into the errors.txt file
        catch (Exception ex)
        {
            try
            {
                var log = $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}]\n{ex}\n\n";
                File.AppendAllText("errors.txt", log);
            }
            catch
            {}//this empty "catch" statement is used to make sure no errors make the code loop infinitly
            return false;
        }
    }
    //Below: notes handling. NoweNote send notes to the server, and UpdateNote updates it. DeleteNote is not in use right now (future feture)
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
    //Below: tasks handling. The content = "-" part is beacuse of the server issue, which will be fixed in the future
    public async Tasks.Task<bool> NewTask(Task task)
    {
        var payload = new
        {
            username = this.Username,
            token = this.Token,
            title = task.Name,
            content = "-",
            category = task.Category,
            tags = task.Tags,
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
            tags = task.Tags,
            category = task.Category,
            priority = task.Priority.ToString(),
            deadline = task.EndDate?.ToString("dd.MM.yyyy"),
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
    //Below: TaskList handling. There is just NewTaskList function working right now, but the deletion and editing are future fetures
    public async Tasks.Task<bool> NewTaskList(TaskList task_list)
    {
        var payload = new
        {
            username = this.Username,
            token = this.Token,
            title = task_list.Name,
            tags = task_list.Tags,
            category = task_list.Category,
            priority = task_list.Priority.ToString(),
            tasks = task_list.components.OfType<Task>().Select(t => new
            {
                title = t.Name,
                deadline = t.EndDate?.ToString("dd.MM.yyyy"),
            }).ToList()
        };

        try
        {
            var response = await PersonalHttpClient.PostAsJsonAsync(
                $"{BaseLink}/api/lists/new",
                payload
            );

            if (!response.IsSuccessStatusCode) return false;
            var json = await response.Content.ReadFromJsonAsync<JsonElement>();
            var id = json
                .GetProperty("data")
                .GetProperty("task_id")
                .GetInt32();
            task_list.SetId(id);
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
            {}
            return false;
        }
    }
}