using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Microsoft.Web.WebView2.Core;
using Microsoft.Win32;
using Newtonsoft.Json;
using NoNote.util;

namespace NoNote
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    ///
    public partial class MainWindow
    {
        public static string myFolder;
        private string current_file_path = null;
        private string myNoteFolder;

        public MainWindow()
        {
            InitializeComponent();
            initEditor();
            this.KeyDown += keyDownHandler;
        }

        private void keyDownHandler(object sender, KeyEventArgs e)
        {
            if (Keyboard.Modifiers == ModifierKeys.Control && e.Key == Key.S)
            {
                getEditorValue();
            }
        }

        public void initEditor()
        {
            string appDir = AppDomain.CurrentDomain.BaseDirectory;

            myFolder = appDir;
            this.myNoteFolder = myFolder + "\\assets\\editor\\note";
            NoteWebServer noteWebServer = new NoteWebServer();
            noteWebServer.WebServerPath = myFolder + "\\assets\\editor";
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
                string file_path = myFolder + "\\assets\\editor\\note\\hello.md";
                LoadFile(file_path);
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
                    string path = myFolder + "\\assets\\editor\\note\\img\\test.png";
                    string imgPath = FileUtil.saveImgFile(path, data);
                    insertEditorImg($"![img](http://127.0.0.1:5050/note/img/{new FileInfo(imgPath).Name})");
                }
            }
        }

        private void LoadFile(string filePath)
        {
            if (!File.Exists(filePath)) return;
            string content = FileUtil.getTextFile(filePath);
            current_file_path = filePath;
            content = System.Text.RegularExpressions.Regex.Escape(content);
            mybrowser.CoreWebView2.ExecuteScriptAsync($"ameSetValue('{content}')");
            string fileName = Path.GetFileName(filePath);
            fileName_Title.Text = fileName;
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
                FileUtil.saveTextFile(current_file_path, value);
            }
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

        private void close_window(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void minimize_window(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }

        private void addNewNote()
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "MD files (*.md)|*.md;";
            saveFileDialog.DefaultExt = "md";
            saveFileDialog.Title = "New Note File";
            saveFileDialog.FileName = "myNote.md";
            saveFileDialog.InitialDirectory = Path.Combine(myFolder, "assets\\editor\\note");
            saveFileDialog.RestoreDirectory = true;
            bool? result = saveFileDialog.ShowDialog();
            if (result == true)
            {
                string filePath = saveFileDialog.FileName;
                FileUtil.saveTextFile(filePath,"Hello~NoNote");
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
            foreach (var noteFile in getNotes(myNoteFolder))
            {
                MenuItem menuItem = new MenuItem();
                menuItem.Header = Path.GetFileName(noteFile);
                menuItem.Click += ((o, args) => { LoadFile(noteFile); });
                contextMenu.Items.Add(menuItem);
            }

            if (contextMenu != null)
                contextMenu.IsOpen = true;
        }

        private void createFileContextMenu()
        {
        }

        private string[] getNotes(string notepath)
        {
            string[] files = Directory.GetFiles(notepath);
            return files;
        }

        // private void Logo_Button_Click(object sender, RoutedEventArgs e)
        // {
        //     ContextMenu contextMenu = ((FrameworkElement)sender).ContextMenu;
        //     if (contextMenu == null) return;
        // }
    }
}