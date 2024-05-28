using ImGuiNET;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoImGui.Data;
using MonoImGui.Enums;
using Serilog;
using System.Diagnostics;
using System.IO;
using Color = Microsoft.Xna.Framework.Color;
using Num = System.Numerics;

namespace MonoImGui
{
    public class Main : Game
    {
        public static bool ScrollLogToBottom { get; set; }

        private GraphicsDeviceManager _graphics;
        private ImGuiRenderer _imGuiRenderer;
        private bool _dummyBoolIsOpen = true;
        private bool _logOpen = false;

        public Main()
        {
            _graphics = new GraphicsDeviceManager(this);
            _graphics.PreferredBackBufferWidth = 600;
            _graphics.PreferredBackBufferHeight = 350;

            // Currently not usable in DesktopGL because of this bug:
            // https://github.com/MonoGame/MonoGame/issues/7914
            //_graphics.PreferMultiSampling = true;

            Window.AllowUserResizing = true;
            IsMouseVisible = true;

            Content.RootDirectory = "Content";
        }

        protected override void Initialize()
        {
            Window.Title = "MonoImGui Tool";

            _graphics.GraphicsDevice.PresentationParameters.BackBufferWidth = _graphics.PreferredBackBufferWidth;
            _graphics.GraphicsDevice.PresentationParameters.BackBufferHeight = _graphics.PreferredBackBufferHeight;

            _imGuiRenderer = new ImGuiRenderer(this);

            base.Initialize();

            MonoLog.LogInfoHeadline(FontAwesome.FlagCheckered, "APP-INITIALIZED");
        }

        protected override void LoadContent()
        {
            // Setting the working directory of the tool.
            // In this case it's the "Content" directory.
            Directory.SetCurrentDirectory(Path.Combine(Directory.GetCurrentDirectory(), "Content"));
            Log.Debug($"WorkingDir: {Directory.GetCurrentDirectory()}");
            Log.Debug($"LocalContentDir: {AppSettings.LocalContentPath}");
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            _imGuiRenderer.BeforeLayout(gameTime);
            ImGuiLayout();
            _imGuiRenderer.AfterLayout();

            base.Draw(gameTime);
        }

        private void ImGuiLayout()
        {
            var mainWindowFlags =
                ImGuiWindowFlags.NoTitleBar | ImGuiWindowFlags.NoCollapse | ImGuiWindowFlags.NoResize
                | ImGuiWindowFlags.NoMove | ImGuiWindowFlags.NoBringToFrontOnFocus | ImGuiWindowFlags.NoNavFocus
                | ImGuiWindowFlags.NoBackground;

            var windowFlags = ImGuiWindowFlags.NoCollapse | ImGuiWindowFlags.NoDecoration
                | ImGuiWindowFlags.NoSavedSettings | ImGuiWindowFlags.MenuBar 
                /*| ImGuiWindowFlags.AlwaysVerticalScrollbar*/;

            ImGuiViewportPtr viewport = ImGui.GetMainViewport();

            ImGui.SetNextWindowPos(viewport.Pos);
            ImGui.SetNextWindowSize(viewport.Size);
            ImGui.SetNextWindowViewport(viewport.ID);
            if (ImGui.Begin("Main", ref _dummyBoolIsOpen, mainWindowFlags))
            {
                ImGui.SetNextWindowPos(viewport.Pos);
                ImGui.SetNextWindowSize(viewport.Size);
                ImGui.SetNextWindowViewport(viewport.ID);
                if (ImGui.Begin("Content", ref _dummyBoolIsOpen, windowFlags))
                {
                    MenuBar();

                    if (_logOpen)
                    {
                        ImGui.Spacing(); ImGui.Spacing(); ImGui.Spacing(); ImGui.Spacing();

                        ImGui.PushStyleColor(ImGuiCol.FrameBg, ImGui.GetStyle().Colors[(int)ImGuiCol.WindowBg]);
                        ImGui.TextUnformatted(MonoSink.Output.ToString());
                        ImGui.PopStyleColor();

                        ImGui.Spacing(); ImGui.Spacing(); ImGui.Spacing(); ImGui.Spacing();

                        if (ImGui.Button($"{FontAwesome.StepBackward} Back", new Num.Vector2(ImGui.GetContentRegionAvail().X, 0)))
                        {
                            _logOpen = false;
                        }

                        //BUG: SetScrollHere currently doesn't work on InputTextMultiline.
                        //ImGui.InputTextMultiline("##LogText", ref logText, 999999, ImGui.GetContentRegionAvail(), ImGuiInputTextFlags.ReadOnly);

                        if (ScrollLogToBottom && ImGui.GetScrollMaxY() > ImGui.GetScrollY())
                        {
                            ScrollLogToBottom = false;
                            ImGui.SetScrollHereY(1.0f);
                        }
                    }
                    else
                    {
                        ImGui.Indent();
                        ImGui.TextWrapped(@$"Welcome {FontAwesome.HandPeace}

This template project makes it easier to start a new MonoGame tool with ImGui.NET and Serilog integrations.

It contains some very basic stuff, so you need to update everything to your needs.

Also please don't forget to update the text in the 'About' menu to your own info or alternatively remove it.

Have a nice day {FontAwesome.Smile}

:: BlizzCrafter {FontAwesome.Heart}");
                    }
                    ImGui.End();
                }
                ImGui.End();
            }
        }

        #region ImGui Widgets

        private void MenuBar()
        {
            if (ImGui.BeginMenuBar())
            {
                if (ImGui.BeginMenu("File"))
                {
                    ImGui.SeparatorText("New");
                    if (ImGui.MenuItem($"{FontAwesome.Plus} Add Content"))
                    {
                        // Your add content logic here.
                    }
                    ImGui.SeparatorText("App");
                    if (ImGui.MenuItem($"{FontAwesome.Save} Save"))
                    {
                        // Your save logic here.
                    }
                    if (ImGui.MenuItem($"{FontAwesome.WindowClose} Exit"))
                    {
                        // Your exit logic here.
                        Exit();
                    }
                    ImGui.EndMenu();
                }
                if (ImGui.BeginMenu("Options"))
                {
                    ImGui.SeparatorText("View");
                    if (ImGui.MenuItem($"{(_logOpen ? $"{FontAwesome.EyeSlash} Close Log" : $"{FontAwesome.Eye} Show Log")}"))
                    {
                        ScrollLogToBottom = true;
                        _logOpen = !_logOpen;
                    }
                    ImGui.EndMenu();
                }

                if (ImGui.BeginMenu("Help"))
                {
                    if (ImGui.BeginMenu($"{FontAwesome.FileArchive} Logs"))
                    {
                        if (ImGui.MenuItem("All"))
                        {
                            if (File.Exists(AppSettings.AllLogPath))
                            {
                                ProcessStartInfo process = new(AppSettings.AllLogPath)
                                {
                                    UseShellExecute = true
                                };
                                Process.Start(process);
                            }
                            else ModalDescriptor.SetFileNotFound(AppSettings.AllLogPath, "Note: this log file will only be created on certain events.");
                        }
                        if (ImGui.MenuItem("Important"))
                        {
                            if (File.Exists(AppSettings.ImportantLogPath))
                            {
                                ProcessStartInfo process = new(AppSettings.ImportantLogPath)
                                {
                                    UseShellExecute = true
                                };
                                Process.Start(process);
                            }
                            else ModalDescriptor.SetFileNotFound(AppSettings.ImportantLogPath, "Note: this log file gets created on errors or important events.");
                        }
                        ImGui.EndMenu();
                    }
                    if (ImGui.MenuItem($"{FontAwesome.AddressBook} About"))
                    {
                        ModalDescriptor.SetAbout();
                    }
                    ImGui.EndMenu();
                }
                ImGui.EndMenuBar();
            }

            if (ModalDescriptor.IsOpen)
            {
                ImGui.OpenPopup(ModalDescriptor.Title);
                PopupModal(ModalDescriptor.Title, ModalDescriptor.Message);
            }
        }

        private bool PopupModal(string title, string message)
        {
            ImGuiWindowFlags modalWindowFlags = ImGuiWindowFlags.NoSavedSettings | ImGuiWindowFlags.NoMove | ImGuiWindowFlags.NoDecoration
            | ImGuiWindowFlags.Modal | ImGuiWindowFlags.Popup | ImGuiWindowFlags.NoCollapse | ImGuiWindowFlags.AlwaysAutoResize;

            ImGuiViewportPtr viewport = ImGui.GetMainViewport();

            ImGui.SetNextWindowPos(viewport.GetCenter(), ImGuiCond.Always, new Num.Vector2(0.5f));
            if (ImGui.BeginPopupModal(ModalDescriptor.Title, ref _dummyBoolIsOpen, modalWindowFlags))
            {
                var buttonCountRightAligned = 1;
                var buttonWidth = 60f;
                ImGui.SeparatorText(title);

                ImGui.SetCursorPos(new Num.Vector2(ImGui.GetStyle().ItemSpacing.X / 2f, ImGui.GetFrameHeight()));
                ImGui.PushTextWrapPos(viewport.Size.X / 2f);
                ImGui.TextWrapped(message);
                ImGui.PopTextWrapPos();

                ImGui.Spacing(); ImGui.Spacing(); ImGui.Spacing(); ImGui.Spacing();

                if (ModalDescriptor.MessageType == MessageType.About)
                {
                    ImGui.Spacing(); ImGui.Spacing(); ImGui.Spacing(); ImGui.Spacing();

                    ImGui.PushStyleColor(ImGuiCol.Button, ImGui.GetStyle().Colors[(int)ImGuiCol.PlotHistogram]);
                    ImGui.PushStyleColor(ImGuiCol.ButtonHovered, ImGui.GetStyle().Colors[(int)ImGuiCol.PlotHistogramHovered]);
                    ImGui.PushStyleColor(ImGuiCol.ButtonActive, ImGui.GetStyle().Colors[(int)ImGuiCol.PlotHistogram]);
                    if (ImGui.Button($"{FontAwesomeBrands.Github} MonoImGui Tool", new Num.Vector2(ImGui.GetContentRegionAvail().X, 0)))
                    {
                        ProcessStartInfo process = new(AppSettings.GitHubRepoURL)
                        {
                            UseShellExecute = true
                        };
                        Process.Start(process);
                    }
                    ImGui.PopStyleColor(); ImGui.PopStyleColor(); ImGui.PopStyleColor();
                }

                ImGui.Spacing(); ImGui.Spacing(); ImGui.Spacing(); ImGui.Spacing();

                bool hasDelete = (ModalDescriptor.MessageType & MessageType.Delete) != 0;
                bool hasCancel = (ModalDescriptor.MessageType & MessageType.Cancel) != 0;
                if (hasCancel) buttonCountRightAligned++;

                if (hasDelete)
                {
                    ImGui.PushStyleColor(ImGuiCol.Button, ImGui.GetStyle().Colors[(int)ImGuiCol.TabActive]);
                    ImGui.PushStyleColor(ImGuiCol.ButtonHovered, ImGui.GetStyle().Colors[(int)ImGuiCol.TabHovered]);
                    ImGui.PushStyleColor(ImGuiCol.ButtonActive, ImGui.GetStyle().Colors[(int)ImGuiCol.Tab]);
                    ImGui.SetCursorPosX(ImGui.GetStyle().ItemSpacing.X);
                    if (ImGui.Button($"{FontAwesome.TrashAlt}", new Num.Vector2(buttonWidth, 0)))
                    {
                        // Your delete logic here.
                        return true;
                    }
                    ImGui.PopStyleColor(); ImGui.PopStyleColor(); ImGui.PopStyleColor();
                }

                ImGui.SameLine(ImGui.GetContentRegionMax().X - (buttonWidth * buttonCountRightAligned) - ImGui.GetStyle().ItemSpacing.X * buttonCountRightAligned);

                //if (HasError) ImGui.BeginDisabled();
                if (ImGui.Button("OK", new Num.Vector2(buttonWidth, 0)) || (ImGui.IsKeyPressed(ImGuiKey.Enter) /*&& !HasError*/))
                {
                    if (ModalDescriptor.MessageType == MessageType.AddContent)
                    {
                        // Your add content logic here.
                    }
                    else if (ModalDescriptor.MessageType == MessageType.EditContent)
                    {
                        // Your edit content logic here.
                    }
                    ClosePopupModal();
                    return true;
                }
                //if (HasError) ImGui.EndDisabled();

                if (hasCancel)
                {
                    ImGui.SameLine();
                    if (ImGui.Button("Cancel", new Num.Vector2(buttonWidth, 0)) || ImGui.IsKeyPressed(ImGuiKey.Escape))
                    {
                        ClosePopupModal();
                        return false;
                    }
                }
                ImGui.EndPopup();
            }
            return false;
        }
        private void ClosePopupModal()
        {
            ModalDescriptor.Reset();
            ImGui.CloseCurrentPopup();
        }

        #endregion
    }
}
