using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Button = UnityEngine.UI.Button;

namespace Assets.Scripts.Editor.Pages
{
    public class EditorBasicsPage : EditorPageManager
    {
        public InputField TxtAudioFilePath;
        public InputField TxtDestinationSJsonFilename;
        public InputField TxtDestinationFolder;

        public Button BtnBack;
        public Button BtnNext;
        public Button BtnBrowseAudioFile;

        public Text TxtErrorMessage;
        public Text LblDestinationFolderPath;
        public override EditorPage EditorPage
        {
            get { return EditorPage.Basics; }
        }

        void Awake()
        {
            BtnBack.onClick.AddListener(BtnBack_OnClick);
            BtnNext.onClick.AddListener(BtnNext_OnClick);
            BtnBrowseAudioFile.onClick.AddListener(BtnBrowseAudioFile_Click);

        }

        void OnEnable()
        {
            Parent.EventSystem.SetSelectedGameObject(TxtAudioFilePath.gameObject);

            if (!string.IsNullOrEmpty(Parent.SongsHomePath))
            {
                LblDestinationFolderPath.text =
                    LblDestinationFolderPath.text.Replace("@DestinationFullFolder", Parent.SongsHomePath);
            }
        }

        public override void HandleInput (InputEvent inputEvent)
        {
            switch (inputEvent.Action)
            {
                case InputAction.Back:
                    BtnBack_OnClick();
                    break;
            }
        }

        public void BtnBack_OnClick()
        {
            Parent.CurrentPage = EditorPage.MainMenu;
        }

        public void BtnNext_OnClick()
        {
            try
            { 
                var err = ValidateInputs();
                TxtErrorMessage.text = err;

                if (err == null)
                {
                    CreateSongTemplate();
                    var newSong = GetDefaultNewSong();

                    Parent.CurrentSong = newSong;
                    Parent.CurrentPage = EditorPage.Details;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        void BtnBrowseAudioFile_Click()
        {
            Parent.RequestBrowseFile(".ogg;.mp3", AudioFileSelected);
        }

        private void AudioFileSelected(string result)
        {

            Parent.CurrentPage = this.EditorPage;

            if (result != null)
            {
                TxtAudioFilePath.text = result;
            }
        }

        private SongData GetDefaultNewSong()
        {
            var destSJson = Path.Combine(Parent.SongsHomePath, TxtDestinationFolder.text, TxtDestinationSJsonFilename.text);
            var audioFile = Path.GetFileName(TxtAudioFilePath.text);
            return new SongData
            {
                ID = Guid.NewGuid().ToString(),
                ChartAuthor = Parent.CoreManager.Settings.DefaultChartAuthor,
                AudioFile = audioFile,
                AudioPath = TxtAudioFilePath.text,
                SjsonFilePath = destSJson,
                BeatsPerMeasure = 4,
                Version = 1,
                SongCharts = new List<SongChart>
                {
                    new SongChart
                    {
                        Difficulty = Difficulty.Beginner,
                        DifficultyLevel = 1,
                        Group = "Main",
                        Notes = new string[0]
                    },
                    new SongChart
                    {
                        Difficulty = Difficulty.Medium,
                        DifficultyLevel = 3,
                        Group = "Main",
                        Notes = new string[0]
                    },
                    new SongChart
                    {
                        Difficulty = Difficulty.Hard,
                        DifficultyLevel = 5,
                        Group = "Main",
                        Notes = new string[0]
                    },
                    new SongChart
                    {
                        Difficulty = Difficulty.Expert,
                        DifficultyLevel = 7,
                        Group = "Main",
                        Notes = new string[0]
                    }
                },
            };
        }

        private void CreateSongTemplate()
        {
            if (TxtAudioFilePath.text == null)
            {
                return;
            }

            var destFolder = Path.Combine(Parent.SongsHomePath, TxtDestinationFolder.text);
            var destAudio = Path.Combine(destFolder, Path.GetFileName(TxtAudioFilePath.text));

            if (!Directory.Exists(destFolder))
            {
                Directory.CreateDirectory(destFolder);
            }

            if (!File.Exists(destAudio))
            {
                File.Copy(TxtAudioFilePath.text, destAudio, true);
                Debug.Log($"Copied audio file to {destAudio}");
            }
        }

        private string ValidateInputs()
        {

            if (string.IsNullOrWhiteSpace(TxtAudioFilePath.text))
            {
                return "Audio File Path cannot be null or empty.";
            }
            if (string.IsNullOrWhiteSpace(TxtDestinationSJsonFilename.text))
            {
                return "Destination SJson Filename cannot be null or empty.";
            }
            if (string.IsNullOrWhiteSpace(TxtDestinationFolder.text))
            {
                return "Destination Folder cannot be null or empty.";
            }

            if (!File.Exists(TxtAudioFilePath.text))
            {
                return "Audio File Path does not exist.";
            }

            if (!TxtDestinationSJsonFilename.text.ToUpper().EndsWith(".SJSON"))
            {
                TxtDestinationSJsonFilename.text += ".sjson";
            }

            return null;
        }
    }

}
