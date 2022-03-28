using UnityEngine;
using UnityEngine.UI;

public class CreditsManager : ScreenManager
{

    public float ScrollSpeed = 2.0f;

    public RectTransform CreditsContainer;
    public GameObject RhythmistContainer;
    public Text LeadRhythmistName;

    [Header("Sounds")] 
    public AudioSource SfxBack;

    private float _containerHeight;

    void Awake()
    {
        FindCoreManager();
        var pos = CreditsContainer.localPosition;
        _containerHeight = CreditsContainer.sizeDelta.y;
        CreditsContainer.localPosition = new Vector3(pos.x, _containerHeight * -0.5f, pos.z);
    }

    private void Start()
    {
        var leadRhythmist = CoreManager.ProfileManager.GetLeadRhythmist();
        
        if (leadRhythmist == null)
        {
            RhythmistContainer.SetActive(false);
            return;
        }

        RhythmistContainer.SetActive(true);
        LeadRhythmistName.text = leadRhythmist.Name;

    }

    void FixedUpdate()
    {
        var pos = CreditsContainer.localPosition;
        var newY = pos.y + ScrollSpeed;

        if (newY > _containerHeight / 2)
        {
            newY -= _containerHeight;
        }
        CreditsContainer.localPosition = new Vector3(pos.x, newY, pos.z);
    }

    public override void OnPlayerInput(InputEvent inputEvent)
    {
        switch (inputEvent.Action)
        {
            case "B":
            case "Back":
                SfxBack.PlayUnlessNull();
                SceneTransition(GameScene.Options);
                break;
        }
    }

}
