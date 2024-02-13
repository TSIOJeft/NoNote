using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Input;
using Microsoft.Web.WebView2.Core;
using Newtonsoft.Json;
using NoNote.config;
using NoNote.util;
using Application = System.Windows.Application;
using ContextMenu = System.Windows.Controls.ContextMenu;
using KeyEventArgs = System.Windows.Input.KeyEventArgs;
using MenuItem = System.Windows.Controls.MenuItem;
using MessageBox = System.Windows.MessageBox;
using SaveFileDialog = Microsoft.Win32.SaveFileDialog;

namespace NoNote
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    ///
    public partial class MainWindow
    {
        private string current_file_path = null;
        private NoteWebServer noteWebServer;

        public MainWindow()
        {
            InitializeComponent();
            initEditor();
            taskPanel();

            this.KeyDown += keyDownHandler;
        }

        private void keyDownHandler(object sender, KeyEventArgs e)
        {
            if (Keyboard.Modifiers == ModifierKeys.Control && e.Key == Key.S)
            {
                if (current_file_path == null)
                {
                    SaveFileDialog saveFileDialog = new SaveFileDialog();
                    saveFileDialog.Filter = "MD files (*.md)|*.md;";
                    saveFileDialog.DefaultExt = "md";
                    saveFileDialog.Title = "New Note File";
                    saveFileDialog.FileName = "myNote.md";
                    saveFileDialog.InitialDirectory = Path.Combine(ConfigUtil.configArray.workFolder, "note");
                    saveFileDialog.RestoreDirectory = true;
                    bool? result = saveFileDialog.ShowDialog();
                    if (result == true)
                    {
                        string filePath = saveFileDialog.FileName;
                        FileUtil.saveTextFile(filePath, "Hello~NoNote");
                        current_file_path = filePath;
                        getEditorValue();
                        LoadFile(current_file_path);
                    }
                }
                else
                {
                    getEditorValue();
                }
            }
        }

        public void initEditor()
        {
            noteWebServer = new NoteWebServer();
            noteWebServer.WebServerPath = ConfigUtil.myFolder + "\\assets\\editor";
            noteWebServer.startListen();
            // Console.WriteLine(Path.Combine(appDir, @"assets\editor\editor.html"));
            // CefSettings cefSettings = new CefSettings();
            // Cef.Initialize(cefSettings);
            // mybrowser.CoreWebView2.Navigate("https://www.google.com");
            initWebView();
        }

        public async void initWebView()
        {
            await this.mybrowser.EnsureCoreWebView2Async(null);
            this.mybrowser.CoreWebView2.Navigate("http://127.0.0.1:5050/");
            this.mybrowser.CoreWebView2.WebMessageReceived += WebMesReceived;
        }

        private void WebMesReceived(object sender, CoreWebView2WebMessageReceivedEventArgs args)
        {
            dynamic my_web_data = JsonConvert.DeserializeObject<dynamic>(args.TryGetWebMessageAsString());

            if (my_web_data == null) return;
            int event_code = my_web_data["event"];
            if (event_code == 0)
            {
                LoadLastFile();
            }
            else if (event_code == 1)
            {
                string mes = my_web_data["mes"];
                if (!string.IsNullOrEmpty(mes))
                {
                    FileUtil.saveTextFile(current_file_path, mes);
                }
            }
            else if (event_code == 2)
            {
                Console.WriteLine("receive file");
                var fileStream = my_web_data["mes"];
                using (MemoryStream memoryStream = new MemoryStream())
                {
                    using (BinaryWriter writer = new BinaryWriter(memoryStream))
                    {
                        foreach (var single in fileStream)
                        {
                            writer.Write((byte)single);
                        }
                    }

                    byte[] data = memoryStream.ToArray();
                    memoryStream.Close();
                    string path = Path.Combine(ConfigUtil.configArray.workFolder, "note", "img\\test.png");
                    string imgPath = FileUtil.saveImgFile(path, data);
                    insertEditorImg($"![img](http://127.0.0.1:5050/note/img/{new FileInfo(imgPath).Name})");
                }
            }
            else if (event_code == 3)
            {
                notice_ellipse.Visibility = Visibility.Visible;
            }
        }

        private void LoadLastFile()
        {
            if (ConfigUtil.configArray.lastFile != null)
            {
                current_file_path = ConfigUtil.configArray.lastFile;
                LoadFileTitle();
            }
        }

        private void LoadFile(string filePath)
        {
            if (!File.Exists(filePath)) return;
            string content = FileUtil.getTextFile(filePath);
            current_file_path = filePath;
            content = System.Text.RegularExpressions.Regex.Escape(content);
            mybrowser.CoreWebView2.ExecuteScriptAsync($"ameSetValue('{content}')");
            ConfigUtil.configArray.lastFile = filePath;
            new ConfigUtil().saveConfig();
            LoadFileTitle();
        }

        private void LoadFileTitle()
        {
            string fileName = Path.GetFileName(current_file_path);
            if (fileName == null) return;
            fileName_Title.Text = fileName.Substring(0, fileName.LastIndexOf(".", StringComparison.Ordinal));
        }

        private async void insertEditorValue(string value)
        {
        }

        private async void insertEditorImg(string value)
        {
            var result = await mybrowser.CoreWebView2.ExecuteScriptWithResultAsync($"ameInsertValue('{value}')");
            if (result.Succeeded)
            {
            }
        }

        private async void getEditorValue()
        {
            var result = await mybrowser.CoreWebView2.ExecuteScriptWithResultAsync($"ameGetValue()");
            if (result.Succeeded)
            {
                string value;
                int code;
                result.TryGetResultAsString(out value, out code);
                saveEditorValue(current_file_path, value);
            }
        }

        private void saveEditorValue(string filePath, string value)
        {
            FileUtil.saveTextFile(filePath, value);
            notice_ellipse.Visibility = Visibility.Hidden;
        }

        private void MainWindow_OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                DragMove();
            }
        }

        public void saveFile(String content)
        {
            Console.WriteLine(content);
        }


        private void addNewNote()
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "MD files (*.md)|*.md;";
            saveFileDialog.DefaultExt = "md";
            saveFileDialog.Title = "New Note File";
            saveFileDialog.FileName = "myNote.md";
            saveFileDialog.InitialDirectory = Path.Combine(ConfigUtil.configArray.workFolder, "note");
            saveFileDialog.RestoreDirectory = true;
            bool? result = saveFileDialog.ShowDialog();
            if (result == true)
            {
                string filePath = saveFileDialog.FileName;
                FileUtil.saveTextFile(filePath, "Hello~NoNote");
                LoadFile(filePath);
            }
        }

        private void addNewNoteButtonClick(object sender, EventArgs e)
        {
            addNewNote();
        }

        private void FileName_Title_OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            ContextMenu contextMenu = ((FrameworkElement)sender).ContextMenu;
            if (contextMenu == null) return;
            contextMenu.Items.Clear();
            MenuItem addFileItem = new MenuItem();
            addFileItem.Header = "+";
            addFileItem.Click += addNewNoteButtonClick;
            contextMenu.Items.Add(addFileItem);
            foreach (var noteFile in getNotes(Path.Combine(ConfigUtil.configArray.workFolder, "note")))
            {
                MenuItem menuItem = new MenuItem();
                menuItem.Header = Path.GetFileName(noteFile);
                menuItem.Click += ((o, args) => { LoadFile(noteFile); });
                contextMenu.Items.Add(menuItem);
            }

            contextMenu.IsOpen = true;
        }

        private void createFileContextMenu()
        {
        }

        private string[] getNotes(string notepath)
        {
            string[] files = { };
            if (!Directory.Exists(notepath)) return files;
            files = Directory.GetFiles(notepath);
            return files;
        }

        // private void Logo_Button_Click(object sender, RoutedEventArgs e)
        // {
        //     ContextMenu contextMenu = ((FrameworkElement)sender).ContextMenu;
        //     if (contextMenu == null) return;
        // }

        private void Logo_Button_Click(object sender, RoutedEventArgs e)
        {
            ContextMenu contextMenu = ((FrameworkElement)sender).ContextMenu;
            if (contextMenu == null) return;
            contextMenu.IsOpen = true;
        }

        private void Work_Folder_Item_Click(object sender, RoutedEventArgs e)
        {
            using (var dialog = new FolderBrowserDialog())
            {
                dialog.SelectedPath = ConfigUtil.configArray.workFolder;
                DialogResult result = dialog.ShowDialog();
                if (result == System.Windows.Forms.DialogResult.OK)
                {
                    string selectedPath = dialog.SelectedPath;
                    ConfigUtil.configArray.workFolder = dialog.SelectedPath;
                    if (!Directory.Exists(selectedPath + "\\note")) Directory.CreateDirectory(selectedPath + "\\note");
                    this.current_file_path = null;
                    new ConfigUtil().saveConfig();
                }
            }
        }

        private void maxmize_window(object sender, RoutedEventArgs e)
        {
            if (this.WindowState != WindowState.Maximized)
            {
                this.WindowState = WindowState.Maximized;
            }
            else
            {
                this.WindowState = WindowState.Normal;
            }
        }

        private void close_window(object sender, RoutedEventArgs e)
        {
            Properties.Settings.Default.WindowHeight = (int)this.Height;
            Properties.Settings.Default.WindowWidth = (int)this.Width;
            this.WindowState = WindowState.Minimized;
            this.Hide();
            Properties.Settings.Default.Save();
            Properties.Settings.Default.Reload();
        }

        private void minimize_window(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }

        private void taskPanel()
        {
            NotifyIcon ni = new System.Windows.Forms.NotifyIcon();
            ni.Icon = new System.Drawing.Icon(AppDomain.CurrentDomain.BaseDirectory + @"/icon/nonote.ico");
            ni.Visible = true;
            ni.Text = "NoNote";
            ni.Click +=
                (sender, args) =>
                {
                    this.WindowState = WindowState.Minimized;
                    this.Show();
                    Activate();
                    Focus();
                    this.WindowState = WindowState.Normal;
                };

            System.Windows.Forms.MenuItem menuExit = new System.Windows.Forms.MenuItem();
            menuExit.Text = "退出";
            menuExit.Click += ((sender, args) =>
            {
                if (noteWebServer != null) noteWebServer.stopListen();
                Environment.Exit(Environment.ExitCode);
            });
            ni.ContextMenu =
                new System.Windows.Forms.ContextMenu(new System.Windows.Forms.MenuItem[] { menuExit });
        }
    }
}