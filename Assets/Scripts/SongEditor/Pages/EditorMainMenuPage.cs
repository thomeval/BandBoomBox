using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Assets.Scripts.Editor.Pages
{
    public class EditorMainMenuPage : EditorPageManager
    {
        public override EditorPage EditorPage
        {
            get { return EditorPage.MainMenu; }
        }

        public Button DefaultButton;
        void Awake()
        {
        }

        void OnEnable()
        {
            Parent.EventSystem.SetSelectedGameObject(DefaultButton.gameObject);
        }

        public void BtnEditExistingSong_OnClick()
        {
            Parent.IsExistingSong = true;
            Parent.RequestBrowseFile(".sjson", SjsonFileSelected);
        }

        public void BtnCreateNewSong_OnClick()
        {
            Parent.IsExistingSong = false;
            Parent.CurrentPage = EditorPage.Basics;
        }

        public void BtnBackToMainMenu_OnClick()
        {
            Parent.SceneTransition(GameScene.MainMenu);
        }

        public void BtnOpenMainSongsFolder_OnClick()
        {
            Helpers.OpenFolderWindow(Parent.SongsHomePath);
        }

        public override void HandleInput (InputEvent inputEvent)
        {
            switch (inputEvent.Action)
            {
                case "B":
                case "Back":
                    BtnBackToMainMenu_OnClick();
                    break;
            }
        }

        public void SjsonFileSelected(string target)
        {
            if (target == null)
            {
                Parent.CurrentPage = this.EditorPage;
                return;
            }

            Parent.LoadSong(target);
            Parent.CurrentPage = EditorPage.Details;
        }
    }

}
