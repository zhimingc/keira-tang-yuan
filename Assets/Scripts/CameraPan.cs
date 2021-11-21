using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CameraPan : MonoBehaviour
{
    [SerializeField]
    public UnityEngine.UI.Image imagePrefab;
    private UnityEngine.UI.Image image;

    [SerializeField]
    public Canvas canvas;

    [SerializeField]
    public bool fitToX = true;

    [SerializeField]
    [Range(1, 5)]
    public float imageSizeRelativeToScreen = 1;

    private float imageZoom;
    private Vector2 extraPixels;

    [SerializeField]
    [Range(1, 10)]
    public int zonesX;

    [SerializeField]
    [Range(1, 10)]
    public int zonesY;

    [SerializeField]
    public UnityEngine.UI.Image zonePrefab;

    public List<UnityEngine.UI.Image> zones;

    [SerializeField]
    public float smoothingTime = 0.4f;

    private float smoothTimer;
    private Vector2 startLocation;
    private Vector2 targetLocation;

    private void Awake()
    {
        image = Instantiate(imagePrefab, canvas.transform) as UnityEngine.UI.Image;
    }

    // Start is called before the first frame update
    void Start()
    {
        smoothTimer = 0;
        SetImageSize();
        if (zonesX > 1 || zonesY > 1)
            GenerateZones();
    }

    void SetImageSize() 
    {
        image.SetNativeSize();
        Vector2 referenceResolution = GetComponent<CanvasScaler>().referenceResolution;
        if (fitToX)
        {
            image.rectTransform.sizeDelta = new Vector2(referenceResolution.x * imageSizeRelativeToScreen, //Scale width
                                                        referenceResolution.x * imageSizeRelativeToScreen / image.overrideSprite.rect.width * image.overrideSprite.rect.height); //Scale height, maintaining aspect ratio
        }
        else 
        {
            image.rectTransform.sizeDelta = new Vector2(referenceResolution.y * imageSizeRelativeToScreen / image.overrideSprite.rect.height * image.overrideSprite.rect.width, //Scale width, maintaining aspect ratio
                                                        referenceResolution.y * imageSizeRelativeToScreen); //Scale height, 

        }

        extraPixels.x = image.rectTransform.sizeDelta.x - Screen.width;
        extraPixels.y = image.rectTransform.sizeDelta.y - Screen.height;
    }

    void GenerateZones()
    {
        if (zonesX <= 0 || zonesY <= 0)
            return;
        
        Vector2 zoneSize = new Vector2(Screen.width / (float)zonesX, Screen.height / (float)zonesY);
        for (int i = 0; i < zonesX; ++i)
        {
            for (int j = 0; j < zonesY; ++j)
            {
                UnityEngine.UI.Image newZonePrefab = Instantiate(zonePrefab, canvas.transform, false);
                newZonePrefab.rectTransform.sizeDelta = zoneSize;
                newZonePrefab.GetComponent<BoxCollider2D>().size = zoneSize;

                Color translucent = newZonePrefab.color;
                translucent.a = 0.0f;
                newZonePrefab.color = translucent;

                Vector3 newPos = newZonePrefab.rectTransform.localPosition;
                newPos.x = -Screen.width / 2.0f + zoneSize.x * (i + 0.5f);
                newPos.y = -Screen.height / 2.0f + zoneSize.y * (j + 0.5f);
                newZonePrefab.rectTransform.localPosition = newPos;

                newZonePrefab.GetComponent<ZoneBehaviour>().index = new Vector2(i, j);
                zones.Add(newZonePrefab);
            }
        }
    }

    public void OnZoneMouseOver(Vector2 index) 
    {
        //convert index to [-0.5f, 0.5f] range
        Vector2 normalizedZone = new Vector2(index.x / (zonesX - 1.0f) - 0.5f, index.y / (zonesY - 1.0f) - 0.5f);
        Vector2 newPos;
        newPos.x = -normalizedZone.x * extraPixels.x; 
        newPos.y = -normalizedZone.y * extraPixels.y;

        MoveBackgroundCentre(newPos);
    }

    void MoveBackgroundCentre(Vector2 pos)
    {
        //if (smoothTimer > 0)
        //    image.rectTransform.localPosition = targetLocation;
        smoothTimer = smoothingTime;
        startLocation = image.rectTransform.localPosition;
        targetLocation = pos;

    }

    void HandleMouseMovement() 
    {
        if (Input.GetAxis("Mouse X") != 0 || Input.GetAxis("Mouse Y") != 0)
        {
            var normalizedMousePos = new Vector2(Input.mousePosition.x / Screen.width - 0.5f, Input.mousePosition.y / Screen.height - 0.5f);

            if (!MouseInScreen(normalizedMousePos))
                return;

            // Vector3 newPos = image.rectTransform.localPosition;
            // newPos.x = normalizedMousePos.x * extraPixels.x;
            // newPos.y = normalizedMousePos.y * extraPixels.y;
            // MoveBackgroundCentre(newPos);

        }
    }

    bool MouseInScreen(Vector2 normalizedMousePos) 
    {
        return !(normalizedMousePos.x > 0.5f || normalizedMousePos.x < -0.5f ||
                normalizedMousePos.y > 0.5f || normalizedMousePos.y < -0.5f);
    }

    // Update is called once per frame
    void Update()
    {
        SetImageSize();
        HandleMouseMovement();
    }

    public void SetBackgroundImage(Sprite texture)
    {
        image.sprite = texture;
        SetImageSize();
    }

    private void LateUpdate()
    {
        if (smoothTimer > 0)
        {
            image.rectTransform.localPosition = Vector2.Lerp(startLocation, targetLocation, 1.0f - (smoothTimer / smoothingTime));
            smoothTimer -= Time.deltaTime;
            if (smoothTimer <= 0)
            {
                image.rectTransform.localPosition = targetLocation;
                smoothTimer = 0;
            }
        }
        else 
        {
            smoothTimer = 0;
        }
    }
}
